import DrawerBreadcrumb from "@/components/wizard/common/DrawerTitle"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import ConfigWizardTestPanel from "@/components/wizard/common/ConfigWizardTestPanel"
import DeviceTypeSelectionPanel from "@/components/wizard/OutputConfig/DeviceTypeSelectionPanel"
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
  CustomDeviceConfig,
  ShiftRegisterConfig,
} from "@/types/config"
import {
  DeviceReference,
  Controller,
  OutputDeviceType,
} from "@/types/controller"
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

const isOutputDevice = (device: DeviceReference) => {
  return OutputDeviceTypes.includes(device.Type as OutputDeviceType)
}

const OutputDeviceEditor = ({
  configItem,
  onConfigChange,
}: OutputDeviceEditorProps) => {
  // hooks
  const { t } = useTranslation("wizard")
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

  const devices: DeviceReference[] = [
    ...(connectedController?.Devices?.filter(isOutputDevice) ?? []),
  ]

  const configuredDevice: DeviceReference | undefined =
    configItem.Device != null
      ? {
          Name: configItem.Device.Name,
          Type: configItem.Device.Type,
          Label: configItem.Device.Name,
        }
      : undefined

  const completeDevices: DeviceReference[] = withCurrentInList(
    devices,
    configuredDevice,
    (a, b) => a.Name === b.Name,
  )

  const deviceTypes = [
    ...new Set(
      completeDevices.filter(isOutputDevice).map((device) => device.Type),
    ),
  ] as OutputDeviceType[]

  const selectedDeviceType = deviceTypes.find(
    (deviceType) => deviceType === configuredDevice?.Type,
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

  const onDeviceTypeChange = (deviceType: OutputDeviceType | null) => {
    // deselecting the current device type clears the device
    if (!deviceType) {
      updateConfigItem(configItem.Controller, undefined)
      return
    }

    const first = completeDevices.find((device) => device.Type === deviceType)
    const isSameDeviceType = first?.Type === configItem.Device?.Type

    const device = isSameDeviceType
      ? ({ ...configItem.Device, ...first } as OutputDeviceConfig)
      : OutputDeviceConfigDefaults.createByType(
          deviceType,
          first?.Name ?? "",
          first?.Capabilities ?? undefined,
        )
    updateConfigItem(configItem.Controller, device)
  }

  return (
    <div className="flex flex-col gap-4" data-testid="output-device-editor">
      <SectionHeadline
        title={
          <DrawerBreadcrumb
            mode={configItem.Type}
            crumbs={[
              configItem.Name,
              t("Dialog.InputConfigWizard.Action.Title"),
              t("ConfigWizard.Device.Title"),
            ]}
          />
        }
        description={t("ConfigWizard.Device.Description")}
      />

      <WizardSection>
        <DeviceTypeSelectionPanel
          controllers={completeControllers}
          selectedController={configItem.Controller}
          onControllerChange={(controller) => {
            updateConfigItem(controller, undefined)
          }}
          deviceTypes={deviceTypes}
          selectedDeviceType={selectedDeviceType}
          onDeviceTypeChange={onDeviceTypeChange}
        />
      </WizardSection>
      {configItem.Device && configItem.Device.Type === "Output" && (
        <OutputPanel
          variant="details"
          config={configItem.Device as OutputConfig}
          controller={configItem.Controller}
          devices={devices.filter((device) => device.Type === "Output")}
          onChange={(device) => updateConfigItem(configItem.Controller, device)}
        />
      )}
      {configItem.Device && configItem.Device.Type === "ShiftRegister" && (
        <ShiftRegisterPanel
          variant="details"
          config={configItem.Device as ShiftRegisterConfig}
          controller={configItem.Controller}
          devices={devices.filter((device) => device.Type === "ShiftRegister")}
          onChange={(device) => updateConfigItem(configItem.Controller, device)}
        />
      )}
      {configItem.Device && configItem.Device.Type === "LedModule" && (
        <LedModulePanel
          variant="details"
          config={configItem.Device as LedModuleConfig}
          controller={configItem.Controller}
          devices={devices.filter((device) => device.Type === "LedModule")}
          configId={configItem.GUID}
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
      {configItem.Device && configItem.Device.Type === "CustomDevice" && (
        <CustomDevicePanel
          variant="details"
          config={configItem.Device as CustomDeviceConfig}
          controller={configItem.Controller}
          devices={devices.filter((device) => device.Type === "CustomDevice")}
          onChange={(device) => updateConfigItem(configItem.Controller, device)}
        />
      )}
      {configItem.Device && (
        <ConfigWizardTestPanel
          configItem={configItem}
          title={t("ConfigWizard.Device.TestPanel.Title")}
          description={t("ConfigWizard.Device.TestPanel.Description")}
          onTestValueChange={(value) =>
            onConfigChange({
              ...configItem,
              TestValue: value,
            })
          }
        />
      )}
    </div>
  )
}
export default OutputDeviceEditor
