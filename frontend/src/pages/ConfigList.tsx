import { useConfigStore } from "@/stores/configFileStore"
// import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { ConfigItemTable } from "@/components/tables/config-item-table/config-item-table"
import { columns } from "@/components/tables/config-item-table/config-item-table-columns"
import { useEffect } from "react"
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
import ProjectPanel from "@/components/project/ProjectPanel"
import { useProjectStore } from "@/stores/projectStore"
import { useControllerDefinitionsStore } from "@/stores/definitionStore"
import { JoystickDefinition, MidiControllerDefinition } from "@/types/definitions"

const ConfigListPage = () => {
  const [queryParameters] = useSearchParams()

  const {
    items: configItems,
    setItems,
    updateItem,
    updateItems,
  } = useConfigStore()

  const {
    setProject,
    setConfigItems
  } = useProjectStore()

  const {
    setJoystickDefinitions,
    setMidiControllerDefinitions
  } = useControllerDefinitionsStore()

  const mySetItems = (items: IConfigItem[]) => {
    setItems(items)
  }

  useAppMessage("ConfigValuePartialUpdate", (message) => {
    console.log("ConfigValuePartialUpdate", message.payload)
    const update = message.payload as ConfigValuePartialUpdate
    // better performance for single updates
    if (update.ConfigItems.length === 1) {
      updateItem(update.ConfigItems[0], true)
      return
    }
    setItems(update.ConfigItems)
  })

  useAppMessage("ConfigValueRawAndFinalUpdate", (message) => {
    console.log(
      "ConfigValueRawAndFinalUpdate",
      message.payload as ConfigValueRawAndFinalUpdate,
    )
    const update = message.payload as ConfigValueRawAndFinalUpdate
    // update raw and final values for the store items
    const newItems = update.ConfigItems.map((newItem) => {
      const item = configItems.find((i) => i.GUID === newItem.GUID)
      if (item === undefined) return newItem

      return {
        ...item,
        RawValue: newItem.RawValue,
        Value: newItem.Value,
        Status: newItem.Status,
      }
    }) as IConfigItem[]
    updateItems(newItems)
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
      configItems.length === 0 &&
      queryParameters.get("testdata") === "true"
    ) {
      setProject(testProject as Project)
      setJoystickDefinitions(
        [testJsDefinition as JoystickDefinition])

      setMidiControllerDefinitions(
        [testMidiDefinition as MidiControllerDefinition]
      )
    }
  })

  return (
    <div className="flex flex-col gap-4 overflow-y-auto">
      <ProjectPanel />
      {
        <div className="flex flex-col gap-4 overflow-y-auto">
          <ConfigItemTable
            columns={columns}
            data={configItems}
            setItems={mySetItems}
          />
        </div>
      }
    </div>
  )
}

export default ConfigListPage
