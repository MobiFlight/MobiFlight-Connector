import DeviceIcon from "@/components/icons/DeviceIcon"
import { LedModuleConfig } from "@/types/config"
import { Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"

export interface LedModulePanelProps {
  variant: "summary" | "details"
  config: LedModuleConfig
  controller?: Partial<Controller>
}

const LedModulePanel = ({
  config,
  variant,
  controller,
}: LedModulePanelProps) => {
  // TODO: replace placeholder with the actual 7-Segment settings
  if (variant == "details")
    return (
      <Card>
        <CardContent className="flex flex-col gap-1 pt-4">
          <div className="text-lg font-semibold">
            Step 2 - 7-Segment settings
          </div>
          <div className="text-muted-foreground text-sm">
            Address: {config.DisplayLedAddress || "-"} (details not implemented
            yet)
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
