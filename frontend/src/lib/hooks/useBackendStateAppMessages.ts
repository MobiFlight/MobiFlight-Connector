import { useEffect } from "react"
import i18next from "i18next"
import { useAppMessage } from "@/lib/hooks/appMessage"
import { useProjectStore } from "@/stores/projectStore"
import { Project } from "@/types"
import {
  AuthenticationStatus,
  BoardDefinitions,
  ConnectedControllers,
  ControllerBindingsUpdate,
  ExecutionState,
  HubHopState,
  JoystickDefinitions,
  MidiControllerDefinitions,
  ProjectStatus,
  RecentProjects,
} from "@/types/messages"
import Settings from "@/types/settings"
import { useRecentProjects, useSettingsStore } from "@/stores/settingsStore"
import { useControllerStore } from "@/stores/controllerStore"
import { useControllerDefinitionsStore } from "@/stores/definitionStore"
import { useHubHopStateActions } from "@/stores/stateStore"
import { useExecutionStateStore } from "@/stores/executionStateStore"
import { useAuth } from "react-oidc-context"
import {
  JoystickDefinition,
  MidiControllerDefinition,
} from "@/types/definitions"
import { ProjectInfo } from "@/types/project"
import { useSearchParams } from "react-router"
import _ from "lodash"
import { Controller } from "@/types/controller"

export const useBackendStateAppMessages = () => {
  const [queryParameters] = useSearchParams()

  const { project, setProject, setProjectStatus, setControllerBindings } = useProjectStore()
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
  const auth = useAuth()

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
    const projectStatus = message.payload as ProjectStatus
    console.log("ProjectStatus message received", projectStatus)
    setProjectStatus(projectStatus)
  })

  useAppMessage("HubHopState", (message) => {
    const state = message.payload as HubHopState
    setHubHopState(state)
  })

  useAppMessage("ConnectedControllers", (message) => {
    const controllers = (message.payload as ConnectedControllers).Controllers
    setControllers(controllers)
  })

  // Listen for auth state changes from C#
  useAppMessage("AuthenticationStatus", async (message) => {
    const authStatus = message.payload as AuthenticationStatus
    console.log("AuthenticationStatus message received", authStatus)
    try {
      // Trigger silent signin to sync auth state from localStorage
      if (authStatus.Authenticated) {
        await auth.signinSilent()
      } else {
        auth.removeUser() // Clear user to ensure state is updated based on signinSilent result
      }
    } catch (err) {
      console.log("Auth sync after window close:", err)
    }
  })

  useAppMessage("ExecutionState", (message) => {
    console.log("ExecutionState message received", message.payload)
    const { IsRunning, IsTesting } = message.payload as ExecutionState
    setIsRunning(IsRunning)
    setIsTesting(IsTesting)
  })

  useAppMessage("ControllerBindingsUpdate", (message) => {
    const controllerBindings = message.payload as ControllerBindingsUpdate
    console.log("ControllerBindingsUpdate message received", controllerBindings.Bindings)
    setControllerBindings(controllerBindings.Bindings)
  })

  // this is only for easier UI testing
  // while developing the UI
  useEffect(() => {
    if (
      process.env.NODE_ENV === "development" &&
      queryParameters.get("testdata") === "true" &&
      !project // Only if no project loaded yet
    ) {
      ;(async () => {
        const testProject = (
          await import("@/../tests/data/project.testdata.json", {
            assert: { type: "json" },
          })
        ).default as Project
        const testJsDefinition = (
          await import("@/../tests/data/joystick.definition.json", {
            assert: { type: "json" },
          })
        ).default as JoystickDefinition
        const testMidiDefinition = (
          await import("@/../tests/data/midicontroller.definition.json", {
            assert: { type: "json" },
          })
        ).default as MidiControllerDefinition
        const testRecentProjects = (
          await import("@/../tests/data/recentProjects.testdata.json", {
            assert: { type: "json" },
          })
        ).default as ProjectInfo[]
        const testControllers = (
          await import("@/../tests/data/connectedControllers.testdata.json", {
            assert: { type: "json" },
          })
        ).default as Controller[]

        setProject(testProject)
        setRecentProjects(testRecentProjects)
        setJoystickDefinitions([testJsDefinition])
        setMidiControllerDefinitions([testMidiDefinition])
        setControllers(testControllers)
      })()
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
}
