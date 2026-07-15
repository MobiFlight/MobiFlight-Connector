import ComboBox from "@/components/ComboBox"
import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { Input } from "@/components/ui/input"
import { ScrollArea } from "@/components/ui/scroll-area"
import { Switch } from "@/components/ui/switch"
import { preconditionVariants } from "@/components/wizard/variants"
import {
  IConfigItem,
  MobiFlightVariable,
  Precondition,
  PreconditionType,
} from "@/types/config"
import { PreconditionTypes } from "@/types/typesOptions"
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

const ArcazePortOptions = [
  { value: "A", label: "Port A" },
  { value: "B", label: "Port B" },
]

const ARCAZE_PIN_COUNT = 40
const ArcazePinOptions = Array.from({ length: ARCAZE_PIN_COUNT }, (_, i) => ({
  value: `${i + 1}`.padStart(2, "0"),
  label: `Pin ${i + 1}`,
}))

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

  const preconditionPin = {
    Port: precondition.Type === "pin" && (precondition.Pin?.charAt(0) ?? "A"),
    Pin:
      precondition.Type === "pin" &&
      (precondition.Pin?.substring(1, 3) ?? "01"),
  }
  const selectedPort =
    precondition.Type === "pin"
      ? ArcazePortOptions.find((p) => p.value === preconditionPin.Port)
      : ArcazePortOptions[0] // Default to Port A if not a pin type

  const selectedPin =
    precondition.Type === "pin"
      ? ArcazePinOptions.find((p) => p.value === preconditionPin.Pin)
      : ArcazePinOptions[0] // Default to Pin 01 if not a pin type

  const variantStyle = preconditionVariants[precondition.Type]

  return (
    <div
      data-testid="precondition-item-row"
      className={`flex flex-row items-center gap-2 rounded-lg border p-4 py-1 ${variantStyle}`}
    >
      <div className="flex flex-row items-center gap-2">
        <Switch
          checked={precondition.Active}
          onCheckedChange={(checked) =>
            onChange({ ...precondition, Active: !!checked })
          }
        />
      </div>

      {precondition.Type === "config" && (
        <ComboBox
          items={outputConfigs}
          selected={selectedConfig}
          getValue={(c) => c.GUID}
          getLabel={(c) => c.Name}
          isSelected={(c, s) => c.GUID === s?.GUID}
          setSelected={(c) => onChange({ ...precondition, Ref: c?.GUID ?? "" })}
          placeholder={t(
            "Dialog.InputConfigWizard.PreconditionEditor.SelectConfig",
          )}
          widthClass="w-58"
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
          placeholder={t(
            "Dialog.InputConfigWizard.PreconditionEditor.SelectVariable",
          )}
          widthClass="w-58"
        />
      )}

      {precondition.Type === "pin" && (
        <div className="flex flex-row gap-2">
          <ComboBox
            items={ArcazePortOptions}
            selected={selectedPort}
            getValue={(p) => p.value}
            getLabel={(p) => p.label}
            isSelected={(p, s) => p.value === s?.value}
            setSelected={(p) => {
              const updated = {
                ...precondition,
                Pin: `${p?.value ?? ""}${preconditionPin.Pin}`,
              }
              onChange(updated)
            }}
            placeholder={t(
              "Dialog.InputConfigWizard.PreconditionEditor.SelectPort",
            )}
            widthClass="w-28"
            variant="nofilter"
          />
          <ComboBox
            items={ArcazePinOptions}
            selected={selectedPin}
            getValue={(p) => p.value}
            getLabel={(p) => p.label}
            isSelected={(p, s) => p.value === s?.value}
            setSelected={(p) =>
              onChange({
                ...precondition,
                Pin: `${preconditionPin.Port}${p?.value ?? ""}`,
              })
            }
            placeholder={t(
              "Dialog.InputConfigWizard.PreconditionEditor.SelectPin",
            )}
            widthClass="w-28"
            variant="nofilter"
          />
        </div>
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
        placeholder={t(
          "Dialog.InputConfigWizard.PreconditionEditor.ValuePlaceholder",
        )}
        className="w-18"
      />
      {showLogic && (
        <ComboBox
          items={LOGIC_OPTIONS}
          selected={precondition.Logic}
          getValue={(l) => l}
          getLabel={(l) => l}
          isSelected={(l, s) => l === s}
          setSelected={(l) => onChange({ ...precondition, Logic: l ?? "and" })}
          widthClass="w-20"
          variant="nofilter"
        />
      )}
      <Button
        variant="ghost"
        size="icon"
        className="text-destructive hover:text-destructive ml-auto"
        onClick={onDelete}
      >
        <div className="sr-only">
          {t("Dialog.InputConfigWizard.PreconditionEditor.DeletePrecondition")}
        </div>
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

  const handleAdd = (preconditionType: PreconditionType) => {
    // create the right precondition type
    const newPrecondition = { ...EMPTY_PRECONDITION, Type: preconditionType }
    onPreconditionsChange([...preconditions, newPrecondition])
  }

  const preconditionTypes = PreconditionTypes as PreconditionType[]

  return (
    <div className="flex grow flex-col gap-4" data-testid="precondition-editor">
      <div className="flex flex-row justify-between gap-4">
        <div className="flex flex-col gap-2">
          <div className="text-lg font-semibold">
            {t("Dialog.InputConfigWizard.PreconditionEditor.Title")}
          </div>
          <div className="text-muted-foreground text-sm">
            {t("Dialog.InputConfigWizard.PreconditionEditor.Description")}
          </div>
        </div>
        <DropdownMenu>
          <DropdownMenuTrigger asChild className="self-end">
            <Button variant="default" className="w-fit" size={"sm"}>
              <IconPlus className="h-4 w-4" />
              {t("Dialog.InputConfigWizard.PreconditionEditor.AddPrecondition")}
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="start">
            {preconditionTypes.map((preconditionType) => (
              <DropdownMenuItem
                key={preconditionType}
                onClick={() => handleAdd(preconditionType)}
              >
                {t(
                  `Dialog.InputConfigWizard.PreconditionEditor.Types.${preconditionType}`,
                )}
              </DropdownMenuItem>
            ))}
          </DropdownMenuContent>
        </DropdownMenu>
      </div>

      {preconditions.length === 0 ? (
        <div className="text-muted-foreground rounded border p-4 text-center text-sm">
          {t("Dialog.InputConfigWizard.PreconditionEditor.NoPreconditions")}
        </div>
      ) : (
        <ScrollArea className="grow">
          <div className="flex flex-col gap-2">
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
          </div>
        </ScrollArea>
      )}
    </div>
  )
}
export default PreconditionEditor
