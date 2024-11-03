import { Textarea } from "@/components/ui/textarea"

interface TransformationModifierProps {
  modifier: Transformation
}

export const TransformationModifier = (props: TransformationModifierProps) => {
  const { modifier } = props

  return (
    <Textarea>{modifier.Expression}</Textarea>
  )
}
