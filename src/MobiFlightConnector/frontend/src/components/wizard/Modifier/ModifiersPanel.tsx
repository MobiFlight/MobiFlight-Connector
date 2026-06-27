import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import ModifierEditor from "@/components/wizard/Modifier/ModifierEditor"
import ModifierSummary from "@/components/wizard/Modifier/ModifierSummary"
import { IConfigItem } from "@/types"
import { IconEdit, IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

type ModifiersPanelProps = {
  configItem: IConfigItem
  onConfigChange: (configItem: IConfigItem) => void
  openDetailsPanel: () => void
  variant: "summary" | "details"
}

const maxDisplayCount = 4

const ModifiersPanel = ({
  configItem,
  onConfigChange,
  openDetailsPanel,
  variant,
}: ModifiersPanelProps) => {
  const { t } = useTranslation()
  const modifiers = configItem.Modifiers?.Items || []

  return variant === "summary" ? (
    <Card data-testid="modifiers-panel" className="w-full">
      <CardContent className="flex flex-col gap-2 pt-4">
        <div className="text-lg font-semibold">
          {t("Dialog.Modifiers.Title")}
        </div>
        {modifiers.length > 0 ? (
          <div className="flex flex-col gap-2">
            <ModifierSummary
              modifiers={modifiers}
              maxDisplayCount={maxDisplayCount}
            />
            <Button variant="outline" onClick={openDetailsPanel}>
              <IconEdit className="" />
              {t("Dialog.Modifiers.EditButton")}
            </Button>
          </div>
        ) : (
          <div className="flex flex-col gap-2">
            <div className="text-muted-foreground text-sm">
              {t("Dialog.Modifiers.Description")}
            </div>
            <Button variant="outline" onClick={openDetailsPanel}>
              <IconPlus className="" />
              {t("Dialog.Modifiers.AddButton")}
            </Button>
          </div>
        )}
      </CardContent>
    </Card>
  ) : (
    <ModifierEditor
      modifiers={modifiers}
      onModifierChange={(updatedModifiers) =>
        onConfigChange({ ...configItem, Modifiers: { Items: updatedModifiers } })
      }
    />
  )
}
export default ModifiersPanel
