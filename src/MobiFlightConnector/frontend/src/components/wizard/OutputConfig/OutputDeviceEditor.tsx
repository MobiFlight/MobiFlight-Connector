import ComboBox from "@/components/ComboBox"
import { Card, CardContent } from "@/components/ui/card"
import { ControllerComboBox } from "@/components/wizard/components/ConfigTrigger"
import CustomDevicePanel from "@/components/wizard/OutputConfig/OutputDevices/CustomDevicePanel"
import LcdDisplayPanel from "@/components/wizard/OutputConfig/OutputDevices/LcdDisplayPanel"
import LedModulePanel from "@/components/wizard/OutputConfig/OutputDevices/LedModulePanel"
import OutputPanel from "@/components/wizard/OutputConfig/OutputDevices/OutputPanel"
import ServoPanel from "@/components/wizard/OutputConfig/OutputDevices/ServoPanel"
import ShiftRegisterPanel from "@/components/wizard/OutputConfig/OutputDevices/ShiftRegisterPanel"
import StepperPanel from "@/components/wizard/OutputConfig/OutputDevices/StepperPanel"
import { useControllerStore } from "@/stores/controllerStore"
import {
  LedModuleConfig,
  OutputConfig,
  LcdDisplayConfig,
  IDeviceConfig,
  OutputConfigItem,
  ServoConfig,
  StepperConfig,
  OutputDeviceConfig,
  ShiftRegisterConfig,
  CustomDeviceConfig,
} from "@/types/config"
import { BaseDevice, Controller, OutputDeviceType } from "@/types/controller"
import { OutputDeviceConfigDefaults } from "@/types/defaults"
import { OutputDeviceTypes } from "@/types/typesOptions"
import { useTranslation } from "react-i18next"

export interface OutputDeviceEditorProps {
  configItem: OutputConfigItem
  onConfigChange: (config: OutputConfigItem) => void
}

const isSameController = (
  controllerA: Partial<Controller> | undefined,
  controllerB: Partial<Controller> | undefined,
) => {
  if (!controllerA || !controllerB) return false
  return (
    controllerA.Serial === controllerB.Serial ||
    (!controllerA.Serial && controllerA.Name === controllerB.Name)
  )
}

const withCurrentInList = <T,>(
  list: T[],
  current: T | null | undefined,
  isSame: (a: T, b: T) => boolean,
) => {
  if (current && !list.some((item) => isSame(item, current))) {
    return [...list, current]
  }
  return list
}

const isOutputDevice = (device: BaseDevice) => {
  return OutputDeviceTypes.includes(device.Type as OutputDeviceType)
}

const OutputDeviceEditor = ({
  configItem,
  onConfigChange,
}: OutputDeviceEditorProps) => {
  // hooks
  const { t } = useTranslation()
  const { controllers } = useControllerStore()

  const controllersWithOutputDevices = controllers.filter((controller) =>
    controller.Devices?.some((device) =>
      OutputDeviceTypes.includes(device.Type as OutputDeviceType),
    ),
  )

  const connectedController = controllers.find((controller) =>
    isSameController(controller, configItem.Controller),
  )

  const completeControllers: Partial<Controller>[] = withCurrentInList(
    controllersWithOutputDevices,
    configItem.Controller,
    isSameController,
  )

  const devices: BaseDevice[] = [
    ...(connectedController?.Devices?.filter(isOutputDevice) ?? []),
  ]

  const configuredDevice: BaseDevice | undefined =
    configItem.Device != null
      ? {
          Name: configItem.Device.Name,
          Type: configItem.Device.Type,
          Label: configItem.Device.Name,
        }
      : undefined

  const completeDevices: BaseDevice[] = withCurrentInList(
    devices,
    configuredDevice,
    (a, b) => a.Name === b.Name,
  )

  const deviceTypeOptions = [
    ...new Set(completeDevices.map((device) => device.Type)),
  ].map((type) => ({
    label: type,
    value: type,
  }))

  const selectedDeviceTypeOption = deviceTypeOptions.find(
    (option) => option.value === configuredDevice?.Type,
  )

  const updateConfigItem = (
    controller: Partial<Controller> | undefined,
    device: IDeviceConfig | undefined,
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
      Device: device ? device : null,
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
        <CardContent className="flex flex-col gap-4 pt-4">
          <div className="flex flex-col gap-1">
            <div className="text-lg font-semibold">Step 1 - Display type</div>
            <div className="text-muted-foreground text-sm">
              Select the type of device you want to configure.
            </div>
          </div>
          <div className="flex flex-row items-center gap-4">
            <div className="flex flex-col gap-2">
              <ControllerComboBox
                completeControllers={completeControllers}
                selectedController={configItem.Controller}
                onChange={(controller) => {
                  updateConfigItem(controller, undefined)
                }}
              />
            </div>
            <div className="flex flex-col gap-2">
              <ComboBox
                items={deviceTypeOptions}
                getLabel={(item) => item.label}
                getValue={(item) => item.value}
                selected={selectedDeviceTypeOption}
                isSelected={(item, selected) =>
                  selected ? item.value === selected.value : false
                }
                setSelected={(item) => {
                  // deselecting the current device type clears the device
                  if (!item) {
                    updateConfigItem(configItem.Controller, undefined)
                    return
                  }

                  const first = completeDevices.find(
                    (device) => device.Type === item?.value,
                  )
                  const isSameDeviceType =
                    first?.Type === configItem.Device?.Type

                  const device = isSameDeviceType
                    ? ({ ...configItem.Device, ...first } as OutputDeviceConfig)
                    : OutputDeviceConfigDefaults.createByType(
                        first?.Type as OutputDeviceType,
                        first?.Name ?? "",
                      )
                  updateConfigItem(configItem.Controller, device)
                }}
              />
            </div>
          </div>
        </CardContent>
      </Card>
      {configItem.Device && configItem.Device.Type === "Output" && (
        <OutputPanel
          variant="details"
          config={configItem.Device as OutputConfig}
          controller={configItem.Controller}
          devices={devices.filter((device) => device.Type === "Output")}
          onChange={(device) => updateConfigItem(configItem.Controller, device)}
        />
      )}
      {configItem.Device && configItem.Device.Type === "LedModule" && (
        <LedModulePanel
          variant="details"
          config={configItem.Device as LedModuleConfig}
          controller={configItem.Controller}
          devices={devices.filter((device) => device.Type === "LedModule")}
          onChange={(device) => updateConfigItem(configItem.Controller, device)}
        />
      )}
      {configItem.Device && configItem.Device.Type === "LcdDisplay" && (
        <LcdDisplayPanel
          variant="details"
          config={configItem.Device as LcdDisplayConfig}
          controller={configItem.Controller}
          devices={devices.filter((device) => device.Type === "LcdDisplay")}
          onChange={(device) => updateConfigItem(configItem.Controller, device)}
        />
      )}
      {configItem.Device && configItem.Device.Type === "Servo" && (
        <ServoPanel
          variant="details"
          config={configItem.Device as ServoConfig}
          controller={configItem.Controller}
          devices={devices.filter((device) => device.Type === "Servo")}
          onChange={(device) => updateConfigItem(configItem.Controller, device)}
        />
      )}
      {configItem.Device && configItem.Device.Type === "Stepper" && (
        <StepperPanel
          variant="details"
          config={configItem.Device as StepperConfig}
          controller={configItem.Controller}
          devices={devices.filter((device) => device.Type === "Stepper")}
          onChange={(device) => updateConfigItem(configItem.Controller, device)}
        />
      )}
      {configItem.Device && configItem.Device.Type === "ShiftRegister" && (
        <ShiftRegisterPanel
          variant="details"
          config={configItem.Device as ShiftRegisterConfig}
          controller={configItem.Controller}
        />
      )}
      {configItem.Device && configItem.Device.Type === "CustomDevice" && (
        <CustomDevicePanel
          variant="details"
          config={configItem.Device as CustomDeviceConfig}
          controller={configItem.Controller}
          devices={devices.filter((device) => device.Type === "CustomDevice")}
          onChange={(device) => updateConfigItem(configItem.Controller, device)}
        />
      )}
    </div>
  )
}
export default OutputDeviceEditor
