import DeviceIcon from "@/components/icons/DeviceIcon"
import { OutputConfig } from "@/types/config"
import { Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"

export interface OutputPanelProps {
  variant: "summary" | "details"
  config: OutputConfig
  controller?: Partial<Controller>
}

const OutputPanel = ({ config, variant, controller }: OutputPanelProps) => {
  if (variant == "summary")
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
          <Badge variant="secondary">
            {config.Name} ({config.Pin})
          </Badge>
        </div>
        {config.PwmMode && (
          <div className="flex flex-col gap-1">
            <Label>Options</Label>
            <Badge variant="secondary">PWM Mode On</Badge>
          </div>
        )}
      </div>
    )

  // TODO: replace placeholder with the actual Output settings
  return (
    <Card>
      <CardContent className="flex flex-col gap-1 pt-4">
        <div className="text-lg font-semibold">Step 2 - Output settings</div>
        <div className="text-muted-foreground text-sm">
          Pin: {config.Pin || "-"} (details not implemented yet)
        </div>
      </CardContent>
    </Card>
  )
}

export default OutputPanel
