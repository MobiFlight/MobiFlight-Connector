import { IconX } from "@tabler/icons-react"
import { Table } from "@tanstack/react-table"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
// import { DataTableViewOptions } from "@/app/examples/tasks/components/data-table-view-options"

import { DataTableFacetedFilter } from "./data-table-faceted-filter"
import { IConfigItem } from "@/types"

interface DataTableToolbarProps<TData> {
  table: Table<TData>
  items: IConfigItem[]
}

export function DataTableToolbar<TData>({
  table,
  items
}: DataTableToolbarProps<TData>) {
  const isFiltered = table.getState().columnFilters.length > 0
  const devices = [...new Set(items.map((item) => item.Device))].map((device) => ({ label: device.split("/")[0], value: device }))
  const components = [...new Set(items.map((item) => item.Component))].map((component) => ({ label: component, value: component }))
  const types = [...new Set(items.map((item) => item.Type))].map((type) => ({ label: type, value: type }))

  return (
    <div className="flex items-center justify-between">
      <div className="flex flex-1 items-center space-x-2">
        <Input
          placeholder="Filter items..."
          value={(table.getColumn("Description")?.getFilterValue() as string) ?? ""}
          onChange={(event) =>
            table.getColumn("Description")?.setFilterValue(event.target.value)
          }
          className="h-8 w-[150px] lg:w-[250px]"
        />
        {table.getColumn("Device") && (
          <DataTableFacetedFilter
            column={table.getColumn("Device")}
            title="Devices"
            options={devices}
          />
        )}
        {table.getColumn("Component") && (
          <DataTableFacetedFilter
            column={table.getColumn("Component")}
            title="Component"
            options={components}
          />
        )}
        {table.getColumn("Type") && (
          <DataTableFacetedFilter
            column={table.getColumn("Type")}
            title="Types"
            options={types}
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
