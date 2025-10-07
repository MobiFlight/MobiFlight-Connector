import { TableBody, TableCell } from "@/components/ui/table"
import { SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable"
import { flexRender, Row, Table } from "@tanstack/react-table"
import { DndTableRow } from "../DndTableRow"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { cn } from "@/lib/utils"
import { useState, useEffect, forwardRef } from "react"
import { RowInteractionProvider } from "../RowInteractionContext"
import { IConfigItem } from "@/types"

interface ConfigItemTableBodyProps<TData> {
  table: Table<TData>
  dragItemId: string | undefined
  onDeleteSelected?: () => void
  onToggleSelected?: () => void
}
const ConfigItemTableBody = forwardRef<HTMLTableSectionElement, ConfigItemTableBodyProps<IConfigItem>>(
  ({ table, dragItemId, onDeleteSelected, onToggleSelected }, ref) => {
  const { publish } = publishOnMessageExchange()
  const rows = table.getRowModel().rows
  const [lastSelected, setLastSelected] = useState<Row<IConfigItem> | null>(null)

  const selectedRows = table.getSelectedRowModel().rows
  
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      const supportedKeyPress = ["Delete", " ", "Enter", "Escape", "Backspace"]
      
      if (!supportedKeyPress.includes(e.key)) return
      
      e.preventDefault() // Prevent default spacebar behavior (scrolling)
      e.stopPropagation()

      switch (e.key) {
        case "Backspace":
        case "Delete":
          if (onDeleteSelected) onDeleteSelected()
          break

        // this is the space bar
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
  }, [ table, onDeleteSelected, onToggleSelected ])

  return (
    <TableBody className="dark:bg-zinc-900" ref={ref}>
      <SortableContext
        items={rows.map((row) => row.id)}
        strategy={verticalListSortingStrategy}
      >
        {rows.map((row) => {
          const isSelected = row.getIsSelected()
          const isDragging = dragItemId !== undefined
          const isDragItem = dragItemId === row.id
          const dragClassName =
            isDragging && isSelected ?
              !isDragItem
                ? "is-dragging"
                : "is-first-drag-item"
              : "" 
          return (
            <RowInteractionProvider key={row.id}>
              <DndTableRow
                className={dragClassName}
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
                    table.setRowSelection({ [row.id]: true })
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
                {row.getVisibleCells().map((cell) => {
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

                  return (
                    <TableCell
                      key={cell.id}
                      className={cn(
                        "p-1",
                        className,
                        cellClassName,
                        "group-[.is-dragging]/row:hidden",
                      )}
                    >
                      {flexRender(cell.column.columnDef.cell, {...cell.getContext(), selectedRows: selectedRows})}
                    </TableCell>
                  )
                })}
              </DndTableRow>
            </RowInteractionProvider>
          )
        })}
      </SortableContext>
    </TableBody>
  )
})

export default ConfigItemTableBody
