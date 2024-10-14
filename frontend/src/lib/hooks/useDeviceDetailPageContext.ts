import { IDeviceElement, IDeviceItem } from "@/types"
import { useOutletContext } from "react-router-dom"

export type DeviceDetailContext = {
  device: IDeviceItem;
  element: IDeviceElement;
  updateDevice: (device: IDeviceItem) => void;
};

export const useDeviceDetailPageContext = () => {
  return useOutletContext<DeviceDetailContext>();
}