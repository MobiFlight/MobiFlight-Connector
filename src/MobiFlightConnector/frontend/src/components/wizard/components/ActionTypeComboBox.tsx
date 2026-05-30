import ComboBox from "@/components/ComboBox"

export type  ActionTypeOption = {
  value: string
  label: string
}

export const ActionTypeOptions: ActionTypeOption[] = [
  { value: "MSFS2020CustomInputAction", label: "Microsoft Flight Simulator (all versions)" },
  { value: "XplaneInputAction", label: "X-Plane (all versions)" },
  { value: "ProSimInputAction", label: "ProSim" },
  { value: "VariableInputAction", label: "MobiFlight - Variable" },
  { value: "RetriggerInputAction", label: "MobiFlight - Retrigger switches" },
  { value: "KeyInputAction", label: "MobiFlight - Keyboard Input" },
  { value: "VJoyInputAction", label: "MobiFlight - Virtual Joystick input (vJoy)" },
  { value: "FsuipcOffsetInputAction", label: "FSUIPC - Offset" },
  { value: "PmdgEventIdInputAction", label: "FSUIPC - PMDG - Event ID" },
  { value: "LuaMacroInputAction", label: "FSUIPC - Lua Macro" },
  { value: "JeehellInputAction", label: "FSUIPC - Jeehell - Events" },
  { value: "EventIdInputAction", label: "FSUIPC - EventID" },
]

export type ActionTypeProps = {
  selectedActionType?: ActionTypeOption
  setSelectedActionType?: (option: ActionTypeOption | undefined) => void
}

const ActionTypeComboBox = ({ selectedActionType, setSelectedActionType }: ActionTypeProps) => {
  console.log("Selected Action Type:", selectedActionType)
  return (
    <div className="flex flex-col gap-2">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">
          Action Type
        </div>
        <div className="text-muted-foreground text-sm">
          Select the type of action you want to perform
        </div>
      </div>
      <ComboBox
        selected={selectedActionType}
        items={ActionTypeOptions}
        getLabel={(item) => item.label}
        getValue={(item) => item.value}
        isSelected={(item) => item.value === selectedActionType?.value}
        setSelected={(item) => {
          setSelectedActionType?.(item || undefined)
        }}
        widthClass="w-100"
      />
    </div>
  )
}
export default ActionTypeComboBox
