import DeviceIcon from "@/components/icons/DeviceIcon"
import { LcdDisplayConfig } from "@/types/config"
import { DeviceReference, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Input } from "@/components/ui/input"
import OutputDeviceComboBox from "@/components/wizard/OutputConfig/OutputDevices/OutputDeviceComboBox"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import { useTranslation } from "react-i18next"

export interface LcdDisplayPanelProps {
  variant: "summary" | "details"
  config: LcdDisplayConfig
  controller?: Partial<Controller>
  devices?: DeviceReference[]
  onChange?: (config: LcdDisplayConfig) => void
}

const LcdDisplayPanel = ({
  config,
  variant,
  controller,
  devices = [],
  onChange,
}: LcdDisplayPanelProps) => {
  const { t } = useTranslation("wizard")
  if (variant == "details") {
    const selectedDevice = devices.find(
      (device) => device.Name === config.Address,
    )
    const LCD_ROWS = selectedDevice?.Capabilities?.Rows ?? 1
    const LCD_COLS = selectedDevice?.Capabilities?.Columns ?? 8

    const lines = Array.from(
      { length: LCD_ROWS },
      (_, index) => config.Lines?.[index] ?? "",
    )

    const updateLine = (index: number, text: string) => {
      const newLines = [...lines]
      newLines[index] = text
      onChange?.({ ...config, Lines: newLines })
    }

    return (
      <WizardSection>
        <SectionHeadline
          title={t("ConfigWizard.Device.LcdDisplay.Title")}
          description={t("ConfigWizard.Device.LcdDisplay.Description")}
        />
        <div className="flex flex-col gap-2">
          <Label>{t("ConfigWizard.Device.LcdDisplay.Label")}</Label>
          <OutputDeviceComboBox
            devices={devices}
            deviceType="LcdDisplay"
            selectedName={config.Address}
            onChange={(address) =>
              onChange?.({ ...config, Address: address, Name: address })
            }
          />
        </div>
        <div className="flex flex-col gap-2">
          <Label>{t("ConfigWizard.Device.LcdDisplay.Lines")}</Label>
          {lines.map((line, index) => (
            <Input
              key={index}
              aria-label={`Line ${index + 1}`}
              className={`w-auto font-mono`}
              size={LCD_COLS}
              maxLength={LCD_COLS}
              value={line}
              onChange={(e) => updateLine(index, e.target.value)}
            />
          ))}
        </div>
      </WizardSection>
    )
  }

  return (
    variant == "summary" && (
      <div className="flex grow flex-row items-center justify-between gap-2">
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.Controller")}</Label>
          {controller?.Name}
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.DisplayType")}</Label>
          <span className="flex flex-row gap-2">
            <DeviceIcon variant="LcdDisplay" /> {t("ConfigWizard.Device.LcdDisplay.Label")}
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.Device")}</Label>
          <Badge variant="secondary">{config.Name}</Badge>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.Device.LcdDisplay.Lines")}</Label>
          <Badge variant="secondary">{config.Lines?.length ?? 0}</Badge>
        </div>
      </div>
    )
  )
}

export default LcdDisplayPanel
