import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import ConfigReferenceEditor from "@/components/wizard/ConfigReferences/ConfigReferenceEditor"
import ConfigReferenceSummary from "@/components/wizard/ConfigReferences/ConfigReferenceSummary"
import { useProjectStore } from "@/stores/projectStore"
import { ConfigReference, IConfigItem } from "@/types/config"
import { IconEdit, IconPlus } from "@tabler/icons-react"

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
  const { project, activeConfigFileIndex } = useProjectStore()
  const maxDisplayCount = 5

  const outputConfigs =
    project?.ConfigFiles[activeConfigFileIndex].ConfigItems.filter(
      (item) => item.Type === "OutputConfigItem",
    ) || ([] as IConfigItem[])

  return variant === "summary" ? (
    <Card>
      <CardContent className="flex flex-col gap-2 pt-4">
        <div className="text-lg font-semibold">Config References (optional)</div>
        {configReferences.length > 0 ? (
          <div className="flex flex-col gap-2">
            <ConfigReferenceSummary
              configReferences={configReferences}
              outputConfigs={outputConfigs}
              maxDisplayCount={maxDisplayCount}
            />
            <Button variant="outline" onClick={openDetailsPanel}>
              <IconEdit className="" />
              Config References
            </Button>
          </div>
        ) : (
          <div className="flex flex-col gap-2">
            <div className="text-muted-foreground text-sm">
              The config references define conditions that must be met before
              the action can be executed.
            </div>
            <Button variant="outline" onClick={openDetailsPanel}>
              <IconPlus className="" />
              Config References
            </Button>
          </div>
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
