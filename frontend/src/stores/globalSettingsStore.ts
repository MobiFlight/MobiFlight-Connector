import { create } from 'zustand';
import * as Types from '../types/index';

interface GlobalSettingsState {
    settings: Types.IGlobalSettings
    setSettings: (settings: Types.IGlobalSettings) => void
}

export const useGlobalSettingsStore = create<GlobalSettingsState>((set) => ({
    settings: {} as Types.IGlobalSettings,
    setSettings: (newSettings) => set(() => ({ settings : newSettings })),
}))