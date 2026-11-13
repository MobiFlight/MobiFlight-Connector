import DeviceIcon from "@/components/icons/DeviceIcon"
import { ServoConfig } from "@/types/config"
import { DeviceReference, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Input } from "@/components/ui/input"
import OutputDeviceComboBox from "@/components/wizard/OutputConfig/OutputDevices/OutputDeviceComboBox"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import { useTranslation } from "react-i18next"

export interface ServoPanelProps {
  variant: "summary" | "details"
  config: ServoConfig
  controller?: Partial<Controller>
  // only used by the details variant
  devices?: DeviceReference[]
  onChange?: (config: ServoConfig) => void
}

const ServoPanel = ({
  config,
  variant,
  controller,
  devices = [],
  onChange,
}: ServoPanelProps) => {
  const { t } = useTranslation("wizard")
  if (variant == "details") {
    return (
      <WizardSection>
        <SectionHeadline
          title={t("ConfigWizard.Device.Servo.Title")}
          description={t("ConfigWizard.Device.Servo.Description")}
        />
        <div className="flex flex-col gap-2">
          <Label>{t("ConfigWizard.Device.Servo.Label")}</Label>
          <OutputDeviceComboBox
            devices={devices}
            deviceType="Servo"
            selectedName={config.Address}
            onChange={(address) =>
              onChange?.({ ...config, Address: address, Name: address })
            }
          />
        </div>
        <div className="flex flex-row gap-4">
          <div className="flex flex-col gap-2">
            <Label htmlFor="servo-min">{t("ConfigWizard.Device.Servo.MinValue")}</Label>
            <Input
              id="servo-min"
              type="number"
              value={config.Min}
              onChange={(e) => onChange?.({ ...config, Min: e.target.value })}
            />
          </div>
          <div className="flex flex-col gap-2">
            <Label htmlFor="servo-max">{t("ConfigWizard.Device.Servo.MaxValue")}</Label>
            <Input
              id="servo-max"
              type="number"
              value={config.Max}
              onChange={(e) => onChange?.({ ...config, Max: e.target.value })}
            />
          </div>
          <div className="flex flex-col gap-2">
            <Label htmlFor="servo-max-rotation">{t("ConfigWizard.Device.Servo.MaxRotation")}</Label>
            <Input
              id="servo-max-rotation"
              type="number"
              min={1}
              max={100}
              value={config.MaxRotationPercent}
              onChange={(e) =>
                onChange?.({ ...config, MaxRotationPercent: e.target.value })
              }
            />
          </div>
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
            <DeviceIcon variant="Servo" /> {t("ConfigWizard.Device.Servo.Label")}
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.Device")}</Label>
          <Badge variant="secondary">{config.Name}</Badge>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.Device.Servo.Range")}</Label>
          <Badge variant="secondary">
            {config.Min} – {config.Max}
          </Badge>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.Device.Servo.MaxRotation")}</Label>
          <Badge variant="secondary">{config.MaxRotationPercent} %</Badge>
        </div>
      </div>
    )
  )
}

export default ServoPanel
