import { ComboBox } from "@/components/mobiflight/ComboBox"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardFooter, CardHeader } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Switch } from "@/components/ui/switch"
import { useConfigStore } from "@/stores/configFileStore"
import { Label } from "@radix-ui/react-label"
import { IconHelp, IconPlus } from "@tabler/icons-react"
import { useEffect, useState } from "react"
import {
  Link,
  useLocation,
  useNavigate,
  useParams,
} from "react-router-dom"
import { FsuipcEventSettings, SimConnectVarEventSettings, VariableEventSettings, XplaneEventSettings } from "@/types/config"

const ConfigDetailPage = () => {
  const { configId, projectId } = useParams()
  const config = useConfigStore((state) =>
    state.items.find((c) => c.GUID === configId),
  )

  const [testValue, setTestValue] = useState("1")
  const [testResultValue, setTestResultValue] = useState("1")
  const navigate = useNavigate()
  const location = useLocation()
  const [activeEdit, setActiveEdit] = useState<string>("")

  useEffect(() => {
    console.log(config)
    console.log(location.pathname)
  }, [config, location])

  return (
    <div className="flex flex-col gap-8">
      {/* Breadcrumb */}
      <div className="flex flex-row items-center gap-4" role="navigation">
        <Link
          to={`/configs/${projectId}`}
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

      {/* <ConfigDetailEvent
          config={config!}
          editMode={activeEdit === "event"}
          onCancelEditMode={() => setActiveEdit(``)}
          onEnterEditMode={() => setActiveEdit(`event`)}
          onSaveEditMode={() => {}}
        ></ConfigDetailEvent> */}
      {/* Event, Modify, Action, Context */}

      <div className="just flex flex-row gap-4">
        <div className="grid grid-cols-3 w-2/3 gap-4">
          <Card>
            <CardHeader className="pt-3">
              <h2 className="text-2xl">Event</h2>
              <p className="text-xs">Define device or simulator events</p>
            </CardHeader>
            <CardContent className="">
              <div>{config?.Event.Type}</div>
              <div>
                {config?.Event.Type === "SIMCONNECT" ? (config?.Event.Settings as SimConnectVarEventSettings).Value : ""}
                {config?.Event.Type === "FSUIPC" ? (config?.Event.Settings as FsuipcEventSettings).Offset : ""}
                {config?.Event.Type === "VARIABLE" ? (config?.Event.Settings as VariableEventSettings).Name : ""}
                {config?.Event.Type === "XPLANE" ? (config?.Event.Settings as XplaneEventSettings).Path : ""}
              </div>
            </CardContent>
            <CardFooter className="py-0">
              <Button onClick={() => setActiveEdit(`modify`)}>Edit</Button>
            </CardFooter>
          </Card>
          <Card>
            <CardHeader className="">
              <h3 className="text-xl">Modifiers</h3>
              <p className="text-xs">Modify your event value (optional)</p>
            </CardHeader>
            <CardContent className="text-center">
              {
                config?.Modifiers.map((m, i) => (
                  <div key={i}>{m.Type}</div>
                ))
              }
            </CardContent>
            <CardFooter className="flex flex-row items-center justify-between">
              <Button onClick={() => setActiveEdit(`modify`)}>Edit</Button>
              <IconHelp>Learn more</IconHelp>
            </CardFooter>
          </Card>
          <Card>
            <CardHeader className="">
              <h3 className="text-xl">Action</h3>
              <p className="text-xs">Do something with your sim or device</p>
            </CardHeader>
            <CardContent className="text-center">
              {
                config?.Action.Type
              }
            </CardContent>
            <CardFooter className="flex flex-row items-center justify-between">
              <Button onClick={() => setActiveEdit(`action`)}>Edit</Button>
              <IconHelp>Learn more</IconHelp>
            </CardFooter>
          </Card>
        </div>
        <Card className="w-1/3 bg-background">
          <CardHeader className="">
            <h3 className="text-xl">Context</h3>
            <p className="text-xs">Define when this config will be applied</p>
          </CardHeader>
          <CardContent className="text-center align-middle">
            {
              config?.Context.Preconditions.map((p, i) => (
                <div key={i}>{p.Type}</div>
              ))
            }
          </CardContent>
          <CardFooter className="flex flex-row items-center justify-between">
            <Button onClick={() => setActiveEdit(`context`)}>Edit</Button>
            <IconHelp>Learn more</IconHelp>
          </CardFooter>
        </Card>
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
