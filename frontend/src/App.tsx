import { Outlet, useNavigate, useOutlet, useSearchParams } from "react-router"
import StartupProgress from "./components/StartupProgress"
import { useEffect, useState } from "react"
import { useAppMessage } from "./lib/hooks/appMessage"
import { Project, StatusBarUpdate } from "./types"
import i18next from "i18next"
import Settings from "./types/settings"
import _ from "lodash"
import { useProjectStore } from "./stores/projectStore"
import { MainMenu } from "./components/MainMenu"
import { useSettingsStore } from "./stores/settingsStore"
import { useControllerDefinitionsStore } from "./stores/definitionStore"
import {
  HubHopState,
  JoystickDefinitions,
  MidiControllerDefinitions,
  OverlayState,
} from "./types/messages"
import {
  useKeyAccelerators,
  GlobalKeyAccelerators,
} from "./lib/hooks/useKeyAccelerators"
import LoaderOverlay from "./components/tables/config-item-table/LoaderOverlay"
import { Toaster } from "./components/ui/sonner"
import { useTheme } from "@/lib/hooks/useTheme"
import { ToastNotificationHandler } from "./components/notifications/ToastNotificationHandler"
import { useHubHopStateActions } from "./stores/stateStore"

function App() {
  const [queryParameters] = useSearchParams()
  const navigate = useNavigate()
  const { setProject, setHasChanged } = useProjectStore()
  const { setSettings } = useSettingsStore()
  const { setJoystickDefinitions, setMidiControllerDefinitions } =
    useControllerDefinitionsStore()

  const setHubHopState = useHubHopStateActions()
  useKeyAccelerators(GlobalKeyAccelerators, true)
  const outlet = useOutlet()
  const [overlayVisible, setOverlayVisible] = useState(false)
  const { theme } = useTheme()
  const windowSize = { x: window.innerWidth, y: window.innerHeight }

  // State for startup progress from app messages
  const [appStartupProgress, setAppStartupProgress] = useState<StatusBarUpdate>(
    {
      Value: 0,
      Text: "Starting...",
    },
  )

  const queryProgressValue = Number.parseInt(
    queryParameters.get("progress")?.toString() ?? "0",
  )

  const startupProgress =
    queryProgressValue > 0
      ? {
          Value: queryProgressValue,
          Text:
            queryProgressValue === 100 ? "Loading complete..." : "Loading...",
        }
      : appStartupProgress

  useAppMessage("StatusBarUpdate", (message) => {
    setAppStartupProgress(message.payload as StatusBarUpdate)
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
    console.log(
      "JoystickDefinitions message received",
      joystickDefinitions.Definitions,
    )
    setJoystickDefinitions(joystickDefinitions.Definitions)
  })

  useAppMessage("MidiControllerDefinitions", (message) => {
    const definitions = message.payload as MidiControllerDefinitions
    console.log(
      "MidiControllerDefinitions message received",
      definitions.Definitions,
    )
    setMidiControllerDefinitions(definitions.Definitions)
  })

  useAppMessage("ProjectStatus", (message) => {
    const projectStatus = message.payload as { HasChanged: boolean }
    console.log("ProjectStatus message received", projectStatus)
    setHasChanged(projectStatus.HasChanged)
  })

  useAppMessage("OverlayState", (message) => {
    const overlayState = message.payload as OverlayState
    console.log("OverlayState message received", overlayState)
    setOverlayVisible(overlayState.Visible)
  })

  useAppMessage("HubHopState", (message) => {
    const state = message.payload as HubHopState
    setHubHopState(state)
  })

  useEffect(() => {
    if (startupProgress.Value == 100) {
      console.log("Finished loading, navigating to config page")
      navigate("/config")
    }
  }, [startupProgress.Value, navigate])

  return (
    <>
      {overlayVisible && (
        <LoaderOverlay open={overlayVisible} onOpenChange={setOverlayVisible} />
      )}
      {outlet ? (
        <div className="flex h-svh flex-row p-0 select-none">
          {/* <Sidebar /> */}
          <div className="flex grow flex-col">
            <MainMenu />

            {/* Uncomment the Navbar if needed */}
            {/* <Navbar /> */}
            <div className="flex grow flex-col overflow-hidden p-2">
              <Outlet />
            </div>
            <div className="flex flex-row justify-end gap-2 px-5">
              <div className="text-muted-foreground text-xs">
                {windowSize.x}x{windowSize.y}
              </div>
              <div className="text-muted-foreground text-xs">
                MobiFlight 2025
              </div>
              <div className="text-muted-foreground text-xs">Version 1.0.0</div>
            </div>
          </div>
        </div>
      ) : (
        <StartupProgress
          value={startupProgress.Value}
          text={startupProgress.Text}
        />
      )}
      <ToastNotificationHandler />
      <Toaster
        expand
        visibleToasts={4}
        toastOptions={{ duration: 10000 }}
        position="bottom-right"
        theme={theme}
        className="flex w-full justify-center ![--width:540px]"
      />
    </>
  )
}

export default App
