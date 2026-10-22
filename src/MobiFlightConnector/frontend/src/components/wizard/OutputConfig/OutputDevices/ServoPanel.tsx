import DeviceIcon from "@/components/icons/DeviceIcon"
import { ServoConfig } from "@/types/config"
import { Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"

export interface ServoPanelProps {
  variant: "summary" | "details"
  config: ServoConfig
  controller?: Partial<Controller>
}

const ServoPanel = ({ config, variant, controller }: ServoPanelProps) => {
  // TODO: replace placeholder with the actual Servo settings
  if (variant == "details")
    return (
      <Card>
        <CardContent className="flex flex-col gap-1 pt-4">
          <div className="text-lg font-semibold">Step 2 - Servo settings</div>
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
