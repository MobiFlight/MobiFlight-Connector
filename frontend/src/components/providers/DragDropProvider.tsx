import React, { createContext, useContext, useCallback, useState } from "react"
import { 
  DndContext, 
  DragStartEvent, 
  DragEndEvent, 
  closestCenter, 
  useSensors, 
  useSensor, 
  MouseSensor, 
  TouchSensor 
} from "@dnd-kit/core"
import { restrictToVerticalAxis } from "@dnd-kit/modifiers"
import { snapToCursor } from "@/lib/dnd-kit/snap-to-cursor"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandResortConfigItem } from "@/types/commands"
import { IConfigItem } from "@/types"
import { Table } from "@tanstack/react-table"

/**
 * The drag state that persists throughout the entire drag operation
 * This data survives tab switches and component re-renders
 */
interface DragState {
  draggedItems: IConfigItem[]    // The actual items being dragged (full objects)
  sourceConfigIndex: number      // Which config file the drag started from
  isDragging: boolean            // Whether a drag is currently active
}

/**
 * Context interface - what components can access via useConfigItemDragContext()
 */
interface ConfigItemDragContextType {
  dragState: DragState | null                            // Current drag operation state
  table: Table<IConfigItem> | null                       // Current table instance for getting selections
  setTable: (table: Table<IConfigItem> | null) => void   // Function to register table with context
}

// Create the React context with default values
const ConfigItemDragContext = createContext<ConfigItemDragContextType>({
  dragState: null,
  table: null,
  setTable: () => {}
})

/**
 * Hook for components to access the drag context
 * Better name than useConfigItemDrag - clearly indicates it's accessing context
 */
export const useConfigItemDragContext = () => useContext(ConfigItemDragContext)

/**
 * Props for the drag provider component
 */
interface ConfigItemDragProviderProps {
  children: React.ReactNode        // Child components that can participate in drag/drop
  currentConfigIndex: number       // Index of the currently active config file
  setItems: (items: IConfigItem[]) => void // Function to update items in current config
}

/**
 * Provider component that manages ALL drag-and-drop logic
 * This replaces the complex useConfigItemDragDrop hook + DragDropProvider combination
 * 
 * Key benefits:
 * - Single source of truth for drag state
 * - Survives tab switches because provider stays mounted
 * - Simple API for child components
 * - Clear separation of concerns
 */
export function ConfigItemDragProvider({ 
  children, 
  currentConfigIndex,
  setItems
}: ConfigItemDragProviderProps) {
  
  // State: Current table instance (set by ConfigItemTable when it mounts)
  const [table, setTable] = useState<Table<IConfigItem> | null>(null)
  
  // State: Current drag operation (null when not dragging)
  const [dragState, setDragState] = useState<DragState | null>(null)

  // Configure what input methods can trigger drag operations
  const sensors = useSensors(
    useSensor(MouseSensor, {}),     // Mouse drag support
    useSensor(TouchSensor, {})      // Touch drag support for mobile
  )

  /**
   * Called when user starts dragging an item
   * This is where we capture what items are being dragged and from which config
   */
  const handleDragStart = useCallback((event: DragStartEvent) => {
    console.log("🚀 Drag start - Current config:", currentConfigIndex)
    
    // Can't drag without a table instance
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
      
      // Add it to our dragged items list
      const draggedItem = draggedRow.original
      if (!draggedItems.some(item => item.GUID === draggedItem.GUID)) {
        draggedItems = [draggedItem]  // Replace with just this item
      }
    }

    // Create drag state that will persist throughout the operation
    const newDragState: DragState = {
      draggedItems,                    // The actual item objects (not just IDs)
      sourceConfigIndex: currentConfigIndex,  // Remember where we started
      isDragging: true
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
   * This is where we determine what to do based on where they dropped
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

    // Determine where the items are being dropped
    // If over.data.current?.configIndex exists, it's a cross-config drop to that config
    // Otherwise, it's a same-config reorder
    const targetConfigIndex = over.data.current?.configIndex ?? currentConfigIndex
    
    const isCrossConfigDrop = targetConfigIndex !== currentDragState.sourceConfigIndex

    console.log("📍 Drop analysis:", {
      sourceConfig: currentDragState.sourceConfigIndex,
      targetConfig: targetConfigIndex,
      isCrossConfig: isCrossConfigDrop,
      draggedItemCount: currentDragState.draggedItems.length
    })

    // CROSS-CONFIG DROP: Moving items between different config files
    if (isCrossConfigDrop) {
      console.log("🔄 Cross-config drop - moving items between configs")
      
      // Send command to backend to move items between config files
      publishOnMessageExchange().publish({
        key: "CommandResortConfigItem",
        payload: {
          items: currentDragState.draggedItems,           // The items to move
          sourceFileIndex: currentDragState.sourceConfigIndex,  // Where they came from
          targetFileIndex: targetConfigIndex,            // Where they're going
          newIndex: 0  // Always add to the top of the target config for simplicity
        }
      } as CommandResortConfigItem)
      
      return  // Cross-config drops are handled entirely by the backend
    }

    // SAME-CONFIG DROP: Reordering items within the current config
    console.log("📝 Same-config drop - reordering within current config")

    // No-op: item dropped on itself
    if (active.id === over.id) {
      console.log("↩️ Item dropped on itself - no change needed")
      return
    }

    // Get the GUIDs of all selected items (these will be moved together)
    const selectedIds = currentDragState.draggedItems.map(
      (item) => (item as IConfigItem).GUID,
    )

    // Find the original position of the dragged item
    const data = table?.getCoreRowModel().rows.map(row => row.original) ?? []
    const originalIndex = (data as IConfigItem[]).findIndex(
      (item) => item.GUID === active.id,
    )

    // Remove all selected items from the data array
    let newData = (data as IConfigItem[]).filter(
      (item) => !selectedIds.includes(item.GUID),
    )
    
    // Find where to insert the items (position of the drop target)
    const newIndex = newData.findIndex((item) => item.GUID === over.id)

    // Determine insertion position based on drag direction
    // If dragging down: insert after the target
    // If dragging up: insert before the target
    const dragDirectionOffset = newIndex >= originalIndex ? 1 : 0

    // Get the actual items being moved (in their original order)
    const draggedData = (data as IConfigItem[]).filter((item) =>
      selectedIds.includes(item.GUID),
    )

    // Reconstruct the array with items in their new positions
    newData = [
      ...newData.slice(0, newIndex + dragDirectionOffset), // Items before insertion point
      ...draggedData, // The moved items
      ...newData.slice(newIndex + dragDirectionOffset), // Items after insertion point
    ]

    // Update the local state with the new order
    setItems(newData)
    
    // For now, send to backend and let it handle the reordering
    publishOnMessageExchange().publish({
      key: "CommandResortConfigItem", 
      payload: {
        items: currentDragState.draggedItems,
        newIndex: 0,  // Backend will need to calculate proper position
        // Could add more info here to help backend determine position
      }
    } as CommandResortConfigItem)
    
  }, [dragState, currentConfigIndex])

  // Context value that child components can access
  const contextValue: ConfigItemDragContextType = {
    dragState,    // Current drag operation state (null when not dragging)
    table,        // Current table instance
    setTable      // Function for table to register itself
  }

  return (
    // Provide context to child components
    <ConfigItemDragContext.Provider value={contextValue}>
      {/* The actual DnD functionality wrapper */}
      <DndContext
        sensors={sensors}                           // How drag is triggered
        collisionDetection={closestCenter}         // How drop targets are detected
        modifiers={[                               // How drag behavior is modified
          snapToCursor,                            // Drag preview follows cursor
          restrictToVerticalAxis                   // Only allow vertical dragging
        ]}
        onDragStart={handleDragStart}              // What happens when drag starts
        onDragEnd={handleDragEnd}                  // What happens when drag ends
      >
        {children}
      </DndContext>
    </ConfigItemDragContext.Provider>
  )
}