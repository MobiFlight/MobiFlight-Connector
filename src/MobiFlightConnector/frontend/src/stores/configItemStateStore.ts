import { create } from "zustand"
import { IConfigValueOnlyItem } from "@/types/config"

interface ConfigItemState {
  values: Record<string, IConfigValueOnlyItem>
  updateConfigItemState: (items: IConfigValueOnlyItem[]) => void
  setConfigItemState: (items: IConfigValueOnlyItem[]) => void
}

export const useConfigItemStateStore = create<ConfigItemState>((set) => ({
  values: {},
  updateConfigItemState: (items) => set((state) => {
    const newValues = { ...state.values }
    items.forEach(item => {
      newValues[item.GUID] = { GUID: item.GUID, RawValue: item.RawValue, Value: item.Value, Status: item.Status }
    })
    return { values: newValues }
  }),
  setConfigItemState: (items) => set(() => {
    const newValues: Record<string, IConfigValueOnlyItem> = {}
    items.forEach(item => {
      newValues[item.GUID] = { GUID: item.GUID, RawValue: item.RawValue, Value: item.Value, Status: item.Status }
    })
    return { values: newValues }
  })
}))

export const useConfigItemStateValue = (guid: string | undefined) =>
  useConfigItemStateStore((s) => (guid ? s.values[guid] : undefined))

export const useConfigItemStateUpdate = () =>
  useConfigItemStateStore((s) => s.updateConfigItemState)

export const useConfigItemStateSet = () =>
  useConfigItemStateStore((s) => s.setConfigItemState)
