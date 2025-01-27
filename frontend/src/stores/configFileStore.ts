import { create } from 'zustand';
import * as Types from '../types/index';

interface ConfigState {
    items: Types.IConfigItem[]
    setItems: (items: Types.IConfigItem[]) => void
    updateItem: (item: Types.IConfigItem) => void
    updateItems: (items: Types.IConfigItem[]) => void
}

export const useConfigStore = create<ConfigState>((set) => ({
    items: [],
    setItems: (newItems) => set(() => ({ items : newItems })),
    updateItem: (item: Types.IConfigItem) => set((state) => ({ items : state.items.map((existingItem) => existingItem.GUID === item.GUID ? item : existingItem) })),
    updateItems: (configItems: Types.IConfigItem[]) => set((state) => {
      const updateMap = new Map(configItems.map(item => [item.GUID, item]))
      const mergedItems = state.items.map((item) => {
        return updateMap.get(item.GUID) || item
      })
      return { items: mergedItems }
    })
}))