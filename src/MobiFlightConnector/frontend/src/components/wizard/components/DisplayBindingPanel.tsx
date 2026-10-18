import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import OutputDeviceSummary from "@/components/wizard/components/OutputDevices/OutputDeviceSummary"
import { OutputConfig, OutputConfigItem } from "@/types/config"
import { IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export interface DisplayBindingPanelProps {
  config: OutputConfigItem
}

const DisplayBindingPanel = ({ config }: DisplayBindingPanelProps) => {
  const { t } = useTranslation()

  const hasOutputDevice = config.Device !== undefined
  const hasInputAction = config.DeviceType === "InputAction"
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
            <OutputDeviceSummary config={config.Device as OutputConfig} />
          </div>
        )}

        { hasInputAction && (
          <div className="flex flex-row gap-2">
            {/* Display the input action here */}
            Input Action Editor
          </div>
        )}

        { (!hasOutputDevice && !hasInputAction) && (
        <div className="flex flex-row gap-2">
          {/* Add your display binding controls here */}
          <Button size={"sm"} variant="outline" onClick={() => {}}>
            <IconPlus />
            Output device
          </Button>
          <Button size={"sm"} variant="outline" onClick={() => {}}>
            <IconPlus />
            Input action
          </Button>
        </div>
        )}
      </CardContent>
    </Card>
  )
}
export default DisplayBindingPanel
