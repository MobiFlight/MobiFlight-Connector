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

  const connectedController = selectedController
    ? controllers.find(
        (controller) =>
          controller.Serial === selectedController.Serial ||
          (!selectedController.Serial &&
            controller.Name === selectedController.Name),
      )
    : undefined

  const hasControllerOption = selectedController
    ? controllers.some(
        (controller) =>
          controller.Serial === selectedController.Serial ||
          (!selectedController.Serial &&
            controller.Name === selectedController.Name),
      )
    : false

  const completeControllers: Partial<Controller>[] =
    selectedController && !hasControllerOption
      ? [...controllers, selectedController]
      : controllers

  const selectedMatchesConfigController =
    selectedController != null &&
    configItem.Controller != null &&
    (selectedController.Serial === configItem.Controller.Serial ||
      (!selectedController.Serial &&
        selectedController.Name === configItem.Controller.Name))

  const devices: BaseDevice[] = [...(connectedController?.Devices ?? [])]

  const configuredDevice: BaseDevice | undefined =
    configItem.Device != null
      ? {
          Name: configItem.Device.Name,
          Type: configItem.Device.Type,
          Label: configItem.Device.Name,
        }
      : configItem.DeviceName
        ? {
            Name: configItem.DeviceName,
            Type: (configItem.DeviceType as string) ?? "Unknown",
            Label: configItem.DeviceName,
          }
        : undefined

  const [selectedDevice, setSelectedDevice] = useState<BaseDevice | undefined>(
    configuredDevice,
  )

  if (
    selectedMatchesConfigController &&
    configuredDevice != null &&
    !devices.some((device) => device.Name === configuredDevice.Name)
  ) {
    devices.push(configuredDevice)
  }

  const updateConfigItem = (
    controller: Partial<Controller> | undefined,
    device: BaseDevice | undefined,
  ) => {
    setConfigItem({
      ...configItem,
      Controller: controller,
      Device: device
        ? {
            Name: device.Name,
            Type: device.Type,
          }
        : null,
      DeviceName: device?.Name ?? null,
      DeviceType: device?.Type ?? null,
    })
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Define trigger</CardTitle>
        <CardDescription>
          The trigger defines the conditions or events that will activate this
          configuration.
        </CardDescription>
      </CardHeader>
      <CardContent className="flex flex-row items-center gap-4">
        <Button className="flex-1">Scan for input</Button>
        <div className="font-md">Or select manually:</div>
        <ComboBox
          getLabel={(controller) => (controller as Controller).Name}
          getValue={(controller) =>
            (controller as Controller).Serial ?? (controller as Controller).Name
          }
          isSelected={(controller, selected) =>
            (controller as Controller).Serial === selected?.Serial ||
            (!(controller as Controller).Serial &&
              (controller as Controller).Name === selected?.Name)
          }
          items={completeControllers}
          selected={selectedController}
          placeholder="Select controller..."
          searchPlaceholder="Search controller..."
          emptyText="No controller found."
          setSelected={(controller) => {
            setSelectedController(controller)
            setSelectedDevice(undefined)
            updateConfigItem(controller, undefined)
          }}
        />

        <ComboBox
          getLabel={(device) => (device as BaseDevice)?.Label}
          getValue={(device) => (device as BaseDevice)?.Name}
          isSelected={(device, selected) =>
            (device as BaseDevice).Name === selected?.Name
          }
          items={devices}
          selected={selectedDevice}
          placeholder="Select device..."
          searchPlaceholder="Search device..."
          emptyText="No device found."
          disabled={!selectedController}
          setSelected={(device) => {
            setSelectedDevice(device)
            updateConfigItem(selectedController, device)
          }}
        />
      </CardContent>
    </Card>
  )
}
export default ConfigTrigger
