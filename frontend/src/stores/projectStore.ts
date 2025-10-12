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
      dropIndex: number,
    ) => void

    restoreItemsToOriginalPositions: (
      draggedItems: IConfigItem[],
      currentConfigIndex: number,
      sourceConfigIndex: number,
      originalPositions: Map<string, number>,
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
      dropIndex: number,
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

        // Step 2: Remove items from target config
        // this is necessary because we can have dragMove and dragDrop
        // which both move items, so we need to ensure no duplicates
        const targetItems = isMoveBetweenConfigs        
          ? ((configFiles[targetConfigIndex]?.ConfigItems ||
              []) as IConfigItem[])
          : sourceItemsWithoutDragged

        const targetItemsWithoutDragged = targetItems.filter(
          (item) => !draggedItemIds.includes(item.GUID),
        )
        // Step 3: Create final target array
        const finalTargetItems = [
          ...targetItemsWithoutDragged.slice(0, dropIndex),
          ...draggedItems,
          ...targetItemsWithoutDragged.slice(dropIndex),
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
    // In projectStore.ts, add this action
    restoreItemsToOriginalPositions: (
      draggedItems: IConfigItem[],
      currentConfigIndex: number,
      sourceConfigIndex: number,
      originalPositions: Map<string, number>,
    ) =>
      set((state) => {
        if (state.project === null) return state

        console.log("🔄 Store: Restoring items to original positions:", {
          draggedItems: draggedItems.map((i) => i.GUID),
          currentConfigIndex,
          sourceConfigIndex,
          originalPositions: Array.from(originalPositions.entries()),
        })

        const configFiles = [...state.project.ConfigFiles]

        // If items are in a different config, move them back first
        if (currentConfigIndex !== sourceConfigIndex) {
          // Remove items from current config
          const currentItems = [
            ...(configFiles[currentConfigIndex]?.ConfigItems || []),
          ]
          const draggedItemIds = draggedItems.map((item) => item.GUID)
          const currentItemsWithoutDragged = currentItems.filter(
            (item) => !draggedItemIds.includes(item.GUID),
          )
          configFiles[currentConfigIndex] = {
            ...configFiles[currentConfigIndex],
            ConfigItems: currentItemsWithoutDragged,
          }
        }

        // Get source config items and remove dragged items
        const sourceItems = [
          ...(configFiles[sourceConfigIndex]?.ConfigItems || []),
        ]
        const draggedItemIds = draggedItems.map((item) => item.GUID)
        const workingList = sourceItems.filter(
          (item) => !draggedItemIds.includes(item.GUID),
        )

        // Sort items by original position (descending) to avoid index shifts
        const itemsToRestore = Array.from(originalPositions.entries())
          .map(([guid, originalIndex]) => ({
            item: draggedItems.find((item) => item.GUID === guid)!,
            originalIndex,
          }))
          .sort((a, b) => a.originalIndex - b.originalIndex)

        // Insert each item at its original position
        itemsToRestore.forEach(({ item, originalIndex }) => {
          workingList.splice(originalIndex, 0, item)
        })

        // Update source config with restored items
        configFiles[sourceConfigIndex] = {
          ...configFiles[sourceConfigIndex],
          ConfigItems: workingList,
        }

        console.log("✅ Store: Restoration complete")

        return {
          project: {
            ...state.project,
            ConfigFiles: configFiles,
          },
        }
      }),
  },
}))

export const useProjectStoreActions = () =>
  useProjectStore((state) => state.actions)
