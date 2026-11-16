import { cn } from "@/lib/utils"
import { useSortable } from "@dnd-kit/sortable"
import React, {
  CSSProperties,
  forwardRef,
  useCallback,
  useContext,
} from "react"
import { CSS } from "@dnd-kit/utilities"
import { ConfigItemDragVisualContext } from "@/components/providers/ConfigItemContext"

interface DndTableRowProps extends React.HTMLAttributes<HTMLTableRowElement> {
  "dnd-itemid": string
  ref?: (node: Element | null | undefined) => void
}

export const DndTableRow = forwardRef<HTMLTableRowElement, DndTableRowProps>(
  ({ className, ...props }, ref) => {
    const { draggedItemIds, isInsideTable } = useContext(
      ConfigItemDragVisualContext,
    )

    const { setNodeRef, transform, transition, active, listeners } =
      useSortable({
        id: props["dnd-itemid"],
        data: { type: "row" },
      })

    const combinedRef = useCallback(
      (node: HTMLTableRowElement | null) => {
        setNodeRef(node)
        if (typeof ref === "function") {
          ref(node)
        } else if (ref) {
          ;(ref as React.MutableRefObject<HTMLTableRowElement | null>).current =
            node
        }
      },
      [setNodeRef, ref],
    )

    const dndStyle: CSSProperties = {
      transform: CSS.Transform.toString(transform),
      transition,
      zIndex: 1000,
    }

    const isDragging = draggedItemIds.has(props["dnd-itemid"])
    const isActive = active?.id === props["dnd-itemid"]
    const dragStyle = isDragging
      ? isActive
        ? "opacity-0"
        : "opacity-0 collapse"
      : ""
    const outsideTableStyle =
      !isInsideTable && isDragging ? "opacity-0 collapse" : ""

    return (
      <tr
        style={dndStyle}
        ref={combinedRef}
        {...listeners}
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
