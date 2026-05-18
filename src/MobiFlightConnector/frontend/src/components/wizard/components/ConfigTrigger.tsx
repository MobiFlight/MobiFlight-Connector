import ComboBox from "@/components/ComboBox"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"

import { useControllerStore } from "@/stores/controllerStore"
import { IConfigItem } from "@/types/config"
import { BaseDevice, Controller } from "@/types/controller"
import { useState } from "react"

export type ConfigTriggerProps = {
  configItem: IConfigItem
  setConfigItem: (item: IConfigItem) => void
}

const ConfigTrigger = ({ configItem, setConfigItem }: ConfigTriggerProps) => {
  const { controllers } = useControllerStore()
  const [selectedController, setSelectedController] = useState<
    Partial<Controller> | undefined
  >(configItem.Controller)

  const devices = selectedController
    ? controllers.find((c) => c.Serial === selectedController.Serial)?.Devices
    : []
  console.log("devices", devices)

  const [selectedDevice, setSelectedDevice] = useState<
    Partial<IConfigItem["Device"]> | undefined
  >(configItem.Device)

  return (
    <Card>
      <CardHeader>
        <CardTitle>Define trigger</CardTitle>
        <CardDescription>
          The trigger defines the conditions or events that will activate this
          configuration.
        </CardDescription>
      </CardHeader>
      <CardContent className="flex flex-row gap-4 items-center">
        <Button variant="outline" className="flex-1">
          Scan for input
        </Button>
        <div className="font-md">Or select manually:</div>
        <ComboBox
          getLabel={(controller) => (controller as Controller).Name}
          getValue={(controller) => (controller as Controller).Serial}
          isSelected={(controller, selected) =>
            (controller as Controller).Serial === selected?.Serial
          }
          items={controllers}
          selected={selectedController}
          setSelected={(controller) => {
            setSelectedController(controller)
          }}
        />

        <ComboBox
          getLabel={(device) => (device as BaseDevice)?.Label}
          getValue={(device) => (device as BaseDevice)?.Name}
          isSelected={(device, selected) =>
            (device as BaseDevice).Name === selected?.Name
          }
          items={devices || []}
          selected={selectedDevice}
          setSelected={(device) => {
            setSelectedDevice(device)
          }}
        />
      </CardContent>
    </Card>
  )
}
export default ConfigTrigger
