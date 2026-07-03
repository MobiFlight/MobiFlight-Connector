import { Input } from "@/components/ui/input"
import { Comparison, ComparisonOperators } from "@/types/modifier"
import { IconMathFunction } from "@tabler/icons-react"
import { Label } from "@/components/ui/label"

import ComboBox from "@/components/ComboBox"
import { Badge } from "@/components/ui/badge"
import { Trans, useTranslation } from "react-i18next"

export const ComparisonPanelTrigger = ({
  modifier,
}: {
  modifier: Comparison
}) => {
  const { t } = useTranslation()
  return (
    <div className="flex flex-row items-center gap-2">
      <div className="text-md w-32 px-2 text-left font-semibold">
        {t("Dialog.Modifiers.Type.Comparison.Label")}
      </div>
      <div className="flex flex-row items-center gap-2">
        <Trans
          shouldUnescape={true}
          i18nKey="Dialog.Modifiers.Type.Comparison.Summary"
          values={{
            operator: modifier.Operand,
            value: modifier.Value,
            ifValue: modifier.IfValue,
            elseValue: modifier.ElseValue ?? "$",
          }}
          components={{
            badge: <Badge variant={"secondary"} />,
            span: <span className="text-sm font-semibold" />,
          }}
        />
      </div>
    </div>
  )
}

export const ComparisonPanelContent = ({
  modifier,
  onChange,
}: {
  modifier: Comparison
  onChange: (updated: Comparison) => void
}) => {
  const { t } = useTranslation()

  const availableOperators = ComparisonOperators
  const selectedDirection = availableOperators.find(
    (option) => option === modifier.Operand,
  )

  const setSelectedDirection = (item: string | null) => {
    if (item) {
      onChange({
        ...modifier,
        Operand: item as "=" | "!=" | "<" | ">" | "<=" | ">=",
      })
    }
  }

  return (
    <>
      <div className="text-muted-foreground text-sm">
        {t("Dialog.Modifiers.Type.Comparison.Description")}
      </div>
      <div className="flex flex-row items-center gap-4">
        <div className="flex flex-row gap-1">
          <Label htmlFor="current">
            {t("Dialog.Modifiers.Type.Comparison.IfCurrentValue")}
          </Label>
        </div>
        <div className="flex flex-col gap-1">
          <Label htmlFor="operator">
            {t("Dialog.Modifiers.Type.Comparison.Operator")}
          </Label>
          <ComboBox
            id="operator"
            items={availableOperators}
            selected={selectedDirection}
            getLabel={(item) => item}
            getValue={(item) => item}
            isSelected={(item) => item === selectedDirection}
            setSelected={(item) => {
              setSelectedDirection(item ? item : null)
            }}
            variant="nofilter"
            widthClass="w-20"
          />
        </div>
        <div className="flex flex-col gap-1">
          <Label htmlFor="start">
            {t("Dialog.Modifiers.Type.Comparison.Value")}
          </Label>
          <div className="relative flex flex-row items-center">
            <Input
              id="start"
              className="text-code pl-8"
              value={modifier.Value}
              onChange={(e) => onChange({ ...modifier, Value: e.target.value })}
            />
            <IconMathFunction className="stroke-muted-foreground bg-accent absolute rounded-l-sm px-1" />
          </div>
        </div>
        <div className="flex flex-col gap-1">
          <Label htmlFor="if">
            {t("Dialog.Modifiers.Type.Comparison.Then")}
          </Label>
          <div className="relative flex flex-row items-center">
            <Input
              id="if"
              className="text-code pl-8"
              value={modifier.IfValue}
              onChange={(e) =>
                onChange({ ...modifier, IfValue: e.target.value })
              }
            />
            <IconMathFunction className="stroke-muted-foreground bg-accent absolute rounded-l-sm px-1" />
          </div>
        </div>
        <div className="flex flex-col gap-1">
          <Label htmlFor="else">
            {t("Dialog.Modifiers.Type.Comparison.Else")}
          </Label>
          <div className="relative flex flex-row items-center">
            <Input
              className="text-code pl-8"
              id="else"
              value={modifier.ElseValue}
              onChange={(e) =>
                onChange({ ...modifier, ElseValue: e.target.value })
              }
            />
            <IconMathFunction className="stroke-muted-foreground bg-accent absolute rounded-l-sm px-1" />
          </div>
        </div>
      </div>
    </>
  )
}
