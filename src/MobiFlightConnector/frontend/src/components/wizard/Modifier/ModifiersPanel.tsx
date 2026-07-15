import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import ModifierEditor from "@/components/wizard/Modifier/ModifierEditor"
import ModifierSummary from "@/components/wizard/Modifier/ModifierSummary"
import { cn } from "@/lib/utils"
import { IConfigItem } from "@/types"
import { IconEdit } from "@tabler/icons-react"
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
  const hasModifiers = modifiers.length > 0

  return variant === "summary" ? (
    <Card
      data-testid="modifiers-panel"
      className={cn(
        "w-full shadow-none transition-shadow hover:shadow-md",
        hasModifiers && "border-foreground/30 shadow-sm",
      )}
    >
      <CardContent className="flex flex-col gap-4 pt-4">
        <div className="flex flex-row items-start justify-between gap-4">
          <div className="flex flex-col gap-2">
            <div className="text-lg font-semibold">
              {t("Dialog.Modifiers.Title")}
            </div>
            <div className="text-muted-foreground text-sm">
              {t("Dialog.Modifiers.Description")}
            </div>
          </div>
          {hasModifiers && (
            <Button
              variant="ghost"
              size={"sm"}
              onClick={openDetailsPanel}
              aria-label={t("Dialog.Modifiers.EditButton")}
            >
              <IconEdit className="" />
            </Button>
          )}
        </div>

        <ModifierSummary
          rawValue={liveData.rawValue ?? "?"}
          finalValue={liveData.finalValue ?? "?"}
          modifiers={modifiers}
          maxDisplayCount={maxDisplayCount}
        />
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
