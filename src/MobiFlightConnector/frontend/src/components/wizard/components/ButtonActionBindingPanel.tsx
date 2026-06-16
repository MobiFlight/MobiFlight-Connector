import { Label } from "@/components/ui/label"
import { ActionSummary } from "@/components/wizard/components/ActionEditor"
import { Action, ButtonTrigger } from "@/types/config"
import { useTranslation } from "react-i18next"

export type ButtonActionBindingPanelProps = {
  trigger?: ButtonTrigger
  onActionEdit: (
    action: Action,
    onConfigChange: (config: Action) => void,
  ) => void
  onTriggerChange: (trigger: ButtonTrigger) => void
}

const ButtonActionBindingPanel = ({
  trigger,
  onActionEdit,
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

  const handleOnActionChange = (tab: string, action: Action) => {
    onTriggerChange({
      ...current,
      [tab]: action,
    })
  }

  return (
    <div
      data-testid="button-action-panel"
      className="flex flex-col gap-4 rounded-md border px-6 py-3 shadow-md"
    >
      <div className="flex flex-col gap-1">
        <div className="text-lg font-semibold">Actions</div>
        <div className="text-muted-foreground text-sm">
          Define the actions for each event.
        </div>
      </div>
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
          <div
            className="flex flex-row items-center gap-4 rounded-md border p-2"
            key={tab}
          >
            <div className="flex w-32 flex-col gap-1">
              <Label>Event</Label>
              <div>{t(`Dialog.InputConfigWizard.Button.Tabs.${tab}`)}</div>
            </div>
            <ActionSummary
              action={action}
              onActionEdit={() => {
                const currentTab = tab
                const currentAction = action!
                onActionEdit(currentAction, (newAction) =>
                  handleOnActionChange(currentTab, newAction),
                )
              }}
            />
          </div>
        )
      })}
    </div>
  )
}
export default ButtonActionBindingPanel
