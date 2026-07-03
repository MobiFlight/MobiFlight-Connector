import {
  TransformationPanelContent,
  TransformationPanelTrigger,
} from "@/components/wizard/Modifier/Panels/TransformationPanel"
import {
  SubstringPanelContent,
  SubstringPanelTrigger,
} from "@/components/wizard/Modifier/Panels/SubstringPanel"
import {
  PaddingPanelContent,
  PaddingPanelTrigger,
} from "@/components/wizard/Modifier/Panels/PaddingPanel"
import {
  InterpolationPanelContent,
  InterpolationPanelTrigger,
} from "@/components/wizard/Modifier/Panels/InterpolationPanel"
import {
  ComparisonPanelContent,
  ComparisonPanelTrigger,
} from "@/components/wizard/Modifier/Panels/ComparisonPanel"
import {
  BlinkPanelContent,
  BlinkPanelTrigger,
} from "@/components/wizard/Modifier/Panels/BlinkPanel"
import { Modifier } from "@/types/modifier"
import { useState } from "react"
import { useTranslation } from "react-i18next"
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible"
import { IconChevronDown, IconChevronUp, IconTrash } from "@tabler/icons-react"
import { Switch } from "@/components/ui/switch"
import { Button } from "@/components/ui/button"

type ModifierItemProps = {
  modifier: Modifier
  onChange: (updated: Modifier) => void
  onDelete: () => void
  onMoveUp?: () => void
  onMoveDown?: () => void
  isFirst?: boolean
  isLast?: boolean
}

export const ModifierItem = ({
  modifier,
  onChange,
  onDelete,
  onMoveUp,
  onMoveDown,
  isFirst,
  isLast,
}: ModifierItemProps) => {
  const { t } = useTranslation()
  const [open, setOpen] = useState(false)

  const modifierContent =
    modifier.Type === "Transformation" ? (
      <TransformationPanelContent modifier={modifier} onChange={onChange} />
    ) : modifier.Type === "Substring" ? (
      <SubstringPanelContent modifier={modifier} onChange={onChange} />
    ) : modifier.Type === "Padding" ? (
      <PaddingPanelContent modifier={modifier} onChange={onChange} />
    ) : modifier.Type === "Interpolation" ? (
      <InterpolationPanelContent modifier={modifier} onChange={onChange} />
    ) : modifier.Type === "Comparison" ? (
      <ComparisonPanelContent modifier={modifier} onChange={onChange} />
    ) : modifier.Type === "Blink" ? (
      <BlinkPanelContent modifier={modifier} onChange={onChange} />
    ) : null

  const modifierTrigger =
    modifier.Type === "Transformation" ? (
      <TransformationPanelTrigger modifier={modifier} />
    ) : modifier.Type === "Substring" ? (
      <SubstringPanelTrigger modifier={modifier} />
    ) : modifier.Type === "Padding" ? (
      <PaddingPanelTrigger modifier={modifier} />
    ) : modifier.Type === "Interpolation" ? (
      <InterpolationPanelTrigger modifier={modifier} />
    ) : modifier.Type === "Comparison" ? (
      <ComparisonPanelTrigger modifier={modifier} />
    ) : (
      modifier.Type === "Blink" && <BlinkPanelTrigger modifier={modifier} />
    )

  return (
    <div
      className="flex flex-col gap-2 rounded-md border py-0.5 shadow-sm"
      data-testid="modifier-item"
    >
      <Collapsible open={open} onOpenChange={setOpen} className="flex flex-col">
        <div className="group flex flex-row items-center gap-2 px-1">
          <div className="flex flex-col items-center justify-center">
            <Button
              className="group-hover:text-foreground text-muted-foreground h-5 w-5 p-1"
              variant="ghost"
              onClick={onMoveUp}
              disabled={isFirst}
            >
              <IconChevronUp />
              <span className="sr-only">
                {t("Dialog.Modifiers.Editor.MoveUp")}
              </span>
            </Button>
            <Button
              className="group-hover:text-foreground text-muted-foreground h-5 w-5 p-1"
              variant="ghost"
              onClick={onMoveDown}
              disabled={isLast}
            >
              <IconChevronDown />
              <span className="sr-only">
                {t("Dialog.Modifiers.Editor.MoveDown")}
              </span>
            </Button>
          </div>
          <Switch
            id="active"
            checked={modifier.Active}
            onCheckedChange={(checked) =>
              onChange({ ...modifier, Active: checked })
            }
          />
          <CollapsibleTrigger className="flex grow flex-row items-center justify-between">
            {modifierTrigger}
            <div className="hover:bg-accent hover:text-accent-foreground flex h-8 flex-row items-center justify-center rounded-md px-2 [&_svg]:size-4">
              {!open ? (
                <IconChevronDown className="transition-transform" />
              ) : (
                <IconChevronDown className="rotate-180 transition-transform duration-500" />
              )}
            </div>
          </CollapsibleTrigger>
          <Button onClick={onDelete} size={"sm"} variant="ghost">
            <IconTrash />
            <span className="sr-only">
              {t("Dialog.Modifiers.Editor.DeleteModifier")}
            </span>
          </Button>
        </div>
        <CollapsibleContent className="data-[state=closed]:animate-collapsible-up data-[state=open]:animate-collapsible-down flex flex-col gap-4 overflow-hidden pt-2 pr-7 pb-2 pl-7 data-[state=open]:duration-500 border-t shadow-inner">
          {modifierContent}
        </CollapsibleContent>
      </Collapsible>
    </div>
  )
}
