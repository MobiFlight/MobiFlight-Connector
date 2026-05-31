import { Textarea } from "@/components/ui/textarea"
import MsfsPresetPanel from "@/components/wizard/components/InputActions/MsfsPresetPanel"
import { MsfsInputAction } from "@/types/config"

export type MsfsInputActionPanelProps = {
  config: MsfsInputAction | null
  onConfigChange: (config: MsfsInputAction) => void
}

const MsfsInputActionPanel = ({
  config,
  onConfigChange,
}: MsfsInputActionPanelProps) => {

  const command = config?.Command ?? ""
  return (
    <div className="flex flex-col gap-4">
      <MsfsPresetPanel
        variant="input"
        selectedPresetId={config?.PresetId ?? null}
        setSelectedPreset={(preset) =>
          onConfigChange({
            ...(config as MsfsInputAction),
            PresetId: preset ? preset.id : null,
            Command: preset ? preset.code : null,
          } as MsfsInputAction)
        }
      />
      <div className="flex flex-col gap-2">
        <div className="text-md font-semibold">Code:</div>
        <Textarea
          value={
            command
              ? command
              : "None"
          }
        />
        <div>Supports input value (@) and placeholders ($, #, etc.)</div>
      </div>
    </div>
  )
}
export default MsfsInputActionPanel
