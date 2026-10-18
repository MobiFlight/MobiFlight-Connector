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
import OutputPanel from "./OutputPanel"
import { Button } from "@/components/ui/button"
import { IconEdit } from "@tabler/icons-react"
import CustomDevicePanel from "@/components/wizard/components/OutputDevices/CustomDevicePanel"
import LedModulePanel from "@/components/wizard/components/OutputDevices/LedModulePanel"
import LcdDisplayPanel from "@/components/wizard/components/OutputDevices/LcdDisplayPanel"
import ServoPanel from "@/components/wizard/components/OutputDevices/ServoPanel"
import StepperPanel from "@/components/wizard/components/OutputDevices/StepperPanel"
import ShiftRegisterPanel from "@/components/wizard/components/OutputDevices/ShiftRegisterPanel"

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
  if (config.Type == "Output") {
    return (
      <div className="flex grow flex-row items-center gap-2">
        <OutputPanel
          variant="summary"
          config={config as OutputConfig}
          controller={controller}
        />
        <Button size={"sm"} variant="ghost" onClick={onEdit}>
          <IconEdit size={16} />
        </Button>
      </div>
    )
  }

  if (config.Type == "LedModule") {
    return (
      <div className="flex grow flex-row items-center gap-2">
        <LedModulePanel
          variant="summary"
          config={config as LedModuleConfig}
          controller={controller}
        />
        <Button size={"sm"} variant="ghost" onClick={onEdit}>
          <IconEdit size={16} />
        </Button>
      </div>
    )
  }

  if (config.Type == "LcdDisplay") {
    return (
      <div className="flex grow flex-row items-center gap-2">
        <LcdDisplayPanel
          variant="summary"
          config={config as LcdDisplayConfig}
          controller={controller}
        />
        <Button size={"sm"} variant="ghost" onClick={onEdit}>
          <IconEdit size={16} />
        </Button>
      </div>
    )
  }

  if (config.Type == "Servo") {
    return (
      <div className="flex grow flex-row items-center gap-2">
        <ServoPanel
          variant="summary"
          config={config as ServoConfig}
          controller={controller}
        />
        <Button size={"sm"} variant="ghost" onClick={onEdit}>
          <IconEdit size={16} />
        </Button>
      </div>
    )
  }

  if (config.Type == "Stepper") {
    return (
      <div className="flex grow flex-row items-center gap-2">
        <StepperPanel
          variant="summary"
          config={config as StepperConfig}
          controller={controller}
        />
        <Button size={"sm"} variant="ghost" onClick={onEdit}>
          <IconEdit size={16} />
        </Button>
      </div>
    )
  }

  if (config.Type == "ShiftRegister") {
    return (
      <div className="flex grow flex-row items-center gap-2">
        <ShiftRegisterPanel
          variant="summary"
          config={config as ShiftRegisterConfig}
          controller={controller}
        />
        <Button size={"sm"} variant="ghost" onClick={onEdit}>
          <IconEdit size={16} />
        </Button>
      </div>
    )
  }

  if (config.Type == "CustomDevice") {
    return (
      <div className="flex grow flex-row items-center gap-2">
        <CustomDevicePanel
          variant="summary"
          config={config as CustomDeviceConfig}
          controller={controller}
        />
        <Button size={"sm"} variant="ghost" onClick={onEdit}>
          <IconEdit size={16} />
        </Button>
      </div>
    )
  }

  return null
}

export default OutputDeviceSummary
