import { useConfigStore } from "@/stores/configFileStore"
import { useProjectStore } from "@/stores/projectStore"
// import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { ConfigItemTable } from "@/components/tables/config-item-table/config-item-table"
import { columns } from "@/components/tables/config-item-table/config-item-table-columns"
import { useCallback, useEffect, useState } from "react"
import { publishOnMessageExchange, useAppMessage } from "@/lib/hooks/appMessage"
import {
  ConfigValueFullUpdate,
  ConfigValuePartialUpdate,
  ConfigValueRawAndFinalUpdate,
} from "@/types/messages"
import testdata from "@/../tests/data/configlist.testdata.json" with { type: "json" }
import { IConfigItem } from "@/types"
import { Button } from "@/components/ui/button"

const ConfigListPage = () => {
  const {
    items: configItems,
    setItems,
    updateItem,
    updateItems,
  } = useConfigStore()

  const mySetItems = useCallback(
    (items: IConfigItem[]) => {
      setItems(items)
    },
    [setItems],
  )

  const { project } = useProjectStore()
  const [ activeConfigFile, setActiveConfigFile ] = useState(0)

  useEffect(() => {
    if (project === null) return
    if (project!.ConfigFiles.length === 0) return

    setItems(project?.ConfigFiles[activeConfigFile].ConfigItems)
  }, [project, activeConfigFile, setItems])

  const configFiles = project?.ConfigFiles

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
    setItems(update.ConfigItems)
  })

  // this is only for easier UI testing
  // while developing the UI
  useEffect(() => {
    if (process.env.NODE_ENV === "development" && configItems.length === 0) {
      setItems(testdata)
    }
  }, [setItems])

  const selectActiveFile = (index:number) => {
    setActiveConfigFile(index)
  }

  useEffect(() => {
    publishOnMessageExchange().publish({
      key: "CommandActiveConfigFile",
      payload:{
        index: activeConfigFile,
      }}
    )
  },[activeConfigFile])

  return (
    <div className="flex flex-col gap-4 overflow-y-auto">
        <div>
          <div className="flex flex-row gap-2">
            {configFiles?.map((file, index) => {
              return (
                <Button variant={index === activeConfigFile ? "default" : "outline"} key={file.FileName} value={file.FileName} className="h-8" onClick={() => selectActiveFile(index)}> 
                  {file.Label ?? file.FileName}
                </Button>
              )
            })}
            <Button variant="ghost" className="h-8">Add new config</Button>
          </div>
        </div>
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
