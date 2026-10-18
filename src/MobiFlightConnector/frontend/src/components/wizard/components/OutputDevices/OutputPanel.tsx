import DeviceIcon from "@/components/icons/DeviceIcon"
import { OutputConfig } from "@/types/config"
import { Button } from "@/components/ui/button"
import { IconEdit } from "@tabler/icons-react"

export interface OutputPanelProps {
  variant: "summary" | "details"
  config: OutputConfig
}

const OutputPanel = ({ config, variant }: OutputPanelProps) => {
  return (
    variant == "summary" && (
      <div className="flex flex-row items-center justify-between gap-2">
        <div>
          <DeviceIcon variant="Output" />
        </div>
        <div>{config.Name}</div>
        <div>{config.Pin}</div>
        <div>{config.PwmMode ? "PWM Mode On" : "PWM Mode Off"}</div>
        <Button size={"sm"} variant="ghost" onClick={() => {}}>
          <IconEdit size={16} />
        </Button>
      </div>
    )
  )
}

export default OutputPanel
