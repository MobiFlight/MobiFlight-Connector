import ActionTypeComboBox, {
  ActionTypeOptions,
} from "@/components/wizard/components/ActionTypeComboBox"
import FsuipcOffsetInputActionPanel from "@/components/wizard/components/InputActions/FsuipcOffsetInputActionPanel"
import KeyboardInputActionPanel from "@/components/wizard/components/InputActions/KeyboardInputActionPanel"
import MsfsPresetPanel from "@/components/wizard/components/InputActions/MsfsPresetPanel"
import RetriggerPanel from "@/components/wizard/components/InputActions/RetriggerPanel"
import { VariablePanel } from "@/components/wizard/components/InputActions/VariablePanel"
import VJoyInputActionPanel from "@/components/wizard/components/InputActions/VJoyInputActionPanel"
import {
  Action,
  KeyInputAction,
  MobiFlightVariableAction,
  MsfsInputAction,
  VJoyInputAction,
} from "@/types/config"

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
          selectedPresetId={
            action ? (action as MsfsInputAction).PresetId : null
          }
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
          currentVariable={
            action ? (action as MobiFlightVariableAction).Variable : undefined
          }
          onVariableChange={(variable) => {
            console.log("Selected Variable in Action Editor:", variable)
            onActionChange({
              ...(action as MobiFlightVariableAction),
              Variable: variable,
            } as MobiFlightVariableAction)
          }}
        />
      )}

      {selectedActionType?.value === "RetriggerInputAction" && (
        <RetriggerPanel />
      )}

      {selectedActionType?.value === "VJoyInputAction" && (
        <VJoyInputActionPanel
          config={action ? (action as VJoyInputAction) : null}
          setConfig={(config) => {
            onActionChange({
              ...(action as VJoyInputAction),
              ...config,
            } as VJoyInputAction)
          }}
        />
      )}

      {selectedActionType?.value === "KeyInputAction" && (
        <KeyboardInputActionPanel
          config={action ? (action as KeyInputAction) : null}
          onConfigChange={(config) =>
            onActionChange({
              ...(action as KeyInputAction),
              ...config,
            } as KeyInputAction)
          }
        />
      )}

      {selectedActionType?.value === "FsuipcOffsetInputAction" && (
        <FsuipcOffsetInputActionPanel />
      )}
    </div>
  )
}
export default ActionEditor
