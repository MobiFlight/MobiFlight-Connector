import DeviceIcon from "@/components/icons/DeviceIcon"
import { OutputConfig } from "@/types/config"
import { BaseDevice, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"
import { Checkbox } from "@/components/ui/checkbox"
import { Slider } from "@/components/ui/slider"
import { Switch } from "@/components/ui/switch"

export interface OutputPanelProps {
  variant: "summary" | "details"
  config: OutputConfig
  controller?: Partial<Controller>
  // only used by the details variant
  devices?: BaseDevice[]
  onChange?: (config: OutputConfig) => void
}

const OutputPanel = ({
  config,
  variant,
  controller,
  devices = [],
  onChange,
}: OutputPanelProps) => {
  if (variant == "summary") {
    const selectedPins = config.Pin.split("|").filter((pin) => pin !== "")
    return (
      <div className="flex grow flex-row items-center justify-between gap-2">
        <div className="flex flex-col gap-1">
          <Label>Controller</Label>
          {controller?.Name}
        </div>
        <div className="flex flex-col gap-1">
          <Label>Display type</Label>
          <span className="flex flex-row gap-2">
            <DeviceIcon variant="Output" /> Output
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Device</Label>
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
            <Label>Options</Label>
            <Badge variant="secondary">PWM Mode On</Badge>
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

  return (
    <Card>
      <CardContent className="flex flex-col gap-4 pt-4">
        <div className="flex flex-col gap-1">
          <div className="text-lg font-semibold">Step 2 - Output settings</div>
          <div className="text-muted-foreground text-sm">
            Select one or more pins and configure the brightness.
          </div>
        </div>
        <div className="flex flex-col gap-2">
          <Label>Pins</Label>
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
        <div className="flex flex-row items-center gap-2">
          <Switch
            id="output-pwm-mode"
            checked={config.PwmMode}
            onCheckedChange={(checked) =>
              onChange?.({ ...config, PwmMode: !!checked })
            }
          />
          <Label htmlFor="output-pwm-mode">PWM Mode</Label>
        </div>
        <div className="flex flex-col gap-2">
          <Label>Brightness ({config.Brightness})</Label>
          <Slider
            min={0}
            max={255}
            step={1}
            disabled={!config.PwmMode}
            value={[config.Brightness]}
            onValueChange={([brightness]) =>
              onChange?.({ ...config, Brightness: brightness })
            }
          />
        </div>
      </CardContent>
    </Card>
  )
}

export default OutputPanel
