# Virtual button events (Hold / Repeat / LongRelease / Encoder-from-buttons)

Status: Hold, Repeat, and LongRelease implemented (`SyntheticButtonEventGenerator`, wired into
`MobiFlightCache`/`JoystickManager`/`MidiBoardManager`). Encoder-from-button-pairs and the
precondition-facing button state extension remain design discussion only - see "Non-goals" below.

## Problem

Real button events (`PRESS`, `RELEASE`) run through one shared pipeline:

`mobiFlightCache_OnButtonPressed` (`ExecutionManager.cs:1445`) → `InputEventExecutor.Execute`
(`InputEventExecutor.cs:59`) → precondition check → Modifiers → `cfg.execute(...)` → status/UI
update (`updatedValues`).

`onHold` and `onLongRelease` ("virtual" events, since nothing on the wire actually reports them)
do not go through this pipeline consistently:

- **`onHold`** is fired directly from a `System.Timers.Timer` inside `ButtonInputConfig`
  (`ButtonInputConfig.cs:196-244`), calling `onHold.execute(...)` itself. It skips the Modifiers
  pipeline, skips `cfg.RawValue`/`cfg.Value`/`updatedValues` (so it never reaches the UI —
  `ExecutionManager.updatedValues` is only ever written from the `Execute(e, ...)`/`Execute(cfg, ...)`
  call sites, `ExecutionManager.cs:1461/1469/1087`), skips the shared try/catch → `cfg.Status`
  error reporting, and reuses config refs/cache collection captured once at press time
  (`LastOnPressCacheCollection`/`LastOnPressConfigRefs`) rather than resolving them fresh. It also
  runs on a threadpool thread, racing the thread that drives `Execute()`.
- **`onLongRelease`** does reach the shared pipeline, but only because
  `CheckAndAdaptForLongButtonRelease` (`ButtonInputConfig.cs:273-288`) mutates `args.Value` from
  `RELEASE` to `LONG_RELEASE` *after* `InputEventExecutor` has already computed the log line and
  `cfg.RawValue` from the pre-mutation value (`InputEventExecutor.cs:105,110` run before `:137`).
  The log/UI label says "Release" even when `onLongRelease` is what actually fired.
- `InputConfigItem.GetInputAction` (`InputConfigItem.cs:379-413`) has no `case HOLD` — harmless
  today only because Hold never reaches it, but a silent trap (`default: return null`) waiting for
  the day it does.
- There is no separate identity at all for "the button is still being held and firing again" —
  `RepeatTimer_Elapsed` (`ButtonInputConfig.cs:208-218`) calls the exact same `ExecuteOnHoldAction`
  as the first `HoldTimer_Elapsed` fire, so a repeat looks identical to the initial HOLD in every
  log line and status update. There's no way for a user to see, from the UI, whether repeat
  detection is actually working versus HOLD just firing once.
- `MobiFlightButton.InputEventIdToString` (`MobiFlightButton.cs:18-36`) has no `case HOLD` either —
  a second, independent instance of the same gap as `GetInputAction`'s. A HOLD event's label falls
  through to `"n/a"` today, so even a real HOLD firing through the pipeline wouldn't display
  correctly.
- No unit test exercises actual hold/repeat timing or the long-release boundary; the two existing
  hold tests reach a private `Timer` field via reflection to assert it stopped
  (`InputEventExecutorTests.cs:591-671`), because there's no injectable clock.

## Goals

- One execution path for all button-derived events — real or synthetic — so preconditions,
  Modifiers, status reporting, and UI updates behave identically regardless of which event fired.
- Timer/thread-safety issues resolved by construction, not by discipline.
- The system that detects "is this button held long enough" is testable without reflection or
  real-time sleeps.

## Non-goals (explicitly deferred)

- **Splitting `InputEventArgs` into three typed events with three delegates** (`ButtonInputEvent` /
  `EncoderInputEvent` / `AnalogInputEvent`, replacing the single `ButtonEventHandler`/`InputEventArgs`
  pair used for all three today). This is a real problem — the delegate is called `OnButtonPressed`
  but carries Button, Encoder, and AnalogInput events; `InputEventArgs.Value` is an untyped `double`
  whose meaning depends entirely on `InputType`; `GetInputAction`'s switch-on-`InputType` is exactly
  what let `HOLD` fall through silently. But `InputEventArgs` is touched in 46 files, and the ~15
  `InputAction` subclasses (`FsuipcOffsetInputAction`, `MSFS2020CustomInputAction`, `VariableInputAction`,
  etc.) all consume it identically and generically (`args.Value.ToString()` for `@` placeholder
  substitution) — none of them care about the button/encoder/analog distinction. Worth doing later
  as its own change (three delegates, one shared base class so the generic consumers don't need to
  change), but out of scope here.
- Any change to `Device.Type` (the config-side `TYPE_BUTTON`/`TYPE_ENCODER`/`TYPE_ANALOG` string
  tagging) — separate, pre-existing classification, tangential to this work.
- **Exposing per-controller button state to preconditions** (a `"button"` precondition type,
  answering "is this other physical button currently held," closing the gap where
  `PreconditionChecker.CheckPrecondition` — `PreconditionChecker.cs:28-67` — only supports `"pin"`/
  `"variable"`/`"config"` today and the only current workaround is relaying state through a
  MobiFlight Variable). Real gap, not needed to make Hold/LongRelease/Encoder work, so deferred —
  KISS, smallest workable scope. The one thing to keep in mind while building the generator so this
  doesn't get designed shut: keep its per-button tracking state (pressed / since-when / hold-fired)
  as a small dedicated internal type, not inlined loose fields — see "Generator's internal state"
  below. Promoting that later to something preconditions can query is then a visibility/query-surface
  change, not a rewrite.

## Design

### Ownership: inside the controller, not the config item

Detection state (is this physical button currently pressed, since when, has hold already fired)
does **not** belong on `ButtonInputConfig`/`InputConfigItem`. A physical button can be bound by
multiple `InputConfigItem`s — even across simultaneously-loaded config files, since
`_inputEventExecutors` is keyed per `ConfigFile` and `mobiFlightCache_OnButtonPressed` fans one raw
event out to all of them (`ExecutionManager.cs:1456-1467`). Tracking detection per binding means
duplicate, independent timers for what is physically one button, and was the direct cause of the
`08179c39` bug class (cleanup/lifecycle tied to whether a specific binding happened to configure an
action).

Detection moves to the controller layer instead — a **synthetic button event generator** owned
internally by each controller manager (`MobiFlightCache`, `JoystickManager`, `MidiBoardManager`),
one physical button tracked once regardless of how many configs bind to it.

### Stays transparent to everything downstream

All three controller managers already funnel real events through an identical one-line method:

```csharp
// MobiFlightCache.cs:415-417 / JoystickManager.cs:350-352 / MidiBoardManager.cs:278-280
public void Module_OnButtonPressed(object sender, InputEventArgs e)
{
    OnButtonPressed?.Invoke(sender, e);
}
```

The generator is a private field on each manager, wired into that same funnel:

```csharp
private readonly SyntheticButtonEventGenerator _virtualEvents = new SyntheticButtonEventGenerator();

public MobiFlightCache()
{
    _virtualEvents.OnSyntheticEvent += (s, e) => OnButtonPressed?.Invoke(s, e);
}

public void Module_OnButtonPressed(object sender, InputEventArgs e)
{
    // classify RELEASE vs LONG_RELEASE per bound config before forwarding - usually one event,
    // more if multiple configs are bound to this button with different LongReleaseDelay
    foreach (var classified in _virtualEvents.Observe(e))
        OnButtonPressed?.Invoke(sender, classified);
}
```

`ExecutionManager`'s subscriptions (`mobiFlightCache.OnButtonPressed += ...`,
`joystickManager.OnButtonPressed += ...`, `midiBoardManager.OnButtonPressed += ...`,
`ExecutionManager.cs:218,221,231`) do not change. It has no knowledge the generator exists — it
just sees a stream of already-fully-classified `InputEventArgs`, same as today.

`InputEventExecutor`/`InputConfigItem` need exactly one change: `GetInputAction` gets a `case HOLD`
(and `case REPEAT`, alongside it - both resolve to `onHold`, there is no separate `onRepeat`
binding), since Hold now genuinely flows through it instead of bypassing it. REPEAT exists as its
own `MobiFlightButton.InputEvent` value, distinct from HOLD, purely so the log/UI can show that
repeat detection is what's firing on the Nth tick of a held button, not the same HOLD firing again
with no way to tell them apart.

With detection moved out, `ButtonInputConfig` sheds everything that isn't config data: no more
`HoldTimer`/`RepeatTimer`, `LastOnPressEvent`/`LastOnPressCacheCollection`/`LastOnPressConfigRefs`,
`CanExecute` backchannel, or `CheckAndAdaptForLongButtonRelease`'s in-place mutation. It keeps
`onPress`/`onRelease`/`onLongRelease`/`onHold`, the delay settings, and dispatches on `e.Value` in
`execute()`.

### Stage taxonomy

Three distinct shapes of processing sit in the same funnel, composed in a chain:

- **Inject** (0-to-1, time-triggered): not triggered by any incoming event — triggered by elapsed
  time since one. `HOLD` and its repeat cadence are the only things in this design that are
  actually this shape.
- **Reclassify** (1-to-1, event-triggered): given one incoming event plus a bit of memory, forward
  it unchanged, forward it relabeled, or drop it — nothing fabricated that didn't correspond to a
  real input. RELEASE → LONG_RELEASE promotion is this shape.
- **Aggregate** (N-to-1, event-triggered): given a raw event belonging to a known group, consume it
  and, once the group resolves, emit a differently-typed event instead. Button-pair-to-Encoder
  (below) is this shape.

### No new timer mechanism — ride whichever cadence a device already has

Not every controller gets its button state the same way, so there's no single precedent to copy
uniformly. Two real transports exist side by side under `JoystickManager` alone:

- **DirectInput-polled joysticks**: `JoystickManager.PollTimer` (`JoystickManager.cs:36,46-47`,
  20ms) drives `PollTimer_Tick` (`JoystickManager.cs:106-134`), which loops every joystick it
  manages under one lock and calls `Update()` on each (each wrapped in its own try/catch, so one
  device's failure doesn't block the others in the same tick). One shared tick genuinely serves
  this whole family.
- **HID-push boards** (Bodnar, WinCtrl, WingFlex/Dap500, VKB, Octavi): `Update()` does nothing but
  keep the connection alive (`BodnarBoard.cs:127-134`). Real button events come from
  `HidReportReceiver` (`HidReportReceiver.cs`), which every one of these device classes owns as its
  **own dedicated background thread**, blocking-reading HID reports and raising `OnButtonPressed`
  directly — entirely decoupled from `PollTimer`. Serial boards under `MobiFlightCache` look the
  same shape: events arrive from a callback on their own connection thread, not from any shared
  poll.

So there's no single "the" timer to hang hold/repeat detection off. The right move is to not invent
a new timer mechanism at all, and instead let the generator's periodic check ride whatever cadence
already exists for that specific device's actual transport:

- DirectInput-polled joysticks: piggyback on `JoystickManager.PollTimer_Tick`'s existing loop.
- HID-push boards: piggyback on `HidReportReceiver`'s own read loop, which already wakes up at
  least every `ReadPollTimeoutMilliseconds` (200ms) even with no report to notice its stop flag
  (`HidReportReceiver.cs:23,131-135`) — check on every loop iteration, report or timeout alike.

A `System.Timers.Timer` per device instance would not be expensive if one were needed somewhere —
it's a lightweight registration on the runtime's shared timer queue, not a dedicated OS thread, and
MobiFlight's realistic device counts (tens, not hundreds) make that a non-concern even without the
piggybacking above. But piggybacking means no new timer object is needed at all for either
transport. Isolation between devices sharing one cadence (the DirectInput family) comes from
wrapping each device's own check in its own try/catch inside the loop, the same way
`PollTimer_Tick` already isolates `Update()` failures — not from giving each device a separate
timer. LongRelease doesn't need any of this — it's decided synchronously the moment a real RELEASE
arrives, comparing elapsed time against `LongReleaseDelay`.

This isn't a new concurrency model: `OnButtonPressed` already fires concurrently from independent
threads today (the joystick poll-timer thread; whatever thread `MobiFlightCache`/`MidiBoardManager`
raise events from on serial/MIDI callback), so `ExecutionManager`/`InputEventExecutor` already have
to tolerate it.

### Delay values don't need to align to the tick interval - but RepeatDelay has a floor anyway

Two different questions worth keeping separate, because they sound related but aren't:

- **Does a `HoldDelay`/`RepeatDelay`/`LongReleaseDelay` need to be a multiple of the tick interval
  for detection to work correctly?** No. The check is `now - reference >= TimeSpan.FromMilliseconds(delay)`,
  computed fresh from real timestamps every tick, not an incrementing counter - any positive value
  is handled correctly regardless of alignment. The only consequence of a value that doesn't line up
  with the tick interval is bounded jitter (fires somewhere in `[delay, delay + tickInterval)`,
  never early, never skipped) - the same jitter any polling loop checking "has T elapsed" has,
  imperceptible at the 300ms+ scale these delays actually use. No drift accumulates across repeat
  cycles either, since `LastHoldFire` resets to the real timestamp each time, not a stepped counter.
- **Does `RepeatDelay` need a floor anyway?** Yes, but for a different reason: REPEAT is the only
  one of the three that can fire indefinitely for as long as a button stays held, and every firing
  dispatches to `onHold`, which typically calls into the sim API (FSUIPC/SimConnect/XPlane/a custom
  event). A `RepeatDelay` configured too low - or loaded from an old/hand-edited config predating
  this floor - risks flooding that API fast enough to cause performance issues or instability. This
  is a rate limit for the sim API's sake, not a timer-precision requirement, and it's scoped to
  `RepeatDelay` only: `HoldDelay`/`LongReleaseDelay` each decide at most one classification per
  press/release gesture, so neither carries the same sustained-flooding risk.

  Enforced in one place - `ButtonTimings`'s constructor (`ButtonTimings.MinRepeatDelay`, 200ms) -
  so every path that constructs one (the generator's own fallback, and
  `InputEventExecutor.ResolveButtonTimingsPerConfig`) gets the floor automatically; a positive value
  below it is raised to it, `0` (repeat disabled) is exempt. Not currently enforced at the UI editing
  layer (wherever `HoldDelay`/`RepeatDelay`/`LongReleaseDelay` get set on a `ButtonInputConfig`) -
  someone could still type `10` into the config and it would silently behave as `200` at runtime
  rather than being rejected or corrected at entry time. Worth adding UI-level validation as a
  follow-up so the discrepancy is visible where it's authored, not just where it's enforced.

### Generator's internal state

The generator needs, per physical button (keyed by serial + device name, the same identity
`InputEventExecutor.MatchesControllerAndDeviceName` uses): pressed?, since when, has hold already
fired. That's it for now — not exposed outside the generator, not a public property on the
controller manager, no provider interface. `Joystick` already keeps a live snapshot for its own
diffing purposes (`JoystickState`, `Joystick.cs:39`, refreshed every poll tick to decide what
`OnButtonPressed` events to raise from `UpdateButtons`), so there's precedent for a controller
holding this kind of state — the difference here is scope: keep it private to the generator, as a
small dedicated type rather than loose dictionary/tuple fields, purely so that widening it later
(see the deferred precondition extension above) doesn't mean rebuilding it.

### Encoder-from-button-pairs (joystick-specific)

Some joystick encoders are physically reported as two independent momentary buttons (an INC pulse,
a DEC pulse) rather than a native encoder device. `VKBEncoder`
(`Joysticks/VKB/VKBEncoder.cs`) already does the mirror transformation for one manufacturer — VKB
reports a real rotary encoder as a wrapping counter, and `VKBEncoder.Update()` synthesizes virtual
button `PRESS` events out of the delta. The button-pair case runs the other direction: two button
devices in, one `Encoder` device out.

This is an **aggregate** stage, joystick-specific, composed inside `Joystick`/`JoystickManager`
only (boards and MIDI controllers don't have this hardware quirk). It has to run **before** the
Hold/LongRelease chain in the same manager's funnel: once two buttons are claimed as an encoder
pairing, they must stop surfacing as ordinary bindable/holdable buttons — otherwise a config could
bind `onHold` to what's really half an encoder pulse. The Hold/LongRelease chain only ever sees
`Button`-typed events that survive aggregation.

Fast-vs-normal turning ("5 consecutive left turns in under 1s" → `LEFT_FAST`) is a **reclassify**
stage layered after aggregation, operating on the already-produced `Encoder` events — not an inject
stage, since nothing is fabricated that didn't correspond to a real turn; the already-typed `LEFT`
event just gets relabeled before being forwarded. It needs a short rolling count/window per
direction, not a single last-timestamp the way Hold's threshold check does — a related but not
identical shape of timing-based state.

Open question, not yet resolved: where the button-index-to-encoder pairing is authored. VKB gets it
for free from its own per-manufacturer definition scheme (`VKBEncoder.CreateDevices`,
`VKBEncoder.cs:50-68`); a generalized version needs an equivalent place to declare it per joystick
definition.

## Consequences / open questions to resolve before implementation

- **Delay ownership — resolved.** Initially assumed delay could become a property of the physical
  input/controller, shared across every binding on it. That's wrong: a multi-mode panel (a mode
  switch's precondition determines which config is active for a given physical button) can
  legitimately want different `HoldDelay`/`RepeatDelay`/`LongReleaseDelay` for what is physically
  one button, depending on which config currently governs it. Detection still lives on the
  controller (physical-button-scoped, one canonical timer per button), but the delay *values* it
  uses are resolved from whichever config is currently active with satisfied preconditions -
  `InputEventExecutor.ResolveButtonTimingsPerConfig` does this lookup (reusing the same `GetMatchingInputConfigs`/
  `CheckPreconditions` the normal pipeline already has), `ExecutionManager` composes one resolver
  across every loaded config file and hands it to each controller manager's generator
  (`SyntheticButtonEventGenerator.ResolveTimings`). Resolved once at PRESS time and held for that
  press's lifecycle - not re-resolved every tick, so a mode switch mid-hold doesn't retroactively
  change an in-progress press's timing. If no config currently claims the button, the generator
  falls back to its own fixed defaults and still detects - it fires unconditionally either way,
  same as a real PRESS on an unbound button; the *existing* `Active`/`CheckPreconditions` gating in
  `Execute()` (unchanged) is what actually decides whether anything executes. This means
  preconditions get evaluated twice, but for different questions at different times, not
  redundantly: once at PRESS (which delays govern this press, decided once), and again at every
  fire (should this execute right now, using live state - deliberately not cached, mirroring how
  `ButtonInputConfig`'s old `CanExecute` closure worked before this redesign).

  **Two configs simultaneously active on the same physical button, with different delays, is fully
  supported, not just tolerated.** An earlier version of this resolved to a single "winning" config
  and broadcast the resulting event to every matching config regardless - which was a real
  correctness bug, not just an ambiguity: a config whose own (longer) delay hadn't elapsed yet would
  still get triggered early, by a different config's (shorter) one, the moment that one fired.
  `InputEventExecutor.ResolveButtonTimingsPerConfig` returns *every* currently active,
  precondition-satisfied config bound to a button, each with its own delays - `ExecutionManager`'s
  resolver concatenates these across every loaded config file, no single-winner tie-break. Each
  synthetic HOLD/REPEAT/LONG_RELEASE `SyntheticButtonEventGenerator` produces is stamped with the
  specific config's GUID that governs it (`InputEventArgs.TargetConfigGUID`), and `Execute()` only
  dispatches a targeted event to that one config, skipping every other match - a one-line addition
  to the existing loop. Real events (PRESS/RELEASE with no reclassification) stay untargeted
  (`null`) and broadcast to every matching config, same as always - only the events whose *timing*
  came from one specific config's delay are restricted to that config.

  This does widen `SyntheticButtonEventGenerator`'s internal state: a tracked button now holds one
  `HoldFired`/`LastHoldFire`/delay-set per bound config (not one canonical set for the whole
  button), and a RELEASE can fan out into multiple events - one per bound config, each
  independently classified as RELEASE or LONG_RELEASE against *that* config's own
  `LongReleaseDelay`, so two configs on one button can correctly disagree about whether a given
  release counts as long. `Observe()`'s signature changed accordingly, from returning one
  `InputEventArgs` to a `List<InputEventArgs>` (usually one element - the fan-out is dormant, not
  extra work, for the common single-config-per-button case).
- **Encoder pairing configuration.** Where the button-pair-to-encoder mapping is authored (joystick
  definition file vs. runtime user configuration).
- **Fast-turn threshold shape.** Rolling window/count vs. some other rate measure; not yet designed
  in detail.

## Testing

Once detection has an owner that isn't a bare `System.Timers.Timer` + `DateTime.Now`, inject the
clock (`Func<DateTime> Now`) so tests can fake elapsed time instead of sleeping or reflecting into
private fields. Add coverage for: hold firing once at `HoldDelay` then repeating at `RepeatDelay`;
the long-release boundary (just under vs. just over `LongReleaseDelay`); a config being deactivated
mid-hold actually stopping execution, not just stopping the timer; an exception in `onHold`
surfacing the same `cfg.Status[Device]` error a failing `onPress` would; the encoder fast-turn
threshold.
