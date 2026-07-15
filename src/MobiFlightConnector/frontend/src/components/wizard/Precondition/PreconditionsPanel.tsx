import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import PreconditionEditor from "@/components/wizard/Precondition/PreconditionEditor"
import PreconditionSummary from "@/components/wizard/Precondition/PreconditionSummary"
import { useProjectStore } from "@/stores/projectStore"
import { useVariableStore } from "@/stores/variableStore"
import { IConfigItem, Precondition } from "@/types/config"
import { IconEdit, IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export type PreconditionsPanelProps = {
  preconditions: Precondition[] // Replace with actual type of preconditions
  onPreconditionsChange?: (updatedPreconditions: Precondition[]) => void
  variant: "summary" | "details"
  openDetailsPanel: () => void
}

const PreconditionsPanel = ({
  preconditions,
  onPreconditionsChange,
  variant,
  openDetailsPanel,
}: PreconditionsPanelProps) => {
  const { t } = useTranslation()
  const { project, activeConfigFileIndex } = useProjectStore()
  const { variables } = useVariableStore()
  const maxDisplayCount = 2

  const outputConfigs =
    project?.ConfigFiles[activeConfigFileIndex].ConfigItems.filter(
      (item) => item.Type === "OutputConfigItem",
    ) || ([] as IConfigItem[])

  const hasPreconditions = preconditions.length > 0

  return variant === "summary" ? (
    <Card data-testid="preconditions-panel" className="w-full h-full">
      <CardContent className="flex flex-col gap-1 pt-4">
        <div className="flex flex-row items-start justify-between gap-4">
          <div className="flex flex-col gap-2">
            <div className="text-lg font-semibold">
              {t("Dialog.InputConfigWizard.Preconditions.Title")}
            </div>
            {!hasPreconditions && (
              <div className="text-muted-foreground text-sm">
                {t("Dialog.InputConfigWizard.Preconditions.Description")}
              </div>
            )}
          </div>
          <Button variant="outline" size={"sm"} onClick={openDetailsPanel}>
            {hasPreconditions ? (
              <IconEdit className="" />
            ) : (
              <IconPlus className="" />
            )}
            {hasPreconditions
              ? t("Dialog.InputConfigWizard.Preconditions.EditButton")
              : t("Dialog.InputConfigWizard.Preconditions.AddButton")}
          </Button>
        </div>
        {hasPreconditions && (
          <PreconditionSummary
            preconditions={preconditions}
            outputConfigs={outputConfigs}
            maxDisplayCount={maxDisplayCount}
          />
        )}
      </CardContent>
    </Card>
  ) : (
    <PreconditionEditor
      variables={variables}
      outputConfigs={outputConfigs}
      preconditions={preconditions}
      onPreconditionsChange={onPreconditionsChange ?? (() => {})}
    />
  )
}
export default PreconditionsPanel
