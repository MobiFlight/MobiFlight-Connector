import { TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { cn } from "@/lib/utils"
import { IConfigItem } from "@/types"
import { flexRender, HeaderGroup } from "@tanstack/react-table"
import { forwardRef } from "react"

interface ConfigItemTableHeaderProps<TData> {
  headerGroups: HeaderGroup<TData>[]
}

const ConfigItemTableHeader = forwardRef<
  HTMLTableSectionElement,
  ConfigItemTableHeaderProps<IConfigItem>
>(({ headerGroups }, ref) => {
  return (
    <TableHeader ref={ref} className="group/header bg-slate-500 text-white">
      {headerGroups.map((headerGroup) => (
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
                  "bg-primary sticky top-0 z-50 px-1 text-white dark:bg-blue-950",
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
})

export default ConfigItemTableHeader
