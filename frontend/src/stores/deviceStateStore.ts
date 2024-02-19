import { create } from 'zustand';
import { IDeviceItem } from '../types/index';

export type ExecutionState = "Stopped" | "Running" | "Paused" | "Testing"

interface DevicesState {
    devices: IDeviceItem[]
    setDevices: (devices: IDeviceItem[]) => void
}

export const useDevicesStore = create<DevicesState>((set) => ({
    devices: [] as IDeviceItem[],
    setDevices: (newList) => set(() => ({ devices: newList })),
}))