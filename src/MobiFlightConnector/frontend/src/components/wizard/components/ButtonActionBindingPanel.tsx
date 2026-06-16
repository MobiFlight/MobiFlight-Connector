import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import { ActionSummary } from "@/components/wizard/components/ActionEditor"
import { Action, ButtonTrigger } from "@/types/config"
import { IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export type ButtonActionBindingPanelProps = {
  trigger?: ButtonTrigger
  onActionEdit: (
    action: Action | null,
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
      className="flex flex-col gap-4 rounded-md border px-6 py-3 pb-8 shadow-md"
    >
      <div className="flex flex-col gap-1">
        <div className="text-lg font-semibold">Actions</div>
        <div className="text-muted-foreground text-sm">
          Define the actions for each event.
        </div>
      </div>
      {tabs.map((tab, index) => {
        const action =
          tab === "onPress"
            ? current?.onPress
            : tab === "onRelease"
              ? current?.onRelease
              : tab === "onHold"
                ? current?.onHold
                : current?.onLongRelease

        const isLast = index === tabs.length - 1

        return action?.Type ? (
          <>
            <div
              className="hover:bg-accent/30 flex flex-row items-center gap-4 p-2 rounded-md"
              key={tab}
              onDoubleClick={() =>
                onActionEdit(action, (newAction) =>
                  handleOnActionChange(tab, newAction),
                )
              }
            >
              <div className="flex w-32 flex-col gap-1">
                <Label>Event</Label>
                <div>{t(`Dialog.InputConfigWizard.Button.Tabs.${tab}`)}</div>
              </div>
              <ActionSummary
                action={action}
                onActionEdit={() => {
                  onActionEdit(action, (newAction) =>
                    handleOnActionChange(tab, newAction),
                  )
                }}
              />
            </div>
            {!isLast && <Separator />}
          </>
        ) : (
          <>
            <Button
              className="w-1/2 self-center"
              size={"sm"}
              variant="outline"
              onClick={() => {
                onActionEdit(null, (newAction) =>
                  handleOnActionChange(tab, newAction),
                )
              }}
            >
              <IconPlus />
              {t(`Dialog.InputConfigWizard.Button.Tabs.${tab}`)}
            </Button>
            {!isLast && <Separator />}
          </>
        )
      })}
    </div>
  )
}
export default ButtonActionBindingPanel
