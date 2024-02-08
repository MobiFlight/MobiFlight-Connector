import { create } from 'zustand';
import * as Types from '../types/index';

interface ConfigState {
    items: Types.IConfigItem[]
    setItems: (items: Types.IConfigItem[]) => void
}

export const useConfigStore = create<ConfigState>((set) => ({
    items: [],
    setItems: (items) => set({ items })
}))