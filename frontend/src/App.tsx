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
import { useRecentProjects, useSettingsStore } from "./stores/settingsStore"
import { useControllerDefinitionsStore } from "./stores/definitionStore"
import {
  BoardDefinitions,
  ConnectedControllers,
  ExecutionState,
  HubHopState,
  JoystickDefinitions,
  MidiControllerDefinitions,
  OverlayState,
  RecentProjects,
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

import testProject from "@/../tests/data/project.testdata.json" with { type: "json" }
import testJsDefinition from "@/../tests/data/joystick.definition.json" with { type: "json" }
import testMidiDefinition from "@/../tests/data/midicontroller.definition.json" with { type: "json" }
import testRecentProjects from "@/../tests/data/recentProjects.testdata.json" with { type: "json" }
import testControllers from "@/../tests/data/connectedControllers.testdata.json" with { type: "json" }

import {
  MidiControllerDefinition,
  JoystickDefinition,
} from "@/types/definitions"
import DebugInfo from "@/components/DebugInfo"
import { useExecutionStateStore } from "@/stores/executionStateStore"
import { ProjectInfo } from "@/types/project"
import { useControllerStore } from "@/stores/controllerStore"

function App() {
  const [queryParameters] = useSearchParams()
  const navigate = useNavigate()
  const { project, setProject, setHasChanged } = useProjectStore()
  const { setRecentProjects } = useRecentProjects()
  const { setSettings } = useSettingsStore()
  const { setControllers } = useControllerStore()
  const {
    setBoardDefinitions,
    setJoystickDefinitions,
    setMidiControllerDefinitions,
  } = useControllerDefinitionsStore()

  const { setIsRunning, setIsTesting } = useExecutionStateStore()

  const setHubHopState = useHubHopStateActions()
  useKeyAccelerators(GlobalKeyAccelerators, true)
  const outlet = useOutlet()
  const [overlayVisible, setOverlayVisible] = useState(false)
  const { theme } = useTheme()

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

  useAppMessage("RecentProjects", (message) => {
    const recentProjects = message.payload as RecentProjects
    setRecentProjects(recentProjects.Projects)
    console.log("List of Recent Projects received", recentProjects.Projects)
  })

  useAppMessage("Settings", (message) => {
    const settings = message.payload as Settings
    console.log("Settings message received", settings)
    setSettings(settings)

    const language = settings.Language.split("-")[0]
    if (!_.isEmpty(language)) i18next.changeLanguage(settings.Language)
    else i18next.changeLanguage()
  })

  useAppMessage("BoardDefinitions", (message) => {
    const boardDefinitions = message.payload as BoardDefinitions
    console.log(
      "BoardDefinitions message received",
      boardDefinitions.Definitions,
    )
    setBoardDefinitions(boardDefinitions.Definitions)
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

  useAppMessage("ConnectedControllers", (message) => {
    const controllers = (message.payload as ConnectedControllers).Controllers
    setControllers(controllers)
  })

  useEffect(() => {
    if (startupProgress.Value == 100 && location.pathname == "/index.html") {
      console.log("Finished loading, navigating to home")
      navigate("/home")
    }
  }, [startupProgress.Value, navigate])

  // this is only for easier UI testing
  // while developing the UI
  useEffect(() => {
    if (
      process.env.NODE_ENV === "development" &&
      queryParameters.get("testdata") === "true" &&
      !project // Only if no project loaded yet
    ) {
      setProject(testProject as Project)
      setRecentProjects(testRecentProjects as ProjectInfo[])
      setJoystickDefinitions([testJsDefinition as JoystickDefinition])

      setMidiControllerDefinitions([
        testMidiDefinition as MidiControllerDefinition,
      ])

      setControllers(testControllers as ConnectedControllers["Controllers"])
    }
  }, [
    project,
    queryParameters,
    setJoystickDefinitions,
    setMidiControllerDefinitions,
    setProject,
    setRecentProjects,
    setControllers,
  ])

  useAppMessage("ExecutionState", (message) => {
    console.log("ExecutionState message received", message.payload)
    const { IsRunning, IsTesting } = message.payload as ExecutionState
    setIsRunning(IsRunning)
    setIsTesting(IsTesting)
  })

  return (
    <>
      {overlayVisible && (
        <LoaderOverlay open={overlayVisible} onOpenChange={setOverlayVisible} />
      )}
      {outlet ? (
        <div className="flex h-svh flex-row overflow-hidden p-0 select-none">
          {/* <Sidebar /> */}
          <div className="flex grow flex-col">
            <MainMenu />

            {/* Uncomment the Navbar if needed */}
            {/* <Navbar /> */}
            <div className="flex grow flex-col overflow-hidden">
              <Outlet />
            </div>
            <DebugInfo />
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
