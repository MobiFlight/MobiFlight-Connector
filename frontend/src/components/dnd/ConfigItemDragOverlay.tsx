import { DragOverlay } from "@dnd-kit/core"
import { useConfigItemDragContext } from "@/lib/hooks/useConfigItemDragContext"
import ConfigItemTabDragOverlay from "./ConfigItemTabDragOverlay"
import { cn } from "@/lib/utils"
import ConfigItemTableDragOverlay from "./ConfigItemTableDragOverlay"

export function ConfigItemDragOverlay() {
  const { dragState } = useConfigItemDragContext()

  // Simple boolean checks instead of complex object initialization
  const isDraggingWithItems =
    dragState?.ui.isDragging && (dragState?.items.draggedItems?.length ?? 0) > 0
  const isInsideTable = dragState?.ui.isInsideTable ?? true

  if (!isDraggingWithItems) {
    return null
  }

  return (
    isDraggingWithItems && (
      <DragOverlay className="cursor-grabbing">
        <div>
          {isInsideTable ? (
            <ConfigItemTableDragOverlay 
              data-testid="config-item-drag-overlay-inside-table" 
              items={dragState?.items.draggedItems ?? []} 
            />
          ) : (
            <ConfigItemTabDragOverlay
              data-testid="config-item-drag-overlay-outside-table"
              className={cn(
                "transition-all duration-300 ease-in-out",
                isInsideTable ? "scale-0 opacity-0" : "opacity-100",
              )}
              items={dragState?.items.draggedItems ?? []}
            />
          )}
        </div>
      </DragOverlay>
    )
  )
}
