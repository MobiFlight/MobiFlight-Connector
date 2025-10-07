import React, { useCallback, useState } from "react"
import { 
  DndContext, 
  DragStartEvent, 
  DragEndEvent,
  useSensors, 
  useSensor, 
  MouseSensor, 
  TouchSensor, 
  DragMoveEvent,
  pointerWithin
} from "@dnd-kit/core"
import { restrictToVerticalAxis } from "@dnd-kit/modifiers"
import { snapToCursor } from "@/lib/dnd-kit/snap-to-cursor"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandResortConfigItem } from "@/types/commands"
import { IConfigItem } from "@/types"
import { Table } from "@tanstack/react-table"
import { ConfigItemDragOverlay } from "@/components/dnd/ConfigItemDragOverlay"
import { ConfigItemDragContext } from "./ConfigItemContext"

/**
 * The drag state that persists throughout the entire drag operation
 * This data survives tab switches and component re-renders
 */
export interface DragState {
  dragItem: IConfigItem | null
  draggedItems: IConfigItem[]    // The actual items being dragged (full objects)
  sourceConfigIndex: number      // Which config file the drag started from
  isDragging: boolean            // Whether a drag is currently active
  isInsideTable: boolean      // Whether the drag is currently over a valid table
}

/**
 * Context interface - what components can access via useConfigItemDragContext()
 */
export interface ConfigItemDragContextType {
  dragState: DragState | null
  table: Table<IConfigItem> | null                       
  setTable: (table: Table<IConfigItem> | null) => void
  setTableContainerRef: (element: Element | null) => void
}

/**
 * Props for the drag provider component
 */
interface ConfigItemDragProviderProps {
  children: React.ReactNode        
  currentConfigIndex: number       
  // Function to update config items in the project store
  // This makes drag-drop independent of table implementation
  updateConfigItems: (configIndex: number, items: IConfigItem[]) => void
  // Function to get current config items from project store
  getConfigItems: (configIndex: number) => IConfigItem[]
}

/**
 * Provider component that manages ALL drag-and-drop logic
 * Key insight: Works directly with project store, not table data
 * This makes it independent of table implementation, virtualization, filters, etc.
 */
export function ConfigItemDragProvider({ 
  children, 
  currentConfigIndex,
  updateConfigItems,
  getConfigItems
}: ConfigItemDragProviderProps) {
  
  // State: Current table instance (set by ConfigItemTable when it mounts)
  const [table, setTable] = useState<Table<IConfigItem> | null>(null)
  
  // State: Current drag operation (null when not dragging)
  const [dragState, setDragState] = useState<DragState | null>(null)

  const [tableContainerRef, setTableContainerRefInternal] = useState<Element | null>(null)

  // Configure what input methods can trigger drag operations
  const sensors = useSensors(
    useSensor(MouseSensor, {}),     // Mouse drag support
    useSensor(TouchSensor, {})      // Touch drag support for mobile
  )

  const setTableContainerRef = useCallback((element: Element | null) => {
    console.log("🔧 setTableContainerRef called with:", element)
    setTableContainerRefInternal(element)
  }, [])

  /**
   * Called when user starts dragging an item
   * Captures what items are being dragged and from which config
   */
  const handleDragStart = useCallback((event: DragStartEvent) => {
    console.log("🚀 Drag start - Current config:", currentConfigIndex)
    
    if (!table) {
      console.warn("No table available for drag start")
      return
    }

    // Get all currently selected items from the table
    // These are the items that will be moved as a group
    const selectedRows = table.getSelectedRowModel().rows
    let draggedItems = selectedRows.map(row => row.original)

    // Special case: If the dragged item isn't already selected, select it
    // This allows single-click-drag without requiring pre-selection
    const draggedRow = table.getRowModel().rows.find(row => row.id === event.active.id)
    if (draggedRow && !draggedRow.getIsSelected()) {
      // Select the dragged item
      table.setRowSelection({ [event.active.id]: true })
      // Use just this item as the dragged items
      draggedItems = [draggedRow.original]
    }

    const dragItem = draggedRow ? draggedRow.original : null

    // Create drag state that will persist throughout the operation
    const newDragState: DragState = {
      dragItem: dragItem,
      draggedItems: draggedItems,
      sourceConfigIndex: currentConfigIndex,
      isDragging: true,
      isInsideTable: true,
    }

    setDragState(newDragState)

    console.log("📋 Dragging items:", {
      count: draggedItems.length,
      items: draggedItems.map(item => item.Name || item.GUID),
      sourceConfig: currentConfigIndex
    })
  }, [table, currentConfigIndex])

  /**
   * Called when user drops the dragged items
   * Universal handler for both same-config and cross-config drops
   * Works directly with project store, independent of table implementation
   */
  const handleDragEnd = useCallback((event: DragEndEvent) => {
    const { active, over } = event

    console.log("🎯 Drag end:", { 
      activeId: active.id, 
      overId: over?.id,
      dragState 
    })

    // Clean up: Always clear drag state when drag ends
    const currentDragState = dragState
    setDragState(null)

    // Bail out if drag was cancelled or invalid
    if (!currentDragState || !over?.id || !active.id) {
      console.log("❌ Drag cancelled or invalid")
      return
    }

    // No-op: item dropped on itself
    if (active.id === over.id) {
      console.log("↩️ Item dropped on itself - no change needed")
      return
    }

    // Determine target config from drop target data
    // If no configIndex in drop data, assume same config
    const targetConfigIndex = over.data.current?.configIndex ?? currentConfigIndex
    const dropTargetItemId = over.id

    console.log("📍 Drop analysis:", {
      sourceConfig: currentDragState.sourceConfigIndex,
      targetConfig: targetConfigIndex,
      dropTargetItem: dropTargetItemId,
      draggedCount: currentDragState.draggedItems.length
    })    

    // **Key insight: Universal move strategy**
    // 1. Remove items from source config
    // 2. Add items to target config at the right position
    // This works for both same-config reorder and cross-config moves!

    const isCrossTabDrag = targetConfigIndex !== currentDragState.sourceConfigIndex 

    const sourceItems = getConfigItems(currentDragState.sourceConfigIndex)
    const targetItems = isCrossTabDrag
      ? getConfigItems(targetConfigIndex)  // Different config: get target items
      : sourceItems  // Same config: work with same array

    // Step 0: Determine the source position of the dragged items
    // We need this to calculate draggin up vs down
    const originalIndex = sourceItems.findIndex(
      item => item.GUID === active.id
    )

    // Step 1: Remove dragged items from source
    const draggedItemIds = currentDragState.draggedItems.map(item => item.GUID)
    const sourceItemsWithoutDragged = sourceItems.filter(
      item => !draggedItemIds.includes(item.GUID)
    )

    // Step 2: Find insertion point in target config
    let targetItemsAfterRemoval = targetItems
    
    // If same config, use the array after removal for position calculation
    if (targetConfigIndex === currentDragState.sourceConfigIndex) {
      targetItemsAfterRemoval = sourceItemsWithoutDragged
    }

    // Find where to insert the dragged items
    const dropTargetIndex = targetItemsAfterRemoval.findIndex(
      item => item.GUID === dropTargetItemId
    )

    // Determine insertion position based on drag direction
    // If dragging down: insert after the target
    // If dragging up: insert before the target
    // originalIndex matters only for same-config drags
    const adjustedOriginalIndex = isCrossTabDrag ? 0 : originalIndex
    const dropDirectionOffset = dropTargetIndex >= adjustedOriginalIndex ? 1 : 0

    let insertionIndex = 0  // Default: add to beginning
    if (dropTargetIndex >= 0) {
      // Insert after the drop target
      insertionIndex = dropTargetIndex + dropDirectionOffset
    }

    // Step 3: Insert dragged items at the calculated position
    const finalTargetItems = [
      ...targetItemsAfterRemoval.slice(0, insertionIndex),
      ...currentDragState.draggedItems,
      ...targetItemsAfterRemoval.slice(insertionIndex)
    ]

    // Step 4: Update the project store
    if (targetConfigIndex === currentDragState.sourceConfigIndex) {
      // Same config: single update
      console.log("📝 Same-config reorder - updating items in place")
      updateConfigItems(targetConfigIndex, finalTargetItems)
    } else {
      // Cross-config: update both source and target
      console.log("🔄 Cross-config move - updating both configs")
      updateConfigItems(currentDragState.sourceConfigIndex, sourceItemsWithoutDragged)
      updateConfigItems(targetConfigIndex, finalTargetItems)
    }

    // Step 5: Notify backend about the change
    publishOnMessageExchange().publish({
      key: "CommandResortConfigItem",
      payload: {
        items: currentDragState.draggedItems,           // What items were moved
        sourceFileIndex: currentDragState.sourceConfigIndex,  // Where they came from  
        targetFileIndex: targetConfigIndex,            // Where they went
        newIndex: insertionIndex,                      // Their new position
      }
    } as CommandResortConfigItem)

  }, [dragState, currentConfigIndex, updateConfigItems, getConfigItems])

  const handleDragMove = useCallback((event: DragMoveEvent) => {
    if (!dragState || !tableContainerRef) return
    
    // Get the current mouse position
    const { x, y } = event.activatorEvent as MouseEvent
    const currentX = event.delta.x + x
    const currentY = event.delta.y + y

    // Check if cursor is outside the table container
    const containerRect = tableContainerRef.getBoundingClientRect()
    const isInsideTable = currentX >= containerRect.left && 
                     currentX <= containerRect.right && 
                     currentY >= containerRect.top &&
                     currentY <= containerRect.bottom


    // console.log("📦 Table container bounds:", containerRect)
    // console.log("  Current Mouse Position:", { currentX, currentY })
    // console.log("  Current Offset", { x: event.delta.x, y: event.delta.y })
    
    // Update drag state if boundary crossed
    if (isInsideTable !== dragState.isInsideTable) {
      setDragState(prev => prev ? {
        ...prev,
        isInsideTable: isInsideTable
      } : null)
    }
  }, [dragState, tableContainerRef])

  // Context value that child components can access
  const contextValue: ConfigItemDragContextType = {
    dragState,    // Current drag operation state (null when not dragging)
    table,        // Current table instance
    setTable,      // Function for table to register itself,
    setTableContainerRef, // Function for table to register its container element
  }

  // Dynamic modifiers based on drag state
  const getModifiers = () => {
    if (!(dragState?.isInsideTable ?? true)) {
      // Outside container: free movement for cross-tab dragging
      return [snapToCursor]
    } else {
      // Inside container: restrict to vertical for row reordering
      return [snapToCursor, restrictToVerticalAxis]
    }
  }

  return (
    // Provide context to child components
    <ConfigItemDragContext.Provider value={contextValue}>
      {/* The actual DnD functionality wrapper */}
      <DndContext
        sensors={sensors}
        collisionDetection={pointerWithin}
        modifiers={getModifiers()}
        onDragStart={handleDragStart}
        onDragMove={handleDragMove}
        onDragEnd={handleDragEnd}
      >
        {children}
        <ConfigItemDragOverlay />
      </DndContext>
    </ConfigItemDragContext.Provider>
  )
}