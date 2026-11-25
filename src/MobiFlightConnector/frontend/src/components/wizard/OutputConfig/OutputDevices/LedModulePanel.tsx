import DeviceIcon from "@/components/icons/DeviceIcon"
import { IConfigItem, LedModuleConfig } from "@/types/config"
import { DeviceReference, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import ComboBox from "@/components/ComboBox"
import OutputDeviceComboBox from "@/components/wizard/OutputConfig/OutputDevices/OutputDeviceComboBox"
import { Separator } from "@/components/ui/separator"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import { useProjectStore } from "@/stores/projectStore"
import { useTranslation } from "react-i18next"
import { SevenSegmentButton } from "@/components/wizard/OutputConfig/OutputDevices/7segment/SevenSegmentButton"
import { Button } from "@/components/ui/button"
import { Switch } from "@/components/ui/switch"
import SettingsRow from "@/components/settings/components/SettingsRow"

type Option = { label: string; value: string }

const OptionComboBox = ({
  options,
  value,
  onChange,
  id,
}: {
  options: Option[]
  value: string
  onChange: (value: string) => void
  id?: string
}) => (
  <ComboBox
    widthClass="w-56"
    align="start"
    variant="nofilter"
    items={options}
    getLabel={(option) => option.label}
    getValue={(option) => option.value}
    isSelected={(option, selected) => option.value === selected?.value}
    selected={options.find((option) => option.value === value)}
    id={id}
    setSelected={(option) => option && onChange(option.value)}
  />
)

export interface LedModulePanelProps {
  variant: "summary" | "details"
  config: LedModuleConfig
  controller?: Partial<Controller>
  // only used by the details variant
  devices?: DeviceReference[]
  // the config of this panel, which can't be its own brightness reference
  configId?: string
  onChange?: (config: LedModuleConfig) => void
}

const LedModulePanel = ({
  config,
  variant,
  controller,
  devices = [],
  configId,
  onChange,
}: LedModulePanelProps) => {
  const { t } = useTranslation("wizard")
  const { project, activeConfigFileIndex } = useProjectStore()

  // any other output config can provide the brightness value
  const brightnessReferences: IConfigItem[] =
    project?.ConfigFiles[activeConfigFileIndex]?.ConfigItems.filter(
      (item) => item.Type === "OutputConfigItem" && item.GUID !== configId,
    ) ?? []
  const brightnessReference = brightnessReferences.find(
    (item) => item.GUID === config.DisplayLedBrightnessReference,
  )
  const noBrightnessReference = {
    GUID: "",
    Name: t("ConfigWizard.Device.SevenSegment.AdvancedOptions.Brightness.None"),
  } as IConfigItem

  const paddingCharOptions: Option[] = [
    {
      label: t(
        "ConfigWizard.Device.SevenSegment.AdvancedOptions.PaddingCharacter.Zero",
      ),
      value: "0",
    },
    {
      label: t(
        "ConfigWizard.Device.SevenSegment.AdvancedOptions.PaddingCharacter.Space",
      ),
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
      { length: currentModuleSize },
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
          <div className="flex flex-row gap-4">
            <div
              role="group"
              aria-label={t("ConfigWizard.Device.SevenSegment.UseDigits")}
              className="flex flex-row gap-0.5"
            >
              {digitIndexes.map((digit) => {
                const digitIndex = (
                  currentModuleSize -
                  1 -
                  Number(digit)
                ).toString()
                return (
                  <SevenSegmentButton
                    key={digit}
                    value={(Number(digit) + 1).toString()}
                    position={Number(digit) + 1}
                    digitActive={config.DisplayLedDigits.includes(digitIndex)}
                    decimalPointActive={config.DisplayLedDecimalPoints.includes(
                      digitIndex,
                    )}
                    onClickSegment={() =>
                      onChange?.({
                        ...config,
                        DisplayLedDigits: toggle(
                          config.DisplayLedDigits,
                          digitIndex,
                          !config.DisplayLedDigits.includes(digitIndex),
                        ),
                      })
                    }
                    onClickDecimalPoint={() =>
                      onChange?.({
                        ...config,
                        DisplayLedDecimalPoints: toggle(
                          config.DisplayLedDecimalPoints,
                          digitIndex,
                          !config.DisplayLedDecimalPoints.includes(digitIndex),
                        ),
                      })
                    }
                  />
                )
              })}
            </div>
            <div className="flex flex-row items-center gap-2">
              <Button
                size={"sm"}
                variant="outline"
                onClick={() =>
                  onChange?.({
                    ...config,
                    DisplayLedDigits:
                      config.DisplayLedDigits.length > 0 ? [] : digitIndexes,
                  })
                }
              >
                Toggle digits
              </Button>
              <Button
                size={"sm"}
                variant="outline"
                onClick={() =>
                  onChange?.({
                    ...config,
                    DisplayLedDecimalPoints: [],
                  })
                }
              >
                Clear decimal points
              </Button>
            </div>
          </div>
        </div>
        <Separator />
        <div className="flex flex-col gap-2">

        <div className="flex flex-col gap-1">
          <Label>
            {t("ConfigWizard.Device.SevenSegment.AdvancedOptions.Title")}
          </Label>
          <div className="text-muted-foreground text-sm">
            {t("ConfigWizard.Device.SevenSegment.AdvancedOptions.Description")}
          </div>
        </div>
        <SettingsRow
          label={t(
            "ConfigWizard.Device.SevenSegment.AdvancedOptions.Padding.Label",
          )}
          description={t(
            "ConfigWizard.Device.SevenSegment.AdvancedOptions.Padding.Description",
          )}
          htmlFor="option-left-padding"
          >
          <Switch
            id="option-left-padding"
            aria-label={t("ConfigWizard.Device.SevenSegment.AdvancedOptions.Padding.Label")}
            checked={config.DisplayLedPadding}
            onCheckedChange={(checked) =>
              onChange?.({ ...config, DisplayLedPadding: !!checked })
            }
            />
        </SettingsRow>
        <SettingsRow
          label={t(
            "ConfigWizard.Device.SevenSegment.AdvancedOptions.PaddingCharacter.Label",
          )}
          description={t(
            "ConfigWizard.Device.SevenSegment.AdvancedOptions.PaddingCharacter.Description",
          )}
          htmlFor="option-padding-character"
          >
          <OptionComboBox
            id="option-padding-character"
            options={paddingCharOptions}
            value={config.DisplayLedPaddingChar}
            onChange={(value) =>
              onChange?.({ ...config, DisplayLedPaddingChar: value })
            }
            />
        </SettingsRow>
        <SettingsRow
          label={t(
            "ConfigWizard.Device.SevenSegment.AdvancedOptions.ReverseDigits.Label",
          )}
          description={t(
            "ConfigWizard.Device.SevenSegment.AdvancedOptions.ReverseDigits.Description",
          )}
          >
          <Switch
            id="option-reverse-digits"
            aria-label={t("ConfigWizard.Device.SevenSegment.AdvancedOptions.ReverseDigits.Label")}
            checked={config.DisplayLedReverseDigits}
            onCheckedChange={(checked) =>
              onChange?.({ ...config, DisplayLedReverseDigits: !!checked })
            }
            />
        </SettingsRow>
        <SettingsRow
          label={t(
            "ConfigWizard.Device.SevenSegment.AdvancedOptions.Brightness.Label",
          )}
          description={t(
            "ConfigWizard.Device.SevenSegment.AdvancedOptions.Brightness.Description",
          )}
          htmlFor="option-brightness"
          >
          <ComboBox
            widthClass="w-56"
            align="end"
            items={[noBrightnessReference, ...brightnessReferences]}
            selected={brightnessReference ?? noBrightnessReference}
            getValue={(item) => item.GUID}
            getLabel={(item) => item.Name}
            isSelected={(item, selected) => item.GUID === selected?.GUID}
            setSelected={(item) =>
              onChange?.({
                ...config,
                DisplayLedBrightnessReference: item?.GUID ?? "",
              })
            }
            />
        </SettingsRow>
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
            <DeviceIcon variant="LedModule" />
            {t("ConfigWizard.Device.SevenSegment.Label")}
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
        {(config.DisplayLedPadding ||
          config.DisplayLedReverseDigits ||
          brightnessReference) && (
          <div className="flex flex-col gap-1">
            <Label>{t("ConfigWizard.General.Options")}</Label>
            <span className="flex flex-row gap-1">
              {config.DisplayLedPadding && (
                <Badge variant="secondary">
                  {t(
                    "ConfigWizard.Device.SevenSegment.AdvancedOptions.Padding.Label",
                  )}
                </Badge>
              )}
              {config.DisplayLedReverseDigits && (
                <Badge variant="secondary">
                  {t(
                    "ConfigWizard.Device.SevenSegment.AdvancedOptions.ReverseDigits.Label",
                  )}
                </Badge>
              )}
              {brightnessReference && (
                <Badge variant="secondary">
                  {t("ConfigWizard.Device.SevenSegment.AdvancedOptions.Brightness.Summary", {
                    name: brightnessReference.Name,
                  })}
                </Badge>
              )}
            </span>
          </div>
        )}
      </div>
    )
  )
}

export default LedModulePanel
