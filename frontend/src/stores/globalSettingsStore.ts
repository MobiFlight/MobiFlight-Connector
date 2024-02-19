import { create } from 'zustand';
import * as Types from '../types/index';

interface GlobalSettingsState {
    settings: Types.IGlobalSettings
    updateSetting: (setting : {}) => void
    setSettings: (settings: Types.IGlobalSettings) => void
}

export const useGlobalSettingsStore = create<GlobalSettingsState>((set) => ({
    settings: {} as Types.IGlobalSettings,
    updateSetting: (newSetting) => set((state) => ({ settings : { ...state.settings, ...newSetting } })),
    setSettings: (newSettings) => set(() => ({ settings : newSettings })),
}))