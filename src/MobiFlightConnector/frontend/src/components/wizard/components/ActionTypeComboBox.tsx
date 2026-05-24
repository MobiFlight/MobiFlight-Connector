import ComboBox from "@/components/ComboBox"

export type  ActionTypeOption = {
  value: string
  label: string
}

const ActionTypeOptions: ActionTypeOption[] = [
  { value: "msfs", label: "Microsoft Flight Simulator" },
  { value: "variable", label: "MobiFlight Variable" },
  { value: "retrigger", label: "Retrigger Switches" },
  { value: "keyboard", label: "Keyboard input" },
  { value: "vJoy", label: "vJoy input" },
]

export type ActionTypeProps = {
  selectedActionType?: ActionTypeOption
  setSelectedActionType?: (option: ActionTypeOption | undefined) => void
}

const ActionTypeComboBox = ({ selectedActionType, setSelectedActionType }: ActionTypeProps) => {
  return (
    <div className="flex flex-row gap-2 items-center">
      <div>Action Type</div>
      <ComboBox
        selected={selectedActionType}
        items={ActionTypeOptions}
        getLabel={(item) => item.label}
        getValue={(item) => item.value}
        isSelected={(item) => item.value === selectedActionType?.value}
        setSelected={(item) => {
          setSelectedActionType?.(item || undefined)
        }}
      />
    </div>
  )
}
export default ActionTypeComboBox
