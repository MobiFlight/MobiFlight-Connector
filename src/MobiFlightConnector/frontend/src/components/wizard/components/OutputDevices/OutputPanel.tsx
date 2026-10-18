import DeviceIcon from "@/components/icons/DeviceIcon"
import { OutputConfig } from "@/types/config"
import { Button } from "@/components/ui/button"
import { IconEdit } from "@tabler/icons-react"
import { Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"

export interface OutputPanelProps {
  variant: "summary" | "details"
  config: OutputConfig
  controller?: Partial<Controller>
}

const OutputPanel = ({ config, variant, controller }: OutputPanelProps) => {
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
        <Button size={"sm"} variant="ghost" onClick={() => {}}>
          <IconEdit size={16} />
        </Button>
      </div>
    )
  )
}

export default OutputPanel
