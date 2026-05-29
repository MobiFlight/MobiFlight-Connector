import ComboBox from "@/components/ComboBox"
import { Input } from "@/components/ui/input"
import { MobiFlightVariable } from "@/types/config"
export type VariablePanelProps = {
  currentVariable?: MobiFlightVariable
  onVariableChange: (variable: MobiFlightVariable) => void
}

export const VariablePanel = ({
  currentVariable,
  onVariableChange,
}: VariablePanelProps) => {
  const variableTypeOptions = [
    { value: "number", label: "Number" },
    { value: "string", label: "String" },
  ]

  const variable =
    currentVariable ??
    ({
      TYPE: "number",
      Name: "New Variable",
      Text: "",
      Expression: "$",
    } as MobiFlightVariable)

  const availableVariables = [
    {
      TYPE: "number",
      Name: "Variable 1",
      Text: "Variable 1",
      Expression: "$+1",
    } as MobiFlightVariable,
    {
      TYPE: "string",
      Name: "Variable 2",
      Text: "Variable 2",
      Expression: "$+2",
    } as MobiFlightVariable,
    {
      TYPE: "number",
      Name: "Variable 3",
      Text: "Variable 3",
      Expression: "$+3",
    } as MobiFlightVariable,
  ]

  console.log("Current Variable in Panel:", currentVariable)
  console.log("Selected Variable in Panel:", variable)

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">MobiFlight Variable</div>
        <div className="text-muted-foreground text-sm">Select a variable</div>
      </div>
      <ComboBox
        items={variableTypeOptions}
        getLabel={(item) => item.label}
        getValue={(item) => item.value}
        selected={
          variableTypeOptions.find(
            (option) => option.value === variable?.TYPE,
          ) ?? undefined
        }
        isSelected={(item) => item.value === variable?.TYPE}
        setSelected={(item) => {
          if (item) {
            const updated = {
              ...variable,
              TYPE: item.value,
            } as MobiFlightVariable
            onVariableChange(updated)
          }
        }}
        placeholder="Search variables..."
      />
      <ComboBox
        items={availableVariables}
        getLabel={(item) => item.Name}
        getValue={(item) => item.Name}
        selected={variable ?? undefined}
        isSelected={(item) => item.Name === variable?.Name}
        setSelected={(item) => {
          console.log("Selected Variable in ComboBox:", item)
          if (item) {
            onVariableChange(item)
          }
        }}
        placeholder="Search variables..."
      />
      {variable && (
        <div className="flex flex-col gap-2">
          <div className="text-sm font-medium">Expression</div>
          <Input
            value={variable.Expression}
            onKeyDown={(e) => {
              e.stopPropagation()
            }}
            onChange={(e) => {
              console.log("Updating Variable Expression:", e.target.value)
              onVariableChange({
                ...variable,
                Expression: e.target.value,
              } as MobiFlightVariable)
            }}
            placeholder="Enter expression..."
          />
          <div className="text-muted-foreground text-sm">
            Use <code>$</code> to represent the variable value in expressions,
            e.g. <code>$ * 2</code> to double a number variable.
          </div>
        </div>
      )}
    </div>
  )
}
