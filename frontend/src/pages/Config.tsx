import { useConfigStore } from '@/stores/configFileStore';
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { DataTable } from '@/components/tables/config-item-table/config-item-table';
import { columns } from '@/components/tables/config-item-table/config-item-table-columns';
import { useEffect } from 'react';

const ConfigPage = () => {
  const { items: configItems, setItems } = useConfigStore()

  useEffect(() => {
    if (configItems.length === 0) {
      if (process.env.NODE_ENV === 'development') {
        setItems([
          {
            GUID: 'test-guid-0',
            Active: true,
            Description: 'Landing Lights',
            Device: 'Test device',
            Component: 'Test component',
            Type:"Output",
            Tags: ['Test tag'],
            Status: ['Test status'],
            RawValue: '0',
            ModifiedValue: '0',
          },
          {
            GUID: 'test-guid-1',
            Active: true,
            Description: 'RPM Gauge',
            Device: 'Test device',
            Component: 'Test component',
            Type: 'Stepper',
            Tags: ['Test tag'],
            Status: ['Test status'],
            RawValue: '1500',
            ModifiedValue: '15000',
          },
        ])
      }
    }
  }, [configItems, setItems])

  return (
    <div className='flex flex-col gap-4 overflow-y-auto'>
      <Tabs defaultValue="config-1" className='grow flex flex-col overflow-y-auto'>
        <div>
          <TabsList className='mb-4 mt-0'>
            <TabsTrigger value="config-1">Config one</TabsTrigger>
          </TabsList>
        </div>
        <TabsContent value="config-1" className='mt-0 flex flex-col grow overflow-y-auto'>
          <DataTable columns={columns} data={configItems} />
        </TabsContent>
      </Tabs>
    </div>
  )
}

export default ConfigPage