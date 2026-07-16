import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import ConfigReferenceEditor from "@/components/wizard/ConfigReferences/ConfigReferenceEditor"
import ConfigReferenceSummary from "@/components/wizard/ConfigReferences/ConfigReferenceSummary"
import { cn } from "@/lib/utils"
import { useProjectStore } from "@/stores/projectStore"
import { ConfigReference, IConfigItem } from "@/types/config"
import { IconEdit, IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

type ConfigReferencePanelProps = {
  configReferences: ConfigReference[]
  onConfigReferencesChange?: (configReferences: ConfigReference[]) => void
  variant: "summary" | "details"
  openDetailsPanel: () => void
}
const ConfigReferencePanel = ({
  configReferences,
  onConfigReferencesChange,
  variant,
  openDetailsPanel,
}: ConfigReferencePanelProps) => {
  const { t } = useTranslation()
  const { project, activeConfigFileIndex } = useProjectStore()
  const maxDisplayCount = 2
  const outputConfigs =
    project?.ConfigFiles[activeConfigFileIndex].ConfigItems.filter(
      (item) => item.Type === "OutputConfigItem",
    ) || ([] as IConfigItem[])

  const hasConfigReferences = configReferences.length > 0

  return variant === "summary" ? (
    <Card
      data-testid="config-reference-panel"
      className={cn(
        "h-full w-full shadow-none transition-shadow hover:shadow-md",
        hasConfigReferences && "border-foreground/30 shadow-sm",
      )}
      onDoubleClick={openDetailsPanel}
    >
      <CardContent className="flex flex-col gap-1 pt-4">
        <div className="flex flex-row items-start justify-between gap-4">
          <div className="flex flex-col gap-2">
            <div className="text-lg font-semibold">
              {t("Dialog.InputConfigWizard.ConfigReferences.Title")}
            </div>
            {!hasConfigReferences && (
              <div className="text-muted-foreground text-sm">
                {t("Dialog.InputConfigWizard.ConfigReferences.Description")}
              </div>
            )}
          </div>
          {hasConfigReferences ? (
            <Button
              variant="ghost"
              size={"sm"}
              onClick={openDetailsPanel}
              aria-label={t(
                "Dialog.InputConfigWizard.ConfigReferences.EditButton",
              )}
            >
              <IconEdit />
            </Button>
          ) : (
            <Button
              variant="outline"
              size={"sm"}
              onClick={openDetailsPanel}
              aria-label={t(
                "Dialog.InputConfigWizard.ConfigReferences.AddButton",
              )}
            >
              <IconPlus className="" />
              {t("Dialog.InputConfigWizard.ConfigReferences.Label")}
            </Button>
          )}
        </div>
        {hasConfigReferences && (
          <ConfigReferenceSummary
            configReferences={configReferences}
            outputConfigs={outputConfigs}
            maxDisplayCount={maxDisplayCount}
          />
        )}
      </CardContent>
    </Card>
  ) : (
    <ConfigReferenceEditor
      outputConfigs={outputConfigs}
      configReferences={configReferences}
      onConfigReferencesChange={onConfigReferencesChange ?? (() => {})}
    />
  )
}
export default ConfigReferencePanel
