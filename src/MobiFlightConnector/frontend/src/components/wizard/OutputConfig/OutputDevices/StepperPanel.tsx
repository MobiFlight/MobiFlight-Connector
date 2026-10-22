import DeviceIcon from "@/components/icons/DeviceIcon"
import { StepperConfig } from "@/types/config"
import { BaseDevice, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Switch } from "@/components/ui/switch"
import ComboBox from "@/components/ComboBox"

export interface StepperPanelProps {
  variant: "summary" | "details"
  config: StepperConfig
  controller?: Partial<Controller>
  // only used by the details variant
  devices?: BaseDevice[]
  onChange?: (config: StepperConfig) => void
}

type StepperNumberField =
  | "InputRev"
  | "OutputRev"
  | "TestValue"
  | "Acceleration"
  | "Speed"

const numberFields: { field: StepperNumberField; label: string }[] = [
  { field: "InputRev", label: "Input revolution" },
  { field: "OutputRev", label: "Output revolution" },
  { field: "TestValue", label: "Test value" },
  { field: "Acceleration", label: "Acceleration" },
  { field: "Speed", label: "Speed" },
]

const StepperPanel = ({
  config,
  variant,
  controller,
  devices = [],
  onChange,
}: StepperPanelProps) => {
  if (variant == "details") {
    // Keep the configured stepper visible
    // even if it is currently not available on the controller
    const deviceOptions: BaseDevice[] =
      config.Address &&
      !devices.some((device) => device.Name === config.Address)
        ? [
            ...devices,
            { Name: config.Address, Label: config.Address, Type: "Stepper" },
          ]
        : devices

    const selectedDevice = deviceOptions.find(
      (device) => device.Name === config.Address,
    )

    return (
      <Card>
        <CardContent className="flex flex-col gap-4 pt-4">
          <div className="flex flex-col gap-1">
            <div className="text-lg font-semibold">
              Step 2 - Stepper settings
            </div>
            <div className="text-muted-foreground text-sm">
              Select the stepper and configure how values map to its movement.
            </div>
          </div>
          <div className="flex flex-col gap-2">
            <Label>Stepper</Label>
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
          <div className="grid grid-cols-3 gap-4">
            {numberFields.map(({ field, label }) => (
              <div key={field} className="flex flex-col gap-2">
                <Label htmlFor={`stepper-${field}`}>{label}</Label>
                <Input
                  id={`stepper-${field}`}
                  type="number"
                  value={config[field]}
                  onChange={(e) =>
                    onChange?.({ ...config, [field]: Number(e.target.value) })
                  }
                />
              </div>
            ))}
          </div>
          <div className="flex flex-row items-center gap-2">
            <Switch
              id="stepper-compass-mode"
              checked={config.CompassMode}
              onCheckedChange={(checked) =>
                onChange?.({ ...config, CompassMode: !!checked })
              }
            />
            <Label htmlFor="stepper-compass-mode">Compass mode</Label>
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
            <DeviceIcon variant="Stepper" /> Stepper
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Device</Label>
          <Badge variant="secondary">{config.Name}</Badge>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Steps</Label>
          <Badge variant="secondary">
            {config.InputRev} / {config.OutputRev}
          </Badge>
        </div>
        {config.CompassMode && (
          <div className="flex flex-col gap-1">
            <Label>Options</Label>
            <Badge variant="secondary">Compass Mode</Badge>
          </div>
        )}
      </div>
    )
  )
}

export default StepperPanel
