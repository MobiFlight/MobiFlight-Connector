import DeviceIcon from "@/components/icons/DeviceIcon"
import { StepperConfig } from "@/types/config"
import { Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"

export interface StepperPanelProps {
  variant: "summary" | "details"
  config: StepperConfig
  controller?: Partial<Controller>
}

const StepperPanel = ({ config, variant, controller }: StepperPanelProps) => {
  // TODO: replace placeholder with the actual Stepper settings
  if (variant == "details")
    return (
      <Card>
        <CardContent className="flex flex-col gap-1 pt-4">
          <div className="text-lg font-semibold">Step 2 - Stepper settings</div>
          <div className="text-muted-foreground text-sm">
            Address: {config.Address || "-"} (details not implemented yet)
          </div>
        </CardContent>
      </Card>
    )

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
