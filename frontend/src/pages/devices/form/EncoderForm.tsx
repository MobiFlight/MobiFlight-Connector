import { ComboBox } from "@/components/mobiflight/ComboBox"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { IDeviceElement, IDeviceItem } from "@/types"
import {
  IconArrowLeftRight,
  IconArrowsLeftRight,
  IconHelp,
  IconRefresh,
} from "@tabler/icons-react"

type EncoderFormProps = {
  device: IDeviceItem
  element: IDeviceElement
  setElement: (element: IDeviceElement) => void
}

const EncoderForm = (props: EncoderFormProps) => {
  const { element, device, setElement } = props
  const PinLeft = element.ConfigData["PinLeft"]
  const PinRight = element.ConfigData["PinRight"]
  const Model = element.ConfigData["Model"]
  const freePins = device.Pins.filter(
    (pin) =>
      !pin.Used ||
      pin.Pin === parseInt(element.ConfigData["PinLeft"]) ||
      pin.Pin === parseInt(element.ConfigData["PinRight"])
  )

  const ModelOptions = [
    { value: "0", label: "1 detent per cycle (11)" },
    { value: "1", label: "1 detent per cycle (00)" },
    { value: "2", label: "2 detent per cycle (00, 11)" },
    { value: "3", label: "2 detent per cycle (01, 10)" },
    { value: "4", label: "4 detent per cycle" },
  ]

  console.log("EncoderForm", element, device, PinLeft, PinRight, freePins)

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
          <div className="w-1/4">
            <Label>Encoder type</Label>
            <div>
              <ComboBox
                className="w-full"
                options={ModelOptions}
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
          <IconHelp className="w-8 h-8 stroke-primary hover:stroke-blue-300 transition-all mt-7 cursor-pointer" />
        </div>
        <div className="flex flex-row gap-4">
          <div>
            <Label>Left Pin</Label>
            <div className="w-1/3">
              <ComboBox
                className="w-[120px]"
                options={freePins.map((pin) => ({
                  value: pin.Pin.toString(),
                  label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                }))}
                value={PinLeft}
                onSelect={(value) =>
                  setElement({
                    ...element,
                    ConfigData: { ...element.ConfigData, PinLeft: value },
                  })
                }
              />
            </div>
          </div>
          <IconArrowsLeftRight
            className="stroke-primary hover:stroke-blue-300 transition-all mt-8 cursor-pointer"
            onClick={() => {
              setElement({
                ...element,
                ConfigData: {
                  ...element.ConfigData,
                  PinLeft: PinRight,
                  PinRight: PinLeft,
                },
              })
            }}
          />
          <div>
            <Label>Right Pin</Label>
            <div className="w-1/3">
              <ComboBox
                className="w-[120px]"
                options={freePins.map((pin) => ({
                  value: pin.Pin.toString(),
                  label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                }))}
                value={PinRight}
                onSelect={(value) =>
                  setElement({
                    ...element,
                    ConfigData: { ...element.ConfigData, PinRight: value },
                  })
                }
              />
            </div>
          </div>
        </div>
      </div>
    </>
  )
}

export default EncoderForm
