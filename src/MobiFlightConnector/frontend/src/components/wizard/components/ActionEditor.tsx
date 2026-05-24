import ActionTypeComboBox, {
  ActionTypeOptions,
} from "@/components/wizard/components/ActionTypeComboBox"
import { Action } from "@/types/config"

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
    <>
      <ActionTypeComboBox
        selectedActionType={selectedActionType}
        setSelectedActionType={(option) => {
          if (option) {
            onActionChange({ ...action, Type: option.value })
          }
        }}
      />
    </>
  )
}
export default ActionEditor
