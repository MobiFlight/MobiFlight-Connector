import { Card, CardContent, CardFooter, CardHeader } from "@/components/ui/card"
import {
  AnalogInputForm,
  ButtonForm,
  CustomDeviceForm,
  EncoderForm,
  InputMultiplexerForm,
  InputShiftRegisterForm,
  LcdForm,
  LedModuleForm,
  OutputForm,
  OutputShiftRegisterForm,
  ServoForm,
  StepperForm,
} from "./form"
import DeviceIcon from "@/components/mobiflight/icons/DeviceIcon"
import { useDeviceDetailPageContext } from "@/lib/hooks"
import { IDeviceElement } from "@/types/deviceElements"

const DeviceElementDetailView = () => {
  const { element, device, updateDevice } = useDeviceDetailPageContext()
  const setElement = (element: IDeviceElement) => {
    const formerElement = device!.Elements!.find((e) => e.Id === element.Id)
    const updatePins = device.Pins!

    // determine which config data elements have changed
    // and update the pin information accordingly
    Object.keys(formerElement!.ConfigData).forEach((key) => {
      if (formerElement!.ConfigData[key] !== element.ConfigData[key]) {
        const pin = updatePins.find(
          (pin) => pin.Pin === parseInt(element.ConfigData[key])
        )
        if (pin) {
          pin.Used = true
        }
        const formerPin = updatePins.find(
          (pin) => pin.Pin === parseInt(formerElement!.ConfigData[key])
        )
        if (formerPin) {
          formerPin.Used = false
        }
      }
    })

    const updatedDevice = {
      ...device!,
      Elements: device!.Elements!.map((e) =>
        e.Id === element.Id ? element : e
      ),
      Pins: updatePins,
    }

    updateDevice(updatedDevice)
  }

  return (
    <Card className="grow  select-none bg-transparent shadow-none hover:border-none border-none hover:bg-transparent dark:bg-zinc-700/10 dark:hover:bg-zinc-700/40">
      <CardHeader className="flex flex-col mt-0">
        <div className="flex flex-row gap-2 mt-0 items-center">
          <DeviceIcon variant={element?.Type}></DeviceIcon>
          <div className="text-xl font-semibold">Edit {element?.Type}</div>
        </div>
        <div className="text-sm">Some cool sub title here</div>
      </CardHeader>
      <CardContent>
        {element?.Type == "AnalogInput" && (
          <AnalogInputForm
            device={device!}
            element={element!}
            setElement={setElement}
          />
        )}
        {element?.Type == "Button" && (
          <ButtonForm
            device={device!}
            element={element!}
            setElement={setElement}
          />
        )}
        {element?.Type == "CustomDevice" && <CustomDeviceForm />}
        {element?.Type == "Encoder" && (
          <EncoderForm
            device={device!}
            element={element!}
            setElement={setElement}
          />
        )}
        {element?.Type == "InputMultiplexer" && (
          <InputMultiplexerForm
            device={device!}
            element={element!}
            setElement={setElement}
            allowEditPinSx={false}
          />
        )}
        {element?.Type == "InputShiftRegister" && (
          <InputShiftRegisterForm
            device={device!}
            element={element!}
            setElement={setElement}
          />
        )}
        {element?.Type == "LcdDisplay" && (
          <LcdForm
            device={device!}
            element={element!}
            setElement={setElement}
          />
        )}
        {element?.Type == "LedModule" && (
          <LedModuleForm
            device={device!}
            element={element!}
            setElement={setElement}
          />
        )}
        {element?.Type == "Output" && (
          <OutputForm
            device={device!}
            element={element!}
            setElement={setElement}
          />
        )}
        {element?.Type == "Servo" && (
          <ServoForm
            device={device!}
            element={element!}
            setElement={setElement}
          />
        )}
        {element?.Type == "ShiftRegister" && (
          <OutputShiftRegisterForm
            device={device!}
            element={element!}
            setElement={setElement}
          />
        )}
        {element?.Type == "Stepper" && (
          <StepperForm
            device={device!}
            element={element!}
            setElement={setElement}
          />
        )}
      </CardContent>
      <CardFooter className="flex justify-between"></CardFooter>
    </Card>
  )
}

export default DeviceElementDetailView
