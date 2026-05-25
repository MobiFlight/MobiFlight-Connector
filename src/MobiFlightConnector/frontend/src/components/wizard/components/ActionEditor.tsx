import ActionTypeComboBox, {
  ActionTypeOptions,
} from "@/components/wizard/components/ActionTypeComboBox"
import MsfsPresetPanel from "@/components/wizard/components/InputActions/MsfsPresetPanel"
import { Action, MsfsInputAction } from "@/types/config"

export interface ActionEditorProps {
  action?: Action
  onActionChange: (item: Action) => void
}

const ActionEditor = ({ action, onActionChange }: ActionEditorProps) => {
  console.log("Current Action in Editor:", action)
  const selectedActionType = action
    ? ActionTypeOptions.find((option) => option.value === action.Type)
    : undefined
  return (
    <div className="flex flex-col gap-4">
      <ActionTypeComboBox
        selectedActionType={selectedActionType}
        setSelectedActionType={(option) => {
          if (option) {
            onActionChange({ ...action, Type: option.value })
          }
        }}
      />
      {selectedActionType?.value === "MSFS2020CustomInputAction" && (
        <MsfsPresetPanel
          selectedPresetId={action ? (action as MsfsInputAction).PresetId : null}
          setSelectedPreset={(preset) => {
            onActionChange({
              ...(action as MsfsInputAction),
              PresetId: preset ? preset.id : null,
            } as MsfsInputAction)
          }}
        />
      )}
    </div>
  )
}
export default ActionEditor
