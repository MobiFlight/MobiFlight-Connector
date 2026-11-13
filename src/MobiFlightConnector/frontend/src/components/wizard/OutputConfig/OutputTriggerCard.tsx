import { Button } from "@/components/ui/button"
import { IconPlus } from "@tabler/icons-react"
import { Card, CardContent } from "@/components/ui/card"
import { IConfigItem, OutputConfigItem } from "@/types/config"
import { useTranslation } from "react-i18next"
import SourceSummary from "./SourceSummary"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"

export type OutputTriggerCardProps = {
  configItem: IConfigItem
  setConfigItem: (item: IConfigItem) => void
  navigateToDetailView: (view: string, size?: "small" | "large") => void
  liveData: {
    rawValue: string | null | undefined
    finalValue: string | null | undefined
  }
}

const OutputTriggerCard = ({
  configItem,
  setConfigItem,
  navigateToDetailView,
  liveData
}: OutputTriggerCardProps) => {
  const { t } = useTranslation("wizard")

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
              <IconPlus />
              {t("ConfigWizard.Source.Label")}
            </Button>
          </div>
        ) : (
          <SourceSummary
            liveData={liveData}
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
