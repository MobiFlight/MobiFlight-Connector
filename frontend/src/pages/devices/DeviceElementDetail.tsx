import { Card, CardContent, CardFooter, CardHeader } from "@/components/ui/card"
import { AnalogInputForm, ButtonForm, CustomDeviceForm, EncoderForm, InputMultiplexerForm, InputShiftRegisterForm, LcdForm, OutputForm, OutputShiftRegisterForm, ServoForm, StepperForm } from "./form"
import DeviceIcon from "@/components/mobiflight/icons/DeviceIcon"
import { useDeviceDetailPageContext } from "./DeviceDetail"
import { IDeviceElement } from "@/types/config"

const DeviceElementDetailView = () => {
  const {element, device, updateDevice } = useDeviceDetailPageContext();
  const setElement = (element: IDeviceElement) => {
    const formerElement = device!.Elements!.find((e) => e.Id === element.Id)
    var updatePins = device.Pins
    
    // determine which config data elements have changed
    // and update the pin information accordingly
    Object.keys(formerElement!.ConfigData).forEach((key) => {
      if (formerElement!.ConfigData[key] !== element.ConfigData[key]) {
        const pin = updatePins.find((pin) => pin.Pin === parseInt(element.ConfigData[key]))
        if (pin) {
          pin.Used = true
        }
        const formerPin = updatePins.find((pin) => pin.Pin === parseInt(formerElement!.ConfigData[key]))
        if (formerPin) {
          formerPin.Used = false
        }
      }
    })

    const updatedDevice = {
      ...device!,
      Elements: device!.Elements!.map((e) => e.Id === element.Id ? element : e),
      Pins: updatePins
    }

    updateDevice(updatedDevice)
  }
 
  return (
    <Card className="grow  select-none bg-transparent shadow-none hover:border-none border-none hover:bg-transparent dark:bg-zinc-700/10 dark:hover:bg-zinc-700/40">
      <CardHeader className="flex flex-row gap-2 items-center">
        <div className="flex flex-row gap-4 mt-0">
          <DeviceIcon variant={element?.Type}></DeviceIcon>
          <div className="text-xl font-semibold">Edit {element?.Type}</div>
        </div>
      </CardHeader>
      <CardContent>
        { element.Type == "AnalogInput" && (<AnalogInputForm />) }
        { element.Type == "Button" && (<ButtonForm device={device!} element={element!} setElement={setElement} />) }
        { element.Type == "CustomDevice" && (<CustomDeviceForm />) }
        { element.Type == "Encoder" && (<EncoderForm />) }
        { element.Type == "InputMultiplexer" && (<InputMultiplexerForm />) }
        { element.Type == "InputShiftRegister" && (<InputShiftRegisterForm />) }
        { element.Type == "LcdDisplay" && (<LcdForm />) }        
        { element.Type == "LedModule" && (<EncoderForm />) }
        { element.Type == "Output" && (<OutputForm device={device!} element={element!} setElement={setElement}  />) }
        { element.Type == "Servo" && (<ServoForm />) }
        { element.Type == "ShiftRegister" && (<OutputShiftRegisterForm />) }
        { element.Type == "Stepper" && (<StepperForm />) }
      </CardContent>
      <CardFooter className="flex justify-between">
      </CardFooter>
    </Card>
  )
}

export default DeviceElementDetailView
