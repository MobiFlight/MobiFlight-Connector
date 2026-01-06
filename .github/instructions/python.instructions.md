---
description: 'Python coding conventions and guidelines for MobiFlight CDU integration scripts'
applyTo: '**/*.py'
---

# Python Coding Conventions

## Overview

Python scripts in MobiFlight are primarily used for CDU (Control Display Unit) integration with flight simulators (X-Plane, MSFS). These scripts handle real-time websocket communication between simulator datarefs and WinWing hardware devices.

## Python Instructions

- Write a module-level docstring explaining the script's purpose, supported aircraft/simulators, and architectural approach.
- Ensure functions have descriptive names and include type hints.
- Use descriptive function names that indicate their purpose (e.g., `handle_dataref_updates`, `generate_display_json`).
- Add inline comments for complex logic, character mappings, or non-obvious transformations.
- Function-level docstrings are optional but encouraged for public/complex functions.

## General Instructions

- Always prioritize readability and clarity.
- For algorithm-related code, include explanations of the approach used.
- Write code with good maintainability practices, including comments on why certain design decisions were made.
- Handle edge cases and write clear exception handling with appropriate logging.
- For libraries or external dependencies, mention their usage and purpose in comments.
- Use consistent naming conventions and follow the patterns established in existing scripts.
- Write concise, efficient, and idiomatic code that is also easily understandable.

## Code Style and Formatting

- Follow **PEP 8** style guide for Python with the following project-specific adaptations:
  - Line length may exceed 79 characters for readability (up to ~120 characters is acceptable).
  - Use 4 spaces for indentation.
- Group imports in this order:
  1. Standard library imports
  2. Third-party imports (e.g., `websockets`, `asyncio`)
  3. Local/project imports
- Use blank lines to separate logical sections of code.
- Constants should be defined at module level using `UPPER_SNAKE_CASE`.

## Async Patterns

MobiFlight scripts heavily use asynchronous programming. Follow these patterns:

```python
async def main():
    """Entry point for async operations."""
    available_devices = await get_available_devices()
    
    tasks = []
    for device in available_devices:
        queue = asyncio.Queue()
        tasks.append(asyncio.create_task(handle_dataref_updates(queue, device)))
        tasks.append(asyncio.create_task(handle_device_update(queue, device)))
    
    await asyncio.gather(*tasks)


if __name__ == "__main__":
    asyncio.run(main())
```

## Logging

- Use the `logging` module for all diagnostic output.
- Configure logging in the `main()` function or at script entry.
- Use appropriate log levels:
  - `logging.info()` - Connection status, important state changes
  - `logging.warning()` - Recoverable issues, missing optional features
  - `logging.error()` - Errors that affect functionality
  - `logging.debug()` - Detailed diagnostic information

```python
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s'
)
```

## Enums and Constants

Use `StrEnum` and `IntEnum` for device types, colors, and other categorical values:

```python
from enum import StrEnum, IntEnum

class CduDevice(StrEnum):
    Captain = "cdu_0"
    CoPilot = "cdu_1"
    
    def get_endpoint(self) -> str:
        match self:
            case CduDevice.Captain:
                return WS_CAPTAIN
            case CduDevice.CoPilot:
                return WS_CO_PILOT
            case _:
                raise KeyError(f"Invalid device specified {self}")


class MfCharSize(IntEnum):
    Large = 0
    Small = 1
```

## WebSocket Communication

Follow established patterns for websocket reconnection and error handling:

```python
async def handle_device_update(queue: asyncio.Queue, device: CduDevice):
    """Translates and sends dataref updates to MobiFlight."""
    endpoint = device.get_endpoint()
    logging.info("Connecting to CDU device %s", device)
    
    async for websocket in websockets.connect(endpoint):
        logging.info("Connected successfully to CDU device %s", device)
        while True:
            values = await queue.get()
            try:
                display_json = generate_display_json(values)
                await websocket.send(display_json)
            except websockets.exceptions.ConnectionClosed:
                logging.error("WebSocket connection closed... Attempting to reconnect")
                await queue.put(values)  # Re-queue failed message
                break
```

## Character Mapping

Define character and color mappings as module-level dictionaries:

```python
CHAR_MAP = {
    "$": "☐",      # ballot box
    "`": "°",      # degrees
    "←": "\u2190", # left arrow
    "→": "\u2192", # right arrow
}

COLOR_MAP = {
    1: "g",  # green
    2: "c",  # cyan
    4: "e",  # grey
}
```

## Module-Level Docstring Example

```python
"""
Adds support for the [Aircraft Name] in [Simulator]

[Brief description of what the script does]

In order to support multiple CDU devices seamlessly, a dynamic approach is taken 
whereby an enum class is defined that contains the supported devices.
A device is considered "supported" if it exists in the aircraft.

Upon script start, MobiFlight is probed (get_available_devices()) to detect the 
devices connected to the PC. Any device that returns a successful response is then tracked.

Two tasks are started independently for each available CDU device:
1. handle_dataref_updates -> Listens to the simulator for dataref updates
2. handle_device_update   -> Dispatches updates to MobiFlight to update that CDU
"""
```

## Edge Cases and Error Handling

- Handle websocket disconnections gracefully with automatic reconnection.
- Re-queue failed messages to ensure data is eventually delivered.
- Use try/except blocks around external API calls and data parsing.
- Log errors with sufficient context for debugging.

```python
try:
    decoded_value = base64.b64decode(value).decode(errors="ignore").replace("\x00", " ")
except Exception as e:
    logging.error(f"Error decoding value: {e}")
    continue
```

## Testing

- Unit tests for Python scripts are not currently required but are encouraged for complex transformation logic.
- Manual testing with actual hardware is the primary validation method.
- When adding tests, place them in a `tests/` subdirectory within `Scripts/`.

## Available libraries in python runtime

MobiFlight uses a bundled python environment which is limited to the following libraries:
- `websockets` (>=14.0) - WebSocket client/server
- `asyncio` - Async programming
- `json` - JSON serialization
- `logging` - Diagnostic output
- `SimConnect` (>=0.4) - For MSFS integration (optional, per-script)
- `gql` (>=3.5) - For GraphQL-based simulators (optional, per-script)