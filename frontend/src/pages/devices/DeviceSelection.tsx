import DeviceIcon from "@/components/mobiflight/icons/DeviceIcon";
import { DeviceElementTypes } from "@/types/deviceElements";

const DeviceSelection = () => {
  const availableDeviceTypes = DeviceElementTypes;

  return (
    <div className="flex flex-row flex-wrap gap-4">
      {availableDeviceTypes.map((deviceType) => {
        return (
          <div
            key={deviceType}
            className="h-24 w-24 group flex flex-col gap-1 items-center justify-center hover:bg-slate-200 py-2 px-4 rounded-md cursor-pointer border-2 border-slate-200"
          >
            <DeviceIcon className="w-12 h-12" variant={deviceType}></DeviceIcon>
            <div className="">{deviceType}</div>
          </div>
        );
      })}
    </div>
  );
};

export default DeviceSelection;
