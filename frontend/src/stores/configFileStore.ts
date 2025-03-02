import { create } from "zustand"
import * as Types from "../types/index"

interface ConfigState {
  items: Types.IConfigItem[]
  clearItems: () => void
  setItems: (items: Types.IConfigItem[]) => void
  updateItem: (item: Types.IConfigItem, upsert?: boolean) => void
  updateItems: (items: Types.IConfigItem[]) => void
}

export const useConfigStore = create<ConfigState>((set) => ({  
  items: [],
  clearItems: () => set({ items: [] }),
  setItems: (newItems) => set(() => ({ items: newItems })),
  updateItem: (item: Types.IConfigItem, upsert = false) =>
    set((state) => {
      const itemIndex = state.items.findIndex(
        (existingItem) => existingItem.GUID === item.GUID,
      )
      if (itemIndex !== -1) {
        // Update existing item
        const updatedItems = [...state.items]
        updatedItems[itemIndex] = item
        return { items: updatedItems }
      } else if (upsert) {
        // Insert new item if upsert is true
        return { items: [...state.items, item] }
      }
      return state
    }),
  updateItems: (configItems: Types.IConfigItem[]) =>
    set((state) => {
      const updateMap = new Map(configItems.map((item) => [item.GUID, item]))
      const mergedItems = state.items.map((item) => {
        return updateMap.get(item.GUID) || item
      })
      return { items: mergedItems }
    }),
}))
