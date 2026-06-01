import ComboBox from "@/components/ComboBox"
import { Input } from "@/components/ui/input"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { publishOnMessageExchange, useAppMessage } from "@/lib/hooks/appMessage"
import { useVJoyControllerStore } from "@/stores/controllerStore"
import { VJoyInputAction } from "@/types/config"
import { VJoyDefinitionsUpdate } from "@/types/messages"
import { Switch } from "@/components/ui/switch"
import { Label } from "@/components/ui/label"

export type VJoyInputActionPanelProps = {
  config: VJoyInputAction | null
  setConfig: (item: VJoyInputAction) => void
}

const VJoyInputActionPanel = ({
  config,
  setConfig,
}: VJoyInputActionPanelProps) => {
  const { publish } = publishOnMessageExchange()
  const { vJoyDefinitions, setVJoyDefinitions } = useVJoyControllerStore()

  useAppMessage("VJoyDefinitionsUpdate", (message) => {
    const { Definitions } = message.payload as VJoyDefinitionsUpdate
    setVJoyDefinitions(Definitions)
  })

  if (vJoyDefinitions.length === 0) {
    publish({
      key: "CommandRefreshPresets",
      payload: {
        type: "vjoy",
      },
    })
  }

  const vJoyOptions = vJoyDefinitions.map((def) => ({
    label: `vJoy Device ${def.Id}`,
    value: def.Id,
  }))

  const selectedDevice = vJoyDefinitions.find(
    (def) => def.Id === config?.vJoyID,
  )

  const selectedDeviceOption = vJoyOptions.find((def) => {
    return def.value === config?.vJoyID
  })

  const axisOptions = selectedDevice
    ? Object.keys(selectedDevice.Axis).filter(
        (key) => (selectedDevice.Axis as Record<string, boolean>)[key],
      )
    : []

  const buttonOptions = selectedDevice
    ? Array.from({ length: selectedDevice.Buttons }, (_, i) => i + 1)
    : []

  const activeTab = config?.axisString ? "axis" : "button"

  return (
    <div className="flex flex-col gap-4">
      <ComboBox
        placeholder="Select vJoy device..."
        items={vJoyOptions}
        getLabel={(item) => item.label}
        getValue={(item) => item.value.toString()}
        isSelected={(item) => item.value === selectedDeviceOption?.value}
        selected={selectedDeviceOption}
        setSelected={(item) =>
          setConfig({
            ...(config ?? {}),
            vJoyID: item ? Number(item.value) : undefined,
          } as VJoyInputAction)
        }
        widthClass="w-100"
        variant="nofilter"
      />

      <Tabs
        defaultValue={activeTab}
        onValueChange={(e) => {
          if (e === "button") {
            // we are switching type to button, unset AXIS
            setConfig({
              ...(config ?? {}),
              axisString: "",
            } as VJoyInputAction)
          } else {
            // we are switching type to axis, set buttonNr to -1
            setConfig({
              ...(config ?? {}),
              buttonNr: -1,
            } as VJoyInputAction)
          }
        }}
      >
        <TabsList>
          <TabsTrigger key="button" value="button">
            Button
          </TabsTrigger>
          <TabsTrigger key="axis" value="axis">
            Axis
          </TabsTrigger>
        </TabsList>
        <TabsContent key="button" value="button">
          <div className="flex flex-col gap-4 pt-2">
            <Label htmlFor="buttonNr">Button number</Label>
            <ComboBox
              placeholder="Select button..."
              items={buttonOptions}
              getLabel={(item) => `Button ${item}`}
              getValue={(item) => item.toString()}
              isSelected={(item) => item === config?.buttonNr}
              selected={buttonOptions.find((item) => item === config?.buttonNr)}
              setSelected={(item) =>
                setConfig({
                  ...(config ?? {}),
                  buttonNr: item ? Number(item) : undefined,
                } as VJoyInputAction)
              }
              variant="nofilter"
            />
            <Label htmlFor="buttonCommand">Button state</Label>
            <div className="flex flex-row items-center gap-2">
              <Switch
                id="buttonCommand"
                checked={config?.buttonComand ?? false}
                onCheckedChange={(checked) =>
                  setConfig({
                    ...(config ?? {}),
                    buttonComand: checked,
                  } as VJoyInputAction)
                }
              />
              <span className="text-sm">{config?.buttonComand ? "Pressed" : "Released"}</span>
            </div>
          </div>
        </TabsContent>
        <TabsContent key="axis" value="axis">
          <div className="flex flex-col gap-4 pt-2">
            <ComboBox
              placeholder="Select axis..."
              items={axisOptions}
              getLabel={(item) => item}
              getValue={(item) => item}
              isSelected={(item) => item === config?.axisString}
              selected={axisOptions.find((item) => item === config?.axisString)}
              setSelected={(item) =>
                setConfig({
                  ...(config ?? {}),
                  axisString: item ? item : undefined,
                } as VJoyInputAction)
              }
              variant="nofilter"
            />
            <Label htmlFor="axisValue">Axis value</Label>
            <Input
              id="axisValue"
              value={config?.sendValue ?? "1"}
              onChange={(e) =>
                setConfig({
                  ...(config ?? {}),
                  sendValue: e.target.value,
                } as VJoyInputAction)
              }
            />
          </div>
        </TabsContent>
      </Tabs>
    </div>
  )
}
export default VJoyInputActionPanel
