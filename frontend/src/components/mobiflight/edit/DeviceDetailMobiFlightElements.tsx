import DeviceIcon from "@/components/mobiflight/icons/DeviceIcon"
import { IDeviceElement, IDeviceItem } from "@/types"
import { DeviceElementType } from "@/types/index"
import { IconDotsVertical, IconTrash } from "@tabler/icons-react"
import { NavLink } from "react-router-dom"

export type MobiFlightDeviceEditPanelProps = {
  device: IDeviceItem
  onElementDelete: (device: IDeviceElement) => void
}

export const MobiFlightDeviceEditPanel = (
  props: MobiFlightDeviceEditPanelProps
) => {
  const { device, onElementDelete } = props
  
  return (
    <div className="w-full h-full flex flex-col p-4 overflow-y-auto gap-2 overscroll-contain">
      {(device?.Elements ?? []).map((element) => {
        return (
          <NavLink
          key={element.Id}
            className={(navData) =>
              navData.isActive
                ? "bg-slate-200 dark:bg-slate-800 border-2 rounded-md transition-all"
                : "border-2 rounded-md transition-all dark:bg-background"
            }
            to={`elements/${element.Id}`}
          >
            <div
              className="flex flex-row items-center py-2 pr-4 cursor-pointer group transition-opacity hover:dark:bg-gray-700 rounded-md "
            >
              <IconDotsVertical className="group-hover:opacity-100 opacity-0 text-gray-400"></IconDotsVertical>
              <div className="flex flex-row grow gap-4 items-center">
                <DeviceIcon
                  className="w-8 h-8"
                  variant={element.Type as DeviceElementType}
                ></DeviceIcon>
                <div className="grow">{element.Name}</div>
                <div className="group-hover:opacity-100 opacity-0">
                  <IconTrash role="button" className="text-gray-400" onClick={()=>{onElementDelete(element)}}></IconTrash>
                </div>
              </div>
            </div>
          </NavLink>
        )
      })}
    </div>
  )
}

export default MobiFlightDeviceEditPanel
