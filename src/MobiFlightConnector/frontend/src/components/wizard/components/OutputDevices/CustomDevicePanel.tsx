import DeviceIcon from "@/components/icons/DeviceIcon"
import { CustomDeviceConfig } from "@/types/config"
import { Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"

export interface CustomDevicePanelProps {
  variant: "summary" | "details"
  config: CustomDeviceConfig
  controller?: Partial<Controller>
}

const CustomDevicePanel = ({
  config,
  variant,
  controller,
}: CustomDevicePanelProps) => {
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
            <DeviceIcon variant="CustomDevice" /> Custom Device
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Device</Label>
          <Badge variant="secondary">{config.CustomName || config.Name}</Badge>
        </div>
        {config.CustomType && (
          <div className="flex flex-col gap-1">
            <Label>Type</Label>
            <Badge variant="secondary">{config.CustomType}</Badge>
          </div>
        )}
      </div>
    )
  )
}

export default CustomDevicePanel
