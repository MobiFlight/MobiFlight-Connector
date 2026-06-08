import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import ActionEditor from "@/components/wizard/components/ActionEditor"
import { ButtonTrigger } from "@/types/config"
import { useTranslation } from "react-i18next"

export type ButtonActionBindingPanelProps = {
  trigger?: ButtonTrigger
  onTriggerChange: (trigger: ButtonTrigger) => void
}

const ButtonActionBindingPanel = ({
  trigger,
  onTriggerChange,
}: ButtonActionBindingPanelProps) => {
  const { t } = useTranslation()
  const tabs = ["onPress", "onRelease", "onHold", "onLongRelease"]
  const defaultButtonTrigger: ButtonTrigger = {
    onPress: undefined,
    onRelease: undefined,
    onHold: undefined,
    onLongRelease: undefined,
    HoldDelay: 350,
    LongReleaseDelay: 350,
    RepeatDelay: 0,
  }

  const current = trigger ?? defaultButtonTrigger

  return (
    <Tabs data-testid="button-action-panel" defaultValue={tabs[0]}>
      <TabsList>
        {tabs.map((tab) => (
          <TabsTrigger key={tab} value={tab}>
            {t(`Dialog.InputConfigWizard.Button.Tabs.${tab}`)}
          </TabsTrigger>
        ))}
      </TabsList>
      {tabs.map((tab) => {
        const action =
          tab === "onPress"
            ? current?.onPress
            : tab === "onRelease"
              ? current?.onRelease
              : tab === "onHold"
                ? current?.onHold
                : current?.onLongRelease
        return (
          <TabsContent key={tab} value={tab}>
            <ActionEditor
              action={action}
              onActionChange={(action) =>
                onTriggerChange({
                  ...current,
                  [tab]: action,
                })
              }
            />
          </TabsContent>
        )
      })}
    </Tabs>
  )
}
export default ButtonActionBindingPanel
