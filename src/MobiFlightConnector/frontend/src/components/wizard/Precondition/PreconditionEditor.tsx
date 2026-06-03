import ComboBox from "@/components/ComboBox"
import { Button } from "@/components/ui/button"
import { Checkbox } from "@/components/ui/checkbox"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { preconditionVariants } from "@/components/wizard/variants"
import { IConfigItem, MobiFlightVariable, Precondition } from "@/types/config"
import { IconPlus, IconTrash } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export interface PreconditionEditorProps {
  variables: MobiFlightVariable[]
  outputConfigs: IConfigItem[]
  preconditions: Precondition[]
  onPreconditionsChange: (preconditions: Precondition[]) => void
}

const OPERAND_OPTIONS: Precondition["Operand"][] = [
  "=",
  "<>",
  "<",
  ">",
  "<=",
  ">=",
]
const LOGIC_OPTIONS: Precondition["Logic"][] = ["and", "or"]
const TYPE_OPTIONS = [
  { value: "config", label: "Config" },
  { value: "variable", label: "Variable" },
]

type PreconditionItemRowProps = {
  precondition: Precondition
  variables: MobiFlightVariable[]
  outputConfigs: IConfigItem[]
  onChange: (updated: Precondition) => void
  onDelete: () => void
  showLogic: boolean
}

const PreconditionItemRow = ({
  precondition,
  variables,
  outputConfigs,
  onChange,
  onDelete,
  showLogic,
}: PreconditionItemRowProps) => {
  const { t } = useTranslation()
  const selectedConfig =
    precondition.Type === "config"
      ? outputConfigs.find((c) => c.GUID === precondition.Ref)
      : undefined

  const selectedVariable =
    precondition.Type === "variable"
      ? variables.find((v) => v.Name === precondition.Ref)
      : undefined

  const variantStyle = preconditionVariants[precondition.Type]

  return (
    <div
      data-testid="precondition-item-row"
      className={`flex flex-row gap-2 rounded-lg border p-4 py-1 ${variantStyle}`}
    >
      <div className="flex flex-row items-center gap-2">
        <Checkbox
          checked={precondition.Active}
          onCheckedChange={(checked) =>
            onChange({ ...precondition, Active: !!checked })
          }
        />
        <Label className="text-sm">{t("Wizard.PreconditionEditor.Active")}</Label>
      </div>

      <ComboBox
        items={TYPE_OPTIONS}
        selected={TYPE_OPTIONS.find((t) => t.value === precondition.Type)}
        getValue={(t) => t.value}
        getLabel={(t) => t.label}
        isSelected={(t, s) => t.value === s?.value}
        setSelected={(t) =>
          onChange({ ...precondition, Type: t?.value as "config" | "variable" | "pin" ?? "config", Ref: "" })
        }
        placeholder={t("Wizard.PreconditionEditor.TypePlaceholder")}
        widthClass="w-32"
        variant="nofilter"
      />

      {precondition.Type === "config" && (
        <ComboBox
          items={outputConfigs}
          selected={selectedConfig}
          getValue={(c) => c.GUID}
          getLabel={(c) => c.Name}
          isSelected={(c, s) => c.GUID === s?.GUID}
          setSelected={(c) => onChange({ ...precondition, Ref: c?.GUID ?? "" })}
          placeholder={t("Wizard.PreconditionEditor.SelectConfig")}
          widthClass="w-48"
        />
      )}

      {precondition.Type === "variable" && (
        <ComboBox
          items={variables}
          selected={selectedVariable}
          getValue={(v) => v.Name}
          getLabel={(v) => v.Name}
          isSelected={(v, s) => v.Name === s?.Name}
          setSelected={(v) => onChange({ ...precondition, Ref: v?.Name ?? "" })}
          placeholder={t("Wizard.PreconditionEditor.SelectVariable")}
          widthClass="w-48"
        />
      )}

      <ComboBox
        items={OPERAND_OPTIONS}
        selected={precondition.Operand}
        getValue={(o) => o}
        getLabel={(o) => o}
        isSelected={(o, s) => o === s}
        setSelected={(o) => onChange({ ...precondition, Operand: o ?? "=" })}
        variant="nofilter"
        widthClass="w-18"
      />
      <Input
        value={precondition.Value}
        onChange={(e) => onChange({ ...precondition, Value: e.target.value })}
        placeholder={t("Wizard.PreconditionEditor.ValuePlaceholder")}
      />
      {showLogic && (
        <div className="flex flex-row items-center gap-2">
          <ComboBox
            items={LOGIC_OPTIONS}
            selected={precondition.Logic}
            getValue={(l) => l}
            getLabel={(l) => l}
            isSelected={(l, s) => l === s}
            setSelected={(l) =>
              onChange({ ...precondition, Logic: l ?? "and" })
            }
            widthClass="w-24"
            variant="nofilter"
          />
        </div>
      )}
      <Button
        variant="ghost"
        size="icon"
        className="text-destructive hover:text-destructive ml-auto"
        onClick={onDelete}
      >
        <div className="sr-only">{t("Wizard.PreconditionEditor.DeletePrecondition")}</div>
        <IconTrash className="h-4 w-4" />
      </Button>
    </div>
  )
}

const EMPTY_PRECONDITION: Precondition = {
  Type: "config",
  Ref: "",
  Pin: "",
  Operand: "=",
  Value: "",
  Logic: "and",
  Active: true,
}

const PreconditionEditor = ({
  variables,
  outputConfigs,
  preconditions,
  onPreconditionsChange,
}: PreconditionEditorProps) => {
  const { t } = useTranslation()
  const handleChange = (index: number, updated: Precondition) => {
    onPreconditionsChange(
      preconditions.map((p, i) => (i === index ? updated : p)),
    )
  }

  const handleDelete = (index: number) => {
    onPreconditionsChange(preconditions.filter((_, i) => i !== index))
  }

  const handleAdd = () => {
    onPreconditionsChange([...preconditions, { ...EMPTY_PRECONDITION }])
  }

  return (
    <div className="flex flex-col gap-4" data-testid="precondition-editor">
      <div className="text-lg font-semibold">{t("Wizard.PreconditionEditor.Title")}</div>
      <div className="text-muted-foreground text-sm">
        {t("Wizard.PreconditionEditor.Description")}
      </div>

      {preconditions.length === 0 && (
        <div className="text-muted-foreground rounded border p-4 text-center text-sm">
          {t("Wizard.PreconditionEditor.NoPreconditions")}
        </div>
      )}

      {preconditions.map((precondition, index) => (
        <PreconditionItemRow
          key={index}
          precondition={precondition}
          variables={variables}
          outputConfigs={outputConfigs}
          onChange={(updated) => handleChange(index, updated)}
          onDelete={() => handleDelete(index)}
          showLogic={index < preconditions.length - 1}
        />
      ))}

      <Button variant="outline" className="self-start" onClick={handleAdd}>
        <IconPlus className="h-4 w-4" />
        {t("Wizard.PreconditionEditor.AddPrecondition")}
      </Button>
    </div>
  )
}
export default PreconditionEditor
