import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import ActionEditor from "@/components/wizard/components/ActionEditor"
import { ButtonTrigger } from "@/types/config"

export type ButtonActionBindingPanelProps = {
  trigger?: ButtonTrigger
  onTriggerChange: (trigger: ButtonTrigger) => void
}

const ButtonActionBindingPanel = (props: ButtonActionBindingPanelProps) => {
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

  const current = props.trigger ?? defaultButtonTrigger

  return (
    <Tabs defaultValue={tabs[0]}>
      <TabsList>
        {tabs.map((trigger) => (
          <TabsTrigger key={trigger} value={trigger}>
            {trigger}
          </TabsTrigger>
        ))}
      </TabsList>
      {tabs.map((trigger) => {
        const action =
          trigger == "onPress"
            ? props.trigger?.onPress
            : trigger === "onRelease"
              ? props.trigger?.onRelease
              : trigger === "onHold"
                ? props.trigger?.onHold
                : props.trigger?.onLongRelease
        return (
          <TabsContent key={trigger} value={trigger}>
            <ActionEditor
              action={action}
              onActionChange={(action) => {
                props.onTriggerChange({
                  ...current,
                  [trigger]: action,
                })
              }}
            />
          </TabsContent>
        )
      })}
    </Tabs>
  )
}
export default ButtonActionBindingPanel
