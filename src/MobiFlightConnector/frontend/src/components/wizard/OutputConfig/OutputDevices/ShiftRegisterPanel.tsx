import DeviceIcon from "@/components/icons/DeviceIcon"
import { ShiftRegisterConfig } from "@/types/config"
import { Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"

export interface ShiftRegisterPanelProps {
  variant: "summary" | "details"
  config: ShiftRegisterConfig
  controller?: Partial<Controller>
}

const ShiftRegisterPanel = ({
  config,
  variant,
  controller,
}: ShiftRegisterPanelProps) => {
  // TODO: replace placeholder with the actual Shift Register settings
  if (variant == "details")
    return (
      <Card>
        <CardContent className="flex flex-col gap-1 pt-4">
          <div className="text-lg font-semibold">
            Step 2 - Shift Register settings
          </div>
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
            <DeviceIcon variant="ShiftRegister" /> Shift Register
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Device</Label>
          <Badge variant="secondary">
            {config.Name} ({config.Pin})
          </Badge>
        </div>
        {config.PWM && (
          <div className="flex flex-col gap-1">
            <Label>Options</Label>
            <Badge variant="secondary">PWM Mode On</Badge>
          </div>
        )}
      </div>
    )
  )
}

export default ShiftRegisterPanel
