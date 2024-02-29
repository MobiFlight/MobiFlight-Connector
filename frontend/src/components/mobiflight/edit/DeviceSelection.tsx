import DeviceIcon from "@/components/mobiflight/icons/DeviceIcon"
import { publishOnMessageExchange, useAppMessage } from "@/lib/hooks"
import { cn } from "@/lib/utils"
import { IDeviceItem } from "@/types"
import {
  DeviceElementRequiredPins,
  DeviceElementType,
  DeviceElementTypes,
  IDeviceElement,
} from "@/types/deviceElements.d"
import { DeviceElementCreateResponse } from "@/types/messages"
import createElement from "@/lib/elements"
import { use } from "i18next"
import { useTranslation } from "react-i18next"

interface DeviceSelectionProps {
  device: IDeviceItem
  onDeviceSelected: (message: DeviceElementCreateResponse) => void
}

export const DeviceSelection = (props: DeviceSelectionProps) => {
  const availableDeviceTypes = DeviceElementTypes

  const device = props.device
  const { t } = useTranslation()
  const addNewElement = (elementType: DeviceElementType) => {
    const newElement = createElement(elementType, device)
    props.onDeviceSelected({
      Device : device,
      Element : newElement
    })
  }

  const deviceHasFreePins = (
    device: IDeviceItem,
    elementType: IDeviceElement
  ) => {
    const freePins = device.Pins!.filter((pin) => !pin.Used)
    // i2c devices require 2 free i2c pins
    if (elementType.Type === "LcdDisplay") {
      return freePins.filter((pin) => pin.isI2C && !pin.Used).length >= 2
    }
    // another exception is the multiplexer
    // if we have already a multiplexer we can add another one by only using one pin
    // otherwise we need 4 pins
    if (elementType.Type === "InputMultiplexer") {
      const multiplexer = device.Elements!.find(
        (element) => element.Type === "InputMultiplexer"
      )
      if (multiplexer) {
        return freePins.length >= 1
      }
      return freePins.length >= 5
    }
    // other devices have to check for the required pins
    const requiredPins = DeviceElementRequiredPins[elementType.Type]
    return freePins.length >= requiredPins.Pins
  }

  return (
    <div className="grid grid-flow-row grid-cols-4 gap-4 items-center justify-items-center w-full">
      {availableDeviceTypes.map((deviceType) => {
        const canBeAdded = deviceHasFreePins(device, {
          Type: deviceType,
        } as IDeviceElement)
        return (
          <div
            onClick={() => {
              if (canBeAdded) addNewElement(deviceType)
            }}
            key={deviceType}
            className={cn(
              "border dark:border-none h-32 w-32 group flex flex-col gap-1 items-center justify-center hover:bg-slate-200 dark:hover:bg-slate-800 py-2 px-4 rounded-md cursor-pointer text-center",
              canBeAdded ? "" : "disabled",
            )}
          >
            <DeviceIcon disabled={!canBeAdded} className="w-12 h-12" variant={deviceType}></DeviceIcon>
            <div className="">{t(`device.type.${deviceType}`)}</div>
          </div>
        )
      })}
    </div>
  )
}

export default DeviceSelection
