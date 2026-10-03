# Frontend ↔ Backend messaging architecture

Status: **implemented**. The .NET app hosts a native (no third-party dependency) WebSocket
server that the frontend connects to instead of the WebView2 `postMessage` bridge, with
reconnect/resync handled on both sides. The auth WebView is a deliberate exception - it stays on
`postMessage`, see below.

## Problem

All traffic between the WinForms app (.NET, `MobiFlightConnector`) and the embedded React
frontend used to go through WebView2 `postMessage`. Outbound, `PostMessagePublisher` called
`ThreadSafeWebView2.PostWebMessageAsJsonThreadSafe`, which did a **synchronous `Control.Invoke`**
onto the UI thread for *every single message*. Two producers are hot enough for this to matter:

- `ExecutionManager.FrontendUpdateTimer_Execute` (`MobiFlight/ExecutionManager.cs`), a
  `System.Windows.Forms.Timer` on the **UI thread**, ticking every 200 ms, publishing
  `ConfigValueRawAndFinalUpdate` for every changed config value.
- `MessageExchangeAppender.ProcessTimer_Tick` (`Base/LogAppender/MessageExchangeAppender.cs`),
  a `System.Threading.Timer` on a **threadpool thread**, every 100 ms, one `LogEntry` message
  per queued entry.

## Goal

A sustainable, symmetric, non-blocking message architecture:

- The .NET app hosts a lightweight **WebSocket server** in-process, built on the BCL
  (`System.Net.Sockets.TcpListener` + `System.Net.WebSockets.WebSocket.CreateFromStream`) rather
  than a third-party library - no new dependency to keep updated.
- The frontend is **one client**, with the design leaving room for future clients (mobile
  companion apps) without a rework.
- Both sides can **send and receive**, dispatching internally by message `key` exactly as before
  - the change is to the wire, not to the pub/sub model on either side.
- **Non-blocking is the top priority.** Neither the WinForms UI thread nor the React main thread
  may block on transport I/O.

## Decisions made and why

| Decision | Rationale |
|---|---|
| **BCL `TcpListener` + `WebSocket.CreateFromStream`**, not a library | The project is `net10.0-windows`, SDK-style, with central package management - Kestrel and Fleck were both live options. Fleck is unmaintained. Kestrel is Microsoft-maintained but pulls in `Microsoft.AspNetCore.App` (~15-25 MB on the self-contained win-x86 publish) for a WinForms app that needs exactly one thing: a WebSocket endpoint. The BCL option needs ~40 lines of HTTP Upgrade handshake code (the only protocol code this project owns - framing, masking, fragmentation, ping/pong, close are all the framework's, the same implementation Kestrel itself uses) and adds nothing to the dependency list. |
| Origin allow-list on the server | Loopback-only binding is the real security boundary, but a few lines rejecting a browser `Origin` that isn't the app's own keeps an arbitrary local web page from opening a connection and driving the app. A connection with **no** `Origin` header is allowed - that's how a non-browser client (a future mobile companion app) would connect. |
| The auth WebView (`UserAuthenticationWebView`) stays on `postMessage`, not the WebSocket | It navigates to a **foreign origin** (the identity provider) for most of its lifetime, and `AddCloseButtonHandlerOnNavigationCompleted` injects a back/cancel button there whose click handler posts `CommandUserAuthentication{state:'cancelled'}` via `window.chrome.webview.postMessage`. The server's origin allow-list would reject a socket from that foreign origin, and relaxing it would let any page on the IdP's domain drive the app. On our own origin (`/auth` → `AuthCallback`) it only ever *sends* commands - no `useAppMessage` call exists there - so it needs no outbound path either. `FrontendPanel` deliberately never injects `window.__MOBIFLIGHT__.wsUrl` into this WebView; `WebViewMessageReceiver` gives it a receive-only bridge into `MessageExchange.PublishReceivedMessage`. |
| `CompositePublisher` / `PostMessagePublisher` (outbound) removed | With one WebSocket server broadcasting to all connections, there is nothing left to compose - both the main frontend and (had it needed one) any other WebView already receive a `Broadcast`. `PostMessagePublisher`'s inbound half survives as `WebViewMessageReceiver`, used only by the auth WebView. |
| `WebsocketPublisher.cs` (`ClientWebSocket`) deleted | Never used in production. Didn't wrap payloads in the `Message<T>` envelope (no `key` - would have silently broken routing), used `async void`, and had a fixed 4 KB receive buffer with no `EndOfMessage` reassembly loop. Not a template to copy from. |
| Transport indirection on the frontend | Components must only ever see `{key, payload}` messages, never the transport. This is what lets Playwright fixtures keep working via the (still-live) `postMessageTransport`, since the E2E suite runs against the Vite dev server with no .NET backend at all. |
| Backpressure / coalescing | **KISS - not built.** No queueing beyond one unbounded per-connection send channel (needed only because `WebSocket.SendAsync` permits one outstanding call at a time), no per-type coalescing policy, until profiling shows a need. If revisited: coalesce latest-wins value-update types (`ConfigValueRawAndFinalUpdate`, `MobiFlightVariablesUpdate`, `StatusBarUpdate`), never drop reliable/state types (`Project`, `Settings`, `Notification`, `ExecutionState`, ...). |

## Architecture

```
  .NET (WinForms)                                 Browser (WebView2 / dev browser)
  ---------------                                 --------------------------------
  producers ──► MessageExchange.Publish<T>        components ──► publish(CommandMessage)
                      │                                                │
                      ▼                                                ▼
             IMessagePublisher                                   messageClient
        (WebSocketServerPublisher)                     (key→handlers map, single dispatch)
                      │                                                │
                      ▼                                                ▼
               MessageServer (BCL)     ◄══ ws://127.0.0.1:PORT ══►  MessageTransport
             broadcast to all sessions                       (WebSocketTransport | PostMessageTransport)
                      │                                                ▲
                      ▼                                                │
        MessageExchange.PublishReceivedMessage            useAppMessage(key, cb)
        → runs on the calling thread by default;
          SubscribeOnUiThread marshals explicitly
                      │
                      ▼
             Subscribe<T> / SubscribeOnUiThread<T> handlers (WinForms)

  UserAuthenticationWebView (foreign-origin during login) ──postMessage──► WebViewMessageReceiver
                                                                              │
                                                                              ▼
                                                            MessageExchange.PublishReceivedMessage
```

Both directions carry the identical envelope `{ key, payload }`
(`MobiFlight/BrowserMessages/Message.cs`). Each side owns a dispatcher keyed on `key`. The
transport itself is dumb - it only moves strings.

## Threading contract

`MessageExchange.PublishReceivedMessage` used to read `SynchronizationContext.Current` **live,
per incoming message**, falling back to running subscribers **inline** if it was null. That
worked only because WebView2 raised `WebMessageReceived` on the UI thread. A WebSocket delivers on
a threadpool thread instead, where `Current` is null - which would have run every subscriber
inline on the socket thread, a cross-thread WinForms violation for most of `MainForm`'s handlers.

Rather than restore a blanket marshal-to-UI-thread default, the default was **inverted**:

- `MessageExchange.Subscribe<T>(...)` runs the callback on whatever thread delivered the
  message - no marshal, by default.
- `MessageExchange.SubscribeOnUiThread<T>(...)` posts the callback onto the UI
  `SynchronizationContext`, captured once via `SetSynchronizationContext` in
  `MainForm.InitializeMessaging()`.

Every one of the 18 existing subscribers (9 in `MainForm`, 9 in `ExecutionManager`) was marked
`SubscribeOnUiThread` to preserve exact prior behavior, with an explicit reason recorded at each
call site - see `MainForm.InitializeFrontendSubscriptions` and
`ExecutionManager.InitializeFrontendSubscriptions`. Two exceptions stayed on plain `Subscribe`:
`CommandOpenLinkInBrowser` (no WinForms/shared state at all) and `CommandConfigContextMenu` in
`MainForm` (its handler, `OpenOutputConfigWizardForId`, already marshals itself via
`InvokeRequired`/`Invoke`).

**`ExecutionManager`'s 9 are not WinForms-bound** - they're marked `SubscribeOnUiThread` for a
different reason: they mutate `ConfigItems`, a plain `List<IConfigItem>`, while
`FrontendUpdateTimer_Execute` (a `System.Windows.Forms.Timer`, i.e. UI-thread) enumerates it bare
in `UpdateInputPreconditions()` every 200 ms. The UI-thread marshal is today's implicit lock
between the two; removing it without giving `ConfigItems` real synchronization reintroduces an
intermittent `InvalidOperationException: Collection was modified`. Don't unmark these as part of
moving screens to React without addressing `ConfigItems` first - the comments at each site say so.

As more of the UI moves to React, the expectation is that `SubscribeOnUiThread` call sites shrink
one at a time, each removal reviewable on its own, rather than the coupling being an invisible
global default.

## Startup / readiness / resync

`frontendReady` (`MainForm.cs`) is a **one-shot latch** - a second `Ready` message for `/start` is
a no-op for the one-time boot work (`BoardDefinitions.LoadDefinitions`, `InitializeExecutionManager()`,
etc.), which must not repeat. But a WebSocket connection **can drop and reconnect** (backend
restart, dev HMR, sleep/wake, network blip) in a way WebView2 `postMessage` never did, and without
a resync the frontend would stay blank after one.

**Backend**: two distinct methods, not one reused for both cases - a resync frontend isn't in a
fresh-boot state, so replaying the boot sequence (e.g. `StatusBarUpdate{Text:"Starting..."}`) is
wrong on a running app.

- `PublishStartupState()` - `OnFrontendReady`'s original state-push tail, unchanged, called
  **once**, gated by the `frontendReady` latch.
- `PublishFullState()` - a snapshot of current live state for the reconnect case: `Project`,
  `ProjectStatus`, `ExecutionState`, `ConnectedControllers`, `MobiFlightVariablesUpdate`,
  `Settings`/definitions, recent projects. Excludes transient one-off events (`Notification`,
  `OverlayState`, `AuthenticationStatus`) and anything the 200 ms tick already keeps current
  (`ConfigValueRawAndFinalUpdate`).

**Frontend**: `messageClient` subscribes to `transport.onOpen` and re-publishes
`CommandFrontendState{route:"/start", state:"ready"}` on every open **after the first**.
`StartupProgress.tsx` stays the sole first-time trigger - at the very first socket open React
hasn't mounted its handlers yet, so sending Ready from the transport layer that early would push
state into a void.

`MessageExchange.Publish` still has no buffering - anything published before a client is connected
is dropped, same as before. `PublishFullState()` is what covers the reconnect case.

## Message envelope & serialization

`Message<T>` (`MobiFlight/BrowserMessages/Message.cs`): `{ key: string; payload: T }`, key
defaults to `payload.GetType().Name`. Wrapping happens **in the publisher**
(`WebSocketServerPublisher.Publish`), not in `MessageExchange` - serialized with a bare
`JsonConvert.SerializeObject` (no shared `JsonSerializerSettings`; polymorphism is handled by
`[JsonConverter]` attributes on the model types themselves, e.g.
`Incoming/Converter/InputActionConverter.cs`).

Incoming (`MessageExchange.PublishReceivedMessage`, now `public` so `WebViewMessageReceiver` can
be wired to it directly): two-stage deserialize - first as `Message<object>` to read `key`, look
up the registered `Type` for that key, then deserialize `payload` into that concrete type.
Unknown keys log a warning and are dropped.

## Environment / library constraints

- `MobiFlightConnector.csproj` is SDK-style (`Microsoft.NET.Sdk`), `net10.0-windows`, central
  package management via `Directory.Packages.props`. No `Microsoft.NET.Sdk.Web`, no
  `FrameworkReference` - the transport needs neither.
- Production serves the frontend from `https://mobiflight.app`, intercepted locally by
  `StaticPageWebResourceRequestHandler` (not a real network request) - Debug serves it live from
  `http://localhost:5173`. `ws://127.0.0.1` from an `https://` page is a *potentially
  trustworthy* origin under the mixed-content spec and Chromium permits it.
- No firewall/port-in-use handling exists for other listeners in the codebase (the
  `websocket-sharp`-based `WebSocketServer` on port 8320 in `JoystickManager.cs`, serving the
  WinCtrl CDU displays - a separate subsystem, unaffected by this work - has none either).
  `MessageServer` binds loopback-only (sidesteps the firewall prompt) and falls back to an
  ephemeral port if the configured one (`Properties.Settings.Default.FrontendWebSocketPort`,
  default `8321`) is taken, logging the actual bound port/URL rather than leaving the app running
  with no publisher.

## Deliberately deferred - do not build yet

- Backpressure / per-type coalescing of high-frequency updates.
- Frontend `requestAnimationFrame`-batching of incoming updates into `configItemStateStore`.
- Per-session subscription filtering - needed once a mobile client wants a subset of messages
  rather than the full broadcast.
- LAN binding, auth/token handshake for non-loopback clients.
- Migrating the WinCtrl CDU server off `websocket-sharp` onto the native transport.
- Reworking the Playwright fixture (`frontend/tests/fixtures/MobiFlightPage.ts`) onto the
  transport abstraction - it still mocks `window.chrome.webview` directly, which works because
  `postMessageTransport` is exactly that path.
