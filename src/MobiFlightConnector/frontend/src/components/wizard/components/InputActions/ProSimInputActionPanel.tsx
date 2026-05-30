import { ProSimInputAction } from "@/types/config"

export type ProSimInputActionPanelProps = {
  config: ProSimInputAction | null
  onConfigChange: (config: ProSimInputAction) => void
}

const ProSimInputActionPanel = ({
  config,
  onConfigChange,
}: ProSimInputActionPanelProps) => {
  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">
          ProSim Input Action
        </div>
        <div className="text-muted-foreground text-sm">
          Select a preset to configure your input actions
        </div>
      </div>
    </div>
  )
}
export default ProSimInputActionPanel
