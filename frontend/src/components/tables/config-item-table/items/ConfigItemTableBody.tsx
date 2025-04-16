import { TableBody, TableCell } from "@/components/ui/table"
import { SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable"
import { flexRender, Row, Table } from "@tanstack/react-table"
import { DndTableRow } from "../DndTableRow"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { cn } from "@/lib/utils"
import { useState, useEffect } from "react"

interface ConfigItemTableBodyProps<TData> {
  table: Table<TData> 
}
const ConfigItemTableBody = <TData,>({
  table
}: ConfigItemTableBodyProps<TData>) => {
  const { publish } = publishOnMessageExchange()
  const rows = table.getRowModel().rows
  const [lastSelected, setLastSelected] = useState<Row<TData> | null>(null)

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      const selectedRows = table.getSelectedRowModel().rows

      if (e.key === "Delete" && selectedRows.length > 0) {
        // Handle bulk delete
        selectedRows.forEach((row) => {
          publish({
            key: "CommandConfigContextMenu",
            payload: { action: "delete", item: row.original },
          })
        })
      } else if (e.key === " " && selectedRows.length > 0) {
        // Handle bulk toggle active/inactive
        e.preventDefault() // Prevent default spacebar behavior (scrolling)
        selectedRows.forEach((row) => {
          publish({
            key: "CommandConfigContextMenu",
            payload: {
              action: "toggle",
              item: row.original,
            },
          })
        })
      }
    }

    window.addEventListener("keydown", handleKeyDown)
    return () => window.removeEventListener("keydown", handleKeyDown)
  }, [table, publish])
  
  return (
    <TableBody className="dark:bg-zinc-900">
      <SortableContext
        items={rows.map((row) => row.id)}
        strategy={verticalListSortingStrategy}
      >
        {
          rows.map((row) => {
          return (
            <DndTableRow
              key={row.id}
              data-state={row.getIsSelected() && "selected"}
              dnd-itemid={row.id}
              onClick={(e): void => {
                e.stopPropagation()
                if (e.shiftKey) {
                  if (lastSelected) {
                    const lastIndex = rows.findIndex((r) => r.id === lastSelected.id)
                    const currentIndex = rows.findIndex((r) => r.id === row.id)
                    const range = [lastIndex, currentIndex].sort((a, b) => a - b)
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
                  <TableCell key={cell.id} className={cn("p-1", className, cellClassName)}>
                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
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
