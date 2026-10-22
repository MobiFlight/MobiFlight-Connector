import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { IConfigItem, OutputConfigItem } from "@/types/config"
import { useTranslation } from "react-i18next"
import SourceSummary from "./SourceSummary"

export type OutputTriggerCardProps = {
  configItem: IConfigItem
  // setConfigItem: (item: IConfigItem) => void
  navigateToDetailView: (view: string, size?: "small" | "large") => void
}

const OutputTriggerCard = ({
  configItem,
  // setConfigItem,
  navigateToDetailView,
}: OutputTriggerCardProps) => {
  const { t } = useTranslation()

  const outputConfigItem = configItem as OutputConfigItem

  return (
    <Card
      data-testid="trigger-panel"
      className="border-primary transition-shadow hover:shadow-md"
    >
      <CardContent className="flex flex-col gap-4 pt-4">
        <div className="flex flex-col gap-2">
          <div className="text-lg font-semibold">
            {t("Dialog.InputConfigWizard.ConfigTrigger.Title")}
          </div>
          <div className="text-muted-foreground text-sm">
            {t("Dialog.InputConfigWizard.ConfigTrigger.Description")}
          </div>
        </div>
        {outputConfigItem.Source == null ? (
          <div className="flex flex-row items-center gap-2">
            <Button
              onClick={() => navigateToDetailView("source")}
              className="px-5"
              size="sm"
            >
              Select value
            </Button>
          </div>
        ) : (
          <SourceSummary source={outputConfigItem.Source} onEdit={() => navigateToDetailView("source")} />
        )}
      </CardContent>
    </Card>
  )
}
export default OutputTriggerCard
