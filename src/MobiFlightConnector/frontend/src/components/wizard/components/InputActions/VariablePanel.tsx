import ComboBox from "@/components/ComboBox"
import { Input } from "@/components/ui/input"
import { useVariableStore } from "@/stores/variableStore"
import { MobiFlightVariable } from "@/types/config"
import { Label } from "@/components/ui/label"
import { Trans, useTranslation } from "react-i18next"
import { Separator } from "@/components/ui/separator"
export type VariablePanelProps = {
  currentVariable?: MobiFlightVariable
  onVariableChange: (variable: MobiFlightVariable) => void
}

export const VariablePanel = ({
  currentVariable,
  onVariableChange,
}: VariablePanelProps) => {
  const { t } = useTranslation()
  const variableTypeOptions = [
    { value: "number", label: "Number" },
    { value: "string", label: "String" },
  ]
  const { variables } = useVariableStore()

  const variable =
    currentVariable ??
    ({
      TYPE: "number",
      Name: "New Variable",
      Text: "",
      Expression: "$",
    } as MobiFlightVariable)

  const availableVariables = variables ?? []

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
        isSelected={(item) => item.Name === variable?.Name && item.TYPE === variable?.TYPE}
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
