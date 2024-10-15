import { create } from 'zustand';
import * as Types from '../types/index';

interface ConfigState {
    items: Types.IConfigItem[]
    setItems: (items: Types.IConfigItem[]) => void
    updateItem: (item: Types.IConfigItem) => void
}

export const useConfigStore = create<ConfigState>((set) => ({
    items: [],
    setItems: (newItems) => set(() => ({ items : newItems })),
    updateItem: (item: Types.IConfigItem) => set((state) => ({ items : state.items.map((existingItem) => {
          return existingItem.GUID === item.GUID ? item : existingItem
        })
      })
    )
}))