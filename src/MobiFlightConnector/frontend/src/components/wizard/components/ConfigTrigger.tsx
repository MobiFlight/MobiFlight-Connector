import ComboBox from "@/components/ComboBox"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { publishOnMessageExchange, useAppMessage } from "@/lib/hooks/appMessage"

import { useControllerStore } from "@/stores/controllerStore"
import { CommandScanForInput } from "@/types/commands"
import { IConfigItem } from "@/types/config"
import { BaseDevice, Controller } from "@/types/controller"
import { ScanForInputResult } from "@/types/messages"
import { IconLoader2, IconTrash } from "@tabler/icons-react"
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
    })
  }

  const [scanning, setScanning] = useState(false)

  useAppMessage("ScanForInputResult", (message) => {
    console.log("ScanForInputResult message received", message.payload)
    const { Controller, Device } = message.payload as ScanForInputResult
    setSelectedController(Controller)
    setSelectedDevice({ ...Device, Label: Device.Name })
    updateConfigItem(Controller, Device)
    setScanning(false)
  })

  const scanForInput = () => {
    const { publish } = publishOnMessageExchange()
    publish({
      key: "CommandScanForInput",
      payload: {
        isScanning: !scanning,
      },
    } as CommandScanForInput)
    setScanning(!scanning)
  }

  return (
    <Card>
      <CardContent className="flex flex-col gap-4 pt-4">
        <div className="flex flex-col gap-2">
          <div className="text-lg font-semibold">Trigger</div>
          <div className="text-muted-foreground text-sm">
            The trigger defines the conditions or events that will activate this
            configuration.
          </div>
        </div>
        <div className="flex flex-row gap-2 items-end">
          <Button onClick={scanForInput} className="w-50" size="sm">
            {scanning ? (
              <div className="flex flex-row items-center gap-2 text-sm">
                <IconLoader2 className="animate-spin" />
                Use any input
              </div>
            ) : (
              "Scan for input"
            )}
          </Button>
          <div className="flex flex-col gap-2">
            <ComboBox
              widthClass="w-50"
              getLabel={(controller) => (controller as Controller).Name}
              getValue={(controller) =>
                (controller as Controller).Serial ??
                (controller as Controller).Name
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
          </div>

          <div className="flex flex-col gap-2">
            <ComboBox
              widthClass="w-50"
              getLabel={(device) =>
                (device as BaseDevice)?.Label ?? (device as BaseDevice)?.Name
              }
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
          </div>
          <Button
            variant="ghost"
            className="gap-1"
            onClick={() => updateConfigItem(undefined, undefined)}
          >
            <IconTrash className="mr-2" />
            Clear selection
          </Button>
        </div>
      </CardContent>
    </Card>
  )
}
export default ConfigTrigger
