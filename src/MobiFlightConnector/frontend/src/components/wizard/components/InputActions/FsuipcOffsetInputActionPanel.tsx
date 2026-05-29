import { FsuipcOffsetInputAction } from "@/types/config"

export type FsuipcOffsetInputActionPanelProps = {
  config: FsuipcOffsetInputAction | null
  onConfigChange: (config: FsuipcOffsetInputAction) => void
}

const FsuipcOffsetInputActionPanel = ({
  config,
  onConfigChange,
}: FsuipcOffsetInputActionPanelProps) => {

  const availablePresets: FsuipcOffsetInputAction[] = [
  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">FSUIPC Offset</div>
        <div className="text-muted-foreground text-sm">Select an offset</div>
      </div>
      <div>

      </div>
    </div>
  )
}
export default FsuipcOffsetInputActionPanel
