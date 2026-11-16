import ComboBox from "@/components/ComboBox"
import { Input } from "@/components/ui/input"
import { useVariableStore } from "@/stores/variableStore"
import { ConfigItemType, MobiFlightVariable } from "@/types/config"
import { Label } from "@/components/ui/label"
import { Trans, useTranslation } from "react-i18next"
import { Separator } from "@/components/ui/separator"
import { useEffect } from "react"
import { Badge } from "@/components/ui/badge"
import CodeValueLabel from "@/components/wizard/common/CodeValueLabel"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
export type VariablePanelProps = {
  variant: "summary" | "details"
  mode: ConfigItemType
  currentVariable?: MobiFlightVariable
  onVariableChange: (variable: MobiFlightVariable) => void
}

const defaultVariable = {
  TYPE: "number",
  Text: "",
  Expression: "$",
} as MobiFlightVariable

export const VariablePanel = ({
  variant,
  currentVariable,
  mode,
  onVariableChange,
}: VariablePanelProps) => {
  const { t } = useTranslation("wizard")
  const variableTypeOptions = [
    {
      value: "number",
      label: t("ConfigWizard.Input.InputActions.Variable.VariableType.Number"),
    },
    {
      value: "string",
      label: t("ConfigWizard.Input.InputActions.Variable.VariableType.String"),
    },
  ]
  const { variables } = useVariableStore()
  useEffect(() => {
    if (!currentVariable) {
      onVariableChange({
        ...defaultVariable,
        Name: t("ConfigWizard.Input.InputActions.Variable.DefaultName"),
      })
    }
  }, [onVariableChange, currentVariable, t])

  if (!currentVariable) {
    return null
  }

  const variable = currentVariable
  const availableVariables = variables ?? []
  const variableTypeLabel =
    variableTypeOptions.find((option) => option.value === variable.TYPE)
      ?.label ?? variable.TYPE

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-2">
        <div className="flex w-1/2 flex-col gap-1">
          <Label htmlFor="variable">
            {t("ConfigWizard.Input.InputActions.Variable.VariableNameLabel")}
          </Label>
          <div className="flex flex-row items-center gap-2">
            <span className="text-sm">{variable.Name}</span>
            <Badge variant="outline">{variableTypeLabel}</Badge>
          </div>
        </div>
        {mode === "InputConfigItem" && (
          <div className="flex w-1/2 flex-col gap-1">
            <Label htmlFor="code">
              {t("ConfigWizard.Input.InputActions.Variable.ExpressionLabel")}
            </Label>
            <CodeValueLabel id="code" className="max-w-100">
              {variable.Expression ??
                t("ConfigWizard.Input.InputActions.Variable.ExpressionNone")}
            </CodeValueLabel>
          </div>
        )}
      </div>
    )
  }

  return (
    <WizardSection>
      <SectionHeadline
        title={t("ConfigWizard.Input.InputActions.Variable.Title")}
        description={t("ConfigWizard.Input.InputActions.Variable.Description")}
      />
      <div className="flex flex-col gap-4">
        <Label>
          {t("ConfigWizard.Input.InputActions.Variable.ExistingVariable")}
        </Label>
        <ComboBox
          items={availableVariables}
          getLabel={(item) => `${item.Name} (${item.TYPE})`}
          getValue={(item) => item.Name}
          selected={variable ?? undefined}
          isSelected={(item) =>
            item.Name === variable?.Name && item.TYPE === variable?.TYPE
          }
          setSelected={(item) => {
            if (item) {
              onVariableChange(item)
            }
          }}
        />
        <Separator />
        <div className="flex flex-row gap-2">
          <div className="flex flex-col gap-2">
            <Label>
              {t("ConfigWizard.Input.InputActions.Variable.VariableTypeLabel")}
            </Label>
            <ComboBox
              widthClass="w-26"
              align="start"
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
              variant="nofilter"
            />
          </div>
          <div className="flex flex-col gap-2">
            <Label>
              {t("ConfigWizard.Input.InputActions.Variable.VariableNameLabel")}
            </Label>
            <Input
              className="max-w-50 truncate"
              value={variable?.Name ?? ""}
              onChange={(e) => {
                onVariableChange({
                  ...variable,
                  Name: e.target.value,
                } as MobiFlightVariable)
              }}
              placeholder={t(
                "ConfigWizard.Input.InputActions.Variable.VariableNamePlaceholder",
              )}
            />
          </div>
          {mode === "InputConfigItem" && variable && (
            <div className="flex flex-col gap-2">
              <Label>
                {t("ConfigWizard.Input.InputActions.Variable.ExpressionLabel")}
              </Label>
              <Input
                className="font-mono text-sm whitespace-nowrap"
                value={variable.Expression}
                onKeyDown={(e) => {
                  e.stopPropagation()
                }}
                onChange={(e) =>
                  onVariableChange({
                    ...variable,
                    Expression: e.target.value,
                  } as MobiFlightVariable)
                }
                placeholder={t(
                  "ConfigWizard.Input.InputActions.Variable.ExpressionPlaceholder",
                )}
              />
              <div className="text-muted-foreground text-sm">
                <Trans
                  i18nKey="ConfigWizard.Input.InputActions.Variable.ExpressionHelp"
                  ns="wizard"
                >
                  Use <code>$</code> to represent the variable value in
                  expressions, e.g. <code>$ * 2</code> to double a number
                  variable.
                </Trans>
              </div>
            </div>
          )}
        </div>
      </div>
    </WizardSection>
  )
}
