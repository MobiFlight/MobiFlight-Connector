import { useConfigStore } from "@/stores/configFileStore"
// import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { ConfigItemTable } from "@/components/tables/config-item-table/config-item-table"
import { columns } from "@/components/tables/config-item-table/config-item-table-columns"
import { useCallback, useEffect } from "react"
import { useAppMessage } from "@/lib/hooks/appMessage"
import { ConfigValueUpdate } from "@/types/messages"
import testdata from "@/../tests/data/configlist.testdata.json" with { type: "json" }
import { IConfigItem } from "@/types"

const ConfigListPage = () => {
  const {
    items: configItems,
    setItems,
    updateItem,
    updateItems
  } = useConfigStore()

  const mySetItems = useCallback((items: IConfigItem[]) => {
    setItems(items)
  }, [setItems])
    

  useAppMessage("ConfigValueUpdate", (message) => {
    const update = message.payload as ConfigValueUpdate
    console.log("ConfigValueUpdate", update.ConfigItems)
    // better performance for single updates
    if (update.ConfigItems.length === 1) {
      console.log("updateItem", update.ConfigItems[0])
      updateItem(update.ConfigItems[0], true)
      return
    }
    updateItems(update.ConfigItems)
  })

  // this is only for easier UI testing
  // while developing the UI
  useEffect(() => {
    if (process.env.NODE_ENV === "development" && configItems.length === 0) {
      setItems(testdata)
    }
  },[setItems])

  return (
    // <div className='flex flex-col gap-4 overflow-y-auto'>
    //   <Tabs defaultValue="config-1" className='grow flex flex-col overflow-y-auto'>
    //     <div>
    //       <TabsList className='mb-4 mt-0'>
    //         <TabsTrigger value="config-1">Config one</TabsTrigger>
    //       </TabsList>
    //     </div>
    //     <TabsContent value="config-1" className='mt-0 flex flex-col grow overflow-y-auto'>
    <div className="flex flex-col gap-4 overflow-y-auto">
      <ConfigItemTable columns={columns} data={configItems} setItems={mySetItems}/>
    </div>
    //     </TabsContent>
    //   </Tabs>
    // </div>
  )
}

export default ConfigListPage
