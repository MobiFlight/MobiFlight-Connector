# WinCtrl CDU scripts

These scripts mirror the simulated aircraft CDU screen to a physical CDU panel. Right now the only supported hardware is the WinCTRL CDU, but it would be interesting to support also other vendors and community built hardware.

**Note:** For historical reasons the name "winwing" is used the codebase, but this refers to WinCtrl.

When MobiFlight detects a WinCTRL CDU device, it reads the name of the currently loaded aircraft, and checks if it is one of those listed in [`ScriptMappings.json`](ScriptMappings.json). This file maps aircraft name to a python script that interfaces with the aircraft in question: The display contents are fetched from the simulated aircraft, using whatever programming API the aircraft or simulator vendor has made available. The data is then formatted and sent to a specific websocket API on MobiFlight to be displayed on the screen.

```json
{
  "VendorId": "0x4098",
  "ProductIds": [ "WinwingCDUs" ],
  "AircraftMatchPattern": "pmdg 737",
  "ScriptName": "pmdg_737_winwing_cdu.py"
}
```

 The current aircraft string is checked against `AircraftMatchPattern`, and if a match is found, the corresponding script will be run. The matching pattern can contain plain substrings and regular expressions.
 - On MSFS, the `MatchPattern` is compared with the path component of the current aircraft's `aircraft.cfg`
-  on X-Plane it is the content of the aircraft name dataref `sim/aircraft/view/acf_ui_name`
  
The match patterns should be defined so, that only one script will match the aircraft in use. If your script supports several aircraft types or variants, you can also have multiple entries for it.

## MobiFlight WebSocket API

MobiFlight runs a WebSocket server on `ws://localhost:8320`. The endpoints are:

| Endpoint | Device |
| --- | --- |
| `ws://localhost:8320/winwing/cdu-captain` | Captain-side CDU |
| `ws://localhost:8320/winwing/cdu-co-pilot` | First officer CDU |
| `ws://localhost:8320/winwing/cdu-observer` | Observer CDU |

Currently te Websocket API only supports the WinCTRL CDU screen.

Each script connects to MobiFlight's local WebSocket server and sends JSON messages describing what to show on the CDU screen.

### Display message

Send this to update the screen:

```json
{
  "Target": "Display",
  "Data": [ [], ["A", "w", 0], ["B", "g", 1], [] ]
}
```

`Data` is a flat array of exactly `rows × columns` cells (14 rows × 24 columns = **336 cells**), in row-major order (left to right, top to bottom). Each cell is either:

- `[]` — empty / space
- `["char", "color", size]` — a visible character

#### Color codes

| Code | Color |
| --- | --- |
| `"w"` | White |
| `"g"` | Green |
| `"a"` | Amber |
| `"o"` | Blue |
| `"c"` | Cyan |
| `"m"` | Magenta |
| `"r"` | Red |
| `"y"` | Yellow |
| `"e"` | Grey |
| `"k"` | Khaki |

#### Size values

| Value | Size |
| --- | --- |
| `0` | Large |
| `1` | Small |

### Font message

Send this to switch the font used to render text on the CDU screen:

```json
{
  "Target": "Font",
  "Data": "AirbusThales"
}
```

Available font names (matching the `.dat` files in [`Winwing/Fonts/`](Fonts) folder.

- `AirbusThales`
- `Boeing`
- `Collins`

## Running scripts

MobiFlight ships a bundled Python runtime with all required packages pre-installed (`websockets`, `SimConnect`, etc.) — there is nothing extra to install on the host machine.

The runtime is shipped as `Python\RuntimeAndPackages.n.n.n.zip` inside the MobiFlight installation folder and is extracted automatically on first launch. Once extracted, the Python executable is at:

```text
C:\Users\<username>\AppData\Local\MobiFlight\MobiFlight Connector\Python\n.n.n\python.exe
```
The n.n.n represents the version number currently installed with MobiFlight.

The WebSocket server on port 8320 is only started by MobiFlight when a Winwing CDU device is physically detected — without hardware connected, the port is not open and scripts will get a connection refused error.

To run a script manually for testing (with MobiFlight running and a CDU device connected), open PowerShell and run:

```powershell
cd "$env:LOCALAPPDATA\MobiFlight\MobiFlight Connector"
.\Python\3.14.2\python.exe Scripts\Winwing\your_script.py
```
In case 3.14.2 does not work, see if any later version is being bundled with MobiFlight.

Set `LOGLEVEL=DEBUG` in the environment to get verbose output — MobiFlight sets this automatically when launching scripts based on its own log level setting.

## Adding support for a new aircraft

**Write the script** — add a `.py` file to this directory. Use an existing script for a similar aircraft as a starting point. The python script needs to read the MCDU screen contents from the aircraft via some access method provided by the airplane developer, and should then translate it to be displayed on the CDU screen.

**Let others know what you are working on** by opening a thread on #development in Discord, so that  many people don't accidentally work on the same feature, unaware of each other.

**Register the mapping** — add an entry to [`Scripts/ScriptMappings.json`](../ScriptMappings.json)


Once your script is working, **open a pull request** to add it to MobiFlight — include the aircraft add-on name and version you tested against in the PR description. If you need help, let us know in the discussion thread!
