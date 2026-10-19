import { Card, CardContent } from "@/components/ui/card"
import {
  ControllerComboBox,
  DeviceComboBox,
} from "@/components/wizard/components/ConfigTrigger"
import OutputPanel from "@/components/wizard/components/OutputDevices/OutputPanel"
import { useControllerStore } from "@/stores/controllerStore"
import { OutputConfig, OutputConfigItem } from "@/types/config"
import { BaseDevice, Controller, OutputDeviceType } from "@/types/controller"
import { OutputDeviceTypes } from "@/types/typesOptions"
import { useState } from "react"
import { useTranslation } from "react-i18next"

export interface OutputDeviceEditorProps {
  configItem: OutputConfigItem
  onConfigChange: (config: OutputConfigItem) => void
}

const OutputDeviceEditor = ({
  configItem,
  onConfigChange,
}: OutputDeviceEditorProps) => {
  const { t } = useTranslation()
  const { controllers } = useControllerStore()

  const [selectedController, setSelectedController] = useState<
    Partial<Controller> | undefined
  >(configItem.Controller)

  const deviceConfig = configItem?.Device as OutputConfig
  const controller = configItem?.Controller

  const connectedController = selectedController
    ? controllers.find(
        (controller) =>
          controller.Serial === selectedController.Serial ||
          (!selectedController.Serial &&
            controller.Name === selectedController.Name),
      )
    : undefined

  // Add selected controller to the list
  // because it won't be included in the list of controllers
  // if it's not currently connected
  const completeControllers: Partial<Controller>[] =
    selectedController && !connectedController
      ? [...controllers, selectedController]
      : controllers

  const selectedMatchesConfigController =
    selectedController != null &&
    configItem.Controller != null &&
    (selectedController.Serial === configItem.Controller.Serial ||
      (!selectedController.Serial &&
        selectedController.Name === configItem.Controller.Name))

  const inputDeviceFilter = (device: BaseDevice) => {
    return OutputDeviceTypes.includes(device.Type as OutputDeviceType)
  }

  const devices: BaseDevice[] = [
    ...(connectedController?.Devices?.filter(inputDeviceFilter) ?? []),
  ]

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
    onConfigChange({
      ...configItem,
      Controller: controller
        ? {
            ...controller,
            // unset Devices
            // before sending it back to backend.
            Devices: undefined,
          }
        : undefined,
      Device: device
        ? {
            Name: device.Name,
            Type: device.Type,
          }
        : null,
    })
  }

  return (
    <div className="flex flex-col gap-4" data-testid="source-editor">
      <div className="flex flex-col gap-1">
        <div className="text-lg font-semibold">
          {t("Dialog.ConfigWizard.Output.Title")}
        </div>
        <div className="text-muted-foreground text-sm">
          {t("Dialog.ConfigWizard.Output.Description")}
        </div>
      </div>
      <Card>
        <CardContent className="pt-4">
          <div className="flex flex-col gap-1">
            <div className="text-lg font-semibold">Step 1 - Display type</div>
            <div className="text-muted-foreground text-sm">
              Select the type of device you want to configure.
            </div>
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div className="flex flex-col gap-2">
              <ControllerComboBox
                completeControllers={completeControllers}
                selectedController={selectedController}
                onChange={(controller) => {
                  setSelectedController(controller)
                  setSelectedDevice(undefined)
                  updateConfigItem(controller, undefined)
                }}
              />
            </div>
            <div className="flex flex-col gap-2">
              <DeviceComboBox
                devices={devices}
                selectedDevice={selectedDevice}
                selectedController={selectedController}
                onChange={(device) => {
                  setSelectedDevice(device)
                  updateConfigItem(selectedController, device)
                }}
              />
            </div>
          </div>
        </CardContent>
      </Card>
      {configItem?.Device?.Type == "Output" && (
        <OutputPanel
          variant="details"
          config={deviceConfig}
          controller={controller}
        />
      )}
    </div>
  )
}
export default OutputDeviceEditor
