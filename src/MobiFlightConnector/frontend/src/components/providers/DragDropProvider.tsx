import React, { useCallback, useEffect, useMemo, useState } from "react"
import { flushSync } from "react-dom"
// Note: the library's DragDropProvider is not this file's ConfigItemDragProvider
import { DragDropProvider } from "@dnd-kit/react"
import type { DragStartEvent, DragEndEvent, DragOverEvent } from "@dnd-kit/dom"
import { IConfigItem } from "@/types"
import { Table } from "@tanstack/react-table"
import { ConfigItemDragOverlay } from "@/components/dnd/ConfigItemDragOverlay"
import {
  ConfigItemDragContext,
  ConfigItemDragVisualContext,
} from "./ConfigItemContext"
import { useProjectStoreActions } from "@/stores/projectStore"
import {
  calculateInsertionIndex,
  executeDrop,
  extractDropContext,
  validateDragEnd,
} from "@/lib/dnd/utilities"
import { CustomPointerSensor } from "@/lib/dnd-kit/CustomPointerSensor"
import { SnapToCursor } from "@/lib/dnd-kit/snap-to-cursor"
import { RestrictToBottomOfParentElement } from "@/components/dnd/modifiers/restrictToBottomOfParentElement"
import { RestrictToVerticalAxisInsideTable } from "@/lib/dnd-kit/restrict-to-vertical-axis-inside-table"
import { configure } from "@dnd-kit/abstract"
/**
 * The drag state that persists throughout the entire drag operation
 * This data survives tab switches and component re-renders
 */
export interface DragState {
  items: {
    dragItem: IConfigItem | null
    draggedItems: IConfigItem[] // The actual items being dragged (full objects)
    draggedItemIds: ReadonlySet<string>
    originalPositions: Map<string, number> // GUID -> original index
  }

  configs: {
    source: number // Which config file the drag started from, needed for restore
    current: number // Which config file the drag is currently over
  }

  ui: {
    isDragging: boolean // Whether a drag is currently active
    isInsideTable: boolean // Whether the drag is currently over a valid table
    hoveredTabIndex: number // If dragging over a tab, which tab index
    overItemId: string | null
    activeTabIndex?: number // If drag ended over a tab, which tab index
  }
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

export interface ConfigItemDragVisualContextType {
  draggedItemIds: ReadonlySet<string>
  isInsideTable: boolean
  dragItemId: string | null
  overItemId: string | null
}

const EMPTY_DRAGGED_ITEM_IDS: ReadonlySet<string> = new Set()

// How long a drag must rest on a tab before that tab opens
const TAB_HOVER_DELAY_MS = 600

// One pointer sensor covers mouse, touch and pen.
// Defined once so the provider receives a stable array.
const SENSORS = [CustomPointerSensor]
/**
 * Props for the drag provider component
 */
interface ConfigItemDragProviderProps {
  children: React.ReactNode
  initialConfigIndex: number
  // Function to update config items in the project store
  // This makes drag-drop independent of table implementation
  updateConfigItems: (configIndex: number, items: IConfigItem[]) => void
  // Function to get current config items from project store
  getConfigItems: (configIndex: number) => IConfigItem[]
  selectActiveFile: (index: number) => void
}

/**
 * Provider component that manages ALL drag-and-drop logic
 */
export function ConfigItemDragProvider({
  children,
  initialConfigIndex,
  getConfigItems,
  selectActiveFile,
}: ConfigItemDragProviderProps) {
  // State: Current table instance (set by ConfigItemTable when it mounts)
  const [table, setTable] = useState<Table<IConfigItem> | null>(null)

  // State: Current drag operation (null when not dragging)
  const [dragState, setDragState] = useState<DragState | null>(null)

  const [tableContainerRef, setTableContainerRefInternal] =
    useState<Element | null>(null)

  const setTableContainerRef = useCallback((element: Element | null) => {
    setTableContainerRefInternal(element)
  }, [])

  const { moveItemsBetweenConfigs, restoreItemsToOriginalPositions } =
    useProjectStoreActions()

  const modifiers = useMemo(
    () => [
      SnapToCursor,
      RestrictToVerticalAxisInsideTable,
      configure(RestrictToBottomOfParentElement, {
        element: tableContainerRef,
      }),
    ],
    [tableContainerRef],
  )

  /**
   * Called when user starts dragging an item
   * Captures what items are being dragged and from which config
   */
  const handleDragStart = useCallback(
    (event: DragStartEvent) => {
      console.log("🚀 Drag start - Initial config:", initialConfigIndex)

      if (!table) {
        console.warn("No table available for drag start")
        return
      }

      // Get all currently selected items from the table
      // These are the items that will be moved as a group
      const selectedRows = table.getSelectedRowModel().rows
      let draggedItems = selectedRows.map((row) => row.original)

      // Special case: If the dragged item isn't already selected, select it
      // This allows single-click-drag without requiring pre-selection
      const sourceId = event.operation.source?.id
      const draggedRow = table
        .getRowModel()
        .rows.find((row) => row.id === sourceId)
      if (draggedRow && !draggedRow.getIsSelected()) {
        // Select the dragged item
        table.setRowSelection({ [draggedRow.id]: true })
        // Use just this item as the dragged items
        draggedItems = [draggedRow.original]
      }

      const dragItem = draggedRow ? draggedRow.original : null
      const draggedItemIds = new Set(draggedItems.map((item) => item.GUID))

      const originalPositions = new Map<string, number>()
      table.getRowModel().rows.forEach((row, index) => {
        if (!draggedItemIds.has(row.original.GUID)) return
        originalPositions.set(row.original.GUID, index)
      })

      // Create drag state that will persist throughout the operation
      const newDragState: DragState = {
        items: {
          dragItem: dragItem,
          draggedItems: draggedItems,
          draggedItemIds,
          originalPositions: originalPositions,
        },

        configs: {
          source: initialConfigIndex,
          current: initialConfigIndex,
        },

        ui: {
          isDragging: true,
          isInsideTable: true,
          hoveredTabIndex: -1,
          overItemId: null,
          activeTabIndex: initialConfigIndex,
        },
      }

      setDragState(newDragState)

      console.log("📋 Dragging items:", {
        count: newDragState.items.draggedItems.length,
        items: newDragState.items.draggedItems.map(
          (item) => item.Name || item.GUID,
        ),
        sourceConfig: newDragState.configs.source,
      })
    },
    [table, initialConfigIndex],
  )

  const moveItemsToHoveredTab = useCallback(
    (hoveredTabIndex: number) => {
      if (!dragState) return
      moveItemsBetweenConfigs(
        dragState.items.draggedItems,
        dragState.configs.current,
        hoveredTabIndex,
        0,
      )
      setDragState((prev) =>
        prev
          ? {
              ...prev,
              configs: {
                ...prev.configs,
                current: hoveredTabIndex,
              },
              ui: {
                ...prev.ui,
                hoveredTabIndex,
                isInsideTable: false,
                overItemId: null,
              },
            }
          : null,
      )
    },
    [dragState, moveItemsBetweenConfigs],
  )

  const hoveredTabIndex = dragState?.ui.hoveredTabIndex ?? -1
  const currentConfigIndex = dragState?.configs.current ?? -1

  // Move the items and open the tab together, so the items are never in a tab
  // the user cannot see. Leaving the tab or ending the drag clears the timer.
  useEffect(() => {
    if (hoveredTabIndex === -1 || hoveredTabIndex === currentConfigIndex) return

    const timeout = setTimeout(() => {
      moveItemsToHoveredTab(hoveredTabIndex)
      selectActiveFile(hoveredTabIndex)
    }, TAB_HOVER_DELAY_MS)

    return () => clearTimeout(timeout)
  }, [
    hoveredTabIndex,
    currentConfigIndex,
    moveItemsToHoveredTab,
    selectActiveFile,
  ])

  /**
   * Simplified drag cancellation - restore items to initial config
   */
  const handleDragCancel = useCallback(() => {
    const currentDragState = dragState

    console.log("❌ Drag cancelled")
    setDragState(null)

    if (!currentDragState) return

    console.log("🔄 Restoring items to original positions:", {
      from: currentDragState.configs.current,
      to: currentDragState.configs.source,
      itemCount: currentDragState.items.draggedItems.length,
    })

    // Single store operation that handles everything
    restoreItemsToOriginalPositions(
      currentDragState.items.draggedItems,
      currentDragState.configs.current,
      currentDragState.configs.source,
      currentDragState.items.originalPositions,
    )

    // Switch back to original tab first
    selectActiveFile(currentDragState.configs.source)
  }, [dragState, restoreItemsToOriginalPositions, selectActiveFile])

  /**
   * Called when user drops the dragged items
   * Universal handler for both same-config and cross-config drops
   * Works directly with project store, independent of table implementation
   */
  const handleDragEnd = useCallback(
    (event: DragEndEvent) => {
      const { source, target } = event.operation

      console.log("🎯 Drag end:", {
        sourceId: source?.id,
        targetId: target?.id,
        canceled: event.canceled,
        dragState,
      })

      flushSync(() => {
        // Escape (or any other cancel) arrives here with the canceled flag
        if (event.canceled) {
          handleDragCancel()
          return
        }

        // Clean up: Always clear drag state when drag ends
        const currentDragState = dragState
        setDragState(null)

        // Validate the drag operation
        const validation = validateDragEnd(event, currentDragState)
        if (!validation.isValid) {
          console.log(`❌ ${validation.reason}`)
          if (validation.reason === "Dropped outside valid zone") {
            handleDragCancel()
          }
          return
        }

        // Extract drop context and config information
        const { dropContext, sourceConfigIndex, targetConfigIndex } =
          extractDropContext(event, currentDragState!, getConfigItems)

        // Calculate where to insert the items
        const insertionIndex = calculateInsertionIndex(dropContext)

        console.log("📍 Insertion calculation:", {
          hoveringOverTab: dropContext.hoveringOverTab,
          dropOnPlaceholder: dropContext.dropOnPlaceholder,
          itemsWithoutDraggedLength: dropContext.itemsWithoutDragged.length,
          insertionIndex,
        })

        // Execute the drop operation
        executeDrop(
          currentDragState!,
          { sourceConfigIndex, targetConfigIndex, insertionIndex },
          moveItemsBetweenConfigs,
        )
      })
    },
    [dragState, getConfigItems, handleDragCancel, moveItemsBetweenConfigs],
  )

  const handleDragOver = useCallback(
    (event: DragOverEvent) => {
      if (!dragState || !tableContainerRef) return

      // Collect all state changes first
      const stateUpdates: Partial<DragState> = {}

      const target = event.operation.target
      const hoveringOverTab = target?.data?.type === "tab"
      const hoveringOverTable = ["table", "row"].includes(target?.data?.type)

      if (hoveringOverTable && !dragState.ui.isInsideTable) {
        console.log("➡️ Entered table area")
        stateUpdates.ui = {
          ...dragState.ui,
          isInsideTable: true,
        }
      } else if (!hoveringOverTable && dragState.ui.isInsideTable) {
        console.log("⬅️ Left table area")
        stateUpdates.ui = {
          ...dragState.ui,
          isInsideTable: false,
        }
      }

      // Only update UI state - no store operations here
      if (hoveringOverTab) {
        const hoveredTabIndex = target?.data?.index

        if (hoveredTabIndex !== dragState.ui.hoveredTabIndex) {
          console.log("🎯 Tab hover detected:", hoveredTabIndex)
          stateUpdates.ui = {
            ...(stateUpdates.ui ?? dragState.ui),
            hoveredTabIndex,
          }
        }
      } else {
        // Left tab area
        if (dragState.ui.hoveredTabIndex !== -1) {
          console.log("⬅️ Left tab area")
          stateUpdates.ui = {
            ...(stateUpdates.ui ?? dragState.ui),
            hoveredTabIndex: -1,
          }
        }
      }

      const overItemId =
        target?.data?.type === "row" ? String(target.id) : null
      if (overItemId !== dragState.ui.overItemId) {
        stateUpdates.ui = {
          ...(stateUpdates.ui ?? dragState.ui),
          overItemId,
        }
      }

      // Single state update at the end
      if (Object.keys(stateUpdates).length > 0) {
        setDragState((prev) =>
          prev
            ? {
                ...prev,
                ...stateUpdates,
              }
            : null,
        )
      }
    },
    [dragState, tableContainerRef],
  )

  // Context value that child components can access
  const contextValue: ConfigItemDragContextType = {
    dragState, // Current drag operation state (null when not dragging)
    table, // Current table instance
    setTable, // Function for table to register itself,
    setTableContainerRef, // Function for table to register its container element
  }

  const draggedItemIds =
    dragState?.items.draggedItemIds ?? EMPTY_DRAGGED_ITEM_IDS
  const dragItemId = dragState?.items.dragItem?.GUID ?? null
  const overItemId = dragState?.ui.overItemId ?? null
  const dragVisualContextValue = useMemo(
    () => ({
      draggedItemIds,
      isInsideTable: dragState?.ui.isInsideTable ?? true,
      dragItemId,
      overItemId,
    }),
    [draggedItemIds, dragState?.ui.isInsideTable, dragItemId, overItemId],
  )

  return (
    // Provide context to child components
    <ConfigItemDragContext.Provider value={contextValue}>
      <ConfigItemDragVisualContext.Provider value={dragVisualContextValue}>
        {/* The actual DnD functionality wrapper */}
        <DragDropProvider
          sensors={SENSORS}
          modifiers={modifiers}
          onDragStart={handleDragStart}
          onDragOver={handleDragOver}
          onDragEnd={handleDragEnd}
        >
          {children}
          <ConfigItemDragOverlay />
        </DragDropProvider>
      </ConfigItemDragVisualContext.Provider>
    </ConfigItemDragContext.Provider>
  )
}
