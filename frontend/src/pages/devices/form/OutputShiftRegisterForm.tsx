import { ComboBox } from "@/components/mobiflight/ComboBox"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { IDeviceElement, IDeviceItem } from "@/types"
import { IconArrowsLeftRight } from "@tabler/icons-react"

type OutputShiftRegisterProps = {
  device: IDeviceItem
  element: IDeviceElement
  setElement: (element: IDeviceElement) => void
}

const OutputShiftRegisterForm = (props: OutputShiftRegisterProps) => {
  const { element, device, setElement } = props
  const PinData = element.ConfigData["PinData"]
  const PinLatch = element.ConfigData["PinLatch"]
  const PinClock = element.ConfigData["PinClock"]
  const NumberOfModules = element.ConfigData["NumberOfModules"]
  const NumberOfModulesOptions = Array.from({length:8}, (_, i)=> ({ value: `${i+1}`, label: `${i+1}` }) )
  
  const freePins = device.Pins.filter(
    (pin) =>
      !pin.Used ||
      pin.Pin === parseInt(element.ConfigData["PinData"]) ||
      pin.Pin === parseInt(element.ConfigData["PinClock"]) ||
      pin.Pin === parseInt(element.ConfigData["PinLatch"])
  )

  return (
    <>
      <div className="flex flex-col gap-8">
        <div className="w-1/4">
          <Label>Name</Label>
          <Input
            name="Name"
            value={element.Name}
            onChange={(e) => setElement({ ...element, Name: e.target.value })}
          />
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
            className="stroke-primary mt-8 cursor-pointer hover:stroke-blue-300 transition-all"
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
            className="stroke-primary mt-8 cursor-pointer hover:stroke-blue-300 transition-all"
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
                  ConfigData: { ...element.ConfigData, NumberOfModules: value },
                })
              }
            />
          </div>
        </div>
      </div>
    </>
  )
}

export default OutputShiftRegisterForm
