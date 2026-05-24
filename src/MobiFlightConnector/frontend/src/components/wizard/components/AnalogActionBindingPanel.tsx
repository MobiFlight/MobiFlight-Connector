import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import ActionEditor from "@/components/wizard/components/ActionEditor"
import { AnalogTrigger } from "@/types/config"

export type AnalogActionBindingPanelProps = {
  trigger?: AnalogTrigger
  onTriggerChange: (trigger: AnalogTrigger) => void
}

const AnalogActionBindingPanel = ({
  trigger,
  onTriggerChange,
}: AnalogActionBindingPanelProps) => {

  const defaultAnalogTrigger: AnalogTrigger = {
    onChange: undefined,
  }

  const current = trigger ?? defaultAnalogTrigger

  return (
    <Tabs defaultValue={"onChange"}>
      <TabsList>
        <TabsTrigger value={"onChange"}>{"onChange"}</TabsTrigger>
      </TabsList>
      <TabsContent value={"onChange"}>
        <ActionEditor
          action={trigger?.onChange}
          onActionChange={(action) => {
            onTriggerChange({
              ...current,
              onChange: action,
            })
          }}
        />
      </TabsContent>
    </Tabs>
  )
}
export default AnalogActionBindingPanel
