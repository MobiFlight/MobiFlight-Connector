import { DragOverlay as DndKitDragOverlay } from "@dnd-kit/core"
import { useConfigItemDragContext } from "@/components/providers/DragDropProvider"
import ConfigItemTableDragOverlay from "./ConfigItemTableDragOverlay"
import ConfigItemTabDragOverlay from "./ConfigItemTabDragOverlay"

export function ConfigItemDragOverlay() {
  const { dragState: rawDragState } = useConfigItemDragContext()

  const dragState = rawDragState ?? { isDragging: false, draggedItems: [], isInsideTable: true }

  const isDraggingWithItems = dragState?.isDragging && dragState?.draggedItems.length > 0
  const itemCount = dragState.draggedItems.length
  const firstItem = itemCount > 0 ? dragState.draggedItems[0] : null
  const isInsideTable = dragState.isInsideTable

  console.log(`🛟 Rendering DragOverlay - isDragging: ${dragState.isDragging}, itemCount: ${itemCount}, isInsideTable: ${isInsideTable}`)

  return isDraggingWithItems && (
    <DndKitDragOverlay>
      {isInsideTable ? (
        <ConfigItemTableDragOverlay
          firstItem={firstItem}
          itemCount={itemCount}
        />
      ) : (
        <ConfigItemTabDragOverlay items={dragState.draggedItems} />
      )}
    </DndKitDragOverlay>
  )
}
