import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import ConfigWizardTestPanel from "@/components/wizard/common/ConfigWizardTestPanel"
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
      data-testid="modifier-panel"
      className={cn(
        "w-full shadow-none transition-shadow hover:shadow-md",
        hasModifiers && "border-foreground/30 shadow-sm",
      )}
      onDoubleClick={openDetailsPanel}
    >
      <CardContent className="flex flex-col gap-4 pt-4">
        <div className="flex flex-row items-start justify-between gap-4">
          <SectionHeadline
            title={t("Dialog.Modifiers.Title")}
            description={t("Dialog.Modifiers.Description")}
          />
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
    <div className="flex grow flex-col gap-4">
      <ModifierEditor
        mode={configItem.Type}
        configName={configItem.Name}
        modifiers={modifiers}
        onModifierChange={(updatedModifiers) =>
          onConfigChange({
            ...configItem,
            Modifiers: { Items: updatedModifiers },
          })
        }
      />
      {configItem.Type === "OutputConfigItem" && (
        <ConfigWizardTestPanel
          configItem={configItem}
          title={t("Dialog.Modifiers.Editor.TestPanel.Title")}
          description={t("Dialog.Modifiers.Editor.TestPanel.Description")}
          onTestValueChange={(value) => {
            onConfigChange({
              ...configItem,
              TestValue: value
            })
          }}
        />
      )}
    </div>
  )
}
export default ModifiersPanel
