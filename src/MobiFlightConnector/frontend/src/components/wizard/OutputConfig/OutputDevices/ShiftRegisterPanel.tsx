import DeviceIcon from "@/components/icons/DeviceIcon"
import { ShiftRegisterConfig } from "@/types/config"
import { DeviceReference, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Checkbox } from "@/components/ui/checkbox"
import { Separator } from "@/components/ui/separator"
import OutputDeviceComboBox from "@/components/wizard/OutputConfig/OutputDevices/OutputDeviceComboBox"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import { useTranslation } from "react-i18next"

// pins are stored with this prefix, e.g. "Output 0|Output 3"
const PIN_PREFIX = "Output"
const DEFAULT_PIN_COUNT = 8

export interface ShiftRegisterPanelProps {
  variant: "summary" | "details"
  config: ShiftRegisterConfig
  controller?: Partial<Controller>
  // only used by the details variant
  devices?: DeviceReference[]
  onChange?: (config: ShiftRegisterConfig) => void
}

const ShiftRegisterPanel = ({
  config,
  variant,
  controller,
  devices = [],
  onChange,
}: ShiftRegisterPanelProps) => {
  const { t } = useTranslation("wizard")

  const selectedPins = config.Pin.split("|").filter((pin) => pin !== "")

  if (variant == "summary") {
    return (
      <div className="flex grow flex-row items-center justify-between gap-2">
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.Controller")}</Label>
          {controller?.Name}
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.DisplayType")}</Label>
          <span className="flex flex-row gap-2">
            <DeviceIcon variant="ShiftRegister" />
            {t("ConfigWizard.Device.ShiftRegister.Label")}
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.Device")}</Label>
          <Badge variant="secondary">{config.Address}</Badge>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.Device.ShiftRegister.Pins")}</Label>
          <div className="flex flex-row gap-2">
            {selectedPins.map((pin) => (
              <Badge key={pin} variant="secondary">
                {pin}
              </Badge>
            ))}
          </div>
        </div>
      </div>
    )
  }

  const selectedDevice = devices.find(
    (device) => device.Name === config.Address,
  )
  const pinCount = selectedDevice?.Capabilities?.Pins ?? DEFAULT_PIN_COUNT

  // Keep configured pins visible
  // even if they are currently not available on the shift register
  const pins = [
    ...new Set([
      ...Array.from(
        { length: pinCount },
        (_, index) => `${PIN_PREFIX} ${index}`,
      ),
      ...selectedPins,
    ]),
  ]

  const togglePin = (pin: string, checked: boolean) => {
    const newPins = checked
      ? [...selectedPins, pin]
      : selectedPins.filter((selectedPin) => selectedPin !== pin)
    onChange?.({ ...config, Pin: newPins.join("|") })
  }

  return (
    <WizardSection>
      <SectionHeadline
        title={t("ConfigWizard.Device.ShiftRegister.Title")}
        description={t("ConfigWizard.Device.ShiftRegister.Description")}
      />
      <div className="flex flex-col gap-2">
        <Label>{t("ConfigWizard.Device.ShiftRegister.Device")}</Label>
        <OutputDeviceComboBox
          devices={devices}
          deviceType="ShiftRegister"
          selectedName={config.Address}
          onChange={(address) =>
            // pins belong to a shift register,
            // so they are reset when the shift register changes
            onChange?.({ ...config, Address: address, Name: address, Pin: "" })
          }
        />
      </div>
      <Separator />
      <div className="flex flex-col gap-2">
        <Label>{t("ConfigWizard.Device.ShiftRegister.List")}</Label>
        <div className="grid grid-cols-4 gap-2">
          {pins.map((pin) => {
            const id = `shift-register-pin-${pin.replace(/\s+/g, "-")}`
            return (
              <div key={pin} className="flex flex-row items-center gap-2">
                <Checkbox
                  id={id}
                  checked={selectedPins.includes(pin)}
                  onCheckedChange={(checked) => togglePin(pin, !!checked)}
                />
                <Label htmlFor={id}>{pin}</Label>
              </div>
            )
          })}
        </div>
      </div>
    </WizardSection>
  )
}

export default ShiftRegisterPanel
