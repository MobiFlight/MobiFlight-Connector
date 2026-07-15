import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import ConfigReferenceEditor from "@/components/wizard/ConfigReferences/ConfigReferenceEditor"
import ConfigReferenceSummary from "@/components/wizard/ConfigReferences/ConfigReferenceSummary"
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
  const maxDisplayCount = 5

  const outputConfigs =
    project?.ConfigFiles[activeConfigFileIndex].ConfigItems.filter(
      (item) => item.Type === "OutputConfigItem",
    ) || ([] as IConfigItem[])

  const hasConfigReferences = configReferences.length > 0

  return variant === "summary" ? (
    <Card data-testid="config-references-panel" className="w-full h-full">
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
          <Button variant="outline" size={"sm"} onClick={openDetailsPanel}>
            {hasConfigReferences ? (
              <IconEdit className="" />
            ) : (
              <IconPlus className="" />
            )}
            {hasConfigReferences
              ? t("Dialog.InputConfigWizard.ConfigReferences.EditButton")
              : t("Dialog.InputConfigWizard.ConfigReferences.AddButton")}
          </Button>
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
