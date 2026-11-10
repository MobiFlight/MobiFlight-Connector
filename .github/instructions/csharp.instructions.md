---
description: 'Guidelines for building C# applications in MobiFlight'
applyTo: '**/*.cs'
---

# C# Development for MobiFlight

## Language and Platform
- The solution targets .NET 10 (`net10.0-windows`, x86); use C# 14 features.
- The UI is moving to the WebView2-based React frontend; WinForms is legacy and being phased out, so build new UI in the frontend.
- Prefer modern syntax in new and touched code: collection expressions (`[]`, `[.. a, .. b]`), switch expressions, pattern matching, primary constructors, records for immutable data.
- Do not rewrite untouched code just to modernize its syntax.
- Nullable reference types are enabled only in some projects (e.g. Moza, WwFcu); follow each project's `.csproj` setting.
- Follow the naming and formatting of the surrounding file (PascalCase, `_camelCase` private fields, `I` interface prefix, K&R braces, 4 spaces).

## General Instructions
- Make only high confidence suggestions when reviewing code changes.
- Keep comments short, one line where possible; comment why, not what.
- Public APIs get a brief XML `<summary>`; add `<param>`, `<returns>` and `<exception>` only when not obvious.
- Reference GitHub issues in comments where relevant (e.g., `// Issue 1423: Handle...`).
- Handle edge cases and write clear exception handling.
- Log errors and important events using `Log.Instance.log()` with `LogSeverity.Error`, `Info` or `Debug`.
- Never silently swallow exceptions without logging.

## Code Structure
- Return early to reduce nesting; use private helper methods to break down complex logic.
- Extract complex conditions in if statements (and loops) into variables with descriptive names.
- Use regions sparingly in production code, only for very large classes (tests group with `#region`).
- Use `EventHandler` or `EventHandler<T>` with `OnEvent?.Invoke(this, args)`, and unsubscribe in disposal methods.

## Null Handling
- Follow the surrounding file for null checks; prefer `is null` / `is not null` in new code.
- Use `ArgumentNullException.ThrowIfNull` for public method arguments.
- Use `string.IsNullOrEmpty()` or `string.IsNullOrWhiteSpace()` for string validation.

## Threading and UI Interactions
- Check `InvokeRequired` before updating UI elements from background threads, and marshal with `Invoke()` or `BeginInvoke()`.
- Use `Task.Run()` for long-running operations to avoid blocking the UI.
- Use `ConfigureAwait(false)` for async operations that don't need UI context.
- Avoid `async void` except for event handlers.
- `MessageExchange.Subscribe<T>` runs the callback on the delivering thread. Use `SubscribeOnUiThread<T>` when the handler touches WinForms or shared state such as `ExecutionManager.ConfigItems` (see `docs/architecture/frontend-backend-messaging.md`).
- Use a dedicated `Lock` field for synchronous mutual exclusion:
  `private readonly Lock _gate = new();` then `lock (_gate) { ... }`.
- Never lock on `this`, `typeof(...)`, strings, or any publicly reachable object.
- Use Concurrent collections (`System.Collections.Concurrent`) instead of locked `Dictionary`/`List`

## Serial Communication and Device Management
- Always dispose of communication resources properly (serial ports, connections).
- Handle device connect/disconnect events gracefully and log all device state changes.
- Use timeouts for communication commands to prevent hanging.

## Project-Specific Patterns
- Use `ExecutionManager` as the central coordination point for application state.
- Use `MessageExchange.Instance` for publishing messages to the frontend.
- When adding a frontend message or command, update both sides: the C# types and handlers in `MobiFlight/BrowserMessages` and the TypeScript types in `types/messages.d.ts` / `types/commands.d.ts`.
- Use `Properties.Settings.Default` for persistent configuration.
- Use `AppTelemetry.Instance` for tracking user actions (when enabled).
- Follow the existing cache pattern for flight sim connections (e.g., `SimConnectCache`, `Fsuipc2Cache`).

## Configuration and Serialization
- Use JSON (Newtonsoft.Json) for configuration file formats, and validate data after deserialization.
- A change to a persisted format needs a migration so existing user files keep working; see `Base/Migration/` for examples.
- Provide meaningful error messages for configuration errors, and notify the user when a migration runs.

## Internationalization
- Do not hardcode user-facing strings; they belong in the frontend translations (`t()`), not in C# resource files.

## Testing
- Tests use MSTest and Moq; write unit tests for business logic, utility methods and public class methods.
- Follow existing test naming conventions (e.g., `MethodName_ShouldBehavior_WhenCondition`).
- Test both success and error paths, plus edge cases like null inputs, empty collections and boundary values.
- Use `[DataRow]` attributes for parameterized tests.
- Use the specific Assert methods (`Assert.IsGreaterThan`, `Assert.IsNull`, ...), never `IsTrue`/`IsFalse`/`AreEqual` wrapping a comparison (MSTEST0037). Common cases: `Assert.HasCount(n, collection)` instead of `Assert.AreEqual(n, collection.Count)`; `Assert.IsEmpty(collection)` instead of `Assert.AreEqual(0, collection.Count)`; `Assert.Contains(item, collection)` instead of `Assert.IsTrue(collection.Contains(item))`.
- Test layout and structure rules are in `copilot-instructions.md`; how to run the tests is in `src/MobiFlightConnector/frontend/README.md`.
