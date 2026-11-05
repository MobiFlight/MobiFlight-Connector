import TwoStateIcon from "@/components/icons/TwoStateIcon"
import Input from "@/components/Input"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import messageExchange from "@/lib/messageExchange"
import { useConfigItemStateValue } from "@/stores/configItemStateStore"
import { ConnectorValue, IConfigItem } from "@/types/config"
import { ConnectorValueType } from "@/types/connectorValue"
import { CommandConfigContextMenu } from "@/types/commands"
import { IconFlask, IconPlayerStopFilled } from "@tabler/icons-react"
import { useEffect, useRef, useState } from "react"
import { useTranslation } from "react-i18next"
import { Card, CardContent } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import DeviceIcon from "@/components/icons/DeviceIcon"

export interface ConfigWizardTestPanelProps {
  configItem: IConfigItem | null
  title: string
  description: string
}

// These two methods should be moved
// to a utility file for handling ConnectorValue conversions
const toText = (value?: ConnectorValue) =>
  value?.type === ConnectorValueType.String
    ? (value.String ?? "")
    : String(value?.Float64 ?? "")

const toConnectorValue = (text: string): ConnectorValue => {
  const number = Number(text)
  return text.trim() !== "" && !isNaN(number)
    ? { type: ConnectorValueType.Float, Float64: number, String: null }
    : { type: ConnectorValueType.String, Float64: 0, String: text }
}

const ConfigWizardTestPanel = ({
  configItem,
  title,
  description,
}: ConfigWizardTestPanelProps) => {
  const { t } = useTranslation()
  const { publish } = messageExchange
  const [testValue, setTestValue] = useState(() =>
    toText(configItem?.TestValue),
  )
  const [isTesting, setIsTesting] = useState(false)

  const runtimeState = useConfigItemStateValue(configItem?.GUID)

  // On closing the wizard, ensure the test is stopped if it was running
  // We have to keep track of the latest state across renders
  const latest = useRef({ isTesting, configItem })
  useEffect(() => {
    latest.current = { isTesting, configItem }
  })

  // This runs only on unmount
  useEffect(
    () => () => {
      if (!latest.current.isTesting) return
      publish({
        key: "CommandConfigContextMenu",
        payload: { action: "testStop", item: latest.current.configItem },
      } as CommandConfigContextMenu)
    },
    [publish],
  )

  const publishTestStart = (value: string) =>
    publish({
      key: "CommandConfigContextMenu",
      payload: {
        action: "testStart",
        item: { ...configItem, TestValue: toConnectorValue(value) },
      },
    } as CommandConfigContextMenu)

  const handleValueChange = (value: string) => {
    setTestValue(value)
    if (isTesting) publishTestStart(value)
  }

  const handleMenuItemClick = ({ action }: { action: "test" | "stop" }) => {
    if (action === "test") {
      setIsTesting(true)
      publishTestStart(testValue)
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

  return (
    <Card>
      <CardContent className="flex flex-col gap-4">
        <div className="flex flex-col gap-1 pt-4">
          <div className="text-lg font-semibold">{title}</div>
          <div className="text-muted-foreground text-sm">{description}</div>
        </div>
        <div className="flex grow flex-row items-end gap-2">
          <div className="flex flex-col gap-1">
            <Label>
              {t("ConfigList.Header.RawValue")}
            </Label>
            <Input
              placeholder="Enter raw value"
              value={testValue}
              onChange={handleValueChange}
              className="max-w-16"
            />
          </div>
          <div className="inline-flex">
            <Button
              disabled={!configItem}
              variant="outline"
              size="sm"
              className="h-8 px-2 text-md gap-1 [&_svg]:size-6"
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
          <div className="mt-1 h-3 flex-1 border-t border-dashed border-gray-800 mb-1"></div>
          <Badge variant="secondary" className="h-8">
            {`${configItem?.Modifiers?.Items?.length ?? "0"} modifiers`}
          </Badge>
          <div className="mt-1 h-3 flex-1 border-t border-dashed border-gray-800 mb-1"></div>
          <div className="flex flex-col gap-1">
            <Label>
              {t("ConfigList.Header.FinalValue")}
            </Label>
            <Badge variant="secondary" className="h-8">
              {isTesting ? runtimeState?.Value : "?"}
            </Badge>
          </div>
          <div className="mt-1 h-3 w-4 border-t border-dashed border-gray-800 mb-1"></div>
          <DeviceIcon  variant={configItem?.Device?.Type} />
        </div>
      </CardContent>
    </Card>
  )
}
export default ConfigWizardTestPanel
