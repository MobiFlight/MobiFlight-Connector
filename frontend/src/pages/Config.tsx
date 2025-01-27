import { useConfigStore } from '@/stores/configFileStore';
// import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
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
            "Active": true,
            "GUID": "8698d38c-1e1d-4ec1-a89a-b01e36fc89df",
            "ModuleSerial": "miniCOCKPIT miniFCU/ SN-3GC-D38",
            "Name": "SPEED",
            "RawValue": null,
            "Type": "MobiFlight.OutputConfigItem",
            "Value": null,
            "Device": {
              "Name": "LCD Display",
              "Type": "CustomDevice"
            }
          },
          {
            GUID: 'test-guid-0',
            Active: true,
            Name: 'Landing Lights',
            ModuleSerial: 'MobiFlight Board/123456',
            Type: "Output",
            RawValue: '0',
            Value: '0',
            Device: {
              "Name": "LED 1",
              "Type": "Output"
            }
          },
          {
            GUID: 'test-guid-1',
            Active: true,
            Name: 'Landing Lights',
            ModuleSerial: 'MobiFlight Board/123456',
            Type: "Output",
            RawValue: '0',
            Value: '0',
            Device: {
              "Name": "LED 1",
              "Type": "Output"
            }
          },
          {
            GUID: 'test-guid-2',
            Active: true,
            Name: 'RPM',
            ModuleSerial: 'MobiFlight Board/123456',
            Device: {
              "Name": "RPM Gauge",
              "Type": "Stepper"
            },
            Type: 'Stepper',
            RawValue: '1500',
            Value: '15000',
          },
        ])
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