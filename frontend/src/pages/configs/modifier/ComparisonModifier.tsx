import { Input } from "@/components/ui/input"

interface ComparisonModifierProps {
  modifier: Comparison
}

export const ComparisonModifier = (props: ComparisonModifierProps) => {
  const { modifier } = props
  return (
    <div className="flex w-full flex-row items-center gap-2">
      <div className="font-bold">If</div>
      <Input value={modifier.Operand} />
      <div className="font-bold">=</div>
      <Input value={modifier.Value} />
      <div className="font-bold">then</div>
      <Input value={modifier.IfValue} />
      <div className="font-bold">else</div>
      <Input value={modifier.ElseValue} />
    </div>
  )
}
