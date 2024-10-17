import { ComboBox } from "@/components/mobiflight/ComboBox"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardFooter, CardHeader } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Switch } from "@/components/ui/switch"
import { useConfigStore } from "@/stores/configFileStore"
import { Label } from "@radix-ui/react-label"
import { IconHelp, IconPlus } from "@tabler/icons-react"
import { useCallback, useEffect, useState } from "react"
import { Link, useParams } from "react-router-dom"
import { Projects } from "@/../tests/fixtures/data/projects"
import { Project } from "@/types"
import ConfigDetailEvent from "./ConfigDetailEvent"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { isEqual } from "lodash"
import { usePublishOnMessageExchange } from "@/lib/hooks"
import { UpdateConfigMessage } from "@/types/messages"

const ConfigDetailPage = () => {
  const { publish } = usePublishOnMessageExchange();
  
  const { configId, projectId } = useParams()
  const items = useConfigStore((state) => state.items) 
  const updateItem = useConfigStore((state) => state.updateItem)  
  const config = items.find((item) => item.GUID === configId)

  const [tempConfig, setTempConfig] = useState({ ...config! })
  const [configHasChanged, setConfigHasChanged] = useState(false);

  const [testValue, setTestValue] = useState("1")
  const [testResultValue] = useState("1")
  const [activeEdit, setActiveEdit] = useState<string>("")
  const project = Projects.find((p: Project) => p.id === projectId)

  useEffect(() => {
    const hasChanged = !isEqual(config, tempConfig)
    setConfigHasChanged(hasChanged)
  }, [config, tempConfig])  

  const saveChanges = useCallback(() => {
    console.log("Saving changes", tempConfig)
    updateItem(tempConfig)
    publish({ key: "config.update", payload: tempConfig } as UpdateConfigMessage);
  },[tempConfig])

  return (
    <div className="flex flex-col gap-8">
      {/* Breadcrumb */}
      <div className="flex flex-row items-center gap-4" role="navigation">
        <Link to="/" className="scroll-m-20 text-3xl tracking-tight first:mt-0">
          Project
        </Link>
        <p className="scroll-m-20 text-3xl tracking-tight first:mt-0">&gt;</p>
        <p className="scroll-m-20 text-3xl tracking-tight first:mt-0">
          {project?.name ?? "Default"}
        </p>
        <Link
          to={`/project/${projectId}/configs`}
          className="scroll-m-20 text-3xl tracking-tight first:mt-0"
        >
          Configs
        </Link>
        <p className="scroll-m-20 text-3xl tracking-tight first:mt-0">&gt;</p>
        <p
          className={`scroll-m-20 text-3xl font-bold tracking-tight first:mt-0`}
        >
          {config?.Description}
        </p>
        {
          configHasChanged && <div>
            <Button variant="default" onClick={saveChanges}>Save changes</Button>
            <Button variant="ghost">Discard changes</Button>
          </div>
        }
      </div>
      {/* General options */}
      <div className="flex flex-col gap-4 md:flex-row md:items-center md:gap-32">
        <div className="flex flex-row items-center gap-2">
          <Switch checked={config?.Active}>Active</Switch>
          <Label>Active</Label>
        </div>
        <div className="flex flex-row items-center gap-4">
          <p>Tags</p>
          <ul className="flex flex-row items-center gap-2">
            <li className="flex flex-row items-center gap-2">
              <Badge>Autopilot</Badge>
            </li>
            <li className="flex flex-row items-center gap-2">
              <Badge>Autopilot</Badge>
            </li>
            <li className="flex flex-row items-center gap-2">
              <Badge>Autopilot</Badge>
            </li>
            <li className="flex flex-row items-center gap-2">
              <Badge>Autopilot</Badge>
            </li>
          </ul>
          <Button className="h-6 rounded-xl text-xs" variant={"ghost"}>
            <IconPlus className="size-4"></IconPlus>Add tag
          </Button>
        </div>
      </div>

      <div className="just flex flex-row gap-4">
        <Tabs defaultValue="tab-event" className="w-full">
          <TabsList className="grid w-full grid-cols-4">
            <TabsTrigger value="tab-event">Event{ configHasChanged && <Badge className="ml-2">!</Badge>}</TabsTrigger>
            <TabsTrigger value="tab-modifiers">Modifiers</TabsTrigger>
            <TabsTrigger value="tab-action">Action</TabsTrigger>
            <TabsTrigger value="tab-context">Context</TabsTrigger>
          </TabsList>
          <TabsContent value="tab-event">
            <ConfigDetailEvent
              config={config!}
              editMode={activeEdit === "event"}
              onCancelEditMode={() => {}}
              onEnterEditMode={() => {}}
              onSaveEditMode={() => {}}
              onChange={(newConfig) => {
                setTempConfig(newConfig)
              }}
            ></ConfigDetailEvent>
          </TabsContent>
          <TabsContent value="tab-modifiers">
            <Card>
              <CardHeader className="">
                <h3 className="text-xl">Modifiers</h3>
                <p className="text-xs">Modify your event value (optional)</p>
              </CardHeader>
              <CardContent className="text-center">
                {config?.Modifiers.map((m, i) => <div key={i}>{m.Type}</div>)}
              </CardContent>
              <CardFooter className="flex flex-row items-center justify-between">
                <Button onClick={() => setActiveEdit(`modify`)}>Edit</Button>
                <IconHelp>Learn more</IconHelp>
              </CardFooter>
            </Card>
          </TabsContent>
          <TabsContent value="tab-action">
            <Card>
              <CardHeader className="">
                <h3 className="text-xl">Action</h3>
                <p className="text-xs">Do something with your sim or device</p>
              </CardHeader>
              <CardContent className="text-center">
                {config?.Action.Type}
              </CardContent>
              <CardFooter className="flex flex-row items-center justify-between">
                <Button onClick={() => setActiveEdit(`action`)}>Edit</Button>
                <IconHelp>Learn more</IconHelp>
              </CardFooter>
            </Card>
          </TabsContent>
          <TabsContent value="tab-context">
            <Card className="w-1/3 bg-background">
              <CardHeader className="">
                <h3 className="text-xl">Context</h3>
                <p className="text-xs">
                  Define when this config will be applied
                </p>
              </CardHeader>
              <CardContent className="text-center align-middle">
                {config?.Context.Preconditions.map((p, i) => (
                  <div key={i}>{p.Type}</div>
                ))}
              </CardContent>
              <CardFooter className="flex flex-row items-center justify-between">
                <Button onClick={() => setActiveEdit(`context`)}>Edit</Button>
                <IconHelp>Learn more</IconHelp>
              </CardFooter>
            </Card>
          </TabsContent>
        </Tabs>
      </div>
      <div className="flex flex-col gap-4">
        <div className="flex flex-col">
          <h2 className="text-2xl">Test your settings</h2>
          <p className="text-xs">
            Test your current config settings interactively by defining a custom
            test value.
          </p>
        </div>
        <Card className="flex flex-row items-center">
          <CardContent className="flex-flow flex h-auto items-center gap-4 py-3">
            <Label>Value type</Label>
            <ComboBox
              options={[
                { label: "Number", value: "Number" },
                { label: "String", value: "Text" },
              ]}
              onSelect={() => {}}
              value={"Number"}
            ></ComboBox>
            <Label>Value</Label>
            <Input
              value={testValue}
              className="w-auto"
              onChange={(e) => setTestValue(e.target.value)}
            />
            <Button>Test</Button>
            <Button>Stop</Button>
            <Label className="ml-12">Test result</Label>
            <div className="w-auto min-w-[200px] rounded-lg bg-background px-6 py-2">
              {testResultValue}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}

export default ConfigDetailPage
