import { IConfigItem, Project } from "@/types"
import { create } from "zustand"

interface ProjectState {
  hasChanged: boolean
  project: Project | null
  activeConfigFileIndex: number
  setHasChanged: (hasChanged: boolean) => void
  setProject: (project: Project) => void
  setConfigItems: (index: number, items: IConfigItem[]) => void
  setActiveConfigFileIndex: (index: number) => void // Add this
  updateConfigItems: (index: number, items: IConfigItem[]) => void // Add this
  updateConfigItem: (index: number, item: IConfigItem, upsert: boolean) => void // Add this
  clearProject: () => void
}

export const useProjectStore = create<ProjectState>((set) => ({
  project: null,
  hasChanged: false,
  activeConfigFileIndex: 0,

  setProject: (project) => set({ project: project }),

  setHasChanged: (hasChanged) => set({ hasChanged: hasChanged }),

  setActiveConfigFileIndex: (index) => set({ activeConfigFileIndex: index }),

  setConfigItems: (index, items) =>
    set((state) => {
      if (state.project === null) return state

      // Create completely new Project object with new ConfigFiles array
      const newProject: Project = {
        ...state.project,
        ConfigFiles: state.project.ConfigFiles.map((file, i) =>
          i === index
            ? { ...file, ConfigItems: [...items] } // New ConfigFile with new ConfigItems array
            : file,
        ),
      }

      console.log("Set config items at index", index, items)
      return { project: newProject }
    }),

  updateConfigItems: (index: number, items: IConfigItem[]) =>
    set((state) => {
      if (!state.project?.ConfigFiles[index]) return state

      const currentItems: IConfigItem[] =
        state.project.ConfigFiles[index].ConfigItems ?? []
      const updateMap = new Map<string, IConfigItem>(
        items.map((item) => [item.GUID, item]),
      )
      const mergedItems: IConfigItem[] = currentItems.map((item) => {
        return updateMap.get(item.GUID) || item
      })

      // Create completely new Project object
      const newProject: Project = {
        ...state.project,
        ConfigFiles: state.project.ConfigFiles.map((file, i) =>
          i === index
            ? { ...file, ConfigItems: [...mergedItems] } // New array reference
            : file,
        ),
      }
      return { project: newProject }
    }),

  updateConfigItem: (index: number, updatedItem: IConfigItem, upsert = false) =>
    set((state) => {
      if (!state.project?.ConfigFiles[index]) return state

      const currentItems: IConfigItem[] =
        state.project.ConfigFiles[index].ConfigItems ?? []
      const itemIndex: number = currentItems.findIndex(
        (existingItem) => existingItem.GUID === updatedItem.GUID,
      )

      let updatedItems = [] as IConfigItem[]

      if (itemIndex === -1 && !upsert) {
        return state
      }

      if (itemIndex !== -1) {
        // Update existing item
        updatedItems = [...currentItems]
        updatedItems[itemIndex] = updatedItem
      } else if (upsert) {
        // Insert new item if upsert is true
        updatedItems = [...currentItems, updatedItem]
      }

      // Create completely new Project object
      const newProject: Project = {
        ...state.project,
        ConfigFiles: state.project.ConfigFiles.map((file, i) =>
          i === index
            ? { ...file, ConfigItems: [...updatedItems] } // New array reference
            : file,
        ),
      }
      return { project: newProject }
    }),
  clearProject: () => set({ project: null }),
}))
