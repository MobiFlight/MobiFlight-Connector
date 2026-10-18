import { Card, CardContent } from "@/components/ui/card"
import { IConfigItem } from "@/types/config"
import { BaseDevice, Controller } from "@/types/controller"
import { useTranslation } from "react-i18next"

export type OutputConfigTriggerProps = {
  configItem: IConfigItem
  setConfigItem: (item: IConfigItem) => void
}

const OutputConfigTrigger = ({ configItem, setConfigItem }: OutputConfigTriggerProps) => {
  const { t } = useTranslation()

  const updateConfigItem = (
    controller: Partial<Controller> | undefined,
    device: BaseDevice | undefined,
  ) => {
    setConfigItem({
      ...configItem,
      Controller: controller
        ? {
            ...controller,
            // unset Devices
            // before sending it back to backend.
            Devices: undefined,
          }
        : undefined,
      Device: device
        ? {
            Name: device.Name,
            Type: device.Type,
          }
        : null,
    })
  }

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
        <div className="flex flex-row items-center gap-2">
          {/* This will provide the options for the source values */}
        </div>
      </CardContent>
    </Card>
  )
}
export default OutputConfigTrigger
