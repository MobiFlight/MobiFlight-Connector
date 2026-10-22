import DeviceIcon from "@/components/icons/DeviceIcon"
import { LcdDisplayConfig } from "@/types/config"
import { Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"

export interface LcdDisplayPanelProps {
  variant: "summary" | "details"
  config: LcdDisplayConfig
  controller?: Partial<Controller>
}

const LcdDisplayPanel = ({
  config,
  variant,
  controller,
}: LcdDisplayPanelProps) => {
  // TODO: replace placeholder with the actual LCD Display settings
  if (variant == "details")
    return (
      <Card>
        <CardContent className="flex flex-col gap-1 pt-4">
          <div className="text-lg font-semibold">
            Step 2 - LCD Display settings
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
            <DeviceIcon variant="LcdDisplay" /> LCD Display
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Device</Label>
          <Badge variant="secondary">{config.Name}</Badge>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Lines</Label>
          <Badge variant="secondary">{config.Lines?.length ?? 0}</Badge>
        </div>
      </div>
    )
  )
}

export default LcdDisplayPanel
