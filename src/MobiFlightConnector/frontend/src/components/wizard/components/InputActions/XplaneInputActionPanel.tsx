import ComboBox from "@/components/ComboBox"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import XplanePresetPanel from "@/components/wizard/components/InputActions/XplanePresetPanel"
import { XplaneInputAction } from "@/types/config"

const CODE_TYPE_OPTIONS: ("DataRef" | "Command")[] = ["DataRef", "Command"]

export type XplaneInputActionPanelProps = {
  config: XplaneInputAction | null
  onConfigChange: (config: XplaneInputAction) => void
}

const XplaneInputActionPanel = ({
  config,
  onConfigChange,
}: XplaneInputActionPanelProps) => {
  return (
    <div className="flex flex-col gap-4">
      <XplanePresetPanel
        variant="input"
        selectedPath={config?.Path ?? null}
        onPresetSelect={(preset) =>
          onConfigChange({
            ...(config as XplaneInputAction),
            Path: preset.code,
            InputType: preset.codeType,
          })
        }
      />
      <div className="flex flex-col gap-2">
        <div className="text-md font-semibold">Input Type:</div>
        <ComboBox
          items={CODE_TYPE_OPTIONS}
          selected={(config?.InputType as "DataRef" | "Command") ?? undefined}
          placeholder="Select input type"
          getLabel={(item) => item}
          getValue={(item) => item}
          isSelected={(item) => item === config?.InputType}
          setSelected={(item) => {
            if (!item) return
            onConfigChange({ ...(config as XplaneInputAction), InputType: item })
          }}
          variant="nofilter"
          widthClass="w-48"
        />
      </div>
      <div className="flex flex-col gap-2">
        <Label htmlFor="code">Code:</Label>
        <Textarea
          id="code"
          value={config?.Path ?? ""}
          onChange={(e) =>
            onConfigChange({
              ...(config as XplaneInputAction),
              Path: e.target.value,
            })
          }
          placeholder="Enter path for DataRef or Command, or select a preset above"
        />
        <div className="text-sm text-muted-foreground">
          Supports input value (@) and placeholders ($, #, etc.)
        </div>
      </div>
    </div>
  )
}

export default XplaneInputActionPanel
