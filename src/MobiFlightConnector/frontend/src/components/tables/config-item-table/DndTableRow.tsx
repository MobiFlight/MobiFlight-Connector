import { cn } from "@/lib/utils"
import { useSortable } from "@dnd-kit/react/sortable"
import React, { forwardRef, useCallback, useContext } from "react"
import { ConfigItemDragVisualContext } from "@/components/providers/ConfigItemContext"
import { untransformedPointerIntersection } from "@/lib/dnd-kit/untransformed-pointer-intersection"

interface DndTableRowProps extends React.HTMLAttributes<HTMLTableRowElement> {
  "dnd-itemid": string
  ref?: (node: Element | null | undefined) => void
  index: number
  shiftY?: number
}

export const DndTableRow = forwardRef<HTMLTableRowElement, DndTableRowProps>(
  ({ index, shiftY = 0, className, ...props }, ref) => {
    const { draggedItemIds, isInsideTable, dragItemId } = useContext(
      ConfigItemDragVisualContext,
    )

    const { ref: sortableRef } = useSortable({
      id: props["dnd-itemid"],
      data: { type: "row" },
      index: index,
      plugins: [],
      collisionDetector: untransformedPointerIntersection,
    })

    const combinedRef = useCallback(
      (node: HTMLTableRowElement | null) => {
        sortableRef(node)
        if (typeof ref === "function") {
          ref(node)
        } else if (ref) {
          ;(ref as React.MutableRefObject<HTMLTableRowElement | null>).current =
            node
        }
      },
      [sortableRef, ref],
    )

    const dndStyle: React.CSSProperties = {
      transform: shiftY ? `translate3d(0, ${shiftY}px, 0)` : undefined,
      transition: dragItemId ? "transform 200ms ease" : undefined,
    }

    const isDragging = draggedItemIds.has(props["dnd-itemid"])
    const isActive = dragItemId === props["dnd-itemid"]
    const dragStyle = isDragging
      ? isActive
        ? "opacity-0"
        : "opacity-0 collapse"
      : ""
    const outsideTableStyle =
      !isInsideTable && isDragging ? "opacity-0 collapse" : ""

    return (
      <tr
        role="row"
        style={dndStyle}
        ref={combinedRef}
        className={cn(
          "group/row bg-background hover:bg-selected/45 data-[state=selected]:bg-selected/45 data-[state=selected]:hover:bg-selected dark:data-[state=selected]:bg-selected/45 dark:data-[state=selected]:hover:bg-selected border-b transition-colors",
          dragStyle,
          outsideTableStyle,
          className,
        )}
        {...props}
      />
    )
  },
)

DndTableRow.displayName = "DndTableRow"
