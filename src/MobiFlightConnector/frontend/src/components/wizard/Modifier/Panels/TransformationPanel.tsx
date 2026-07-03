import { Input } from "@/components/ui/input"
import { Transformation } from "@/types/modifier"
import { IconMathFunction } from "@tabler/icons-react"
import { Label } from "@/components/ui/label"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"
import { useTranslation } from "react-i18next"

export const TransformationPanelTrigger = ({
  modifier,
}: {
  modifier: Transformation
}) => {
  const { t } = useTranslation()
  return (
    <div className="flex flex-row items-center gap-2">
      <div className="text-md px-2 font-semibold">
        {t("Dialog.Modifiers.Type.Transformation.Label")}
      </div>
      <CodeValueLabel className="pt-2 text-xs">
        {modifier.Expression}
      </CodeValueLabel>
    </div>
  )
}

export const TransformationPanelContent = ({
  modifier,
  onChange,
}: {
  modifier: Transformation
  onChange: (updated: Transformation) => void
}) => {
  const { t } = useTranslation()
  return (
    <>
      <div className="text-muted-foreground text-sm">
        {t("Dialog.Modifiers.Type.Transformation.Description")}
      </div>
      <div className="flex flex-row items-center gap-2">
        <div className="flex grow flex-col gap-1">
          <Label htmlFor="expression">
            {t("Dialog.Modifiers.Type.Transformation.Expression")}
          </Label>
          <div className="relative flex flex-row items-center">
            <Input
              id="expression"
              className="text-code pl-8"
              value={modifier.Expression}
              onChange={(e) =>
                onChange({ ...modifier, Expression: e.target.value })
              }
            />
            <IconMathFunction className="stroke-muted-foreground bg-accent absolute rounded-l-sm px-1" />
          </div>
        </div>
      </div>
    </>
  )
}
