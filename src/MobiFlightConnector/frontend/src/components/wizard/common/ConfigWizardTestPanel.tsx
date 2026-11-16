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
import { Label } from "@/components/ui/label"
import DeviceIcon from "@/components/icons/DeviceIcon"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"

export interface ConfigWizardTestPanelProps {
  configItem: IConfigItem | null
  title: string
  description: string
  onTestValueChange: (value: ConnectorValue) => void
}

const CONFIG_CHANGE_TEST_DELAY = 150

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
  onTestValueChange
}: ConfigWizardTestPanelProps) => {
  const { t } = useTranslation("wizard")
  const { publish } = messageExchange
  const [testValue, setTestValue] = useState(() =>
    toText(configItem?.TestValue),
  )
  const [isTesting, setIsTesting] = useState(false)

  const runtimeState = useConfigItemStateValue(configItem?.GUID)

  // On closing the wizard, ensure the test is stopped if it was running
  // We have to keep track of the latest state across renders
  const latest = useRef({ isTesting, configItem, testValue })
  useEffect(() => {
    latest.current = { isTesting, configItem, testValue }
  })

  // A running test always shows the current draft,
  // so changes to the config restart the test with the new settings.
  // The delay avoids sending a command on every keystroke.
  useEffect(() => {
    const isTestRunning = latest.current.isTesting
    const hasConfig = !!configItem
    if (!isTestRunning || !hasConfig) return
    const timeout = setTimeout(
      () =>
        publish({
          key: "CommandConfigContextMenu",
          payload: {
            action: "testStart",
            item: {
              ...configItem,
              TestValue: toConnectorValue(latest.current.testValue),
            },
          },
        } as CommandConfigContextMenu),
      CONFIG_CHANGE_TEST_DELAY,
    )
    return () => clearTimeout(timeout)
  }, [configItem, publish])

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
    onTestValueChange(toConnectorValue(value))
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
    <WizardSection data-testid="configwizard-test-panel">
      <SectionHeadline title={title} description={description} />
      <div className="flex grow flex-row items-end gap-2">
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigList.Header.RawValue")}</Label>
          <Input
            placeholder={t("ConfigWizard.TestPanel.RawValuePlaceholder")}
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
            className="text-md h-8 gap-1 px-2 [&_svg]:size-6"
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
        <div className="mt-1 mb-1 h-3 flex-1 border-t border-dashed border-gray-800"></div>
        <Badge variant="secondary" className="h-8">
          {t("ConfigWizard.TestPanel.ModifierCount", {
            count: configItem?.Modifiers?.Items?.length ?? 0,
          })}
        </Badge>
        <div className="mt-1 mb-1 h-3 flex-1 border-t border-dashed border-gray-800"></div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigList.Header.FinalValue")}</Label>
          <Badge variant="secondary" className="h-8">
            {isTesting ? runtimeState?.Value : "?"}
          </Badge>
        </div>
        <div className="mt-1 mb-1 h-3 w-4 border-t border-dashed border-gray-800"></div>
        <DeviceIcon variant={configItem?.Device?.Type} />
      </div>
    </WizardSection>
  )
}
export default ConfigWizardTestPanel
