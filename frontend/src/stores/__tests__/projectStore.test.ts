import { describe, it, expect, beforeEach, afterEach } from 'vitest'
import { useProjectStore } from '../projectStore'
import type { IConfigItem } from '@/types'

// Simple mock config item creator
const createMockItem = (id: string, name: string): IConfigItem => ({
  GUID: id,
  Name: name,
} as IConfigItem)

describe('ProjectStore - moveItemsBetweenConfigs', () => {
  const initialStoreState = useProjectStore.getState()

  beforeEach(() => {
    // Reset the store to initial state
    useProjectStore.setState(initialStoreState)
  })

  afterEach(() => {
    // Clean up after each test
    useProjectStore.getState().clearProject()
  })

  it('should move single item from config 0 to config 1', () => {
    // Arrange
    const item1 = createMockItem('item1', 'Item 1')
    const item2 = createMockItem('item2', 'Item 2')
    
    // Create a mock project with 3 configs for testing
    const mockProject = {
      ConfigFiles: [
        { ConfigItems: [] },
        { ConfigItems: [] },
        { ConfigItems: [] }
      ]
    } as any
    
    // Set the project first
    useProjectStore.getState().setProject(mockProject)
    
    console.log('=== DEBUGGING STORE ===')
    console.log('Initial project state:', useProjectStore.getState().project)
    
    // Set up initial state using the store methods
    useProjectStore.getState().setConfigItems(0, [item1, item2])
    useProjectStore.getState().setConfigItems(1, [])
    
    console.log('After setConfigItems:')
    console.log('Config 0:', useProjectStore.getState().project?.ConfigFiles[0]?.ConfigItems?.map(i => i.GUID))
    console.log('Config 1:', useProjectStore.getState().project?.ConfigFiles[1]?.ConfigItems?.map(i => i.GUID))

    // Verify setup worked
    const beforeState = useProjectStore.getState()
    expect(beforeState.project?.ConfigFiles[0].ConfigItems).toHaveLength(2)
    expect(beforeState.project?.ConfigFiles[1].ConfigItems).toHaveLength(0)

    // Act - Move item1 from config 0 to config 1
    const actions = useProjectStore.getState().actions
    if (actions?.moveItemsBetweenConfigs) {
      console.log('Calling moveItemsBetweenConfigs...')
      actions.moveItemsBetweenConfigs([item1], 0, 1)
    } else {
      console.log('ERROR: moveItemsBetweenConfigs not found!')
    }

    // Get fresh state after the action
    const afterState = useProjectStore.getState()
    
    console.log('After move:')
    console.log('Config 0:', afterState.project?.ConfigFiles[0]?.ConfigItems?.map(i => i.GUID))
    console.log('Config 1:', afterState.project?.ConfigFiles[1]?.ConfigItems?.map(i => i.GUID))

    // Assert
    expect(afterState.project).not.toBeNull()
    
    // Config 0 should only have item2
    expect(afterState.project!.ConfigFiles[0].ConfigItems).toHaveLength(1)
    expect(afterState.project!.ConfigFiles[0].ConfigItems[0].GUID).toBe('item2')
    
    // Config 1 should have item1
    expect(afterState.project!.ConfigFiles[1].ConfigItems).toHaveLength(1)
    expect(afterState.project!.ConfigFiles[1].ConfigItems[0].GUID).toBe('item1')
  })

  it('should not create duplicate items when moving between configs', () => {
    // This test specifically targets the duplicate issue you found
    const item1 = createMockItem('item1', 'Item 1')
    const item2 = createMockItem('item2', 'Item 2')
    
    const mockProject = {
      ConfigFiles: [
        { ConfigItems: [] },
        { ConfigItems: [] }
      ]
    } as any
    
    useProjectStore.getState().setProject(mockProject)
    useProjectStore.getState().setConfigItems(0, [item1, item2])
    useProjectStore.getState().setConfigItems(1, [])

    console.log('=== DUPLICATE TEST ===')
    const beforeState = useProjectStore.getState()
    const beforeItems = beforeState.project!.ConfigFiles.flatMap(cf => cf.ConfigItems)
    console.log('Before move - All items:', beforeItems.map(i => i.GUID))

    // Act
    const actions = useProjectStore.getState().actions
    actions.moveItemsBetweenConfigs([item1], 0, 1)

    // Assert - Check for duplicates across all configs
    const afterState = useProjectStore.getState()
    const afterItems = afterState.project!.ConfigFiles.flatMap(cf => cf.ConfigItems)
    const allGuids = afterItems.map(item => item.GUID)
    const uniqueGuids = new Set(allGuids)
    
    console.log('After move - All items:', allGuids)
    console.log('Unique GUIDs:', Array.from(uniqueGuids))
    console.log('Has duplicates?', allGuids.length !== uniqueGuids.size)

    // Should have no duplicates
    expect(allGuids.length).toBe(uniqueGuids.size)
    expect(uniqueGuids.size).toBe(2)
    expect(uniqueGuids.has('item1')).toBe(true)
    expect(uniqueGuids.has('item2')).toBe(true)
  })

  it('should handle same-config move without creating duplicates', () => {
    const item1 = createMockItem('item1', 'Item 1')
    const item2 = createMockItem('item2', 'Item 2')
    const item3 = createMockItem('item3', 'Item 3')
    
    const mockProject = {
      ConfigFiles: [
        { ConfigItems: [] }
      ]
    } as any
    
    useProjectStore.getState().setProject(mockProject)
    useProjectStore.getState().setConfigItems(0, [item1, item2, item3])

    console.log('=== SAME CONFIG TEST ===')
    const beforeState = useProjectStore.getState()
    console.log('Before same-config move:', beforeState.project!.ConfigFiles[0].ConfigItems.map(i => i.GUID))

    // Act - Move within same config
    const actions = useProjectStore.getState().actions
    actions.moveItemsBetweenConfigs([item1], 0, 0)

    const afterState = useProjectStore.getState()
    const guids = afterState.project!.ConfigFiles[0].ConfigItems.map(item => item.GUID)
    const uniqueGuids = new Set(guids)
    
    console.log('After same-config move:', guids)
    console.log('Has duplicates in same config?', guids.length !== uniqueGuids.size)
    
    expect(guids.length).toBe(uniqueGuids.size) // No duplicates
    expect(afterState.project!.ConfigFiles[0].ConfigItems).toHaveLength(3) // Same count
  })
})