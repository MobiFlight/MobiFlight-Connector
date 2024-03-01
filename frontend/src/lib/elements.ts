import { DeviceElementType, IDeviceElement, IDeviceItem } from "@/types"
import { IDictionary } from "@/types/config"

const createElement = (elementType: DeviceElementType, device: IDeviceItem) => {
  const element = {
    Id: createUniqueId(elementType, device),
    Name: createUniqueName(elementType, device),
    Type: elementType,
    ConfigData: {},
  }
  switch (elementType) {
    case "AnalogInput":
      return createAnalogInput(element, device)
    case "Button":
      return createButton(element, device)
    case "CustomDevice":
      return createCustomDevice(element, device)
    case "Encoder":
      return createEncoder(element, device)
    case "InputMultiplexer":
      return createInputMultiplexer(element, device)
    case "ShiftRegister":
      return createInputShiftRegister(element, device)
    case "Stepper"  :
      return createStepper(element, device)
    case "Servo":
      return createServo(element, device)
    case "InputShiftRegister":
      return createShiftRegister(element, device)
    case "LcdDisplay":
      return createLcdDisplay(element, device)
    case "LedModule":
      return createLedModule(element, device)
    case "Output":
      return createOutput(element, device)
    default:
      throw new Error("Unknown element type")
  }
}

const createUniqueName = (
  elementType: DeviceElementType,
  device: IDeviceItem
) => {
  const existingElements  = device.Elements!.filter((element) => element.Type === elementType).length
  const suffix = existingElements > 0 ? " " + existingElements.toString() : ""
  return (`${elementType}${suffix}`)
}

const createUniqueId = (
  elementType: DeviceElementType,
  device: IDeviceItem
) => {
  return (
    device.Id +
    elementType +
    device.Elements!.filter((element) => element.Type === elementType).length
  )
}

function createAnalogInput(
  element: IDeviceElement,
  device: IDeviceItem
): IDeviceElement {
  const freePins = device.Pins!.filter((pin) => !pin.Used)
  return {
    ...element,
    ConfigData: {
      Pin: freePins[0].Pin.toString(),
    },
  }
}

function createButton(element: IDeviceElement, device: IDeviceItem) {
  const freePins = device.Pins!.filter((pin) => !pin.Used)
  return {
    ...element,
    ConfigData: {
      Pin: freePins[0].Pin.toString(),
    },
  }
}

function createCustomDevice(element: IDeviceElement, device: IDeviceItem) {
  element.Type
  device.Elements
  throw new Error("Function not implemented.")
}

function createEncoder(element: IDeviceElement, device: IDeviceItem) {
  const freePins = device.Pins!.filter((pin) => !pin.Used)
  return {
    ...element,
    ConfigData: {
      Model: "0",
      PinLeft: freePins[0].Pin.toString(),
      PinRight: freePins[1].Pin.toString(),
    },
  }
}

function createInputMultiplexer(element: IDeviceElement, device: IDeviceItem) {
  const freePins = device.Pins!.filter((pin) => !pin.Used)
  const multiplexer = device.Elements!.find(
    (element) => element.Type === "InputMultiplexer"
  )

  const ConfigData = multiplexer ? multiplexer.ConfigData :{
    PinS0: freePins[0].Pin.toString(),
    PinS1: freePins[1].Pin.toString(),
    PinS2: freePins[2].Pin.toString(),
    PinS3: freePins[3].Pin.toString(),
  }

  const PinData = multiplexer ? freePins[0].Pin.toString() : freePins[4].Pin.toString()

  return {
    ...element,
    ConfigData: {
      ...ConfigData,
      PinData: PinData
    },
  }
}

function createShiftRegister(element: IDeviceElement, device: IDeviceItem) {
  const freePins = device.Pins!.filter((pin) => !pin.Used)
  return {
    ...element,
    ConfigData: {
      PinData: freePins[0].Pin.toString(),
      PinLatch: freePins[1].Pin.toString(),
      PinClock: freePins[2].Pin.toString(),
      NumberOfModules: "1",
    },
  }
}

function createInputShiftRegister(
  element: IDeviceElement,
  device: IDeviceItem
) {
  const freePins = device.Pins!.filter((pin) => !pin.Used)
  return {
    ...element,
    ConfigData: {
      PinData: freePins[0].Pin.toString(),
      PinLatch: freePins[1].Pin.toString(),
      PinClock: freePins[2].Pin.toString(),
      NumberOfModules: "1",
    },
  }
}

function createLcdDisplay(element: IDeviceElement, device: IDeviceItem) {
  const freePins = device.Pins!.filter((pin) => !pin.Used && pin.isI2C)
  return {
    ...element,
    ConfigData: {
      Address: "0x27",
      PinSLA: freePins[0].Pin.toString(),
      PinSCL: freePins[1].Pin.toString(),
    },
  }
}

function createLedModule(element: IDeviceElement, device: IDeviceItem) {
  const freePins = device.Pins!.filter((pin) => !pin.Used)
  return {
    ...element,
    ConfigData: {
      Model: "0",
      PinData: freePins[0].Pin.toString(),
      PinLatch: freePins[1].Pin.toString(),
      PinClock: freePins[2].Pin.toString(),
      NumberOfModules: "1",
      Brightness: 15,
    },
  }
}

function createOutput(element: IDeviceElement, device: IDeviceItem) {
  const freePins = device.Pins!.filter((pin) => !pin.Used)
  return {
    ...element,
    ConfigData: {
      Pin: freePins[0].Pin.toString(),
    },
  }
}

export default createElement

function createStepper(element: { Id: string; Name: string; Type: DeviceElementType; ConfigData: IDictionary<string> }, device: IDeviceItem) {
  const freePins = device.Pins!.filter((pin) => !pin.Used)
  return {
    ...element,
    ConfigData: {
      Pin1: freePins[0].Pin.toString(),
      Pin2: freePins[1].Pin.toString(),
      Pin3: freePins[2].Pin.toString(),
      Pin4: freePins[3].Pin.toString(),
      Profile: "0"
    },
  }
}
function createServo(element: { Id: string; Name: string; Type: DeviceElementType; ConfigData: IDictionary<string> }, device: IDeviceItem) {
  const freePins = device.Pins!.filter((pin) => !pin.Used)
  return {
    ...element,
    ConfigData: {
      Pin: freePins[0].Pin.toString(),
    },
  }
}

