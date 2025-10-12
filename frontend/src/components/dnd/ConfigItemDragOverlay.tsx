import { DragOverlay } from "@dnd-kit/core"
import { useConfigItemDragContext } from "@/lib/hooks/useConfigItemDragContext"
import ConfigItemTabDragOverlay from "./ConfigItemTabDragOverlay"
import { cn } from "@/lib/utils"
import { Badge } from "../ui/badge"
import { IconExclamationCircle } from "@tabler/icons-react"

export function ConfigItemDragOverlay() {
  const { dragState } = useConfigItemDragContext()

  // Simple boolean checks instead of complex object initialization
  const isDraggingWithItems =
    dragState?.ui.isDragging && (dragState?.items.draggedItems?.length ?? 0) > 0
  const isInsideTable = dragState?.ui.isInsideTable ?? true

  if (!isDraggingWithItems) {
    return null
  }

  const itemCount = dragState?.items.draggedItems.length ?? 0

  return (
    isDraggingWithItems && (
      <DragOverlay className="cursor-grabbing">
        <div>
          {isInsideTable && itemCount > 1 ? (
            <div
              data-testid="config-item-drag-overlay-inside-table"
              className={cn(
                "border-border bg-background relative flex items-center justify-between rounded-lg border px-4 py-3",
                // Add stacked effect for multiple items using pseudo-elements
                itemCount > 1 && [
                  "before:bg-background before:border-border before:absolute before:inset-0 before:-z-10 before:translate-y-1 before:rounded-md before:border",
                  itemCount > 2 &&
                    "after:bg-background after:border-border after:absolute after:inset-0 after:-z-20 after:translate-y-2 after:rounded-md after:border",
                ],
                "after:shadow-lg",
              )}
            >
              <div>
                <Badge className="ml-6 px-4 text-xs">{itemCount}</Badge>
                <span className="px-7 font-medium">
                  You are moving {itemCount} item(s)
                </span>
              </div>
              <div className="text-muted-foreground ml-4 flex items-center gap-2 text-sm">
                <IconExclamationCircle className="stroke-muted-foreground/80 h-6 w-6" />
                You can drag & drop items here in the list or move them to other tabs
              </div>
            </div>
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
