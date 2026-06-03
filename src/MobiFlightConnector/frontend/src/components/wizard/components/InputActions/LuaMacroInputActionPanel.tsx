import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
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
          Invoke your Lua macro with an optional parameter. Requires an existing Lua macro in FSUIPC.
        </div>
      </div>
      <div className="flex flex-col gap-2">
      <Label htmlFor="macroName">Macro Name:</Label>
      <Input
        id="macroName"
        placeholder="Set macro name"
        value={config?.MacroName ?? ""}
        onChange={(e) =>
          onConfigChange({
            ...(config as LuaMacroInputAction),
            MacroName: e.target.value,
          } as LuaMacroInputAction)
        }
      />
      </div>
      <div className="flex flex-col gap-2">
      <Label htmlFor="macroValue">Macro Value:</Label>
      <Input
        id="macroValue"
        placeholder="Set macro value"
        value={config?.MacroValue ?? ""}
        onChange={(e) =>
          onConfigChange({
            ...(config as LuaMacroInputAction),
            MacroValue: e.target.value,
          } as LuaMacroInputAction)
        }
      />
      </div>
    </div>
  )
}
export default LuaMacroInputActionPanel