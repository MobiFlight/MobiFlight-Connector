import DeviceIcon from "@/components/mobiflight/icons/DeviceIcon"
import { DeviceElementTypes } from "@/types/deviceElements.d"
import { useTranslation } from "react-i18next"

export const DeviceSelection = () => {
  const availableDeviceTypes = DeviceElementTypes
  const { t } = useTranslation()

  return (
    <div className="grid grid-flow-row grid-cols-4 gap-4 items-center justify-items-center w-full">
      {availableDeviceTypes.map((deviceType) => {
        return (
          <div
            key={deviceType}
            className="border dark:border-none h-32 w-32 group flex flex-col gap-1 items-center justify-center hover:bg-slate-200 dark:hover:bg-slate-800 py-2 px-4 rounded-md cursor-pointer text-center"
          >
            <DeviceIcon className="w-12 h-12" variant={deviceType}></DeviceIcon>
            <div className="">{t(`device.type.${deviceType}`)}</div>
          </div>
        )
      })}
    </div>
  )
}

export default DeviceSelection
