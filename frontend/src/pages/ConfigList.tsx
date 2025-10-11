// import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { columns } from "@/components/tables/config-item-table/config-item-table-columns"
import { useCallback, useEffect } from "react"
import { useAppMessage } from "@/lib/hooks/appMessage"
import {
  ConfigValueFullUpdate,
  ConfigValuePartialUpdate,
  ConfigValueRawAndFinalUpdate,
} from "@/types/messages"
import testProject from "@/../tests/data/project.testdata.json" with { type: "json" }
import testJsDefinition from "@/../tests/data/joystick.definition.json" with { type: "json" }
import testMidiDefinition from "@/../tests/data/midicontroller.definition.json" with { type: "json" }
import { IConfigItem, Project } from "@/types"
import { useSearchParams } from "react-router"
import { useProjectStore } from "@/stores/projectStore"
import { useControllerDefinitionsStore } from "@/stores/definitionStore"
import {
  JoystickDefinition,
  MidiControllerDefinition,
} from "@/types/definitions"
import { ConfigItemDragProvider } from "@/components/providers/DragDropProvider"
import ProjectPanel from "@/components/project/ProjectPanel"
import { ConfigItemTable } from "@/components/tables/config-item-table/config-item-table"

const ConfigListPage = () => {
  const [queryParameters] = useSearchParams()

  const {
    project,
    activeConfigFileIndex,
    setActiveConfigFileIndex,
    setProject,
    setConfigItems,
    updateConfigItem,
    updateConfigItems,
  } = useProjectStore()

  const { setJoystickDefinitions, setMidiControllerDefinitions } =
    useControllerDefinitionsStore()

  useAppMessage("ConfigValuePartialUpdate", (message) => {
    console.log("ConfigValuePartialUpdate", message.payload)
    const update = message.payload as ConfigValuePartialUpdate
    // better performance for single updates
    if (update.ConfigItems.length === 1) {
      updateConfigItem(activeConfigFileIndex, update.ConfigItems[0], true)
      return
    }
    setConfigItems(activeConfigFileIndex, update.ConfigItems)
  })

  useAppMessage("ConfigValueRawAndFinalUpdate", (message) => {
    console.log(
      "ConfigValueRawAndFinalUpdate",
      message.payload as ConfigValueRawAndFinalUpdate,
    )
    const update = message.payload as ConfigValueRawAndFinalUpdate
    // update raw and final values for the store items
    const newItems = update.ConfigItems.map((newItem) => {
      const configItems =
        project?.ConfigFiles[activeConfigFileIndex].ConfigItems ?? []

      const item = configItems.find((i) => i.GUID === newItem.GUID)
      if (item === undefined) return newItem

      return {
        ...item,
        RawValue: newItem.RawValue,
        Value: newItem.Value,
        Status: newItem.Status,
      }
    }) as IConfigItem[]
    updateConfigItems(activeConfigFileIndex, newItems)
  })

  useAppMessage("ConfigValueFullUpdate", (message) => {
    console.log("ConfigValueFullUpdate", message)
    const update = message.payload as ConfigValueFullUpdate
    setConfigItems(update.ConfigIndex, update.ConfigItems)
  })

  // this is only for easier UI testing
  // while developing the UI
  useEffect(() => {  
    if (
      process.env.NODE_ENV === "development" &&
      queryParameters.get("testdata") === "true" &&
      !project // Only if no project loaded yet
    ) {
      setProject(testProject as Project)
      setJoystickDefinitions([testJsDefinition as JoystickDefinition])

      setMidiControllerDefinitions([
        testMidiDefinition as MidiControllerDefinition,
      ])
    }
  })

  const configItems =
    project?.ConfigFiles[activeConfigFileIndex]?.ConfigItems ?? []

  // Function to get config items from project store
  const getConfigItems = useCallback((configIndex: number): IConfigItem[] => {
    return project?.ConfigFiles[configIndex]?.ConfigItems ?? []
  }, [project])

  return (
    <div className="flex flex-col gap-4 overflow-y-auto">
      <ConfigItemDragProvider
        initialConfigIndex={activeConfigFileIndex}
        updateConfigItems={setConfigItems}
        getConfigItems={getConfigItems}
        selectActiveFile={setActiveConfigFileIndex}
      >
        <ProjectPanel />
      <div className="flex flex-col gap-4 overflow-y-auto">
        <ConfigItemTable
          columns={columns}
          data={configItems}
        />
      </div>
      </ConfigItemDragProvider>
    </div>
  )
}

export default ConfigListPage
