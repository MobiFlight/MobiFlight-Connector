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
import { ModifierItem } from "@/components/wizard/Modifier/ModifierItem"
import { ScrollArea } from "@/components/ui/scroll-area"

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
    onModifierChange([
      ...modifiers.slice(0, index),
      updated,
      ...modifiers.slice(index + 1),
    ])
  }

  const onMoveUp = (index: number) => {
    if (index === 0) return

    const newModifiers = [...modifiers]
    const temp = newModifiers[index - 1]
    newModifiers[index - 1] = newModifiers[index]
    newModifiers[index] = temp
    onModifierChange(newModifiers)
  }

  const onMoveDown = (index: number) => {
    if (index === modifiers.length - 1) return

    const newModifiers = [...modifiers]
    const temp = newModifiers[index + 1]
    newModifiers[index + 1] = newModifiers[index]
    newModifiers[index] = temp
    onModifierChange(newModifiers)
  }

  return (
    <div className="flex grow flex-col gap-4" data-testid="modifier-editor">
      <div className="flex flex-row justify-between gap-4">
        <div className="flex flex-col gap-1">
          <div className="text-lg font-semibold">
            {t("Dialog.Modifiers.Editor.Title")}
          </div>
          <div className="text-muted-foreground text-sm">
            {t("Dialog.Modifiers.Editor.Description")}
          </div>
        </div>
        <DropdownMenu>
          <DropdownMenuTrigger asChild className="self-end">
            <Button variant="default" className="w-fit" size={"sm"}>
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

      {modifiers.length === 0 ? (
        <div className="text-muted-foreground rounded border p-4 text-center text-sm">
          {t("Dialog.Modifiers.Editor.NoModifiers")}
        </div>
      ) : (
        <ScrollArea className="grow">
          <div className="flex flex-col gap-2">
            {modifiers.map((modifier, index) => (
              <ModifierItem
                key={index}
                modifier={modifier}
                onChange={(updated) => handleChange(index, updated)}
                onDelete={() => handleDelete(index)}
                onMoveUp={onMoveUp ? () => onMoveUp(index) : undefined}
                onMoveDown={onMoveDown ? () => onMoveDown(index) : undefined}
                isFirst={index === 0}
                isLast={index === modifiers.length - 1}
              />
            ))}
          </div>
        </ScrollArea>
      )}
    </div>
  )
}
export default ModifierEditor
