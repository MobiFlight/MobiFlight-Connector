import DeviceIcon from "@/components/icons/DeviceIcon"
import { ServoConfig } from "@/types/config"
import { BaseDevice, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import ComboBox from "@/components/ComboBox"

export interface ServoPanelProps {
  variant: "summary" | "details"
  config: ServoConfig
  controller?: Partial<Controller>
  // only used by the details variant
  devices?: BaseDevice[]
  onChange?: (config: ServoConfig) => void
}

const ServoPanel = ({
  config,
  variant,
  controller,
  devices = [],
  onChange,
}: ServoPanelProps) => {
  if (variant == "details") {
    // Keep the configured servo visible
    // even if it is currently not available on the controller
    const deviceOptions: BaseDevice[] =
      config.Address &&
      !devices.some((device) => device.Name === config.Address)
        ? [
            ...devices,
            { Name: config.Address, Label: config.Address, Type: "Servo" },
          ]
        : devices

    const selectedDevice = deviceOptions.find(
      (device) => device.Name === config.Address,
    )

    return (
      <Card>
        <CardContent className="flex flex-col gap-4 pt-4">
          <div className="flex flex-col gap-1">
            <div className="text-lg font-semibold">Step 2 - Servo settings</div>
            <div className="text-muted-foreground text-sm">
              Select the servo and map the value range to its rotation.
            </div>
          </div>
          <div className="flex flex-col gap-2">
            <Label>Servo</Label>
            <ComboBox
              widthClass="w-56"
              align="start"
              items={deviceOptions}
              getLabel={(device) => device.Label ?? device.Name}
              getValue={(device) => device.Name}
              isSelected={(device, selected) => device.Name === selected?.Name}
              selected={selectedDevice}
              setSelected={(device) => {
                const address = device?.Name ?? ""
                onChange?.({ ...config, Address: address, Name: address })
              }}
            />
          </div>
          <div className="flex flex-row gap-4">
            <div className="flex flex-col gap-2">
              <Label htmlFor="servo-min">Min value</Label>
              <Input
                id="servo-min"
                type="number"
                value={config.Min}
                onChange={(e) => onChange?.({ ...config, Min: e.target.value })}
              />
            </div>
            <div className="flex flex-col gap-2">
              <Label htmlFor="servo-max">Max value</Label>
              <Input
                id="servo-max"
                type="number"
                value={config.Max}
                onChange={(e) => onChange?.({ ...config, Max: e.target.value })}
              />
            </div>
            <div className="flex flex-col gap-2">
              <Label htmlFor="servo-max-rotation">Max rotation (%)</Label>
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
            <DeviceIcon variant="Servo" /> Servo
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Device</Label>
          <Badge variant="secondary">{config.Name}</Badge>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Range</Label>
          <Badge variant="secondary">
            {config.Min} – {config.Max}
          </Badge>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Max rotation</Label>
          <Badge variant="secondary">{config.MaxRotationPercent} %</Badge>
        </div>
      </div>
    )
  )
}

export default ServoPanel
