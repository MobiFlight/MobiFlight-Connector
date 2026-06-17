import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import { ActionSummary } from "@/components/wizard/components/ActionEditor"
import { Action, EncoderTrigger } from "@/types/config"
import { IconEdit, IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export type EncoderActionBindingPanelProps = {
  trigger?: EncoderTrigger
  onActionEdit: (
      action: Action | null,
      onConfigChange: (config: Action) => void,
    ) => void
  onTriggerChange: (trigger: EncoderTrigger) => void
}

const EncoderActionBindingPanel = ({
  trigger,
  onActionEdit,
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

  const handleOnActionChange = (tab: string, action: Action) => {
    onTriggerChange({
      ...current,
      [tab]: action,
    })
  }

  return (
    <div
      data-testid="encoder-action-panel"
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
          tab == "onLeft"
            ? trigger?.onLeft
            : tab === "onRight"
              ? trigger?.onRight
              : tab === "onLeftFast"
                ? trigger?.onLeftFast
                : trigger?.onRightFast

        const isLast = index === tabs.length - 1

        return action?.Type ? (
          <>
            <div
              className="hover:bg-accent/20 flex flex-row items-center gap-4 p-2 rounded-md"
              key={tab}
              onDoubleClick={() =>
                onActionEdit(action, (newAction) =>
                  handleOnActionChange(tab, newAction),
                )
              }
            >
              <div className="flex w-32 flex-col gap-1">
                <Label>Event</Label>
                <div>{t(`Dialog.InputConfigWizard.Encoder.Tabs.${tab}`)}</div>
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
              {t(`Dialog.InputConfigWizard.Encoder.Tabs.${tab}`)}
            </Button>
            {!isLast && <Separator />}
          </>
        )
      })}
    </div>
  )
}
export default EncoderActionBindingPanel
