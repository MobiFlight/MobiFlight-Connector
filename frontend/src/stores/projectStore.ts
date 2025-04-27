import { IConfigItem, Project } from "@/types"
import { create } from "zustand"

interface ProjectState {
  project: Project | null
  setProject: (project: Project) => void
  setConfigItems: (index: number, items: IConfigItem[]) => void
  clearProject: () => void
}

export const useProjectStore = create<ProjectState>((set) => ({
  project: null,
  setProject: (project) => set({ project: project }),
  setConfigItems: (index, items) => set((state) => {
    if (state.project === null) return state
    const newProject = { ...state.project }
    newProject.ConfigFiles[index].ConfigItems = items
    return { project: newProject }
  }),
  clearProject: () => set({ project: null }),
}))