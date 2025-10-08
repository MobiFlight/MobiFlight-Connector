import { DragOverlay } from "@dnd-kit/core"
import { useConfigItemDragContext } from "@/lib/hooks/useConfigItemDragContext"
import ConfigItemTableDragOverlay from "./ConfigItemTableDragOverlay"
import ConfigItemTabDragOverlay from "./ConfigItemTabDragOverlay"
import { cn } from "@/lib/utils"

export function ConfigItemDragOverlay() {
  const { dragState: rawDragState } = useConfigItemDragContext()

  const dragState = rawDragState ?? { isDragging: false, draggedItems: [], isInsideTable: true }

  const isDraggingWithItems = dragState?.isDragging && dragState?.draggedItems.length > 0
  const itemCount = dragState.draggedItems.length
  const firstItem = itemCount > 0 ? dragState.draggedItems[0] : null
  const isInsideTable = dragState.isInsideTable

  return isDraggingWithItems && (
    <DragOverlay className="cursor-grabbing">
      <div>
        <ConfigItemTableDragOverlay
          className={cn(
            "transition-all duration-100 ease-in-out",
            !isInsideTable ? "opacity-0 scale-0" : "opacity-100",
          )}
          firstItem={firstItem}
          itemCount={itemCount}
        />
        <ConfigItemTabDragOverlay
          className={cn(
            "transition-all duration-300 ease-in-out",
            isInsideTable ? "opacity-0 scale-0" : "opacity-100",
          )}
          items={dragState.draggedItems}
        />
      </div>
    </DragOverlay>
  )
}
