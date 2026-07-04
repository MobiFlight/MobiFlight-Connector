import ComboBox from "@/components/ComboBox"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { ScrollArea } from "@/components/ui/scroll-area"
import { Switch } from "@/components/ui/switch"
import { configReferenceVariants } from "@/components/wizard/variants"
import { ConfigReference, IConfigItem } from "@/types/config"
import { IconPlus, IconTrash } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export interface ConfigReferenceEditorProps {
  outputConfigs: IConfigItem[]
  configReferences: ConfigReference[]
  onConfigReferencesChange: (configReferences: ConfigReference[]) => void
}

const SUGGESTED_PLACEHOLDERS = [
  "#",
  "!",
  "?",
  "@",
  "A",
  "B",
  "C",
  "D",
  "E",
  "F",
  "G",
  "H",
  "I",
  "J",
  "K",
  "L",
  "M",
]

type ConfigReferenceItemRowProps = {
  configReference: ConfigReference
  outputConfigs: IConfigItem[]
  onChange: (updated: ConfigReference) => void
  onDelete: () => void
}

const ConfigReferenceItemRow = ({
  configReference,
  outputConfigs,
  onChange,
  onDelete,
}: ConfigReferenceItemRowProps) => {
  const { t } = useTranslation()
  const selectedConfig = outputConfigs.find(
    (c) => c.GUID === configReference.Ref,
  )

  const variantStyle = configReferenceVariants["default"]
  return (
    <div
      data-testid="config-reference-item-row"
      className={`flex flex-row items-center gap-2 rounded-lg border p-4 py-1 ${variantStyle}`}
    >
      <div className="flex flex-row items-center gap-2">
        <Switch
          checked={configReference.Active}
          onCheckedChange={(checked) =>
            onChange({ ...configReference, Active: !!checked })
          }
        />
      </div>

      <ComboBox
        items={outputConfigs}
        selected={selectedConfig}
        getValue={(c) => c.GUID}
        getLabel={(c) => c.Name}
        isSelected={(c, s) => c.GUID === s?.GUID}
        setSelected={(c) =>
          onChange({ ...configReference, Ref: c?.GUID ?? "" })
        }
        placeholder={t(
          "Dialog.InputConfigWizard.ConfigReferenceEditor.SelectConfig",
        )}
        widthClass="flex-1"
      />

      <Input
        value={configReference.Placeholder}
        onChange={(e) =>
          onChange({ ...configReference, Placeholder: e.target.value })
        }
        placeholder="Value"
        className="w-16"
      />

      <Input
        value={configReference.TestValue}
        onChange={(e) =>
          onChange({ ...configReference, TestValue: e.target.value })
        }
        placeholder="Value"
        className="w-16"
      />

      <Button
        variant="ghost"
        size="icon"
        className="text-destructive hover:text-destructive ml-auto"
        onClick={onDelete}
      >
        <div className="sr-only">
          {t(
            "Dialog.InputConfigWizard.ConfigReferenceEditor.DeleteConfigReference",
          )}
        </div>
        <IconTrash className="h-4 w-4" />
      </Button>
    </div>
  )
}

const EMPTY_CONFIG_REFERENCE: ConfigReference = {
  Active: true,
  Ref: "",
  Placeholder: "",
  TestValue: "",
}

const ConfigReferenceEditor = ({
  outputConfigs,
  configReferences,
  onConfigReferencesChange,
}: ConfigReferenceEditorProps) => {
  const { t } = useTranslation()
  const handleChange = (index: number, updated: ConfigReference) => {
    onConfigReferencesChange(
      configReferences.map((c, i) => (i === index ? updated : c)),
    )
  }

  const handleDelete = (index: number) => {
    onConfigReferencesChange(configReferences.filter((_, i) => i !== index))
  }

  const handleAdd = () => {
    const suggestedPlaceholder =
      SUGGESTED_PLACEHOLDERS[
        configReferences.length % SUGGESTED_PLACEHOLDERS.length
      ]
    onConfigReferencesChange([
      ...configReferences,
      { ...EMPTY_CONFIG_REFERENCE, Placeholder: suggestedPlaceholder },
    ])
  }

  return (
    <div
      className="flex grow flex-col gap-4"
      data-testid="config-reference-editor"
    >
      <div className="flex flex-row justify-between gap-4">
        <div className="flex flex-col gap-2">
          <div className="text-lg font-semibold">
            {t("Dialog.InputConfigWizard.ConfigReferenceEditor.Title")}
          </div>
          <div className="text-muted-foreground text-sm">
            {t("Dialog.InputConfigWizard.ConfigReferenceEditor.Description")}
          </div>
        </div>
        <Button
          variant="default"
          className="self-end"
          onClick={handleAdd}
          size={"sm"}
        >
          <IconPlus className="h-4 w-4" />
          {t(
            "Dialog.InputConfigWizard.ConfigReferenceEditor.AddConfigReference",
          )}
        </Button>
      </div>

      {configReferences.length === 0 ? (
        <div className="text-muted-foreground rounded border p-4 text-center text-sm">
          {t(
            "Dialog.InputConfigWizard.ConfigReferenceEditor.NoConfigReferences",
          )}
        </div>
      ) : (
        <ScrollArea className="grow">
          <div className="flex flex-col gap-2">
            {configReferences.map((configReference, index) => (
              <ConfigReferenceItemRow
                key={index}
                configReference={configReference}
                outputConfigs={outputConfigs}
                onChange={(updated) => handleChange(index, updated)}
                onDelete={() => handleDelete(index)}
              />
            ))}
          </div>
        </ScrollArea>
      )}
    </div>
  )
}
export default ConfigReferenceEditor
