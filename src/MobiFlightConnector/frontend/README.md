# MobiFlight Architecture

MobiFlight is a hybrid app:

- **C# backend** handles flight sim integration, hardware communication, and the execution engine
- **Frontend** provides the configuration UI. The frontend is a React + TypeScript app using Vite, Tailwind, and shadcn/ui. It runs inside a WebView2 control embedded in the C# backend window.
- Frontend and backend communicate through a JSON message bridge — the React side calls `PostMessage` and C# handles it via handlers in `BrowserMessages`.

The main backend subsystems are:

- **Execution engine** - reads variables from the flight simulator on a polling timer and applies user-defined mappings to drive output devices
- **Flight sim connectors** - separate integrations for MSFS (SimConnect), FSX/P3D (FSUIPC), and X-Plane
- **Hardware device drivers** - Arduino-based MobiFlight boards, Arcaze USB, MIDI boards, USB HID game controllers, and others
- **Browser message layer** - the IPC bridge between C# and the React UI

---

## Development Workflow

In development you run two things side by side:

- The frontend dev server works conveniently in [Visual Studio Code](https://code.visualstudio.com/), in the provided dev container
- The C# backend in [Visual Studio 2026](https://visualstudio.microsoft.com/vs/community/). When you start the backend with the **Debug** build target it connects to the frontend dev server at `localhost:5173` and loads it automatically.

### Dev container setup

The easiest way to get started is with the included dev container — dependencies and Playwright browsers for unit tests are installed automatically on first startup.

1. [Follow the instructions](https://code.visualstudio.com/docs/devcontainers/tutorial) to install Docker and the Dev Containers extension in Visual Studio Code.
2. Select **Open Folder..** in VS Code, navigate to `src/MobiFlightConnector/frontend/`
3. When prompted by VSCode, select **Reopen in Container**

![Screenshot of the Reopen in Container dialog from VSCode](docs/images/reopen-in-container.png)

The first time takes a few minutes to configure while the necessary images are downloaded and dependencies install. After the container opens VSCode may warning about an auto-configured task to run. Accept the task, and the frontend will start.

The frontend is served at `http://localhost:5173`.

To manually start the frontend, use the command palette (CTRL+SHIFT+P) to select the **Tasks: Run Task** command, then run **Start frontend**.

### C#.NET Dev Environment (Backend)

The backend is a C#/.NET desktop application. and it must be running for full functionality. Once your devcontainer has finished starting up, and the frontend is available at `localhost:5173`, open the backend solution in Visual Studio 2026: `MobiFlightConnector.sln`.

Make sure that the correct project is selected as the startup project inside the solution. The backend project to run is **MobiFlightConnector**.

Before running or publishing the backend, install the .NET 10 SDK separately.
Installing Visual Studio 2026 including .NET installation inside its workload tab may not be sufficient. If the .NET 10 SDK is missing, the application may build or start incorrectly and runtime errors can occur, for example errors related to loading `SimConnect.dll`.

After the correct startup project is selected, run the project with the **Debug** build target.
The backend will connect to the frontend dev server at `localhost:5173` and load it automatically.

#### Publishing the backend

To publish the MobiFlight Connector backend, run the following command from the repository root on the terminal:
```sh
dotnet publish "src/MobiFlightConnector/MobiFlightConnector.csproj" -c "Release" -p:Version="0.0.0.1" -o "dist/MobiFlightConnector-0.0.0.1" --self-contained
```

## Translations (i18n)

Translation files are in `public/locales/{lang}/translation.json`. The app uses `react-i18next` — all user-facing strings must go through `t()`. Core languages that must be complete before a PR can be merged: `en`, `de`, `es`.

```sh
npm run check:i18n                       # run both checks below
npm run check:translations               # missing keys per language
npm run check:translations -- fi         # missing keys for a specific language
npm run check:hardcoded-strings          # components with hardcoded strings
npm run check:hardcoded-strings:verbose
```

Adding a new locale folder under `public/locales/{lang}/` is enough for the React UI itself — i18next discovers and loads it automatically. However, language switching still happens in the legacy WinForms Settings dialog, not in the React UI. To make a new language selectable, add it to `InitializeLanguageComboBox()` in `src/MobiFlightConnector/UI/Panels/Settings/GeneralPanel.cs` (outside `frontend/`).

## Testing

See [tests/README.md](tests/README.md).

## Legacy features

The old WinForms dialogs are legacy. All new UI work goes into the React frontend; the goal is to migrate everything over time. If you are adding a feature that touches the UI, build it in the frontend rather than WinForms. If unsure, discuss your ideas first on Discord #development channel.
