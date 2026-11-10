import DeviceIcon from "@/components/icons/DeviceIcon"
import { LedModuleConfig } from "@/types/config"
import { DeviceReference, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Checkbox } from "@/components/ui/checkbox"
import ComboBox from "@/components/ComboBox"
import OutputDeviceComboBox from "@/components/wizard/OutputConfig/OutputDevices/OutputDeviceComboBox"
import { Separator } from "@/components/ui/separator"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import { useTranslation } from "react-i18next"

type Option = { label: string; value: string }

const OptionComboBox = ({
  options,
  value,
  onChange,
}: {
  options: Option[]
  value: string
  onChange: (value: string) => void
}) => (
  <ComboBox
    widthClass="w-24"
    align="start"
    variant="nofilter"
    items={options}
    getLabel={(option) => option.label}
    getValue={(option) => option.value}
    isSelected={(option, selected) => option.value === selected?.value}
    selected={options.find((option) => option.value === value)}
    setSelected={(option) => option && onChange(option.value)}
  />
)

export interface LedModulePanelProps {
  variant: "summary" | "details"
  config: LedModuleConfig
  controller?: Partial<Controller>
  // only used by the details variant
  devices?: DeviceReference[]
  onChange?: (config: LedModuleConfig) => void
}

const LedModulePanel = ({
  config,
  variant,
  controller,
  devices = [],
  onChange,
}: LedModulePanelProps) => {
  const { t } = useTranslation("wizard")

  const paddingCharOptions: Option[] = [
    { label: "0", value: "0" },
    {
      label: t("ConfigWizard.Device.SevenSegment.AdvancedOptions.PaddingSpace"),
      value: " ",
    },
  ]
  const selectedDevice = devices.find(
    (device) => device.Name === config.DisplayLedAddress,
  )

  const currentModuleSize = Math.min(
    config.DisplayLedModuleSize,
    selectedDevice?.Capabilities?.Digits ?? 8,
  )

  if (variant == "details") {
    // digit 0 is the rightmost digit on the display
    const digitIndexes = Array.from(
      { length: config.DisplayLedModuleSize },
      (_, index) => `${index}`,
    )

    const toggle = (list: string[], digit: string, checked: boolean) =>
      checked ? [...list, digit] : list.filter((item) => item !== digit)

    const updateModuleSize = (value: string) => {
      const size = Number(value)
      // drop digits that are no longer visible
      const isVisible = (digit: string) => Number(digit) < size
      onChange?.({
        ...config,
        DisplayLedModuleSize: size,
        DisplayLedDigits: config.DisplayLedDigits.filter(isVisible),
        DisplayLedDecimalPoints:
          config.DisplayLedDecimalPoints.filter(isVisible),
      })
    }

    const moduleSizeOptions: Option[] = Array.from(
      { length: selectedDevice?.Capabilities?.Digits ?? 8 },
      (_, index) => ({ label: `${index + 1}`, value: `${index + 1}` }),
    )

    return (
      <WizardSection>
        <SectionHeadline
          title={t("ConfigWizard.Device.SevenSegment.Title")}
          description={t("ConfigWizard.Device.SevenSegment.Description")}
        />
        <div className="flex flex-row gap-4">
          <div className="flex flex-col gap-2">
            <Label>{t("ConfigWizard.Device.SevenSegment.Display")}</Label>
            <OutputDeviceComboBox
              devices={devices}
              deviceType="LedModule"
              selectedName={config.DisplayLedAddress}
              onChange={(address) =>
                onChange?.({
                  ...config,
                  DisplayLedAddress: address,
                  Name: address,
                })
              }
            />
          </div>
          <div className="flex flex-col gap-2">
            <Label>{t("ConfigWizard.Device.SevenSegment.Digits")}</Label>
            <OptionComboBox
              options={moduleSizeOptions}
              value={`${currentModuleSize}`}
              onChange={updateModuleSize}
            />
          </div>
        </div>
        <Separator />
        <div className="flex flex-col gap-2">
          <Label>{t("ConfigWizard.Device.SevenSegment.UseDigits")}</Label>
          <div className="text-muted-foreground flex flex-row gap-3 text-xs">
            {digitIndexes.map((digit) => (
              <span key={digit} className="w-4 text-center">
                {Number(digit) + 1}
              </span>
            ))}
          </div>
          <div className="flex flex-row gap-3">
            {digitIndexes.map((digit) => (
              <Checkbox
                key={digit}
                aria-label={`Digit ${digit}`}
                checked={config.DisplayLedDigits.includes(digit)}
                onCheckedChange={(checked) =>
                  onChange?.({
                    ...config,
                    DisplayLedDigits: toggle(
                      config.DisplayLedDigits,
                      digit,
                      !!checked,
                    ),
                  })
                }
              />
            ))}
          </div>
        </div>
        <div className="flex flex-col gap-2">
          <Label>{t("ConfigWizard.Device.SevenSegment.DecimalPoints")}</Label>
          <div className="flex flex-row gap-3">
            {digitIndexes.map((digit) => (
              <Checkbox
                key={digit}
                aria-label={`Decimal point ${digit}`}
                checked={config.DisplayLedDecimalPoints.includes(digit)}
                onCheckedChange={(checked) =>
                  onChange?.({
                    ...config,
                    DisplayLedDecimalPoints: toggle(
                      config.DisplayLedDecimalPoints,
                      digit,
                      !!checked,
                    ),
                  })
                }
              />
            ))}
          </div>
        </div>
        <Separator />
        <div className="flex flex-col gap-2">
          <Label>{t("ConfigWizard.Device.SevenSegment.AdvancedOptions.Title")}</Label>
          <div className="text-muted-foreground text-sm">
            {t("ConfigWizard.Device.SevenSegment.AdvancedOptions.Description")}
          </div>
        </div>
        <div className="flex flex-row items-center gap-4">
          <div className="flex flex-row items-center gap-2">
            <Checkbox
              id="led-padding"
              checked={config.DisplayLedPadding}
              onCheckedChange={(checked) =>
                onChange?.({ ...config, DisplayLedPadding: !!checked })
              }
            />
            <Label htmlFor="led-padding">{t("ConfigWizard.Device.SevenSegment.AdvancedOptions.Padding")}</Label>
          </div>
          <OptionComboBox
            options={paddingCharOptions}
            value={config.DisplayLedPaddingChar}
            onChange={(value) =>
              onChange?.({ ...config, DisplayLedPaddingChar: value })
            }
          />
          <Separator orientation="vertical" className="h-6" />
          <div className="flex flex-row items-center gap-2">
            <Checkbox
              id="led-reverse-digits"
              checked={config.DisplayLedReverseDigits}
              onCheckedChange={(checked) =>
                onChange?.({ ...config, DisplayLedReverseDigits: !!checked })
              }
            />
            <Label htmlFor="led-reverse-digits">{t("ConfigWizard.Device.SevenSegment.AdvancedOptions.ReverseDigits")}</Label>
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
            <DeviceIcon variant="LedModule" />{t("ConfigWizard.Device.SevenSegment.Label")}
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.Device")}</Label>
          <Badge variant="secondary">{config.Name}</Badge>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.Device.SevenSegment.Digits")}</Label>
          <Badge variant="secondary">{config.DisplayLedModuleSize}</Badge>
        </div>
        {(config.DisplayLedPadding || config.DisplayLedReverseDigits) && (
          <div className="flex flex-col gap-1">
            <Label>{t("ConfigWizard.General.Options")}</Label>
            <span className="flex flex-row gap-1">
              {config.DisplayLedPadding && (
                <Badge variant="secondary">{t("ConfigWizard.Device.SevenSegment.AdvancedOptions.Padding")}</Badge>
              )}
              {config.DisplayLedReverseDigits && (
                <Badge variant="secondary">{t("ConfigWizard.Device.SevenSegment.AdvancedOptions.ReverseDigits")}</Badge>
              )}
            </span>
          </div>
        )}
      </div>
    )
  )
}

export default LedModulePanel
