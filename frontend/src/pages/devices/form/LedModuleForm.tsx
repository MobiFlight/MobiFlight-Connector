import { ComboBox } from "@/components/mobiflight/ComboBox"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { IDeviceElement, IDeviceItem } from "@/types"
import { IconArrowsLeftRight, IconHelp } from "@tabler/icons-react"

type LedModuleFormProps = {
  device: IDeviceItem
  element: IDeviceElement
  setElement: (element: IDeviceElement) => void
}

const LedModuleForm = (props: LedModuleFormProps) => {
  const { element, device, setElement } = props
  const Model = element.ConfigData["Model"]
  const PinData = element.ConfigData["PinData"]
  const PinLatch = element.ConfigData["PinLatch"]
  const PinClock = element.ConfigData["PinClock"]
  const freePins = device.Pins.filter(
    (pin) =>
      !pin.Used ||
      pin.Pin === parseInt(element.ConfigData["PinData"]) ||
      pin.Pin === parseInt(element.ConfigData["PinClock"]) ||
      pin.Pin === parseInt(element.ConfigData["PinLatch"])
  )
  const NumberOfModules = element.ConfigData["NumberOfModules"]
  const NumberOfModulesOptions = Array.from({ length: 8 }, (_, i) => ({
    value: `${i + 1}`,
    label: `${i + 1}`,
  }))

  console.log(element.ConfigData)
  return (
    <>
      <div className="flex flex-col gap-8">
        <div className="w-1/2">
          <Label>Name</Label>
          <Input
            className="w-1/2"
            name="Name"
            value={element.Name}
            onChange={(e) => setElement({ ...element, Name: e.target.value })}
          />
        </div>
        <div className="flex flex-row gap-4 items-end">
          <div>
            <Label>Model</Label>
            <div className="w-1/2">
              <ComboBox
                className="w-[240px]"
                options={[
                  { value: "0", label: "MAX7219" },
                  { value: "1", label: "TM1637 - (4 digits)" },
                  { value: "2", label: "TM1637 - (6 digits)" },
                ]}
                value={Model}
                onSelect={(value) =>
                  setElement({
                    ...element,
                    ConfigData: { ...element.ConfigData, Model: value },
                  })
                }
              />
            </div>
          </div>
          <div className="cursor-pointer stroke-primary text-primary  hover:stroke-blue-400  hover:text-blue-400">
            <a onClick={()=>{}} className="flex flex-row mb-2 gap-2">
              <IconHelp></IconHelp>
              <p>Help me choose the right model</p>
            </a>
          </div>
        </div>
        <div className="flex flex-row gap-4">
          <div>
            <Label>Pin Data</Label>
            <div className="w-1/3">
              <ComboBox
                className="w-[120px]"
                options={freePins.map((pin) => ({
                  value: pin.Pin.toString(),
                  label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                }))}
                value={PinData}
                onSelect={(value) =>
                  setElement({
                    ...element,
                    ConfigData: { ...element.ConfigData, PinData: value },
                  })
                }
              />
            </div>
          </div>
          <IconArrowsLeftRight
            className="stroke-primary mt-8 cursor-pointer"
            onClick={() => {
              setElement({
                ...element,
                ConfigData: {
                  ...element.ConfigData,
                  PinData: PinLatch,
                  PinLatch: PinData,
                },
              })
            }}
          />
          <div>
            <Label>Pin Latch / Pin CS</Label>
            <div className="w-1/3">
              <ComboBox
                className="w-[120px]"
                options={freePins.map((pin) => ({
                  value: pin.Pin.toString(),
                  label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                }))}
                value={PinLatch}
                onSelect={(value) =>
                  setElement({
                    ...element,
                    ConfigData: { ...element.ConfigData, PinLatch: value },
                  })
                }
              />
            </div>
          </div>
          <IconArrowsLeftRight
            className="stroke-primary mt-8 cursor-pointer"
            onClick={() => {
              setElement({
                ...element,
                ConfigData: {
                  ...element.ConfigData,
                  PinLatch: PinClock,
                  PinClock: PinLatch,
                },
              })
            }}
          />
          <div>
            <Label>Pin Clock</Label>
            <div className="w-1/3">
              <ComboBox
                className="w-[120px]"
                options={freePins.map((pin) => ({
                  value: pin.Pin.toString(),
                  label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                }))}
                value={PinClock}
                onSelect={(value) =>
                  setElement({
                    ...element,
                    ConfigData: { ...element.ConfigData, PinClock: value },
                  })
                }
              />
            </div>
          </div>
        </div>
        {Model === "0" && (
          <div>
            <Label>Number of modules</Label>
            <div className="w-1/3">
              <ComboBox
                className="w-[120px]"
                options={NumberOfModulesOptions}
                value={NumberOfModules}
                onSelect={(value) =>
                  setElement({
                    ...element,
                    ConfigData: {
                      ...element.ConfigData,
                      NumberOfModules: value,
                    },
                  })
                }
              />
            </div>
          </div>
        )}
      </div>
    </>
  )
}

export default LedModuleForm
