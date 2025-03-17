import { TableBody, TableCell } from "@/components/ui/table"
import { SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable"
import { flexRender, Table } from "@tanstack/react-table"
import { DndTableRow } from "../DndTableRow"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { cn } from "@/lib/utils"

interface ConfigItemTableBodyProps<TData> {
  table: Table<TData>
}
const ConfigItemTableBody = <TData,>({
  table,
}: ConfigItemTableBodyProps<TData>) => {
  const { publish } = publishOnMessageExchange()
  return (
    <TableBody className="dark:bg-zinc-900">
      <SortableContext
        items={table.getRowModel().rows.map((row) => row.id)}
        strategy={verticalListSortingStrategy}
      >
        {table.getRowModel().rows.map((row) => {
          return (
            <DndTableRow
              key={row.id}
              data-state={row.getIsSelected() && "selected"}
              dnd-itemid={row.id}
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

                return (
                  <TableCell key={cell.id} className={cn("p-1", className)}>
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
