import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import { ActionSummary } from "@/components/wizard/components/ActionEditor"
import {
  Action,
  AnalogTrigger,
  ButtonHoldOptions,
  ButtonLongReleaseOptions,
  ButtonTrigger,
  EncoderTrigger,
} from "@/types/config"
import { IconEdit, IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"
export type ActionTrigger = ButtonTrigger | EncoderTrigger | AnalogTrigger
export type ActionBindingPanelProps = {
  variant: "button" | "encoder" | "analog"
  trigger?: ActionTrigger
  onActionEdit: (
    event: string,
    action: Action | null,
    onActionChange: (
      config: Action,
      buttonOptions?: Partial<ButtonHoldOptions> &
        Partial<ButtonLongReleaseOptions>,
    ) => void,
  ) => void
  onTriggerChange: (trigger: ActionTrigger) => void
}
const eventActionMap = {
  button: ["onPress", "onRelease", "onHold", "onLongRelease"],
  encoder: ["onLeft", "onRight", "onLeftFast", "onRightFast"],
  analog: ["onChange"],
}
const ActionBindingPanel = ({
  variant,
  trigger,
  onActionEdit,
  onTriggerChange,
}: ActionBindingPanelProps) => {
  const { t } = useTranslation()
  const events = eventActionMap[variant]
  const defaultButtonTrigger: ButtonTrigger &
    Partial<ButtonHoldOptions> &
    Partial<ButtonLongReleaseOptions> = {
    onPress: undefined,
    onRelease: undefined,
    onHold: undefined,
    onLongRelease: undefined,
    HoldDelay: 350,
    LongReleaseDelay: 350,
    RepeatDelay: 0,
  }
  const current = trigger ?? defaultButtonTrigger
  const handleOnActionChange = (
    event: string,
    action: Action,
    buttonOptions?: Partial<ButtonHoldOptions> &
      Partial<ButtonLongReleaseOptions>,
  ) => {
    onTriggerChange({
      ...current,
      [event]: action,
      ...buttonOptions,
    })
  }
  return (
    <div
      data-testid="action-panel"
      className="flex flex-col gap-2 rounded-md border px-6 py-3 pb-8 shadow-md"
    >
      <div className="flex flex-col gap-1">
        <div className="text-lg font-semibold">
          {t(`Dialog.InputConfigWizard.Action.Title`)}</div>
        <div className="text-muted-foreground text-sm">
          {t(`Dialog.InputConfigWizard.Action.Description`)}
        </div>
      </div>
      {events.map((event, index) => {
        const action = current[event as keyof ActionTrigger] as
          | Action
          | undefined
        const isLast = index === events.length - 1
        const eventLabel = t(
          `Dialog.InputConfigWizard.${variant}.Event.${event}.label`,
        )
        return (
          action?.Type && (
            <div key={event} className="flex flex-col gap-2">
              <div
                className="hover:bg-accent/30 flex flex-row items-center gap-4 rounded-md p-2"
                onDoubleClick={() =>
                  onActionEdit(event, action, (newAction, buttonOptions) =>
                    handleOnActionChange(event, newAction, buttonOptions),
                  )
                }
              >
                <div className="flex w-32 flex-col gap-1">
                  <Label>Event</Label>
                  <div>{eventLabel}</div>
                </div>
                <ActionSummary action={action} />
                <Button
                  size={"sm"}
                  variant="ghost"
                  onClick={() => {
                    onActionEdit(event, action, (newAction, buttonOptions) =>
                      handleOnActionChange(event, newAction, buttonOptions),
                    )
                  }}
                >
                  <IconEdit />
                  <span className="sr-only">
                    {t(`Dialog.InputConfigWizard.Event.Edit`, { eventLabel })}
                  </span>
                </Button>
              </div>
              {!isLast && <Separator />}
            </div>
          )
        )
      })}

      <div className="flex flex-row gap-2">
        {events.map((event) => {
          const action = current[event as keyof ActionTrigger] as
            | Action
            | undefined
          const eventLabel = t(
            `Dialog.InputConfigWizard.${variant}.Event.${event}.label`,
          )
          return (
            !action?.Type && (
              <Button                
                key={event}
                size={"sm"}
                variant="outline"
                onClick={() => {
                  onActionEdit(event, null, (newAction, buttonOptions) =>
                    handleOnActionChange(event, newAction, buttonOptions),
                  )
                }}
              >
                <IconPlus />
                {eventLabel}
              </Button>
            )
          )
        })}
      </div>
    </div>
  )
}
export default ActionBindingPanel
