import TransformationPanel from "@/components/wizard/Modifier/Panels/TransformationPanel"
import SubstringPanel from "@/components/wizard/Modifier/Panels/SubstringPanel"
import PaddingPanel from "@/components/wizard/Modifier/Panels/PaddingPanel"
import InterpolationPanel from "@/components/wizard/Modifier/Panels/InterpolationPanel"
import ComparisonPanel from "@/components/wizard/Modifier/Panels/ComparisonPanel"
import BlinkPanel from "@/components/wizard/Modifier/Panels/BlinkPanel"
import { Modifier } from "@/types/modifier"

type ModifierItemProps = {
  modifier: Modifier
  onChange: (updated: Modifier) => void
  onDelete: () => void
  variant?: "summary" | "editor"
}
export const ModifierItem = ({
  modifier,
  onChange,
  onDelete,
  variant = "editor",
}: ModifierItemProps) => {
  switch (modifier.Type) {
    case "Transformation":
      return <TransformationPanel
        variant={variant}
        modifier={modifier}
        onChange={onChange}
        onDelete={onDelete}
      />
    case "Substring":
      return <SubstringPanel
        variant={variant}
        modifier={modifier}
        onChange={onChange}
        onDelete={onDelete}
      />
    case "Padding":
      return <PaddingPanel
        variant={variant}
        modifier={modifier}
        onChange={onChange}
        onDelete={onDelete}
      />
    case "Interpolation":
      return <InterpolationPanel
        variant={variant}
        modifier={modifier}
        onChange={onChange}
        onDelete={onDelete}
      />
    case "Comparison":
      return <ComparisonPanel
        variant={variant}
        modifier={modifier}
        onChange={onChange}
        onDelete={onDelete}
      />
    case "Blink":
      return <BlinkPanel
        variant={variant}
        modifier={modifier}
        onChange={onChange}
        onDelete={onDelete}
      />
  }
}