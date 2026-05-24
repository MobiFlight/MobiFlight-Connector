import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { ButtonTrigger, EncoderTrigger, AnalogTrigger } from "@/types/config"

type ActionBindingPanelButtonProps = {
  type: "Button"
  trigger?: ButtonTrigger
  onTriggerChange: (trigger?: ButtonTrigger) => void
}

type ActionBindingPanelEncoderProps = {
  type: "Encoder"
  trigger?: EncoderTrigger
  onTriggerChange: (trigger?: EncoderTrigger) => void
}

type ActionBindingPanelAnalogProps = {
  type: "AnalogInput"
  trigger?: AnalogTrigger
  onTriggerChange: (trigger?: AnalogTrigger) => void
}

export type ActionBindingPanelProps =
  | ActionBindingPanelButtonProps
  | ActionBindingPanelEncoderProps
  | ActionBindingPanelAnalogProps

const triggerConfig = {
  Button: ["onPress", "onRelease", "onHold", "onLongRelease"],
  Encoder: ["onLeft", "onRight", "onLeftFast", "onRightFast"],
  AnalogInput: ["onChange"],
} as const

const ActionBindingPanel = (props: ActionBindingPanelProps) => {
  const tabs = triggerConfig[props.type]

  return (
    <Tabs defaultValue={tabs[0]}>
      <TabsList>
        {tabs.map((trigger) => (
          <TabsTrigger key={trigger} value={trigger}>{trigger}</TabsTrigger>
        ))}
      </TabsList>
      {tabs.map((trigger) => (
        <TabsContent key={trigger} value={trigger}>
          
        </TabsContent>
      ))}
    </Tabs>
  )
}
export default ActionBindingPanel
