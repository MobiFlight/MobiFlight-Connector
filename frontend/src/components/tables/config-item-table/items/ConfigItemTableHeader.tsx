import { TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { cn } from "@/lib/utils"
import { flexRender } from "@tanstack/react-table"
import { Table } from "@tanstack/react-table"

interface ConfigItemTableHeaderProps<TData> {
  table: Table<TData>
}

const ConfigItemTableHeader = <TData,>({
  table,
}: ConfigItemTableHeaderProps<TData>) => {
  return (
    <TableHeader className="group/header bg-slate-500 text-white dark:bg-zinc-800">
      {table.getHeaderGroups().map((headerGroup) => (
        <TableRow key={headerGroup.id} className="hover:bg-zinc-800">
          {headerGroup.headers.map((header) => {
            const className =
              (
                header.column.columnDef.meta as {
                  className: string
                }
              )?.className ?? ""
            return (
              <TableHead
                key={header.id}
                className={cn(
                  "sticky top-0 z-50 bg-primary px-1 text-white dark:bg-zinc-800",
                  className,
                )}
              >
                {header.isPlaceholder
                  ? null
                  : flexRender(
                      header.column.columnDef.header,
                      header.getContext(),
                    )}
              </TableHead>
            )
          })}
        </TableRow>
      ))}
    </TableHeader>
  )
}

export default ConfigItemTableHeader
