import { Outlet, useNavigate, useOutlet, useSearchParams } from "react-router"
import StartupProgress from "./components/StartupProgress"
import { useEffect, useState } from "react"
import { useAppMessage } from "./lib/hooks/appMessage"
import { ConfigLoadedEvent, Project, StatusBarUpdate } from "./types"
import { useConfigStore } from "./stores/configFileStore"
import i18next from "i18next"
import Settings from "./types/settings"
import _ from "lodash"
import { useProjectStore } from "./stores/projectStore"
import { MainMenu } from "./components/MainMenu"
import { useSettingsStore } from "./stores/settingsStore"
import { useControllerDefinitionsStore } from "./stores/definitionStore"
import { JoystickDefinitions, MidiControllerDefinitions } from "./types/messages"

function App() {
  const [queryParameters] = useSearchParams()
  const navigate = useNavigate()
  const { setItems } = useConfigStore()
  const { setProject, setHasChanged } = useProjectStore()
  const { setSettings } = useSettingsStore()
  const { setJoystickDefinitions, setMidiControllerDefinitions } = useControllerDefinitionsStore()

  const [startupProgress, setStartupProgress] = useState<StatusBarUpdate>({
    Value: 0,
    Text: "Starting...",
  })

  useAppMessage("StatusBarUpdate", (message) => {
    setStartupProgress(message.payload as StatusBarUpdate)
  })

  useAppMessage("ConfigFile", (message) => {
    console.log("ConfigFile message received", message.payload)
    setItems((message.payload as ConfigLoadedEvent).ConfigItems)
  })

  useAppMessage("Project", (message) => {
    const project = message.payload as Project
    console.log("Project message received", project)
    setProject(project)
  })

  useAppMessage("Settings", (message) => {
    const settings = message.payload as Settings
    console.log("Settings message received", settings)
    setSettings(settings)

    const language = settings.Language.split("-")[0]
    if (!_.isEmpty(language)) i18next.changeLanguage(settings.Language)
    else i18next.changeLanguage()
  })

  useAppMessage("JoystickDefinitions", (message) => {
    const joystickDefinitions = message.payload as JoystickDefinitions
    console.log("JoystickDefinitions message received", joystickDefinitions.Definitions)
    setJoystickDefinitions(joystickDefinitions.Definitions)
  })

  useAppMessage("MidiControllerDefinitions", (message) => {
    const definitions = message.payload as MidiControllerDefinitions
    console.log("MidiControllerDefinitions message received", definitions.Definitions)
    setMidiControllerDefinitions(definitions.Definitions)
  })

  useAppMessage("ProjectStatus", (message) => {
    const projectStatus = message.payload as { HasChanged: boolean }
    console.log("ProjectStatus message received", projectStatus)
    setHasChanged(projectStatus.HasChanged)
  })

  // this allows to get beyond the startup screen
  // by setting the progress to 100 via url parameter
  useEffect(() => {
    // convert string to number
    const value = Number.parseInt(
      queryParameters.get("progress")?.toString() ?? "0",
    )
    if (value == 100) {
      console.log("Finished loading, navigating to config page")
      navigate("/config")
    } else setStartupProgress({ Value: value, Text: "Loading..." })
  }, [navigate, queryParameters])

  useEffect(() => {
    if (startupProgress.Value == 100) {
      console.log("Finished loading, navigating to config page")
      navigate("/config")
    }
  }, [startupProgress.Value, navigate])

  const outlet = useOutlet()

  const windowSize = { x: window.innerWidth, y: window.innerHeight }

  return (
    <>
      {outlet ? (
        <div className="flex h-svh select-none flex-row p-0">
          {/* <Sidebar /> */}
          <div className="flex grow flex-col">
            <MainMenu />

            {/* Uncomment the Navbar if needed */}
            {/* <Navbar /> */}
            <div className="flex grow flex-col overflow-hidden p-2">
              <Outlet />
            </div>
            <div className="flex flex-row justify-end gap-2 px-5">
              <div className="text-xs text-muted-foreground">
                {windowSize.x}x{windowSize.y}
              </div>
              <div className="text-xs text-muted-foreground">
                MobiFlight 2025
              </div>
              <div className="text-xs text-muted-foreground">Version 1.0.0</div>
            </div>
          </div>
          {/* <Toaster
          position="bottom-right"
          offset={48}
        /> */}
        </div>
      ) : (
        <StartupProgress
          value={startupProgress.Value}
          text={startupProgress.Text}
        />
      )}
    </>
  )
}

export default App
