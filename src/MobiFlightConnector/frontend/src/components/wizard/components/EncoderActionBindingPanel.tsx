import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import ActionEditor from "@/components/wizard/components/ActionEditor"
import { EncoderTrigger } from "@/types/config"
import { useTranslation } from "react-i18next"

export type EncoderActionBindingPanelProps = {
  trigger?: EncoderTrigger
  onTriggerChange: (trigger: EncoderTrigger) => void
}

const EncoderActionBindingPanel = ({
  trigger,
  onTriggerChange,
}: EncoderActionBindingPanelProps) => {
  const { t } = useTranslation()
  const tabs = ["onLeft", "onRight", "onLeftFast", "onRightFast"]
  const defaultEncoderTrigger: EncoderTrigger = {
    onLeft: undefined,
    onRight: undefined,
    onLeftFast: undefined,
    onRightFast: undefined,
  }

  const current = trigger ?? defaultEncoderTrigger

  return (
    <Tabs data-testid="encoder-action-panel" defaultValue={tabs[0]}>
      <TabsList>
        {tabs.map((trigger) => (
          <TabsTrigger key={trigger} value={trigger}>
            {t(`Dialog.InputConfigWizard.Encoder.Tabs.${trigger}`)}
          </TabsTrigger>
        ))}
      </TabsList>
      {tabs.map((tab) => {
        const action =
          tab == "onLeft"
            ? trigger?.onLeft
            : tab === "onRight"
              ? trigger?.onRight
              : tab === "onLeftFast"
                ? trigger?.onLeftFast
                : trigger?.onRightFast
        return (
          <TabsContent key={tab} value={tab}>
            <ActionEditor
              action={action}
              onActionChange={(action) => {
                onTriggerChange({
                  ...current,
                  [tab]: action,
                })
              }}
            />
          </TabsContent>
        )
      })}
    </Tabs>
  )
}
export default EncoderActionBindingPanel
