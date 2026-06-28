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
}
export const ModifierItem = ({
  modifier,
  onChange,
  onDelete,
}: ModifierItemProps) => {
  switch (modifier.Type) {
    case "Transformation":
      return <TransformationPanel
        variant="editor"
        modifier={modifier}
        onChange={onChange}
        onDelete={onDelete}
      />
    case "Substring":
      return <SubstringPanel
        variant="editor"
        modifier={modifier}
        onChange={onChange}
        onDelete={onDelete}
      />
    case "Padding":
      return <PaddingPanel
        variant="editor"
        modifier={modifier}
        onChange={onChange}
        onDelete={onDelete}
      />
    case "Interpolation":
      return <InterpolationPanel
        variant="editor"
        modifier={modifier}
        onChange={onChange}
        onDelete={onDelete}
      />
    case "Comparison":
      return <ComparisonPanel
        variant="editor"
        modifier={modifier}
        onChange={onChange}
        onDelete={onDelete}
      />
    case "Blink":
      return <BlinkPanel
        variant="editor"
        modifier={modifier}
        onChange={onChange}
        onDelete={onDelete}
      />
  }
}