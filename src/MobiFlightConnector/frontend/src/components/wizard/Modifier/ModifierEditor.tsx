import { Button } from "@/components/ui/button"
import { Modifier, MODIFIER_TYPES, ModifierFactory } from "@/types/modifier"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"
import TransformationPanel from "@/components/wizard/Modifier/Panels/TransformationPanel"

const ModifierItem = ({
  modifier,
  onChange,
  onDelete,
}: {
  modifier: Modifier
  onChange: (updated: Modifier) => void
  onDelete: () => void
}) => {
  let PanelComponent = null
  switch (modifier.Type) {
    case "Transformation":
      PanelComponent = TransformationPanel
      break
    default:
      return null
  }

  return (
    <PanelComponent
      variant="editor"
      modifier={modifier}
      onChange={onChange}
      onDelete={onDelete}
    />
  )
}

type ModifierEditorProps = {
  modifiers: Modifier[]
  onModifierChange: (modifiers: Modifier[]) => void
}

const ModifierEditor = ({
  modifiers,
  onModifierChange,
}: ModifierEditorProps) => {
  const { t } = useTranslation()
  const modifierTypes = MODIFIER_TYPES

  const handleAdd = (type: string) => {
    const newModifier = ModifierFactory.createModifier(type)
    onModifierChange([...modifiers, newModifier])
  }
  const handleDelete = (index: number) => {
    onModifierChange([
      ...modifiers.slice(0, index),
      ...modifiers.slice(index + 1),
    ])
  }
  const handleChange = (index: number, updated: Modifier) => {
    console.log("Change modifier at index:", index, "to", updated)
    onModifierChange([
      ...modifiers.slice(0, index),
      updated,
      ...modifiers.slice(index + 1),
    ])
  }

  return (
    <div className="flex flex-col gap-4" data-testid="config-reference-editor">
      <div className="text-lg font-semibold">
        {t("Dialog.Modifiers.Editor.Title")}
      </div>
      <div className="text-muted-foreground text-sm">
        {t("Dialog.Modifiers.Editor.Description")}
      </div>

      {modifiers.length === 0 && (
        <div className="text-muted-foreground rounded border p-4 text-center text-sm">
          {t("Dialog.Modifiers.Editor.NoModifiers")}
        </div>
      )}

      {modifiers.map((modifier, index) => (
        <ModifierItem
          key={index}
          modifier={modifier}
          onChange={(updated) => handleChange(index, updated)}
          onDelete={() => handleDelete(index)}
        />
      ))}
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button variant="outline">
            <IconPlus className="h-4 w-4" />
            {t("Dialog.Modifiers.Editor.AddModifier")}
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="start">
          {modifierTypes.map((modifierType) => (
            <DropdownMenuItem
              key={modifierType}
              onClick={() => handleAdd(modifierType)}
            >
              {t(`Dialog.Modifiers.Type.${modifierType}.Label`)}
            </DropdownMenuItem>
          ))}
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  )
}
export default ModifierEditor
