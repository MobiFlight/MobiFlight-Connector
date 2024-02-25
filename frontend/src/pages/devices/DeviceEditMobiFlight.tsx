import DeviceIcon from "@/components/mobiflight/icons/DeviceIcon"
import { IDeviceItem } from "@/types"
import { DeviceElementType } from "@/types/index"
import { IconDots, IconTrash } from "@tabler/icons-react"
import { NavLink, useLocation } from "react-router-dom"

export type MobiFlightDeviceEditPanelProps = {
  device: IDeviceItem
}

export const MobiFlightDeviceEditPanel = (
  props: MobiFlightDeviceEditPanelProps
) => {
  const device = props.device

  return (
    <div className="w-full flex flex-col p-4 overflow-y-auto gap-2">
      {(device?.Elements ?? []).map((element) => {
        return (
          <NavLink className={(navData)=>navData.isActive?"bg-red-500 border-2 rounded-md transition-all":"border-2 rounded-md transition-all"} to={`elements/${element.Id}`}>
            <div
              className="flex flex-row gap-4 items-center py-2 px-4 cursor-pointer"
              key={element.Id}
            >
              <DeviceIcon
                className="w-8 h-8"
                variant={element.Type as DeviceElementType}
              ></DeviceIcon>
              <div className="grow">{element.Name}</div>
              <div className="group-hover:opacity-100 opacity-0">
                <IconTrash className="text-gray-400"></IconTrash>
              </div>
            </div>
          </NavLink>
        )
      })}
    </div>
  )
}

export default MobiFlightDeviceEditPanel
