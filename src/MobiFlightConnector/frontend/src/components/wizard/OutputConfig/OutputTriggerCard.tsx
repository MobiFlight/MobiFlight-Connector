import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { IConfigItem, OutputConfigItem } from "@/types/config"
import { useTranslation } from "react-i18next"
import SourceSummary from "./SourceSummary"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"

export type OutputTriggerCardProps = {
  configItem: IConfigItem
  setConfigItem: (item: IConfigItem) => void
  navigateToDetailView: (view: string, size?: "small" | "large") => void
}

const OutputTriggerCard = ({
  configItem,
  setConfigItem,
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
        <SectionHeadline
          title={t("Dialog.InputConfigWizard.ConfigTrigger.Title")}
          description={t("Dialog.InputConfigWizard.ConfigTrigger.Description")}
        />
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
          <SourceSummary
            source={outputConfigItem.Source}
            onEdit={() => navigateToDetailView("source")}
            onRemove={() => setConfigItem({ ...configItem as OutputConfigItem, Source: undefined } as IConfigItem)}
          />
        )}
      </CardContent>
    </Card>
  )
}
export default OutputTriggerCard
