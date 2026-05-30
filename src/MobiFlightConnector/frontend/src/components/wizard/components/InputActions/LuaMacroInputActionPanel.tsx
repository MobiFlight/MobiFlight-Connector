import { LuaMacroInputAction } from "@/types/config"

export type LuaMacroInputActionPanelProps = {
  config: LuaMacroInputAction | null
  onConfigChange: (config: LuaMacroInputAction) => void
}

const LuaMacroInputActionPanel = ({
  config,
  onConfigChange,
}: LuaMacroInputActionPanelProps) => {
  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">
          Lua Macro Input Action
        </div>
        <div className="text-muted-foreground text-sm">
          Select a preset to configure your input actions
        </div>
      </div>
    </div>
  )
}
export default LuaMacroInputActionPanel