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
  real input. **Revised from the original design:** RELEASE → LONG_RELEASE promotion turned out not
  to fit this shape at the generator level after all — see "LONG_RELEASE is a dispatch-time decision,
  not a generator reclassification" below. What *does* still fit here is nothing generator-side for
  buttons; the encoder fast-turn relabeling further down is the real example of this shape.
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
- **Should `RepeatDelay` have a floor?** Conceptually yes - REPEAT is the only one of the three that
  can fire indefinitely for as long as a button stays held, and every firing dispatches to `onHold`,
  which typically calls into the sim API (FSUIPC/SimConnect/XPlane/a custom event); a value
  configured too low risks flooding that API. But this is **not enforced anywhere in the runtime
  evaluation path** (`ButtonTimings`'s constructor stores `RepeatDelay` exactly as given - no
  clamping). Three reasons: the tick interval itself already imposes a real mechanical floor on how
  fast REPEAT can actually fire, regardless of what's configured; a silent runtime clamp only on the
  *scheduling* side would silently diverge from the raw value a config matches its own events against
  (`ButtonInputConfig.MatchesSyntheticDelay`, see below) unless clamped identically on both sides;
  and configs already exist in the field with sub-floor values authored before this floor was even a
  concept - silently changing their behavior on migration to this pipeline isn't backward compatible.
  `ButtonTimings.MinRepeatDelay`/`ClampRepeatDelay` still exist as a ready-to-use utility, intended
  for config-authoring-time validation (the UI, on save) in a follow-up change - only the UI can tell
  a user *why* a value is rejected, which a silent runtime rewrite can't.

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
  input/controller, shared across every binding on it. That's wrong: a multi-mode panel (several
  mutually-exclusive precondition-gated configs on one physical button) can legitimately want
  different `HoldDelay`/`RepeatDelay` per config (`LongReleaseDelay` isn't resolved here at all
  anymore - see "LONG_RELEASE is a dispatch-time decision" below). Detection still lives on the
  controller (physical-button-scoped, one canonical timer per button), but the `HoldDelay`/
  `RepeatDelay` values it uses are resolved per config bound to that button -
  `InputEventExecutor.ResolveButtonTimingsPerConfig` does this lookup (reusing the same
  `GetMatchingInputConfigs` the normal pipeline already has - not `Active`/`CheckPreconditions`,
  see below), `ExecutionManager` composes one resolver across every loaded config file and hands it
  to each controller manager's generator (`SyntheticButtonEventGenerator.ResolveTimings`). Resolved
  once at PRESS time and held for that press's lifecycle - not re-resolved every tick, so a mode
  switch mid-hold doesn't retroactively change an in-progress press's timing.
  If `ResolveTimings` is unwired, or wired but returns an empty list (no config bound to this
  device/button wants `onHold` *or* `onLongRelease`), the generator doesn't track the press at all -
  PRESS/RELEASE pass through untouched, exactly as without this feature, and RELEASE carries no
  `HeldDurationMs`.
  `Active`/`CheckPreconditions` gating happens exclusively in `Execute()`, not here - resolving a
  binding only answers "is a config bound to this button and what are its delays," never "should it
  currently run." A button's press-time binding set is therefore a purely static, device/button-only
  question: every bound config gets one, active or not, precondition-satisfied or not, and it's
  `Execute()` - evaluating live state at each actual fire - that decides which one (if any) actually
  runs.

  **Two configs simultaneously bound to the same physical button, with different HOLD delays, is
  fully supported, not just tolerated.** An earlier version of this resolved to a single "winning"
  config and broadcast the resulting event to every matching config regardless - which was a real
  correctness bug, not just an ambiguity: a config whose own (longer) delay hadn't elapsed yet would
  still get triggered early, by a different config's (shorter) one, the moment that one fired.
  `InputEventExecutor.ResolveButtonTimingsPerConfig` returns the *distinct* `HoldDelay`/`RepeatDelay`
  settings among every config bound to a button (`.Distinct()` on the `ButtonTimings` struct) -
  `ExecutionManager`'s resolver concatenates these across every loaded config file and dedupes again
  at that level.

  **No config identity is carried through the generator at all** - `SyntheticButtonEventGenerator`
  only ever sees delay *values*, never a config GUID (there is no `ButtonBinding` type; `ResolveTimings`
  is `Func<InputEventArgs, List<ButtonTimings>>`). A produced HOLD/REPEAT just carries the delay
  value that classified it (`InputEventArgs.SyntheticDelayMs`) - `Execute()` asks each bound config
  directly, "is this delay yours?" (`ButtonInputConfig.MatchesSyntheticDelay`, requiring `onHold` to
  be defined before comparing `HoldDelay`/`RepeatDelay` against `SyntheticDelayMs` for a HOLD/REPEAT -
  a config without `onHold` never matches one, regardless of what its unused `HoldDelay`/`RepeatDelay`
  fields happen to hold), true for anything else (PRESS/RELEASE carry no `SyntheticDelayMs`, so those
  broadcast to every matching config, same as always). This is simpler
  than ID-based targeting and self-correcting: two configs that happen to share a delay are
  indistinguishable to the generator by construction, so they're naturally treated as one group
  everywhere - including in the log, which would otherwise show the same physical HOLD/REPEAT once
  per bound config instead of once per distinct setting.

  **REPEAT matches on the (HoldDelay, RepeatDelay) pair, not RepeatDelay alone.** Two configs on one
  button can share a `RepeatDelay` while differing in `HoldDelay` (e.g. a 300ms-hold and a 1000ms-hold
  binding both repeating every 200ms). `Tick()` tracked due repeats keyed on `RepeatDelay` alone, so
  once the 300ms binding started repeating, its REPEAT events - stamped only with `RepeatDelay` -
  would also match the 1000ms config, even though that config's own `HoldDelay` hadn't elapsed yet.
  Fixed by keying `Tick()`'s due-repeat set on the full `ButtonTimings` pair, and stamping the raised
  REPEAT with both `RepeatDelay` (`SyntheticDelayMs`, as before) and the originating binding's
  `HoldDelay` (`InputEventArgs.SyntheticHoldDelayMs`, new) - `MatchesSyntheticDelay`'s REPEAT case now
  requires both to match. HOLD never had this problem - it already matches on its own unambiguous
  `HoldDelay` alone.

  This does widen `SyntheticButtonEventGenerator`'s internal state: a tracked button now holds one
  `HoldFired`/`LastHoldFire`/timings entry per distinct setting (not one canonical set for the whole
  button).

  **`onHold`'s HoldDelay/RepeatDelay is only contributed if `onHold` is actually defined** -
  `ResolveButtonTimingsPerConfig` substitutes `ButtonTimings.NoHold` (`int.MaxValue`, never elapses)
  when `onHold` is null. Otherwise a config's own always-present but unused default `HoldDelay`
  (every `ButtonInputConfig` has *some* value in that field whether or not `onHold` is set) would
  keep HOLD/REPEAT alive for the whole button for as long as *any* config remains bound to it -
  including, confusingly, after the one config that actually wanted them is deleted, as long as some
  other onRelease/onPress-only config happens to still be there. A config with only `onLongRelease`
  (no `onHold`) still needs an entry here despite contributing nothing real to HOLD/REPEAT scheduling
  - it's what keeps the button *tracked at all*, which is what makes `HeldDurationMs` available on
  RELEASE (below). Filtering such a config out entirely, rather than including it with the `NoHold`
  sentinel, was tried and was a real bug: it silently broke LONG_RELEASE for buttons with an
  `onLongRelease`-only config and no `onHold` anywhere.

  **LONG_RELEASE is a dispatch-time decision, not a generator reclassification.** The original design
  for this feature (and an intermediate revision of it) had `SyntheticButtonEventGenerator` decide
  RELEASE-vs-LONG_RELEASE itself, grouping by distinct `LongReleaseDelay` among bound configs the same
  way HOLD groups by `HoldDelay`. That shipped a real, if subtle, log-duplication bug: a button bound
  to one config with a real `LongReleaseDelay` (has `onLongRelease`) and another without one (gets a
  disabling sentinel, since without `onLongRelease` a config's `LongReleaseDelay` is exactly as inert
  as `HoldDelay` is to a config without `onHold`) produces *two distinct* delay values - so a quick
  tap, even though it resolves to the same "plain RELEASE" outcome for both, got logged twice, because
  the generator was reasoning about *settings*, not *outcomes*. RELEASE isn't like HOLD/REPEAT: it's a
  real, physical hardware transition (a press actually ended), not something manufactured out of
  elapsed time with no wire-level counterpart. `SyntheticButtonEventGenerator.Observe()` now always
  raises RELEASE exactly once, as plain `RELEASE`, carrying how long the press lasted
  (`InputEventArgs.HeldDurationMs`) - full stop, no grouping, no per-binding fan-out. Each config
  independently decides, at dispatch time, whether *its own* `LongReleaseDelay` was exceeded and it
  has an `onLongRelease` to hand off to (`ButtonInputConfig.ResolveDispatchedEvent`); if not, `RELEASE`
  dispatches to `onRelease` exactly as it always has, regardless of how long the button was held. Two
  configs on one button can still correctly disagree about whether a given release counts as long -
  that disagreement just lives entirely in `ResolveDispatchedEvent`, not in what gets raised or logged
  at the device level. `Observe()`'s signature is still `List<InputEventArgs>` (from the earlier HOLD/
  REPEAT grouping work), but for RELEASE it's now always exactly one element.

  **Stage 1 shows only physical events; stage 2 always logs, with the delay that produced a
  synthetic one.** The two log stages settled on different jobs: stage 1
  (`ExecutionManager.mobiFlightCache_OnButtonPressed`) answers "was an event raised at all," so it
  only ever logs a real PRESS/RELEASE - `isSyntheticEvent` (`e.SyntheticDelayMs.HasValue`) gates the
  line, meaning HOLD/REPEAT never appear there (LONG_RELEASE already couldn't, since RELEASE is the
  only thing `Observe()` ever raises for a release). Stage 2
  (`InputEventExecutor.Execute()`'s `"Executing ..."` line) answers "did this config actually fire,"
  so it logs uniformly for every event type, physical or synthetic, whenever a config has a matching
  action - and for a synthetic one it also names the delay that produced it, since stage 1 no longer
  will: `AppendSyntheticDelay` appends `:{ms}ms` to the label - `SyntheticDelayMs` (the configured
  HoldDelay/RepeatDelay that fired) for HOLD/REPEAT, and the config's own `LongReleaseDelay` for
  LONG_RELEASE. Both are the *configured setting*, not `HeldDurationMs` (the actual time held) - the
  log answers "what threshold triggered this," and showing the exact elapsed duration for LONG_RELEASE
  (a first attempt did) is misleading there, since it varies release to release and isn't what the
  config is checked against for repeatability. `cfg.RawValue` stays the bare event name in both cases -
  the delay suffix is a log-only detail, not part of the value shown/stored elsewhere.

  This resolved-per-config label is computed once per config, at the top of `Execute()`'s loop, and
  reused for every skip reason below it too ("MobiFlight not running", "Skipping inactive config",
  "Preconditions not satisfied") as well as the final "Executing" line - not just one generic label
  for the whole physical event. Two configs bound to the same button and RELEASE can otherwise
  disagree (one resolves to RELEASE, one to LONG_RELEASE, per "LONG_RELEASE is a dispatch-time
  decision" above), so a single shared label computed before the loop would silently show one
  config's outcome on the other's log line - the "MobiFlight not running" skip originally worked this
  way (computed once, before the loop, from the raw event only) and had exactly that bug.

  **A skip reason only logs for a config that actually has an action bound to the resolved event.**
  All three skip reasons ("MobiFlight not running", "Skipping inactive config", "Preconditions not
  satisfied") are gated on `hasMatchingAction` (`cfg.GetInputAction(e) != null`), checked once per
  config alongside the label resolution above. Without this, e.g. an onPress-only config logged
  "Skipping ... MobiFlight not running" on every RELEASE too - RELEASE was never going to do anything
  for that config, so the line was pure noise. This mirrors "Executing," which likewise only logs -
  and only actually dispatches to the config's action - when `hasMatchingAction` is true: a skip and
  an execute are the same event, just with a different outcome, so they share the same gate.

  **`cfg.RawValue` follows a wider rule than the skip/execute gate above, though: a physical
  PRESS/RELEASE always updates it, matching action or not - only a synthetic HOLD/REPEAT needs one.**
  RawValue is the "last event seen" indicator a user reads off the UI, and a real PRESS/RELEASE is
  genuine hardware state worth showing even on a config with nothing bound to it (e.g. an onPress-only
  config seeing a RELEASE) - it confirms the physical input was received at all. A synthetic HOLD/
  REPEAT has no such standing on its own: without `onHold`, `MatchesSyntheticDelay` already excludes
  the config entirely (above), and even a broadcast that slipped through must not show HOLD on a
  config that never reacts to it - that was the actual "HOLD shows in RawValue for a config with no
  onHold" bug this rule fixes. Concretely: `Execute()`'s top-of-loop gate is
  `if (!hasMatchingAction && isSyntheticEvent) continue;` (skips entirely only for an unmatched
  synthetic event); `cfg.RawValue`/`cfg.Value`/`updatedValues` are set right after the isStarted/
  Active/Preconditions checks pass, unconditionally; the "Executing" log and the actual
  Modifiers/`cfg.execute(...)` dispatch remain gated on `hasMatchingAction` specifically, one level
  further in. So a physical event with no matching action updates RawValue but logs nothing and
  executes nothing; a config that fails isStarted/Active/Preconditions still gets neither, same as
  before this whole rule existed.

  **Modifiers and the action see the dispatched value, not the raw one.** The Modifiers
  pipeline used to seed itself from `e.Value` directly - correct when `dispatchedValue == value`, but
  wrong for a config that resolved to LONG_RELEASE, which fed the pipeline the raw RELEASE(1) instead
  of LONG_RELEASE(2). `Execute()` now resolves `dispatchedNumericValue` alongside the label (the cast
  `dispatchedValue` for a button, `e.Value` unchanged for encoder/analog) and seeds Modifiers from
  that instead. `e.Value` is written back with the Modifiers result afterward as before, so
  `cfg.execute(...)`/`ButtonInputConfig.execute()`'s own dispatch re-check sees the already-resolved
  value too.
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
