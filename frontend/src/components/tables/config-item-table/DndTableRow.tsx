import { cn } from "@/lib/utils"
import { useSortable } from "@dnd-kit/sortable"
import React, { CSSProperties } from "react"
import { CSS } from "@dnd-kit/utilities"
import { useConfigItemDragContext } from "@/lib/hooks/useConfigItemDragContext"

interface DndTableRowProps extends React.HTMLAttributes<HTMLTableRowElement> {
  "dnd-itemid": string
}

export const DndTableRow: React.FC<DndTableRowProps> = ({
  className,
  ...props
}) => {
  const { dragState } = useConfigItemDragContext()

  const { setNodeRef, transform, transition, active, listeners } = useSortable({
    id: props["dnd-itemid"],
    data: { type: "row" },
  })

  const dndStyle: CSSProperties = {
    transform: CSS.Transform.toString(transform),
    transition,
    zIndex: 1000,
  }

  const isDragging =
    dragState?.items?.draggedItems
      ?.map((item) => item.GUID)
      .includes(props["dnd-itemid"]) ?? false
  const isActive = active?.id === props["dnd-itemid"]
  const isInTable = dragState?.ui.isInsideTable ?? true
  const dragStyle = isDragging
    ? isActive
      ? "opacity-0"
      : "opacity-0 collapse"
    : ""
  const outsideTableStyle = !isInTable && isDragging ? "opacity-0 collapse" : ""

  return (
    <tr
      style={dndStyle}
      ref={setNodeRef}
      {...listeners}
      className={cn(
        "group/row bg-background hover:bg-selected data-[state=selected]:bg-selected data-[state=selected]:hover:bg-selected dark:data-[state=selected]:bg-selected dark:data-[state=selected]:hover:bg-selected border-b transition-colors",
        dragStyle,
        outsideTableStyle,
        className,
      )}
      {...props}
    />
  )
}

DndTableRow.displayName = "DndTableRow"
