import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Combobox, ComboboxContent, ComboboxInput, ComboboxItem } from "@/components/ui/combobox"
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs"
import ConfigTrigger from "@/components/wizard/components/ConfigTrigger"
import { useControllerStore } from "@/stores/controllerStore"
import { useControllerDefinitionsStore } from "@/stores/definitionStore"
import { IConfigItem } from "@/types"

export type ConfigWizardProps = {
  configItem: IConfigItem
}
const ConfigWizard = ({ configItem }: ConfigWizardProps) => {
  const { controllers } = useControllerStore()
  const { BoardDefinitions, JoystickDefinitions, MidiControllerDefinitions } = useControllerDefinitionsStore()
  
  console.log("controllers", controllers)
  console.log("BoardDefinitions", BoardDefinitions)
  console.log("JoystickDefinitions", JoystickDefinitions)
  console.log("MidiControllerDefinitions", MidiControllerDefinitions)

  return (
    <div className="flex flex-col gap-4">
      <Tabs defaultValue="input" className="w-full">
        <TabsList>
          <TabsTrigger value="input">Input</TabsTrigger>
          <TabsTrigger value="precondition">Precondition</TabsTrigger>
          <TabsTrigger value="config-references">Config References</TabsTrigger>
        </TabsList>
        <TabsContent value="input" className="flex flex-col gap-4">
          <ConfigTrigger configItem={configItem} setConfigItem={() => {}} />
          <Card>
            <CardHeader>
              <CardTitle>Modify</CardTitle>
              <CardDescription>
                Modify the value provided by the selected trigger.
              </CardDescription>
            </CardHeader>
            <CardContent className="flex flex-row gap-4">
              <div>Modifier input:</div>
              <div>{configItem.RawValue}</div>
              <div>Modifier result:</div>
              <div>{configItem.Value}</div>
              <Button>Edit modifiers</Button>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Action</CardTitle>
              <CardDescription>
                The action defines the operations or tasks that will be executed
                when the trigger conditions are met.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <Tabs defaultValue="onPress" className="w-full">
                <TabsList className="grid w-full grid-cols-2">
                  <TabsTrigger
                    value="onPress"
                    className="data-[state=active]:border-primary [&:not(:first-child)]:border-l"
                  >
                    On Press
                  </TabsTrigger>
                  <TabsTrigger
                    value="onRelease"
                    className="data-[state=active]:border-primary"
                  >
                    On Release
                  </TabsTrigger>
                </TabsList>
                <TabsContent value="onPress" className="mt-4">
                  <p>
                    Configure the action that will be executed when the trigger
                    is activated.
                  </p>
                </TabsContent>
                <TabsContent value="onRelease" className="mt-4">
                  <p>
                    Configure the action that will be executed when the trigger
                    is deactivated.
                  </p>
                </TabsContent>
              </Tabs>
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
    </div>
  )
}
export default ConfigWizard
