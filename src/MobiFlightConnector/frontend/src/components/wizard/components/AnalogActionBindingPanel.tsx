import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { ActionSummary } from "@/components/wizard/components/ActionEditor"
import { Action, AnalogTrigger } from "@/types/config"
import { IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export type AnalogActionBindingPanelProps = {
  trigger?: AnalogTrigger
  onActionEdit: (
    action: Action | null,
    onConfigChange: (config: Action) => void,
  ) => void
  onTriggerChange: (trigger: AnalogTrigger) => void
}

const AnalogActionBindingPanel = ({
  trigger,
  onActionEdit,
  onTriggerChange,
}: AnalogActionBindingPanelProps) => {
  const { t } = useTranslation()

  const defaultAnalogTrigger: AnalogTrigger = {
    onChange: undefined,
  }

  const current = trigger ?? defaultAnalogTrigger

  const handleOnActionChange = (tab: string, action: Action) => {
    onTriggerChange({
      ...current,
      [tab]: action,
    })
  }

  const action = current.onChange

  return (
    <div
      data-testid="button-action-panel"
      className="flex flex-col gap-4 rounded-md border px-6 py-3 pb-8 shadow-md"
    >
      <div className="flex flex-col gap-1">
        <div className="text-lg font-semibold">Actions</div>
        <div className="text-muted-foreground text-sm">
          Define the action for the available events.
        </div>
      </div>
      {action?.Type ? (
        <>
          <div
            className="hover:bg-accent bg-accent/30 flex flex-row items-center gap-4 rounded-md p-2"
            onDoubleClick={() =>
              onActionEdit(action, (newAction) =>
                handleOnActionChange("onChange", newAction),
              )
            }
          >
            <div className="flex w-32 flex-col gap-1">
              <Label>Event</Label>
              <div>{t(`Dialog.InputConfigWizard.Analog.Tabs.onChange`)}</div>
            </div>
            <ActionSummary
              action={action}
              onActionEdit={() => {
                onActionEdit(action, (newAction) =>
                  handleOnActionChange("onChange", newAction),
                )
              }}
            />
          </div>
        </>
      ) : (
        <>
          <Button
            className="w-1/2 self-center"
            size={"sm"}
            variant="outline"
            onClick={() => {
              onActionEdit(null, (newAction) =>
                handleOnActionChange("onChange", newAction),
              )
            }}
          >
            <IconPlus />
            {t(`Dialog.InputConfigWizard.Analog.Tabs.onChange`)}
          </Button>
        </>
      )}
    </div>
  )
}

export default AnalogActionBindingPanel
