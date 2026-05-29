import ActionTypeComboBox, {
  ActionTypeOptions,
} from "@/components/wizard/components/ActionTypeComboBox"
import MsfsPresetPanel from "@/components/wizard/components/InputActions/MsfsPresetPanel"
import { VariablePanel } from "@/components/wizard/components/InputActions/VariablePanel"
import { Action, MobiFlightVariableAction, MsfsInputAction } from "@/types/config"

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
          variant="input"
          selectedPresetId={action ? (action as MsfsInputAction).PresetId : null}
          setSelectedPreset={(preset) => {
            onActionChange({
              ...(action as MsfsInputAction),
              PresetId: preset ? preset.id : null,
            } as MsfsInputAction)
          }}
        />
      )}
      {selectedActionType?.value === "VariableInputAction" && (
        <VariablePanel
          currentVariable={action ? (action as MobiFlightVariableAction).Variable : undefined}
          onVariableChange={(variable) => {
            console.log("Selected Variable in Action Editor:", variable)
            onActionChange({
              ...(action as MobiFlightVariableAction),
              Variable: variable,
            } as MobiFlightVariableAction)
          }}
        />
      )}
    </div>
  )
}
export default ActionEditor
