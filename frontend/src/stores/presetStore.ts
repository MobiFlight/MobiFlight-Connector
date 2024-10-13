import { create } from 'zustand';
import * as Types from '../types/index';

interface HubHopPresetsState {
    presets: Types.Preset[];
    setPresets: (settings: Types.Preset[]) => void
}

export const useMsfsPresetStore = create<HubHopPresetsState>((set) => ({
    presets: [] as Types.Preset[],
    setPresets: (newPresets) => set(() => ({ presets : newPresets })),
}))

export const useXplanePresetStore = create<HubHopPresetsState>((set) => ({
  presets: [] as Types.Preset[],
  setPresets: (newPresets) => set(() => ({ presets : newPresets })),
}))