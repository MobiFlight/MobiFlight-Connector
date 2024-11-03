import { Button } from "@/components/ui/button"
import { ComparisonModifier } from "./ComparisonModifier"
import { TransformationModifier } from "./TransformationModifier"

interface ModifierContainerProps {
  modifier: IModifier
}

export const ModifierContainer = ({ modifier }: ModifierContainerProps) => {
  const m =
    modifier.Type == "Transformation" ? (
      <TransformationModifier modifier={modifier as Transformation} />
    ) : modifier.Type == "Comparison" ? (
      <ComparisonModifier modifier={modifier as Comparison} />
    ) : (
      <p>Unknown modifier type</p>
    )

  return (
    <div>
      <div className="flex flex-row items-center justify-between gap-2 rounded-lg border-2 bg-background/50 px-4 py-2 hover:bg-background/25">
        <div className="w-1/6">{modifier.Type}</div>
        <div className="w-full">{m}</div>
        <div className="flex w-1/6 flex-row justify-center gap-2">
          <Button>Remove</Button>
          <Button>Up</Button>
          <Button>Down</Button>
        </div>
      </div>
    </div>
  )
}
