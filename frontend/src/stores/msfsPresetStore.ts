import { create } from 'zustand';
import * as Types from '../types/index';

interface MsfsPresetsState {
    presets: Types.Preset[];
    setPresets: (settings: Types.Preset[]) => void
}

export const useMsfsPresetStore = create<MsfsPresetsState>((set) => ({
    presets: [] as Types.Preset[],
    setPresets: (newPresets) => set(() => ({ presets : newPresets })),
}))