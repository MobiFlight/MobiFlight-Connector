import DeviceIcon from "@/components/icons/DeviceIcon"
import { LedModuleConfig } from "@/types/config"
import { BaseDevice, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"
import { Checkbox } from "@/components/ui/checkbox"
import ComboBox from "@/components/ComboBox"
import OutputDeviceComboBox from "@/components/wizard/OutputConfig/OutputDevices/OutputDeviceComboBox"

// TODO: use the number of chained modules and the max digits provided by the backend
const MAX_CONNECTORS = 8
const MAX_DIGITS = 8

type Option = { label: string; value: string }

const connectorOptions: Option[] = Array.from(
  { length: MAX_CONNECTORS },
  (_, index) => ({ label: `${index + 1}`, value: `${index + 1}` }),
)

const moduleSizeOptions: Option[] = Array.from(
  { length: MAX_DIGITS },
  (_, index) => ({ label: `${index + 1}`, value: `${index + 1}` }),
)

const paddingCharOptions: Option[] = [
  { label: "0", value: "0" },
  { label: "Space", value: " " },
]

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
  devices?: BaseDevice[]
  onChange?: (config: LedModuleConfig) => void
}

const LedModulePanel = ({
  config,
  variant,
  controller,
  devices = [],
  onChange,
}: LedModulePanelProps) => {
  if (variant == "details") {
    // digit 0 is the rightmost digit on the display
    const digitIndexes = Array.from(
      { length: config.DisplayLedModuleSize },
      (_, index) => `${index}`,
    ).reverse()

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

    return (
      <Card>
        <CardContent className="flex flex-col gap-4 pt-4">
          <div className="flex flex-col gap-1">
            <div className="text-lg font-semibold">
              Step 2 - 7-Segment settings
            </div>
            <div className="text-muted-foreground text-sm">
              Select the module and configure which digits show the value.
            </div>
          </div>
          <div className="flex flex-row gap-4">
            <div className="flex flex-col gap-2">
              <Label>Module</Label>
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
              <Label>Connector</Label>
              <OptionComboBox
                options={connectorOptions}
                value={`${config.DisplayLedConnector}`}
                onChange={(value) =>
                  onChange?.({ ...config, DisplayLedConnector: Number(value) })
                }
              />
            </div>
            <div className="flex flex-col gap-2">
              <Label>Digits</Label>
              <OptionComboBox
                options={moduleSizeOptions}
                value={`${config.DisplayLedModuleSize}`}
                onChange={updateModuleSize}
              />
            </div>
          </div>
          <div className="flex flex-col gap-2">
            <Label>Use digits</Label>
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
            <Label>Decimal points</Label>
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
          <div className="flex flex-row items-center gap-4">
            <div className="flex flex-row items-center gap-2">
              <Checkbox
                id="led-padding"
                checked={config.DisplayLedPadding}
                onCheckedChange={(checked) =>
                  onChange?.({ ...config, DisplayLedPadding: !!checked })
                }
              />
              <Label htmlFor="led-padding">Padding with</Label>
            </div>
            <OptionComboBox
              options={paddingCharOptions}
              value={config.DisplayLedPaddingChar}
              onChange={(value) =>
                onChange?.({ ...config, DisplayLedPaddingChar: value })
              }
            />
            <div className="flex flex-row items-center gap-2">
              <Checkbox
                id="led-reverse-digits"
                checked={config.DisplayLedReverseDigits}
                onCheckedChange={(checked) =>
                  onChange?.({ ...config, DisplayLedReverseDigits: !!checked })
                }
              />
              <Label htmlFor="led-reverse-digits">Reverse digits</Label>
            </div>
          </div>
        </CardContent>
      </Card>
    )
  }

  return (
    variant == "summary" && (
      <div className="flex grow flex-row items-center justify-between gap-2">
        <div className="flex flex-col gap-1">
          <Label>Controller</Label>
          {controller?.Name}
        </div>
        <div className="flex flex-col gap-1">
          <Label>Display type</Label>
          <span className="flex flex-row gap-2">
            <DeviceIcon variant="LedModule" /> 7-Segment
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Device</Label>
          <Badge variant="secondary">{config.Name}</Badge>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Digits</Label>
          <Badge variant="secondary">{config.DisplayLedModuleSize}</Badge>
        </div>
        {(config.DisplayLedPadding || config.DisplayLedReverseDigits) && (
          <div className="flex flex-col gap-1">
            <Label>Options</Label>
            <span className="flex flex-row gap-1">
              {config.DisplayLedPadding && (
                <Badge variant="secondary">Padding</Badge>
              )}
              {config.DisplayLedReverseDigits && (
                <Badge variant="secondary">Reversed</Badge>
              )}
            </span>
          </div>
        )}
      </div>
    )
  )
}

export default LedModulePanel
