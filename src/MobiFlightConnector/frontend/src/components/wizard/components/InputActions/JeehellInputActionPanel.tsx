import { JeehellInputAction } from "@/types/config"

export type JeehellInputActionPanelProps = {
  config: JeehellInputAction | null
  onConfigChange: (config: JeehellInputAction) => void
}

const JeehellInputActionPanel = ({
  config,
  onConfigChange,
}: JeehellInputActionPanelProps) => {
  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">Jeehell Input Action</div>
        <div className="text-muted-foreground text-sm">
          Select a preset to configure your input actions
        </div>
      </div>
    </div>
  )
}
export default JeehellInputActionPanel
