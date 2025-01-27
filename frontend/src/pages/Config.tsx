import { useConfigStore } from '@/stores/configFileStore';
// import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { DataTable } from '@/components/tables/config-item-table/config-item-table';
import { columns } from '@/components/tables/config-item-table/config-item-table-columns';
import { useEffect } from 'react';
import { useAppMessage } from '@/lib/hooks/appMessage';
import { ConfigValueUpdate } from '@/types/messages';
import testdata from './config/testdata.json';

const ConfigPage = () => {
  const { items: configItems, setItems, updateItems, updateItem } = useConfigStore()

  useAppMessage("ConfigValueUpdate", (message) => {
    const update = message.payload as ConfigValueUpdate

    // better performance for single updates
    if (update.ConfigItems.length === 1) {
      updateItem(update.ConfigItems[0])
      return
    }

    updateItems(update.ConfigItems)
  })

  useEffect(() => {
    if (configItems.length === 0) {
      if (process.env.NODE_ENV === 'development') {
        setItems(testdata)
      }
    }
  }, [configItems, setItems])

  return (
    // <div className='flex flex-col gap-4 overflow-y-auto'>
    //   <Tabs defaultValue="config-1" className='grow flex flex-col overflow-y-auto'>
    //     <div>
    //       <TabsList className='mb-4 mt-0'>
    //         <TabsTrigger value="config-1">Config one</TabsTrigger>
    //       </TabsList>
    //     </div>
    //     <TabsContent value="config-1" className='mt-0 flex flex-col grow overflow-y-auto'>
          <DataTable columns={columns} data={configItems} />
    //     </TabsContent>
    //   </Tabs>
    // </div>
  )
}

export default ConfigPage