import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs"
import AnalogActionBindingPanel from "@/components/wizard/components/AnalogActionBindingPanel"
import ButtonActionBindingPanel from "@/components/wizard/components/ButtonActionBindingPanel"
import ConfigTrigger from "@/components/wizard/components/ConfigTrigger"
import EncoderActionBindingPanel from "@/components/wizard/components/EncoderActionBindingPanel"
import { useControllerStore } from "@/stores/controllerStore"
import { useControllerDefinitionsStore } from "@/stores/definitionStore"
import { IConfigItem } from "@/types"
import { useState } from "react"

export type ConfigWizardProps = {
  configItem: IConfigItem
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

const ConfigWizard = ({ configItem }: ConfigWizardProps) => {
  const { controllers } = useControllerStore()
  const { BoardDefinitions, JoystickDefinitions, MidiControllerDefinitions } =
    useControllerDefinitionsStore()
  const [currentConfigItem, setCurrentConfigItem] = useState(configItem)

  console.log("controllers", controllers)
  console.log("BoardDefinitions", BoardDefinitions)
  console.log("JoystickDefinitions", JoystickDefinitions)
  console.log("MidiControllerDefinitions", MidiControllerDefinitions)

  const currentDeviceType = determineInputDeviceType(
    currentConfigItem.Device?.Type,
  )
  console.log("currentDeviceType", currentDeviceType)

  return (
    <div className="flex flex-col gap-4">
      <Tabs defaultValue="input" className="w-full">
        <TabsList>
          <TabsTrigger value="input">Input</TabsTrigger>
          <TabsTrigger value="precondition">Precondition</TabsTrigger>
          <TabsTrigger value="config-references">Config References</TabsTrigger>
        </TabsList>
        <TabsContent value="input" className="flex flex-col gap-4">
          <ConfigTrigger
            configItem={currentConfigItem}
            setConfigItem={(item: IConfigItem) => {
              // Update the configItem state here
              setCurrentConfigItem(item)
            }}
          />
          <Card>
            <CardHeader>
              <CardTitle>Action</CardTitle>
              <CardDescription>
                The action defines the operations or tasks that will be executed
                when the trigger conditions are met.
              </CardDescription>
            </CardHeader>
            <CardContent>
              {currentDeviceType === "Button" && (
                <ButtonActionBindingPanel
                  trigger={currentConfigItem.button}
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
            </CardContent>
          </Card>
        </TabsContent>
        <TabsContent value="precondition" className="mt-4">
          <Card>
            <CardHeader>
              <CardTitle>Context</CardTitle>
              <CardDescription>
                The context defines preconditions or external references which
                can be used in this configuration.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <p>
                Here you will be able to create and edit your config files in a
                step by step process.
              </p>
            </CardContent>
          </Card>
        </TabsContent>
        <TabsContent value="config-references" className="mt-4">
          <p>
            Configure the action that will be executed when the trigger is
            deactivated.
          </p>
        </TabsContent>
      </Tabs>
      <div className="flex flex-row justify-end gap-2">
        <Button variant="outline">Cancel</Button>
        <Button>Save</Button>
      </div>
    </div>
  )
}
export default ConfigWizard
