---
description: 'Guidelines for building C# applications in MobiFlight'
applyTo: '**/*.cs'
---

# C# Development for MobiFlight

## C# Version and Language Features
- Target .NET Framework 4.8 as specified by the project.
- Use C# language features compatible with .NET Framework 4.8 (up to C# 7.3).
- Write clear and concise comments for each function explaining purpose and design decisions.

## General Instructions
- Make only high confidence suggestions when reviewing code changes.
- Write code with good maintainability practices, including comments on why certain design decisions were made.
- Handle edge cases and write clear exception handling.
- For libraries or external dependencies, mention their usage and purpose in comments.
- Log errors and important events using `Log.Instance.log()` with appropriate `LogSeverity` levels.

## Naming Conventions
- Follow PascalCase for class names, method names, and public members.
- Use camelCase for private fields and local variables (prefixed with underscore for private fields where appropriate, e.g., `_cmdMessenger`).
- Prefix interface names with "I" (e.g., `IModuleInfo`, `IConfigItem`).
- Use descriptive names that clearly convey intent (e.g., `execManager`, `UpdateStatusBarModuleInformation`).

## Formatting
- Use 4 spaces per tab indentation (as observed in existing code).
- Place opening braces on the same line for class/method definitions (K&R style).
- Use clear whitespace and newlines for readability.
- Group related code with comments explaining the section's purpose.
- Keep lines reasonably short; break complex expressions across multiple lines.

## Code Structure
- Organize classes with clear separation of concerns.
- Use regions sparingly and only for very large classes with distinct sections.
- Group related methods together logically.
- Place event handlers near related functionality.
- Use delegates and events for decoupled communication between components.
- Return early from methods to reduce nesting and improve readability.
- Use private helper methods to break down complex logic.
- Use speaking variables names for test expressions instead of complex inline expressions.

## Null Handling
- Check for null values explicitly using `== null` or `!= null` (project uses this convention).
- Use `String.IsNullOrEmpty()` or `String.IsNullOrWhiteSpace()` for string validation.
- Return early from methods when null checks fail to reduce nesting.
- Document null behavior in method comments where relevant.

## Error Handling and Logging
- Use try-catch blocks to handle expected exceptions gracefully.
- Log errors using `Log.Instance.log()` with `LogSeverity.Error`.
- Log important state changes with `LogSeverity.Info`.
- Log detailed debugging information with `LogSeverity.Debug`.
- Present user-friendly error messages via dialogs when appropriate.
- Never silently swallow exceptions without logging.

## Threading and UI Interactions
- Check `InvokeRequired` before updating UI elements from background threads.
- Use `Invoke()` or `BeginInvoke()` to marshal calls to the UI thread.
- Use `Task.Run()` for long-running operations to avoid blocking the UI.
- Use `ConfigureAwait(false)` for async operations that don't need UI context.
- Be mindful of potential race conditions with shared state.

## Serial Communication and Device Management
- Always dispose of communication resources properly (serial ports, connections).
- Implement proper connection/disconnection sequences with appropriate delays.
- Handle device connect/disconnect events gracefully.
- Log all device state changes for debugging purposes.
- Use timeouts for communication commands to prevent hanging.

## Testing
- Write unit tests for business logic and utility methods and public class methods.
- Use Moq for creating test doubles of interfaces.
- Follow existing test naming conventions (e.g., `MethodName_ShouldBehavior_WhenCondition`).
- Test both success and error paths.
- Test edge cases like null inputs, empty collections, and boundary values.
- Use `[DataRow]` attributes for parameterized tests.
- Use "Arrange", "Act", "Assert" comments to clearly structure test methods.

## Project-Specific Patterns
- Use `ExecutionManager` as the central coordination point for application state.
- Use `MessageExchange.Instance` for publishing messages to the frontend.
- Use `Properties.Settings.Default` for persistent configuration.
- Use `AppTelemetry.Instance` for tracking user actions (when enabled).
- Follow the existing cache pattern for flight sim connections (e.g., `SimConnectCache`, `Fsuipc2Cache`).

## Event Handling
- Use `EventHandler` or `EventHandler<T>` for event declarations.
- Check if event is null before invoking: `OnEvent?.Invoke(this, args)`.
- Unsubscribe from events in disposal methods to prevent memory leaks.
- Use meaningful event argument types that carry necessary data.

## Configuration and Serialization
- Use JSON for configuration file formats.
- Handle configuration migration gracefully with user notifications.
- Validate configuration data after deserialization.
- Provide meaningful error messages for configuration errors.
- Support both embedded and external configuration files.

## Performance Considerations
- Cache frequently accessed values (e.g., `lastValue` dictionary in `MobiFlightModule`).
- Avoid unnecessary UI updates by checking if values have changed.
- Use dictionaries for fast lookups of devices and modules.
- Dispose of resources promptly to free system resources.
- Consider using `StringBuilder` for building large strings.

## Documentation
- Document public APIs with XML doc comments.
- Include `<summary>`, `<param>`, `<returns>`, and `<exception>` tags.
- Explain complex algorithms or business logic with inline comments.
- Document workarounds and technical debt with comments explaining context.
- Reference GitHub issues in comments where relevant (e.g., `// Issue 1423: Handle...`).

## Flight Simulator Integration
- Support multiple flight sim types (MSFS, FSX, P3D, X-Plane, ProSim).
- Handle connection state changes gracefully with proper UI updates.
- Check connection state before attempting sim operations.
- Provide clear user feedback for connection issues.
- Track which configs require which sim connections and update UI accordingly.

## WinForms Specific
- Initialize components in the correct order (language, settings, logging).
- Save and restore window position, size, and state.
- Use `StartPosition.CenterParent` for modal dialogs.
- Handle minimize to tray functionality properly.
- Update UI state consistently when application state changes.

## Version Compatibility
- Since this is a .NET Framework 4.8 project, avoid suggesting .NET Core/.NET 5+ specific features.
- Do not suggest nullable reference types (C# 8.0 feature not fully supported in .NET Framework).
- Do not suggest switch expressions, pattern matching enhancements, or other C# 8.0+ features.
- Use traditional null checks, if/else statements, and switch statements.
- Use `var` judiciously for local variables when type is obvious.