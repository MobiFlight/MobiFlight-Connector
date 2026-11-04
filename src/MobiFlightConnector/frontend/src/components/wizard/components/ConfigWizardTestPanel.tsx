import TwoStateIcon from "@/components/icons/TwoStateIcon"
import Input from "@/components/Input"
import ToolTip from "@/components/ToolTip"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import messageExchange from "@/lib/messageExchange"
import { IConfigItem } from "@/types"
import { CommandConfigContextMenu } from "@/types/commands"
import { IconFlask, IconPlayerStopFilled } from "@tabler/icons-react"
import { useState } from "react"
import { useTranslation } from "react-i18next"

export interface ConfigWizardTestPanelProps {
  configItem: IConfigItem | null
}

const ConfigWizardTestPanel = ({ configItem }: ConfigWizardTestPanelProps) => {
  const { t } = useTranslation()
  const { publish } = messageExchange
  const [testValue, setTestValue] = useState("123")
  const [isTesting, setIsTesting] = useState(false)

  const handleMenuItemClick = ({ action }: { action: "test" | "stop" }) => {
    if (action === "test") {
      setIsTesting(true)
      publish({
        key: "CommandConfigContextMenu",
        payload: {
          action: "testStart",
          item: configItem,
        },
      } as CommandConfigContextMenu)
    } else {
      setIsTesting(false)
      publish({
        key: "CommandConfigContextMenu",
        payload: {
          action: "testStop",
          item: configItem,
        },
      } as CommandConfigContextMenu)
    }
  }

  const [finalValue, setFinalValue] = useState<null | string>(testValue)

  return (
    <div className="flex grow flex-row items-center gap-4">
      <ToolTip
        content={
          isTesting
            ? t("Project.Execution.Test.StopMode")
              : t("Project.Execution.Test.Start")
        }
      >
        <div className="inline-flex">
          <Button
            disabled={!configItem}
            variant="outline"
            className="text-md gap-1 [&_svg]:size-6"
            onClick={() =>
              handleMenuItemClick({ action: !isTesting ? "test" : "stop" })
            }
          >
            <TwoStateIcon
              state={isTesting}
              primaryIcon={IconFlask}
              secondaryIcon={IconPlayerStopFilled}
              primaryClassName={
                configItem
                  ? "stroke-sky-600"
                  : "fill-none stroke-2 stroke-muted-foreground"
              }
              secondaryClassName="fill-red-700 stroke-red-700"
            />
            <div className="hidden pr-1 lg:inline-flex">
              {!isTesting
                ? t("Project.Execution.Test.Label")
                : t("Project.Execution.Test.Stop")}
            </div>
          </Button>
        </div>
      </ToolTip>
      <div className="text-sm font-medium">
        {t("ConfigList.Header.RawValue")}
      </div>
      <Input
        placeholder="Enter raw value"
        value={testValue}
        onChange={(e) => setTestValue(e)}
        className="max-w-32"
      />
      <div className="mt-1 h-3 w-16 border-t border-dashed border-gray-800"></div>
      <Badge variant="secondary" className="h-8">
        {finalValue}
      </Badge>
      <div className="text-sm font-medium">
        {t("ConfigList.Header.FinalValue")}
      </div>
    </div>
  )
}
export default ConfigWizardTestPanel
