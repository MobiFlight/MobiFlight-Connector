import Settings from "@/types/settings"
import { create } from "zustand"

interface SettingsState {
  settings: Settings | null
  setSettings: (Settings: Settings) => void
}

export const useProjectStore = create<SettingsState>((set) => ({
  settings: null,
  setSettings: (settings) => set({ settings: settings })
}))