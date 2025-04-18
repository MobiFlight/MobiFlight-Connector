import { TableBody, TableCell } from "@/components/ui/table"
import { SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable"
import { flexRender, Row, Table } from "@tanstack/react-table"
import { DndTableRow } from "../DndTableRow"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { cn } from "@/lib/utils"
import { useState, useEffect } from "react"
import { IConfigItem } from "@/types/config"
import { Badge } from "@/components/ui/badge"

interface ConfigItemTableBodyProps<TData> {
  table: Table<TData>,
  firstDragItemId: string | undefined
  onDeleteSelected?: () => void
  onToggleSelected?: () => void
}
const ConfigItemTableBody = <TData,>({
  table,
  firstDragItemId,
  onDeleteSelected,
  onToggleSelected
}: ConfigItemTableBodyProps<TData>) => {
  const { publish } = publishOnMessageExchange()
  const rows = table.getRowModel().rows
  const [lastSelected, setLastSelected] = useState<Row<TData> | null>(null)

  const selectedRows = table.getSelectedRowModel().rows

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      const selectedRows = table.getSelectedRowModel().rows
      const selectedConfigs = selectedRows.map((row) => row.original) as IConfigItem[]

      const supportedKeyPress = [ "Delete", " ", "Enter", "Escape", "Backspace" ]	

      if (!supportedKeyPress.includes(e.key)) return

      if (selectedConfigs.length === 0) return
      e.preventDefault() // Prevent default spacebar behavior (scrolling)

      switch (e.key) {
        case "Backspace":
        case "Delete":
          if (onDeleteSelected) onDeleteSelected()
          break

        case " ":
          if (onToggleSelected) onToggleSelected()
          break
        
        case "Escape":
          table.resetRowSelection()
          break
      }
    }

    window.addEventListener("keydown", handleKeyDown)
    return () => window.removeEventListener("keydown", handleKeyDown)
  })

  return (
    <TableBody className="dark:bg-zinc-900">
      <SortableContext
        items={rows.map((row) => row.id)}
        strategy={verticalListSortingStrategy}
      >
        {rows.map((row) => {
          const isSelected = row.getIsSelected()
          const isDragging = firstDragItemId !== undefined
          const isFirstDragItem = firstDragItemId === row.id
          const dragClassName = isDragging && isSelected && !isFirstDragItem ? "bg-muted opacity-10 is-dragging" : "is-first-drag-item"
          return (
            <DndTableRow
              className={dragClassName}
              key={row.id}
              data-state={row.getIsSelected() && "selected"}
              dnd-itemid={row.id}
              onClick={(e): void => {
                e.stopPropagation()
                if (e.shiftKey) {
                  if (lastSelected) {
                    const lastIndex = rows.findIndex(
                      (r) => r.id === lastSelected.id,
                    )
                    const currentIndex = rows.findIndex((r) => r.id === row.id)
                    const range = [lastIndex, currentIndex].sort(
                      (a, b) => a - b,
                    )
                    for (let i = range[0]; i <= range[1]; i++) {
                      const row = rows[i]
                      if (row.getIsSelected()) continue
                      rows[i].toggleSelected()
                    }
                  } else {
                    row.toggleSelected()
                  }
                } else if (e.ctrlKey || e.metaKey) {
                  row.toggleSelected()
                } else {
                  table.resetRowSelection()
                  row.toggleSelected()
                }
                setLastSelected(row)
              }}
              onDoubleClick={() => {
                publish({
                  key: "CommandConfigContextMenu",
                  payload: { action: "edit", item: row.original },
                })
              }}
            >
              {row.getVisibleCells().map((cell, idx, arr) => {
                const className =
                  (
                    cell.column.columnDef.meta as {
                      className: string
                    }
                  )?.className ?? ""

                const cellClassName =
                  (
                    cell.column.columnDef.meta as {
                      cellClassName: string
                    }
                  )?.cellClassName ?? ""

                // Render badge inside the last cell if this is the first drag item
                const isLastCell = idx === arr.length - 1

                return (
                  <TableCell
                    key={cell.id}
                    className={cn("p-1", className, cellClassName, "group-[.is-dragging]/row:hidden")}
                  >
                    {!isLastCell && flexRender(cell.column.columnDef.cell, cell.getContext())}
                    {isFirstDragItem && isLastCell && 
                      <Badge>{selectedRows.length}</Badge>
                    }
                  </TableCell>
                )
              })}
            </DndTableRow>
          )
        })}
      </SortableContext>
    </TableBody>
  )
}

export default ConfigItemTableBody
