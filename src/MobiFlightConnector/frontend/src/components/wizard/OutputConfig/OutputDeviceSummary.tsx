import { useTranslation } from "react-i18next"
import {
  CustomDeviceConfig,
  LcdDisplayConfig,
  LedModuleConfig,
  OutputConfig,
  OutputDeviceConfig,
  ServoConfig,
  ShiftRegisterConfig,
  StepperConfig,
} from "@/types/config"
import { Controller } from "@/types/controller"
import OutputPanel from "@/components/wizard/OutputConfig/OutputDevices/OutputPanel"
import { Button } from "@/components/ui/button"
import { IconEdit } from "@tabler/icons-react"
import CustomDevicePanel from "@/components/wizard/OutputConfig/OutputDevices/CustomDevicePanel"
import LedModulePanel from "@/components/wizard/OutputConfig/OutputDevices/LedModulePanel"
import LcdDisplayPanel from "@/components/wizard/OutputConfig/OutputDevices/LcdDisplayPanel"
import ServoPanel from "@/components/wizard/OutputConfig/OutputDevices/ServoPanel"
import StepperPanel from "@/components/wizard/OutputConfig/OutputDevices/StepperPanel"
import ShiftRegisterPanel from "@/components/wizard/OutputConfig/OutputDevices/ShiftRegisterPanel"

export interface OutputDeviceSummaryProps {
  config: OutputDeviceConfig
  controller: Partial<Controller> | undefined
  onEdit: () => void
}

const OutputDeviceSummary = ({
  config,
  controller,
  onEdit,
}: OutputDeviceSummaryProps) => {
  const { t } = useTranslation("wizard")
  
  if (config === null) {
    return null
  }

  return (
    <div className="flex grow flex-row items-center gap-2">
      {config.Type === "Output" && (
        <OutputPanel
          variant="summary"
          config={config as OutputConfig}
          controller={controller}
        />
      )}

      {config.Type === "ShiftRegister" && (
        <ShiftRegisterPanel
          variant="summary"
          config={config as ShiftRegisterConfig}
          controller={controller}
        />
      )}

      {config.Type === "LedModule" && (
        <LedModulePanel
          variant="summary"
          config={config as LedModuleConfig}
          controller={controller}
        />
      )}

      {config.Type === "LcdDisplay" && (
        <LcdDisplayPanel
          variant="summary"
          config={config as LcdDisplayConfig}
          controller={controller}
        />
      )}

      {config.Type === "Servo" && (
        <ServoPanel
          variant="summary"
          config={config as ServoConfig}
          controller={controller}
        />
      )}

      {config.Type === "Stepper" && (
        <StepperPanel
          variant="summary"
          config={config as StepperConfig}
          controller={controller}
        />
      )}

      {config.Type === "CustomDevice" && (
        <CustomDevicePanel
          variant="summary"
          config={config as CustomDeviceConfig}
          controller={controller}
        />
      )}

      <Button size={"sm"} variant="ghost" onClick={onEdit}>
        <IconEdit size={16} />
        <span className="sr-only">{t("ConfigWizard.Device.Edit")}</span>
      </Button>
    </div>
  )
}

export default OutputDeviceSummary
