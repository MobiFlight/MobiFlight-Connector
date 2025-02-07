import {
  ColumnDef,
  ColumnFiltersState,
  SortingState,
  VisibilityState,
  flexRender,
  getCoreRowModel,
  getFacetedRowModel,
  getFacetedUniqueValues,
  getFilteredRowModel,
  getSortedRowModel,
  useReactTable,
} from "@tanstack/react-table"

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { useEffect, useRef, useState } from "react"
import { DataTableToolbar } from "./data-table-toolbar"
import { ConfigValueUpdate, IConfigItem } from "@/types"
import { Button } from "@/components/ui/button"
import { publishOnMessageExchange, useAppMessage } from "@/lib/hooks/appMessage"
import { CommandAddConfigItem } from "@/types/commands"
import { useConfigStore } from "@/stores/configFileStore"

interface DataTableProps<TData, TValue> {
  columns: ColumnDef<TData, TValue>[]
  data: TData[]
}

export function ConfigItemTable<TData, TValue>({
  columns,
  data,
}: DataTableProps<TData, TValue>) {
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [columnVisibility, setColumnVisibility] = useState<VisibilityState>({
    Type: false,
  })

  const table = useReactTable({
    data,
    columns,
    onSortingChange: setSorting,
    onColumnFiltersChange: setColumnFilters,
    onColumnVisibilityChange: setColumnVisibility,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getFacetedRowModel: getFacetedRowModel(),
    getFacetedUniqueValues: getFacetedUniqueValues(),
    state: {
      sorting,
      columnFilters,
      columnVisibility,
    },
    defaultColumn: {
      size: 10, //starting column size
      minSize: 1, //enforced during column resizing
      maxSize: 250, //enforced during column resizing
    },
  })

  const { publish } = publishOnMessageExchange()
  const { updateItem } = useConfigStore()
  const tableRef = useRef<HTMLTableElement>(null)
  const prevDataLength = useRef(data.length)

  useEffect(() => {
    if (data.length > prevDataLength.current) {
      const rowElement = tableRef.current?.querySelector(`[data-row-index="${data.length - 1}"]`)
      if (rowElement) {
        rowElement.scrollIntoView({ behavior: "smooth", block: "center" })
      }
    }
    prevDataLength.current = data.length
  }, [data])

  useAppMessage("ConfigValueUpdate", (message) => {
    console.log(message)
    const update = message.payload as ConfigValueUpdate
    // better performance for single updates
    if (update.ConfigItems.length === 1) {
      updateItem(update.ConfigItems[0], true)
    }
  })

  return (
    <div className="flex flex-col grow gap-2 overflow-y-auto">
      <div className="p-2">
        <DataTableToolbar table={table} items={data as IConfigItem[]} />
      </div>
      <div className="flex flex-col overflow-y-auto rounded-lg border border-primary">
        <Table ref={tableRef} className="table-auto">
          <TableHeader className="group/header bg-slate-500 text-white dark:bg-slate-800">
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id} className="hover:bg-slate-800">
                {headerGroup.headers.map((header) => {
                  return (
                    <TableHead
                      key={header.id}
                      className="sticky top-0 z-50 bg-primary px-2 text-white dark:bg-slate-800"
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
          <TableBody className="dark:bg-zinc-900">
            {table.getRowModel().rows?.length ? (
              table.getRowModel().rows.map((row) => (
                <TableRow
                  key={row.id}
                  data-state={row.getIsSelected() && "selected"}
                  data-row-index={row.index}
                  className={row.getValue("Active") ? "" : "text-gray-500"}
                >
                  {row.getVisibleCells().map((cell) => (
                    <TableCell key={cell.id} className="p-1">
                      {flexRender(
                        cell.column.columnDef.cell,
                        cell.getContext(),
                      )}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={columns.length}
                  className="h-24 text-center"
                >
                  No results.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
      <div className="flex justify-start gap-2 p-2 select-none">
        <Button
          variant={"outline"}
          className="border-pink-600 text-pink-600 hover:bg-pink-600 hover:text-white"
          onClick={() =>
            publish({
              key: "CommandAddConfigItem",
              payload: { name: "New Output Config", type: "OutputConfig" },
            } as CommandAddConfigItem)
          }
        >
          Add Output Config
        </Button>
        <Button
          variant={"outline"}
          className="border-teal-600 text-teal-600 hover:bg-teal-600 hover:text-white"
          onClick={() =>
            publish({
              key: "CommandAddConfigItem",
              payload: { name: "New Input Config", type: "InputConfig" },
            } as CommandAddConfigItem)
          }
        >
          Add Input Config
        </Button>
      </div>
    </div>
  )
}
