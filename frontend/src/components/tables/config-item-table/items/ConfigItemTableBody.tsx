import { TableBody, TableCell } from "@/components/ui/table"
import { SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable"
import { flexRender, Row } from "@tanstack/react-table"
import { DndTableRow } from "../DndTableRow"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { cn } from "@/lib/utils"
import { Virtualizer } from "@tanstack/react-virtual"

interface ConfigItemTableBodyProps<TData> {
  virtualizer: Virtualizer<HTMLDivElement, Element>,
  rows: Row<TData>[] 
}
const ConfigItemTableBody = <TData,>({
  virtualizer,
  rows
}: ConfigItemTableBodyProps<TData>) => {
  const { publish } = publishOnMessageExchange()
  return (
    <TableBody className="dark:bg-zinc-900">
      <SortableContext
        items={rows.map((row) => row.id)}
        strategy={verticalListSortingStrategy}
      >
        {
          virtualizer.getVirtualItems().map((virtualRow, index) => {
          const row = rows[virtualRow.index]
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
              style={{
                height: `${virtualRow.size}px`,
                transform: `translateY(${
                  virtualRow.start - index * virtualRow.size
                }px)`,
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
