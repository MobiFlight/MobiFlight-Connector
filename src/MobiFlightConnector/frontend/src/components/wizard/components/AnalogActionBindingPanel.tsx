import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import ActionEditor from "@/components/wizard/components/ActionEditor"
import { AnalogTrigger } from "@/types/config"
import { useTranslation } from "react-i18next"

export type AnalogActionBindingPanelProps = {
  trigger?: AnalogTrigger
  onTriggerChange: (trigger: AnalogTrigger) => void
}

const AnalogActionBindingPanel = ({
  trigger,
  onTriggerChange,
}: AnalogActionBindingPanelProps) => {
  const { t } = useTranslation()

  const defaultAnalogTrigger: AnalogTrigger = {
    onChange: undefined,
  }

  const current = trigger ?? defaultAnalogTrigger

  return (
    <Tabs data-testid="analog-action-panel" defaultValue={"onChange"}>
      <TabsList>
        <TabsTrigger value={"onChange"}>{t("Dialog.InputConfigWizard.Analog.Tabs.onChange")}</TabsTrigger>
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
