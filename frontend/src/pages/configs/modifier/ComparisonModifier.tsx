import { ComboBox } from "@/components/mobiflight/ComboBox"
import { Input } from "@/components/ui/input"
import { useState } from "react"

interface ComparisonModifierProps {
  modifier: Comparison
}

export const ComparisonModifier = (props: ComparisonModifierProps) => {
  const { modifier : originalModifier } = props

  const [ modifier, setModifier ] = useState(originalModifier)
  
  const options = [
    { label: "Equal", value: "=" },
    { label: "Not Equal", value: "!=" },
    { label: "Greater Than", value: ">" },
    { label: "Greater Than or Equal", value: ">=" },
    { label: "Less Than", value: "<" },
    { label: "Less Than or Equal", value: "<=" },
  ]
  return (
    <div className="flex w-full flex-row items-center gap-2">
      <div className="font-bold">If</div>
      <ComboBox options={options} onSelect={ (value) => { 
        setModifier({...modifier, Operand: value})
        setModifier }} value={ modifier.Operand}>        
      </ComboBox>
      <Input value={modifier.Value} />
      <div className="font-bold">then</div>
      <Input value={modifier.IfValue} />
      <div className="font-bold">else</div>
      <Input value={modifier.ElseValue} />
    </div>
  )
}
