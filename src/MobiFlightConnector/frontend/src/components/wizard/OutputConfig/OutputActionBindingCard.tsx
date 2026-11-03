import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import OutputDeviceSummary from "@/components/wizard/OutputConfig/OutputDeviceSummary"
import { OutputConfig, OutputConfigItem } from "@/types/config"
import { IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export interface OutputActionBindingCardProps {
  config: OutputConfigItem
  onEdit: () => void
  onAddOutputDevice: () => void
  onAddButtonAction: () => void
  onAddAnalogAction: () => void
}

const OutputActionBindingCard = ({
  config,
  onEdit,
  onAddOutputDevice,
  onAddButtonAction,
  onAddAnalogAction,
}: OutputActionBindingCardProps) => {
  const { t } = useTranslation()

  // an input action is shown by its own ActionBindingPanel instead of this card
  const hasOutputDevice = config.Device != null

  return (
    <Card
      data-testid="action-panel"
      className="border-primary shadow-md transition-shadow hover:shadow-lg"
    >
      <CardContent className="flex flex-col gap-2 py-4">
        <div className="flex flex-col gap-1">
          <div className="text-lg font-semibold">
            {t(`Dialog.InputConfigWizard.Action.Title`)}
          </div>
          <div className="text-muted-foreground text-sm">
            {t(`Dialog.InputConfigWizard.Action.Description`)}
          </div>
        </div>

        { hasOutputDevice && (
          <div className="flex flex-row gap-2">
            {/* Display the output device here */}
            <OutputDeviceSummary config={config.Device as OutputConfig} controller={config.Controller} onEdit={onEdit} />
          </div>
        )}

        { !hasOutputDevice && (
        <div className="flex flex-row gap-2">
          {/* Add your display binding controls here */}
          <Button size={"sm"} variant="outline" onClick={onAddOutputDevice}>
            <IconPlus />
            Output device
          </Button>
          <Button size={"sm"} variant="outline" onClick={onAddButtonAction}>
            <IconPlus />
            {t("Dialog.ConfigWizard.Output.InputAction.Button")}
          </Button>
          <Button size={"sm"} variant="outline" onClick={onAddAnalogAction}>
            <IconPlus />
            {t("Dialog.ConfigWizard.Output.InputAction.Analog")}
          </Button>
        </div>
        )}
      </CardContent>
    </Card>
  )
}
export default OutputActionBindingCard
