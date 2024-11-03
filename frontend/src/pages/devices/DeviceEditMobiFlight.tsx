import DeviceIcon from "@/components/mobiflight/icons/DeviceIcon";
import { DeviceElementType, IDeviceItem } from "@/types";
import { IconTrash } from "@tabler/icons-react";

export type MobiFlightDeviceEditPanelProps = {
  device: IDeviceItem;
};

export const MobiFlightDeviceEditPanel = (
  props: MobiFlightDeviceEditPanelProps
) => {
  const device = props.device;
  return (
    <div className="w-full flex flex-col p-4 overflow-y-auto gap-2">
      {(device?.Elements ?? []).map((element) => {
        return (
          <div className="group flex flex-row gap-4 items-center hover:bg-slate-200 py-2 px-4 rounded-md cursor-pointer border-2 border-slate-200" key={element.Id}>
            <DeviceIcon className="w-8 h-8" variant={element.Type as DeviceElementType}></DeviceIcon>
            <div className="grow">{element.Name}</div>
            <div className="group-hover:opacity-100 opacity-0"><IconTrash className="text-gray-400"></IconTrash></div>
          </div>
        );
      })}
    </div>
  );
};

export default MobiFlightDeviceEditPanel;
