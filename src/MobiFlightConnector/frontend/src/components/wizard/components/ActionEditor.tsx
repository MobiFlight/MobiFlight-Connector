import ActionTypeComboBox from "@/components/wizard/components/ActionTypeComboBox"
import { Action } from "@/types/config"

export interface ActionEditorProps {
  action?: Action
  onActionChange: (item: Action) => void
}

const ActionEditor = ({ action, onActionChange }: ActionEditorProps) => {
  return (
    <>
      <div>ActionEditor</div>
      <ActionTypeComboBox />
    </>
  )
}
export default ActionEditor
