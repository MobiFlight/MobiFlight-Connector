import { cn } from "@/lib/utils"
import { useSortable } from "@dnd-kit/react/sortable"
import React, { CSSProperties } from "react"
import { useConfigItemDragContext } from "@/lib/hooks/useConfigItemDragContext"

import { RowInteractionProvider } from "./RowInteractionContext"

interface DndTableRowProps extends React.HTMLAttributes<HTMLTableRowElement> {
  "dnd-itemid": string
  "dnd-index": number
}

export const DndTableRow: React.FC<DndTableRowProps> = ({
  className,
  children,
  ...props
}) => {
  const { dragState } = useConfigItemDragContext()

  const isSelectedDragging =
    dragState?.items?.draggedItems
      ?.map((item) => item.GUID)
      .includes(props["dnd-itemid"]) ?? false

  const { ref } = useSortable({
    id: props["dnd-itemid"],
    index: props["dnd-index"],
    data: { type: "row" },
  })

  const dndStyle: CSSProperties = {
    zIndex: 1000,
  }

  const isInTable = dragState?.ui.isInsideTable ?? true

  const dragStyle = isSelectedDragging
    ? "opacity-0"
    : ""

  const outsideTableStyle =
    !isInTable && isSelectedDragging ? "opacity-0 collapse" : ""

  return (
    <RowInteractionProvider>
      <tr
        {...props}
        style={dndStyle}
        role="row"
        ref={ref}
        className={cn(
          "group/row bg-background hover:bg-muted/50 data-[state=selected]:bg-selected/45 data-[state=selected]:hover:bg-selected dark:data-[state=selected]:bg-selected/45 dark:data-[state=selected]:hover:bg-selected border-b cursor-grab active:cursor-grabbing",
          dragStyle,
          outsideTableStyle,
          className,
        )}
      >
        {children}
      </tr>
    </RowInteractionProvider>
  )
}

DndTableRow.displayName = "DndTableRow"