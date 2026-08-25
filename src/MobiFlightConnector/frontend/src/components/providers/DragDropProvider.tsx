import React, { useCallback, useMemo, useState, useRef } from "react"
import { DragDropProvider } from "@dnd-kit/react"
import { DragOperation, Modifier } from "@dnd-kit/abstract"
import {
  PointerSensor,
  PointerActivationConstraints,
  DragDropManager,
} from "@dnd-kit/dom"
import { SnapToCursor } from "@/lib/dnd-kit/snap-to-cursor"
import { IConfigItem } from "@/types"
import { Table } from "@tanstack/react-table"
import { ConfigItemDragOverlay } from "@/components/dnd/ConfigItemDragOverlay"
import { ConfigItemDragContext } from "./ConfigItemContext"
import { useProjectStoreActions } from "@/stores/projectStore"
import { restrictToBoundingRect } from "../dnd/modifiers/restrictToBottomOfParentElement"
import {
  calculateInsertionIndex,
  executeDrop,
  extractDropContext,
  validateDragEnd,
} from "@/lib/dnd/utilities"
/**
 * The drag state that persists throughout the entire drag operation
 * This data survives tab switches and component re-renders
 */

interface DynamicModifierOptions {
  isInsideTable: boolean
  tableContainerRef: Element | null
}

class DynamicModifier extends Modifier<
  DragDropManager,
  DynamicModifierOptions
> {
  override apply(operation: DragOperation) {
    const { isInsideTable, tableContainerRef } = this.options ?? {}
    let { transform } = operation
    const { shape, target } = operation
    transform = new SnapToCursor(this.manager).apply(operation)
    if (isInsideTable) {
      transform = {
        ...transform,
        x: 0,
      }

      if (!shape || !target?.shape || !tableContainerRef) {
        return transform
      }

      const rect = shape.current.boundingRectangle
      const boundingRect = tableContainerRef.getBoundingClientRect()

      transform = restrictToBoundingRect(transform, rect, boundingRect)
    }
    return transform
  }
}

export interface DragState {
  items: {
    dragItem: IConfigItem | null
    draggedItems: IConfigItem[] // The actual items being dragged (full objects)
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
  const lastValidOverIdRef = useRef<string | null>(null)

  // State: Current drag operation (null when not dragging)
  const [dragState, setDragState] = useState<DragState | null>(null)

  const [tableContainerRef, setTableContainerRefInternal] =
    useState<Element | null>(null)

  // Configure what input methods can trigger drag operations
  const preventDragWhileEditing = (element: Element | null): boolean => {
    let currentElement = element

    // Traverse up the DOM tree
    // to check if any parent element is an interactive control or editable
    while (currentElement) {
      if (
        currentElement.tagName === "INPUT" ||
        currentElement.tagName === "TEXTAREA" ||
        currentElement.tagName === "BUTTON" ||
        currentElement.getAttribute("role") === "switch" ||
        currentElement.getAttribute("role") === "checkbox" ||
        currentElement.getAttribute("contenteditable") === "true"
      ) {
        return true
      }
      currentElement = currentElement.parentElement
    }
    return false
  }

  const configuredPointerSensor = PointerSensor.configure({
    preventActivation: (event) => {
      if (event.button !== 0) {
        return true
      }

      // Don't start drag if modifier keys are pressed
      if (event.ctrlKey || event.shiftKey || event.metaKey) {
        return true
      }

      // Prevent drag if clicking inside an editable element
      if (preventDragWhileEditing(event.target as Element)) {
        return true
      }

      return false
    },
    activationConstraints: [
      new PointerActivationConstraints.Distance({
        // Required distance in pixels
        value: 10,
      }),
    ],
  })

  const setTableContainerRef = useCallback((element: Element | null) => {
    setTableContainerRefInternal(element)
  }, [])

  const { moveItemsBetweenConfigs, restoreItemsToOriginalPositions } =
    useProjectStoreActions()

  /**
   * Called when user starts dragging an item
   * Captures what items are being dragged and from which config
   */

  type DragStartEvent = {
    operation: DragOperation
    nativeEvent?: Event
  }
  const handleDragStart = useCallback(
    (event: DragStartEvent) => {
      lastValidOverIdRef.current = null
      const operation = event.operation
      console.log("🚀 Drag start - Initial config:", initialConfigIndex)
      const id = operation.source?.id
      if (!table) {
        console.warn("No table available for drag start")
        return
      }
      if (id === undefined) {
        return
      }

      // Get all currently selected items from the table
      // These are the items that will be moved as a group
      const selectedRows = table.getSelectedRowModel().rows

      let draggedItems = selectedRows.map((row) => row.original)

      // Special case: If the dragged item isn't already selected, select it
      // This allows single-click-drag without requiring pre-selection
      const draggedRow = table.getRowModel().rows.find((row) => row.id === id)
      if (draggedRow && !draggedRow.getIsSelected()) {
        // Select the dragged item
        table.setRowSelection({ [id]: true })
        // Use just this item as the dragged items
        draggedItems = [draggedRow.original]
      }

      const dragItem = draggedRow ? draggedRow.original : null
      const draggedItemsIds = draggedItems.map((item) => item.GUID)

      const originalPositions = new Map<string, number>()
      table.getRowModel().rows.forEach((row, index) => {
        if (!draggedItemsIds.includes(row.original.GUID)) return
        originalPositions.set(row.original.GUID, index)
      })

      // Create drag state that will persist throughout the operation
      const newDragState: DragState = {
        items: {
          dragItem: dragItem,
          draggedItems: draggedItems,
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
              },
            }
          : null,
      )
    },
    [dragState, moveItemsBetweenConfigs],
  )

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
  type DragEndEvent = {
    operation: DragOperation
    canceled: boolean
    nativeEvent?: Event
  }

  const handleDragEnd = useCallback(
    (event: DragEndEvent) => {
      const { operation, canceled } = event
      if (canceled) {
        handleDragCancel()
        return
      }

      if (operation.canceled) {
        handleDragCancel()
        return
      }

      const activeId = operation.source?.id
      const overId = operation.target?.id

      const effectiveOverId =
  overId === activeId && lastValidOverIdRef.current !== null
    ? lastValidOverIdRef.current
    : overId !== undefined
      ? String(overId)
      : undefined

      console.log("🎯 Drag end target:", {
        activeId,
        overId,
        lastValidOverId: lastValidOverIdRef.current,
        effectiveOverId,
      })

      // Clean up: Always clear drag state when drag ends
      const currentDragState = dragState

      // Validate the drag operation
      const validation = validateDragEnd(
        event,
        currentDragState,
        effectiveOverId,
      )

      if (!validation.isValid) {
        console.log(`❌ ${validation.reason}`)
        // No need to check reason — canceled is already handled in validateDragEnd
        if (event.canceled) {
          handleDragCancel()
        }
        return
      }
      setDragState(null)

      const { dropContext, sourceConfigIndex, targetConfigIndex } =
        extractDropContext(
          event,
          currentDragState!,
          getConfigItems,
          effectiveOverId,
        )

      const isCrossConfig = sourceConfigIndex !== targetConfigIndex

      const insertionIndex = calculateInsertionIndex(
        dropContext,
        isCrossConfig,
        activeId,
      )
      console.log("📍 Insertion calculation:", {
        hoveringOverTab: dropContext.hoveringOverTab,
        dropOnPlaceholder: dropContext.dropOnPlaceholder,
        itemsWithoutDraggedLength: dropContext.itemsWithoutDragged.length,
        insertionIndex,
      })

      // Execute the drop operation
      console.log("🔥 FINAL DROP", {
        sourceConfigIndex,
        targetConfigIndex,
        insertionIndex,
        draggedItems: currentDragState?.items.draggedItems.map(
          (item) => item.GUID,
        ),
        targetItems: dropContext.currentItems.map((item) => item.GUID),
      })
      executeDrop(
        currentDragState!,
        { sourceConfigIndex, targetConfigIndex, insertionIndex },
        moveItemsBetweenConfigs,
      )
      if (isCrossConfig) {
        selectActiveFile(targetConfigIndex)
      }
      console.log(
        "🔥 STORE AFTER DROP",
        getConfigItems(targetConfigIndex).map((item) => item.GUID),
      )
    },
    [
      dragState,
      getConfigItems,
      handleDragCancel,
      moveItemsBetweenConfigs,
      selectActiveFile,
    ],
  )
  type DragMoveEvent = {
    operation: DragOperation
    nativeEvent?: Event
  }
  const handleDragMove = useCallback(
    (event: DragMoveEvent) => {
      if (!dragState) return

      // Collect all state changes first
      const stateUpdates: Partial<DragState> = {}
      const { target } = event.operation

      const hoveringOverTab = target?.data?.type === "tab"
      const hoveredTabIndex = target?.data?.index
      // Move items if hovering over a different tab
      if (
        hoveringOverTab &&
        hoveredTabIndex !== undefined &&
        hoveredTabIndex !== dragState.configs.current
      ) {
        moveItemsToHoveredTab(hoveredTabIndex)
        return
      }
      const previousY = event.operation.position.previous?.y

      if (previousY === undefined) {
        return
      }
      const defaultType =
        target?.data?.type ?? (Math.abs(previousY) < 15 && "row")
      const hoveringOverTable = ["table", "row"].includes(
        target?.data?.type ?? defaultType,
      )

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
            ...dragState.ui,
            hoveredTabIndex,
          }
        }
      } else {
        // Left tab area
        if (dragState.ui.hoveredTabIndex !== -1) {
          console.log("⬅️ Left tab area")
          stateUpdates.ui = {
            ...dragState.ui,
            hoveredTabIndex: -1,
          }
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
    [dragState, moveItemsToHoveredTab],
  )

  // Context value that child components can access
  const contextValue: ConfigItemDragContextType = {
    dragState, // Current drag operation state (null when not dragging)
    table, // Current table instance
    setTable, // Function for table to register itself,
    setTableContainerRef, // Function for table to register its container element
  }

  const modifiers = useMemo(
    () =>
      DynamicModifier.configure({
        isInsideTable: dragState?.ui.isInsideTable ?? false,
        tableContainerRef,
      }),
    [dragState?.ui.isInsideTable, tableContainerRef],
  )
  const handleCollision = useCallback(
    (
      event: Parameters<
        NonNullable<
          React.ComponentProps<typeof DragDropProvider>["onCollision"]
        >
      >[0],
    ) => {
      const collision = event.collisions?.[0]

      if (!collision) return

      const collisionId = collision.id

      const draggedIds =
        dragState?.items.draggedItems.map((item) => item.GUID) ?? []

      console.log("Collision:", collisionId)

      // Ignore the item currently being dragged
      if (draggedIds.includes(String(collisionId))) {
        return
      }

      lastValidOverIdRef.current = String(collisionId)

      console.log("✅ Last valid target:", collisionId)
    },
    [dragState],
  )
  return (
    // Provide context to child components
    <ConfigItemDragContext.Provider value={contextValue}>
      {/* The actual DnD functionality wrapper */}
      <DragDropProvider
        sensors={(defaults) => [
          ...defaults.filter((sensor) => sensor !== PointerSensor),
          configuredPointerSensor,
        ]}
        onCollision={handleCollision}
        modifiers={[modifiers]}
        onDragStart={handleDragStart}
        onDragMove={handleDragMove}
        onDragEnd={handleDragEnd}
      >
        {children}
        <ConfigItemDragOverlay />
      </DragDropProvider>
    </ConfigItemDragContext.Provider>
  )
}
