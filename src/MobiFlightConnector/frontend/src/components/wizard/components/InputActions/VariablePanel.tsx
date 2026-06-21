import ComboBox from "@/components/ComboBox"
import { Input } from "@/components/ui/input"
import { useVariableStore } from "@/stores/variableStore"
import { MobiFlightVariable } from "@/types/config"
import { Label } from "@/components/ui/label"
import { Trans, useTranslation } from "react-i18next"
import { Separator } from "@/components/ui/separator"
import { useEffect } from "react"
export type VariablePanelProps = {
  variant: "summary" | "details"
  currentVariable?: MobiFlightVariable
  onVariableChange: (variable: MobiFlightVariable) => void
}

const defaultVariable = {
  TYPE: "number",
  Name: "New Variable",
  Text: "",
  Expression: "$",
} as MobiFlightVariable

export const VariablePanel = ({
  variant,
  currentVariable,
  onVariableChange,
}: VariablePanelProps) => {
  const { t } = useTranslation()
  const variableTypeOptions = [
    { value: "number", label: "Number" },
    { value: "string", label: "String" },
  ]
  const { variables } = useVariableStore()
  useEffect(() => {
    if (!currentVariable) {
      onVariableChange(defaultVariable)
    }
  }, [onVariableChange, currentVariable])

  if (!currentVariable) {
    return null
  }

  const variable = currentVariable
  const availableVariables = variables ?? []

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-8">
        <div className="flex w-1/3 flex-col gap-1">
          <Label htmlFor="variable">
            {t(
              "Dialog.InputConfigWizard.InputActions.Variable.VariableNameLabel",
            )}
          </Label>
          <div>
            {variable.Name} ({variable.TYPE})
          </div>
        </div>
        <div className="flex grow flex-col gap-1">
          <Label htmlFor="code">
            {t("Dialog.InputConfigWizard.InputActions.Common.CodeLabel")}
          </Label>
          <div
            id="code"
            className="bg-accent rounded px-2 py-1 font-mono text-sm whitespace-pre-wrap"
          >
            {variable.Expression ??
              t(
                "Dialog.InputConfigWizard.InputActions.Variable.NoneExpression",
              )}
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-4">
      <Label>
        {t("Dialog.InputConfigWizard.InputActions.Variable.ExistingVariable")}
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
            {t(
              "Dialog.InputConfigWizard.InputActions.Variable.VariableTypeLabel",
            )}
          </Label>
          <ComboBox
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
            {t(
              "Dialog.InputConfigWizard.InputActions.Variable.VariableNameLabel",
            )}
          </Label>
          <Input
            value={variable?.Name ?? ""}
            onChange={(e) => {
              onVariableChange({
                ...variable,
                Name: e.target.value,
              } as MobiFlightVariable)
            }}
            placeholder={t(
              "Dialog.InputConfigWizard.InputActions.Variable.VariableNamePlaceholder",
            )}
          />
        </div>
        {variable && (
          <div className="flex flex-col gap-2">
            <Label>
              {t(
                "Dialog.InputConfigWizard.InputActions.Variable.ExpressionLabel",
              )}
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
                "Dialog.InputConfigWizard.InputActions.Variable.ExpressionPlaceholder",
              )}
            />
            <div className="text-muted-foreground text-sm">
              <Trans i18nKey="Wizard.InputActions.Variable.ExpressionHelp">
                Use <code>$</code> to represent the variable value in
                expressions, e.g. <code>$ * 2</code> to double a number
                variable.
              </Trans>
            </div>
          </div>
        )}
      </div>
    </div>
  )
}
