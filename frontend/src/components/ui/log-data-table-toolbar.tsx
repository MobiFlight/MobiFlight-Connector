import { IconX } from "@tabler/icons-react"
import { Table } from "@tanstack/react-table"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
// import { DataTableViewOptions } from "@/app/examples/tasks/components/data-table-view-options"

import { DataTableFacetedFilter } from "./data-table-faceted-filter"
import { ILogMessage } from "@/types"

interface LogDataTableToolbarProps<TData> {
  table: Table<TData>
  items: ILogMessage[]
}

export function LogDataTableToolbar<TData>({
  table,
  items
}: LogDataTableToolbarProps<TData>) {
  const isFiltered = table.getState().columnFilters.length > 0
  const level = [...new Set(items.map((item) => item.Severity))].map((device) => ({ label: device.split("/")[0], value: device }))
  
  return (
    <div className="flex items-center justify-between">
      <div className="flex flex-1 items-center space-x-2">
        <Input
          placeholder="Filter items..."
          value={(table.getColumn("Message")?.getFilterValue() as string) ?? ""}
          onChange={(event) =>
            table.getColumn("Message")?.setFilterValue(event.target.value)
          }
          className="h-8 w-[150px] lg:w-[250px]"
        />
        {table.getColumn("Severity") && (
          <DataTableFacetedFilter
            column={table.getColumn("Severity")}
            title="Severity"
            options={level}
          />
        )}
        {isFiltered && (
          <Button
            variant="ghost"
            onClick={() => table.resetColumnFilters()}
            className="h-8 px-2 lg:px-3"
          >
            Reset
            <IconX className="ml-2 h-4 w-4" />
          </Button>
        )}
      </div>
      {/* <DataTableViewOptions table={table} /> */}
    </div>
  )
}
