import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import { ActionSummary } from "@/components/wizard/components/ActionEditor"
import { Action, AnalogTrigger, ButtonTrigger, EncoderTrigger } from "@/types/config"
import { IconEdit, IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export type ActionTrigger = ButtonTrigger | EncoderTrigger | AnalogTrigger

export type ActionBindingPanelProps = {
  variant: "button" | "encoder" | "analog"
  trigger?: ActionTrigger
  onActionEdit: (
    action: Action | null,
    onConfigChange: (config: Action) => void,
  ) => void
  onTriggerChange: (trigger: ActionTrigger) => void
}

const eventActionMap = {
  "button": ["onPress", "onRelease", "onHold", "onLongRelease"],
  "encoder": ["onLeft", "onRight", "onLeftFast", "onRightFast"],
  "analog": ["onChange"],
}

const ActionBindingPanel = ({
  variant,
  trigger,
  onActionEdit,
  onTriggerChange,
}: ActionBindingPanelProps) => {
  const { t } = useTranslation()
  const events = eventActionMap[variant]
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
      data-testid={`${variant}-action-panel`}
      className="flex flex-col gap-4 rounded-md border px-6 py-3 pb-8 shadow-md"
    >
      <div className="flex flex-col gap-1">
        <div className="text-lg font-semibold">Actions</div>
        <div className="text-muted-foreground text-sm">
          Define the actions for each event.
        </div>
      </div>
      {events.map((tab, index) => {
        const action = current[tab as keyof ActionTrigger] as Action | undefined

        const isLast = index === events.length - 1

        return action?.Type ? (
          <>
            <div
              className="hover:bg-accent/30 flex flex-row items-center gap-4 rounded-md p-2"
              key={tab}
              onDoubleClick={() =>
                onActionEdit(action, (newAction) =>
                  handleOnActionChange(tab, newAction),
                )
              }
            >
              <div className="flex w-32 flex-col gap-1">
                <Label>Event</Label>
                <div>{t(`Dialog.InputConfigWizard.${variant}.Event.${tab}`)}</div>
              </div>
              <ActionSummary
                action={action}
              />
              <Button
                size={"sm"}
                variant="ghost"
                onClick={() => {
                  console.log("Edit action", action)
                  onActionEdit(action, (newAction) =>
                    handleOnActionChange(tab, newAction),
                  )
                }}
              >
                <IconEdit />
              </Button>
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
              {t(`Dialog.InputConfigWizard.${variant}.Event.${tab}`)}
            </Button>
            {!isLast && <Separator />}
          </>
        )
      })}
    </div>
  )
}
export default ActionBindingPanel
