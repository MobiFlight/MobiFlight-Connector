import { Project } from "@/types"
import { create } from "zustand"

interface ProjectState {
  project: Project | null
  setProject: (project: Project) => void
  clearProject: () => void
}

export const useProjectStore = create<ProjectState>((set) => ({
  project: null,
  setProject: (project) => set({ project: project }),
  clearProject: () => set({ project: null }),
}))