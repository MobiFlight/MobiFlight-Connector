import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import ProSimDataRefPanel from "@/components/wizard/components/InputActions/ProsimDataRefPanel"
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
        <div className="text-lg font-semibold">ProSim Input Action</div>
        <div className="text-muted-foreground text-sm">
          Select a preset to configure your input actions ass
        </div>
      </div>
      <ProSimDataRefPanel
        variant="input"
        selectedPath={config?.Path ?? null}
        onPresetChange={(preset) =>
          onConfigChange({
            ...(config as ProSimInputAction),
            Path: preset.Name,
          } as ProSimInputAction)
        }
      />
      <Separator />
      <Label htmlFor="path">Path:</Label>
      <div id="path" className="rounded border p-2 text-sm">
        {(config?.Path !== "" ? config?.Path : "No preset selected")}
      </div>
      <Label htmlFor="param">Parameter (optional):</Label>
      <Input
        id="param"
        placeholder="Set parameter (optional)"
        value={config?.Expression ?? ""}
        onChange={(e) =>
          onConfigChange({
            ...(config as ProSimInputAction),
            Expression: e.target.value,
          } as ProSimInputAction)
        }
      />
    </div>
  )
}
export default ProSimInputActionPanel
