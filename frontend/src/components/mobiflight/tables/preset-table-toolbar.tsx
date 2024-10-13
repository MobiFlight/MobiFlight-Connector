import { IconX } from "@tabler/icons-react"
import { Table } from "@tanstack/react-table"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
// import { DataTableViewOptions } from "@/app/examples/tasks/components/data-table-view-options"

import { Preset } from "@/types"
import { DataTableFacetedFilter } from "./data-table-faceted-filter"

interface PresetDataTableToolbarProps<TData> {
  table: Table<TData>
  items: Preset[]
}

export function PresetDataTableToolbar<TData>({
  table,
  items,
}: PresetDataTableToolbarProps<TData>) {
  const isFiltered = table.getState().columnFilters.length > 0
  // const level = [...new Set(items.map((item) => item.Severity))].map(
  //   (device) => ({ label: device.split("/")[0], value: device }),
  // )

  const vendor = [...new Set(items.map((item) => item.vendor))].map((v) => ({ label: v, value: v }))
  const aircraft = [...new Set(items.map((item) => item.aircraft))].map((v) => ({ label: v, value: v }))
  const system = [...new Set(items.map((item) => item.system))].map((v) => ({ label: v, value: v }))
  
  return (
    <div className="flex items-center justify-between">
      <div className="flex flex-1 items-center space-x-2">
        <Input
          placeholder="Filter items..."
          value={
            (table.getColumn("label")?.getFilterValue() as string) ?? ""
          }
          onChange={(event) =>
            table.getColumn("label")?.setFilterValue(event.target.value)
          }
          className="w-[150px] lg:w-[250px]"
        />
        {table.getColumn("vendor") && (
          <DataTableFacetedFilter
            column={table.getColumn("vendor")}
            title="Vendor"
            options={vendor}
          />
        )}
        {table.getColumn("aircraft") && (
          <DataTableFacetedFilter
            column={table.getColumn("aircraft")}
            title="Aircraft"
            options={aircraft}
          />
        )}
        {table.getColumn("system") && (
          <DataTableFacetedFilter
            column={table.getColumn("system")}
            title="System"
            options={system}
          />
        )}
        {isFiltered && (
          <Button            
            onClick={() => table.resetColumnFilters()}
            //className="h-8 px-2 lg:px-3"
          >
            Reset
            <IconX className="ml-2 h-4 w-4" />
          </Button>
        )}
      </div>
      { table.getFilteredRowModel().rows.length > 0 && 
        <div>
          { table.getFilteredRowModel().rows.length } /  {table.getCoreRowModel().rows.length} items
        </div>
      }
    </div>
  )
}
