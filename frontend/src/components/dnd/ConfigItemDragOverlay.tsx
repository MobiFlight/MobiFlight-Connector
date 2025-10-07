import { DragOverlay as DndKitDragOverlay } from "@dnd-kit/core"
import { useConfigItemDragContext } from "@/components/providers/DragDropProvider"

export function ConfigItemDragOverlay() {
  const { dragState } = useConfigItemDragContext()

  // Only show overlay when actively dragging
  if (!dragState?.isDragging || !dragState.draggedItems.length) {
    return null
  }

  const firstItem = dragState.draggedItems[0]
  const itemCount = dragState.draggedItems.length

  return (
    <DndKitDragOverlay>
      <div className="bg-background border border-border rounded shadow-lg p-2 min-w-[200px] opacity-95">
        <div className="text-sm font-medium truncate">
          {firstItem.Name || firstItem.GUID}
        </div>
        {itemCount > 1 && (
          <div className="text-xs text-muted-foreground">
            +{itemCount - 1} more items
          </div>
        )}
      </div>
    </DndKitDragOverlay>
  )
}