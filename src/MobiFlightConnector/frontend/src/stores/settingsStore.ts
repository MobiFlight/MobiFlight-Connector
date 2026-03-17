import { ProjectInfo } from "@/types/project"
import Settings from "@/types/settings"
import { create } from "zustand"

interface SettingsState {
  recentProjects: ProjectInfo[]
  settings: Settings | null
  setSettings: (Settings: Settings) => void
  setRecentProjects: (recentProjects: ProjectInfo[]) => void
}

export const useSettingsStore = create<SettingsState>((set) => ({
  settings: null,
  setSettings: (settings) => set({ settings: settings }),
  setRecentProjects: (recentProjects) =>
    set({ recentProjects: recentProjects }),
  recentProjects: [],
}))

export const useRecentProjects = () => {
  return {
    recentProjects: useSettingsStore((state) => state.recentProjects),
    setRecentProjects: useSettingsStore((state) => state.setRecentProjects),
  }
}