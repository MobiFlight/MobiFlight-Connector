import { useCallback, useState } from "react"
import {
  closestCenter,
  useSensor,
  useSensors,
  DragEndEvent,
  MouseSensor,
  TouchSensor,
  DragStartEvent,
  Active,
  Over,
} from "@dnd-kit/core"
import {
  restrictToParentElement,
  restrictToVerticalAxis,
} from "@dnd-kit/modifiers"
import { snapToCursor } from "@/lib/dnd-kit/snap-to-cursor"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandResortConfigItem } from "@/types/commands"
import { IConfigItem } from "@/types"
import { Table } from "@tanstack/react-table"

interface UseConfigItemDragDropProps<TData> {
  table: Table<TData> | null
  data: TData[]
  setItems: (items: TData[]) => void
  configIndex: number
}

/**
 * Hook that manages drag-and-drop functionality for config items
 * Handles both same-config reordering and cross-config moves
 * 
 * @param table - The TanStack table instance with selection state
 * @param data - Current config items array
 * @param setItems - Function to update the config items
 * @param configIndex - Index of the current config file being displayed
 */
export function useConfigItemDragDrop<TData>({
  table,
  data,
  setItems,
  configIndex,
}: UseConfigItemDragDropProps<TData>) {
  // Stores the currently dragged item with enhanced data (source config, selected items)
  const [dragItem, setDragItem] = useState<Active | undefined>(undefined)
  
  // Configure drag sensors - what triggers a drag operation
  const sensors = useSensors(
    useSensor(MouseSensor, {}), // Mouse drag support
    useSensor(TouchSensor, {}), // Touch drag support for mobile
  )

  /**
   * Handles the start of a drag operation
   * - Captures currently selected items from the table
   * - Stores source config index for cross-config moves
   * - Ensures the dragged item is selected if it wasn't already
   */
  const handleDragStart = useCallback(
    (event: DragStartEvent) => {
      console.log("Drag start. Active config index", configIndex)

      const { active } = event

      // Get all currently selected items from the table
      // These are the items that will be moved together
      const draggedItems = table?.getSelectedRowModel().rows.map((row) => row.original) ?? []

      console.log("Dragged items", draggedItems)

      // Enhance the active drag item with our custom data
      // This data persists throughout the drag operation even if we switch tabs
      const enhancedActive = {
        ...active,
        data: {
          ...active.data,
          current: {
            ...active.data.current,
            draggedItems: draggedItems, // Store the actual items being dragged
            sourceConfigIndex: configIndex, // Remember which config we started from
          },
        },
      }

      // Store the enhanced active item in our state
      setDragItem(enhancedActive)

      if (!table) return

      // Find the row that's being dragged
      const draggedRow = table
        .getRowModel()
        .rows.find((row) => row.id === active.id)
      if (!draggedRow) return
      
      // If the dragged item isn't selected, select it
      // This ensures single-click-drag works even without pre-selection
      if (!draggedRow.getIsSelected()) {
        table.setRowSelection({ [active.id]: true })
      }
    },
    [table, configIndex], // Re-create callback when table or config changes
  )

  /**
   * Handles dropping items across different config files
   * - Uses stored draggedItems (not current table selection, which may be lost)
   * - Publishes command to backend to move items between configs
   * 
   * @param active - The dragged item
   * @param over - The drop target
   * @param sourceConfigIndex - Config index where drag started
   * @param targetConfigIndex - Config index where items are being dropped
   */
  const handleCrossConfigDrop = useCallback(
    (
      active: Active,
      over: Over,
      sourceConfigIndex: number,
      targetConfigIndex: number,
    ) => {
      if (!table) return

      console.log(dragItem)

      // Get the items from our stored drag data (not from current table selection)
      // This is crucial because table selection is lost when switching tabs
      const draggedItems = dragItem?.data.current?.draggedItems ?? []

      console.log("Cross-config drop:", {
        draggedItems,
        sourceConfigIndex,
        targetConfigIndex,
      })

      // Send command to backend to move items between config files
      publishOnMessageExchange().publish({
        key: "CommandResortConfigItem",
        payload: {
          items: draggedItems, // The actual items to move
          newIndex: 0, // Always add to the top of the target config
          sourceFileIndex: sourceConfigIndex, // Where they came from
          targetFileIndex: targetConfigIndex, // Where they're going
        },
      })
    },
    [table, dragItem], // Include dragItem dependency to access stored data
  )

  /**
   * Handles the end of a drag operation
   * - Determines if this is a cross-config move or same-config reorder
   * - For cross-config: delegates to handleCrossConfigDrop
   * - For same-config: updates local data array and publishes reorder command
   */
  const handleDragEnd = useCallback(
    (event: DragEndEvent) => {
      const { active, over } = event

      // Bail out if drag was cancelled or incomplete
      if (!over?.id || !active.id || !table) {
        setDragItem(undefined)
        return
      }

      // Determine source and target config indices
      // Source: from our stored drag data (reliable across tab switches)
      // Target: from the drop target's data, fallback to current config
      const sourceConfigIndex =
        dragItem?.data.current?.sourceConfigIndex ?? active.data.current?.sourceConfigIndex
      const targetConfigIndex = over.data.current?.configIndex ?? configIndex

      console.log("Drag end:", {
        dragItem,
        overId: over.id,
      })

      // CROSS-CONFIG DROP: Moving items between different config files
      if (sourceConfigIndex !== targetConfigIndex) {
        handleCrossConfigDrop(
          active,
          over,
          sourceConfigIndex,
          targetConfigIndex,
        )
        setDragItem(undefined) // Clean up drag state
        return
      }

      // SAME-CONFIG DROP: Reordering items within the same config file

      // No-op: item dropped on itself
      if (active.id === over.id) {
        setDragItem(undefined)
        return
      }

      // Get the GUIDs of all selected items (these will be moved together)
      const selectedRows = table.getSelectedRowModel().rows
      const selectedIds = selectedRows.map(
        (row) => (row.original as IConfigItem).GUID,
      )

      // Find the original position of the dragged item
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

      // Send reorder command to backend
      publishOnMessageExchange().publish({
        key: "CommandResortConfigItem",
        payload: {
          items: draggedData, // The items that were moved
          newIndex: newIndex + dragDirectionOffset, // Their new position
        },
      } as CommandResortConfigItem)

      // Clean up drag state
      setDragItem(undefined)
    },
    [data, setItems, table, configIndex, handleCrossConfigDrop, dragItem], // All dependencies
  )

  // Return the drag state and configuration for DndContext
  return {
    dragItem, // Current drag state (for UI feedback)
    sensors, // Drag activation sensors
    handleDragStart, // Drag start handler
    handleDragEnd, // Drag end handler
    dndContextProps: {
      sensors,
      collisionDetection: closestCenter, // How to detect drop targets
      modifiers: [
        snapToCursor, // Drag preview follows cursor
        restrictToVerticalAxis, // Only allow vertical dragging
        // restrictToParentElement, // Commented out to allow cross-tab drops
      ],
      onDragEnd: handleDragEnd,
      onDragStart: handleDragStart,
    },
  }
}