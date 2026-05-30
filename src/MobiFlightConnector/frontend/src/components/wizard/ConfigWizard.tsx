import { Button } from "@/components/ui/button"
import AnalogActionBindingPanel from "@/components/wizard/components/AnalogActionBindingPanel"
import ButtonActionBindingPanel from "@/components/wizard/components/ButtonActionBindingPanel"
import ConfigReferencePanel from "@/components/wizard/components/ConfigReferencePanel"
import ConfigTrigger from "@/components/wizard/components/ConfigTrigger"
import EncoderActionBindingPanel from "@/components/wizard/components/EncoderActionBindingPanel"
import PreconditionPanel from "@/components/wizard/components/PreconditionPanel"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { IConfigItem } from "@/types"
import { useState } from "react"

export type ConfigWizardProps = {
  configItem: IConfigItem
  onClose: () => void
}

const determineInputDeviceType = (
  deviceType: string | undefined,
): "Button" | "Encoder" | "AnalogInput" | null => {
  switch (deviceType) {
    case "InputShiftRegister":
    case "InputMultiplexer":
    case "Button":
      return "Button"
    case "Encoder":
      return "Encoder"
    case "AnalogInput":
      return "AnalogInput"
    default:
      return null // Default to null if type is unknown
  }
}

const ConfigWizard = ({ configItem, onClose }: ConfigWizardProps) => {
  const [currentConfigItem, setCurrentConfigItem] = useState(configItem)

  const currentDeviceType = determineInputDeviceType(
    currentConfigItem.Device?.Type,
  )

  const saveChanges = () => {
    const { publish } = publishOnMessageExchange()
    publish({
      key: "CommandUpdateConfigItem",
      payload: {
        item: currentConfigItem,
      },
    })
    onClose() // Close the wizard after saving
  }

  return (
    <div className="flex flex-col gap-4">
      <ConfigTrigger
        configItem={currentConfigItem}
        setConfigItem={(item: IConfigItem) => {
          // Update the configItem state here
          setCurrentConfigItem(item)
        }}
      />
      <div className="flex flex-row gap-2">
        <div className="w-1/2">
          <PreconditionPanel />
        </div>
        <div className="w-1/2">
          <ConfigReferencePanel />
        </div>
      </div>
      {currentDeviceType === "Button" && (
        <ButtonActionBindingPanel
          trigger={
            currentConfigItem.button ??
            currentConfigItem.inputMultiplexer ??
            currentConfigItem.inputShiftRegister
          }
          onTriggerChange={(trigger) => {
            setCurrentConfigItem({
              ...currentConfigItem,
              button: trigger,
            })
          }}
        />
      )}
      {currentDeviceType === "Encoder" && (
        <EncoderActionBindingPanel
          trigger={currentConfigItem.encoder}
          onTriggerChange={(trigger) => {
            setCurrentConfigItem({
              ...currentConfigItem,
              encoder: trigger,
            })
          }}
        />
      )}
      {currentDeviceType === "AnalogInput" && (
        <AnalogActionBindingPanel
          trigger={currentConfigItem.analog}
          onTriggerChange={(trigger) => {
            setCurrentConfigItem({
              ...currentConfigItem,
              analog: trigger,
            })
          }}
        />
      )}

      <div className="flex flex-row justify-end gap-2">
        <Button variant="outline">Cancel</Button>
        <Button onClick={saveChanges}>Save</Button>
      </div>
    </div>
  )
}
export default ConfigWizard
