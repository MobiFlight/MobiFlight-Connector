import { ProjectInfo } from "@/types/project"
import Settings from "@/types/settings"
import { create } from "zustand"

interface SettingsState {
  recentProjects: ProjectInfo[]
  settings: Settings | null
  setSettings: (Settings: Settings) => void
  setRecentProjects: (recentProjects: ProjectInfo[]) => void
  isConflictOpen: boolean
  setIsConflictOpen: (open: boolean) => void
  cloudSettings: Partial<Settings> | null
  setCloudSettings: (cloudSettings: Partial<Settings> | null) => void
}

export const useSettingsStore = create<SettingsState>((set) => ({
  settings: null,
  setSettings: (settings) => set({ settings: settings }),
  setRecentProjects: (recentProjects) =>
    set({ recentProjects: recentProjects }),
  recentProjects: [],
  isConflictOpen: false,
  setIsConflictOpen: (open) => set({ isConflictOpen: open }),
  cloudSettings: null,
  setCloudSettings: (cloudSettings) => set({ cloudSettings }),
}))

export const useRecentProjects = () => {
  return {
    recentProjects: useSettingsStore((state) => state.recentProjects),
    setRecentProjects: useSettingsStore((state) => state.setRecentProjects),
  }
}

export const useLogSettings = () => {
  return {
    logEnabled: useSettingsStore(
      (state) => state.settings?.LogEnabled ?? false,
    ),
  }
}
