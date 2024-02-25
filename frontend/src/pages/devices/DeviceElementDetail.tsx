import { Card, CardContent, CardFooter, CardHeader } from "@/components/ui/card"
import { useDevicesStore } from "@/stores/deviceStateStore"
import { useParams } from "react-router-dom"
import { AnalogInputForm, ButtonForm, CustomDeviceForm, EncoderForm, InputMultiplexerForm, InputShiftRegisterForm, LcdForm, OutputForm, OutputShiftRegisterForm, ServoForm, StepperForm } from "./form"
import { IDeviceElement } from "@/types/config"

const DeviceElementDetailView = () => {
  const params = useParams()	
  const id = params.id
  const elementId = params.elementId
	const { devices } = useDevicesStore()
	const device = devices.find((device) => device.Id === id)
	const element = device?.Elements.find((element) => element.Id === elementId) as IDeviceElement

  return (
    <Card className="grow">
      <CardHeader className="flex flex-row gap-2 items-center">
        <div className="flex flex-col mt-0">
          <div className="text-xl font-semibold">{element?.Name}</div>
          <div className="text-sm">{element?.Type}</div>
        </div>
      </CardHeader>
      <CardContent>
        { element.Type == "AnalogInput" && (<AnalogInputForm />) }
        { element.Type == "Button" && (<ButtonForm />) }
        { element.Type == "CustomDevice" && (<CustomDeviceForm />) }
        { element.Type == "Encoder" && (<EncoderForm />) }
        { element.Type == "InputMultiplexer" && (<InputMultiplexerForm />) }
        { element.Type == "InputShiftRegister" && (<InputShiftRegisterForm />) }
        { element.Type == "LcdDisplay" && (<LcdForm />) }        
        { element.Type == "LedModule" && (<EncoderForm />) }
        { element.Type == "Output" && (<OutputForm />) }
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
