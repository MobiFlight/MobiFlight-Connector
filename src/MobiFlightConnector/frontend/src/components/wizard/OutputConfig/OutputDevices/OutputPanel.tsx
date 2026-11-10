import DeviceIcon from "@/components/icons/DeviceIcon"
import { OutputConfig } from "@/types/config"
import { DeviceReference, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Checkbox } from "@/components/ui/checkbox"
import { Slider } from "@/components/ui/slider"
import { Switch } from "@/components/ui/switch"
import { Separator } from "@/components/ui/separator"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import { useTranslation } from "react-i18next"

export interface OutputPanelProps {
  variant: "summary" | "details"
  config: OutputConfig
  controller?: Partial<Controller>
  // only used by the details variant
  devices?: DeviceReference[]
  onChange?: (config: OutputConfig) => void
}

const OutputPanel = ({
  config,
  variant,
  controller,
  devices = [],
  onChange,
}: OutputPanelProps) => {
  const { t } = useTranslation("wizard")
  
  if (variant == "summary") {
    const selectedPins = config.Pin.split("|").filter((pin) => pin !== "")
    return (
      <div className="flex grow flex-row items-center justify-between gap-2">
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.Controller")}</Label>
          {controller?.Name}
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.DisplayType")}</Label>
          <span className="flex flex-row gap-2">
            <DeviceIcon variant="Output" />{t("ConfigWizard.Device.Output.Label")}
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.Device")}</Label>
          <div className="flex flex-row gap-2">
            {selectedPins.map((pin) => (
              <Badge key={pin} variant="secondary">
                {pin}
              </Badge>
            ))}
          </div>
        </div>
        {config.PwmMode && (
          <div className="flex flex-col gap-1">
            <Label>{t("ConfigWizard.General.Options")}</Label>
            <Badge variant="secondary">{t("ConfigWizard.Device.Output.DynamicBrightness")}</Badge>
          </div>
        )}
      </div>
    )
  }

  const selectedPins = config.Pin.split("|").filter((pin) => pin !== "")

  // Keep configured pins visible
  // even if they are currently not available on the controller
  const pins = [
    ...new Set([...devices.map((device) => device.Name), ...selectedPins]),
  ]

  const getPinLabel = (pin: string) =>
    devices.find((device) => device.Name === pin)?.Label ?? pin

  const togglePin = (pin: string, checked: boolean) => {
    const newPins = checked
      ? [...selectedPins, pin]
      : selectedPins.filter((selectedPin) => selectedPin !== pin)
    const newPin = newPins.join("|")
    onChange?.({ ...config, Pin: newPin, Name: newPin })
  }

  const isDimmable =
    devices.filter((device) => device.Capabilities?.Pwm).length > 0

  return (
    <WizardSection>
      <SectionHeadline
        title={t("ConfigWizard.Device.Output.Title")}
        description={t("ConfigWizard.Device.Output.Description")}
      />
      <div className="flex flex-col gap-2">
        <Label>{t("ConfigWizard.Device.Output.List")}</Label>
        <div className="grid grid-cols-4 gap-2">
          {pins.map((pin) => (
            <div key={pin} className="flex flex-row items-center gap-2">
              <Checkbox
                id={`output-pin-${pin}`}
                checked={selectedPins.includes(pin)}
                onCheckedChange={(checked) => togglePin(pin, !!checked)}
              />
              <Label htmlFor={`output-pin-${pin}`}>{getPinLabel(pin)}</Label>
            </div>
          ))}
        </div>
      </div>
      <Separator />
      <div className="flex flex-row items-center justify-between gap-2">
        <div className="flex flex-col gap-1">
          <Label htmlFor="output-pwm-mode">{t("ConfigWizard.Device.Output.DynamicBrightness")}</Label>
          {isDimmable ? (
            <span className="text-muted-foreground text-sm">
              {t("ConfigWizard.Device.Output.DynamicBrighnessDescription")}
            </span>
          ) : (
            <div className="text-muted-foreground text-sm">
              {t("ConfigWizard.Device.Output.NoDimmablePins")}
            </div>
          )}
        </div>
        <Switch
          disabled={!isDimmable}
          id="output-pwm-mode"
          checked={config.PwmMode}
          onCheckedChange={(checked) =>
            onChange?.({ ...config, PwmMode: !!checked })
          }
        />
      </div>
      <div
        className={`flex flex-row items-center justify-between gap-2 ${config.PwmMode ? "opacity-50" : ""}`}
        aria-disabled={config.PwmMode}
      >
        <div className="flex w-1/2 flex-col gap-1">
          <Label>{t("ConfigWizard.Device.Output.StaticBrightness")} ({config.Brightness})</Label>
          <div className="text-muted-foreground text-sm">
            {t("ConfigWizard.Device.Output.StaticBrighnessDescription")}
          </div>
        </div>
        <div className="flex w-1/2">
          <Slider
            min={0}
            max={255}
            step={1}
            disabled={config.PwmMode}
            value={[config.Brightness]}
            onValueChange={([brightness]) =>
              onChange?.({ ...config, Brightness: brightness })
            }
          />
        </div>
      </div>
    </WizardSection>
  )
}

export default OutputPanel
