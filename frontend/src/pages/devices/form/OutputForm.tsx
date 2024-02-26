import { ComboBox } from "@/components/mobiflight/ComboBox"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { IDeviceItem } from "@/types"
import { IDeviceElement } from "@/types/config"

type OutputFormProps = {
  device: IDeviceItem
  element: IDeviceElement
  setElement: (element: IDeviceElement) => void
}

const OutputForm = (props : OutputFormProps) => {
	const { element, device, setElement } = props
  const pin = element.ConfigData["Pin"]
  const freePins = device.Pins.filter((pin) => !pin.Used || pin.Pin === parseInt(element.ConfigData["Pin"]))

	return (
<>
		<div className="flex flex-col gap-4">
        <div>
          <Label>Name</Label>
          <Input
            className="w-1/2"
            name="Name"
            value={element.Name}
            onChange={(e) => setElement({ ...element, Name: e.target.value })}
          />
        </div>
        <div>
          <Label>Pin</Label>
          <div className="w-1/3">
            <ComboBox
							showFilter={false}
              className="w-[120px]"
              options={freePins.map((pin) => ({ value: pin.Pin.toString(), label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`}))}
              value={pin}
              onSelect={(value) => setElement({ ...element, ConfigData: { ...element.ConfigData, Pin: value }})}
            />
          </div>
        </div>
      </div>
</>
	)
}

export default OutputForm