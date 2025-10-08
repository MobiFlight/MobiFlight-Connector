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
  actions: {
    // Add the drag-and-drop functions
    moveItemsBetweenConfigs: (
      draggedItems: IConfigItem[],
      sourceConfigIndex: number,
      targetConfigIndex: number,
      dropTargetItemId?: string,
      originalDragIndex?: number,
    ) => void
  }
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
  actions: {
    moveItemsBetweenConfigs: (
      draggedItems: IConfigItem[],
      sourceConfigIndex: number,
      targetConfigIndex: number,
      dropTargetItemId?: string,
      originalDragIndex?: number,
    ) =>
      set((state) => {
        if (state.project === null) return state

        const configFiles = state.project?.ConfigFiles ?? []
        const draggedItemIds = draggedItems.map((item) => item.GUID)

        // Step 1: Remove items from source config
        const sourceItems = (configFiles[sourceConfigIndex]?.ConfigItems ||
          []) as IConfigItem[]
        const sourceItemsWithoutDragged = sourceItems.filter(
          (item) => !draggedItemIds.includes(item.GUID),
        )

        const isMoveBetweenConfigs = sourceConfigIndex !== targetConfigIndex

        // Step 2: Determine target position
        const targetItems = (configFiles[targetConfigIndex]?.ConfigItems ||
          []) as IConfigItem[]
        let targetItemsAfterRemoval = targetItems

        // If same config, calculate positions after removal
        if (!isMoveBetweenConfigs) {
          targetItemsAfterRemoval = sourceItemsWithoutDragged
        }

        let insertionIndex = 0 // Default: add to beginning

        if (dropTargetItemId) {
          const dropTargetIndex = targetItemsAfterRemoval.findIndex(
            (item) => item.GUID === dropTargetItemId,
          )

          if (dropTargetIndex >= 0) {
            const isCrossTabDrag = targetConfigIndex !== sourceConfigIndex
            const adjustedOriginalIndex = isCrossTabDrag
              ? 0
              : originalDragIndex || 0
            const dropDirectionOffset =
              dropTargetIndex >= adjustedOriginalIndex ? 1 : 0
            insertionIndex = dropTargetIndex + dropDirectionOffset
          }
        } else {
          insertionIndex = targetItemsAfterRemoval.length
        }

        // Step 3: Create final target array
        const finalTargetItems = [
          ...targetItemsAfterRemoval.slice(0, insertionIndex),
          ...draggedItems,
          ...targetItemsAfterRemoval.slice(insertionIndex),
        ]

        // Create completely new Project object with new ConfigFiles array
        const newProject: Project = {
          ...state.project,
          ConfigFiles: state.project.ConfigFiles.map((file, i) => {
            if (isMoveBetweenConfigs && i === sourceConfigIndex) {
              return { ...file, ConfigItems: [...sourceItemsWithoutDragged] }
            }
            if (i === targetConfigIndex) {
              return { ...file, ConfigItems: [...finalTargetItems] }
            }
            return file
          }),
        }

        return { project: newProject }
      }),
  },
}))

export const useProjectStoreActions = () => useProjectStore(state => state.actions)
