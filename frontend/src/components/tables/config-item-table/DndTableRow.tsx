import { cn } from "@/lib/utils"
import { useSortable } from "@dnd-kit/sortable"
import React, { CSSProperties } from "react"
import { CSS } from "@dnd-kit/utilities"

interface DndTableRowProps extends React.HTMLAttributes<HTMLTableRowElement> {
  "dnd-itemid": string
}

export const DndTableRow : React.FC<DndTableRowProps> = (({ className, ...props }) => {
  const {
    setNodeRef,
    transform,
    transition,
  } = useSortable({ id: props["dnd-itemid"] })

  const dndStyle: CSSProperties = {
    transform: CSS.Transform.toString(transform),
    transition,
    zIndex: 1000,
  }

  return (
    <tr
      style={dndStyle}
      ref={setNodeRef}
      className={cn(
        "group/row border-b transition-colors hover:bg-muted/80 data-[state=selected]:bg-muted",
        className,
      )}
      {...props}
    />
  )
})

DndTableRow.displayName = "DndTableRow"