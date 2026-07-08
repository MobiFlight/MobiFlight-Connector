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
  liveData: {
    rawValue: string | null | undefined
    finalValue: string | null | undefined
  }
}

const maxDisplayCount = 4

const ModifiersPanel = ({
  configItem,
  onConfigChange,
  openDetailsPanel,
  variant,
  liveData,
}: ModifiersPanelProps) => {
  const { t } = useTranslation()
  const modifiers = configItem.Modifiers?.Items || []

  return variant === "summary" ? (
    <Card data-testid="modifiers-panel" className="w-full">
      <CardContent className="flex flex-col gap-6 pt-4">
        <div className="flex flex-col">
          <div className="text-lg font-semibold">
            {t("Dialog.Modifiers.Title")}
          </div>
          <div className="text-muted-foreground text-sm">
            {t("Dialog.Modifiers.Description")}
          </div>
        </div>

        <ModifierSummary
          rawValue={liveData.rawValue ?? "?"}
          finalValue={liveData.finalValue ?? "?"}
          modifiers={modifiers}
          maxDisplayCount={maxDisplayCount}
        />
        {modifiers.length === 0 ? (
          <Button variant="outline" size={"sm"} onClick={openDetailsPanel}>
            <IconPlus className="" />
            {t("Dialog.Modifiers.AddButton")}
          </Button>
        ) : (
          <Button variant="outline" size={"sm"} onClick={openDetailsPanel}>
            <IconEdit className="" />
            {t("Dialog.Modifiers.EditButton")}
          </Button>
        )}
      </CardContent>
    </Card>
  ) : (
    <ModifierEditor
      modifiers={modifiers}
      onModifierChange={(updatedModifiers) =>
        onConfigChange({
          ...configItem,
          Modifiers: { Items: updatedModifiers },
        })
      }
    />
  )
}
export default ModifiersPanel
