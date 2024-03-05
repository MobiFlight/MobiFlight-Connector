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
import { Link, useParams } from "react-router-dom"

const ConfigDetailPage = () => {
  const { configId, projectId } = useParams()
  const config = useConfigStore((state) =>
    state.items.find((c) => c.GUID === configId),
  )

  const [testValue, setTestValue] = useState("1")
  const [testResultValue, setTestResultValue] = useState("1")  

  useEffect(() => {
    console.log(config) 
  }, [config])
  

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
      <div className="grid grid-cols-4 gap-8">
        <Card>
          <CardHeader className="h-24">
            <h3 className="text-xl">Event</h3>
            <p className="text-xs">Define device or simulator events</p>
          </CardHeader>
          <CardContent className="min-h-64 text-center">
            No settings defined.
          </CardContent>
          <CardFooter className="flex flex-row items-center justify-between">
            <Button>Edit</Button>
            <IconHelp>Learn more</IconHelp>
          </CardFooter>
        </Card>
        <Card>
          <CardHeader className="h-24">
            <h3 className="text-xl">Modifiers</h3>
            <p className="text-xs">Modify your event value (optional)</p>
          </CardHeader>
          <CardContent className="min-h-64 text-center">
            No settings defined.
          </CardContent>
          <CardFooter className="flex flex-row items-center justify-between">
            <Button>Edit</Button>
            <IconHelp>Learn more</IconHelp>
          </CardFooter>
        </Card>
        <Card>
          <CardHeader className="h-24">
            <h3 className="text-xl">Action</h3>
            <p className="text-xs">Do something with your sim or device</p>
          </CardHeader>
          <CardContent className="min-h-64 text-center">
            No settings defined.
          </CardContent>
          <CardFooter className="flex flex-row items-center justify-between">
            <Button>Edit</Button>
            <IconHelp>Learn more</IconHelp>
          </CardFooter>
        </Card>
        <Card>
          <CardHeader className="h-24">
            <h3 className="text-xl">Context</h3>
            <p className="text-xs">Define when this config will be applied</p>
          </CardHeader>
          <CardContent className="min-h-64 text-center align-middle">
            No settings defined.
          </CardContent>
          <CardFooter className="flex flex-row items-center justify-between">
            <Button>Edit</Button>
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
          <CardContent className="py-3 flex-flow flex h-auto items-center gap-4">
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
            <Input value={testValue} className="w-auto" onChange={(e) => setTestValue(e.target.value)}/>
            <Button>Test</Button>
            <Button>Stop</Button>
            <Label className="ml-12">Test result</Label>
            <div className="rounded-lg bg-background w-auto px-6 py-2 min-w-[200px]">{testResultValue}</div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}

export default ConfigDetailPage
