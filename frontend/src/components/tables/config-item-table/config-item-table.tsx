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

import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  useSensor,
  useSensors,
  DragEndEvent,
  MouseSensor,
  TouchSensor,
} from "@dnd-kit/core"

import {
  arrayMove,
  SortableContext,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable"

import { restrictToVerticalAxis } from "@dnd-kit/modifiers"

import { useCallback, useEffect, useRef, useState } from "react"
import { DataTableToolbar } from "./data-table-toolbar"
import { IConfigItem } from "@/types"
import { Button } from "@/components/ui/button"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandAddConfigItem, CommandResortConfigItem } from "@/types/commands"
import { useTranslation } from "react-i18next"
import { DndTableRow } from "@/components/tables/config-item-table/DndTableRow"

interface DataTableProps<TData, TValue> {
  columns: ColumnDef<TData, TValue>[]
  data: TData[],
  setItems: (items: IConfigItem[]) => void
}

export function ConfigItemTable<TData, TValue>({
  columns,
  data,
  setItems
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
    getRowId: (row) => (row as IConfigItem).GUID, //required because row indexes will change
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
  const tableRef = useRef<HTMLTableElement>(null)
  const prevDataLength = useRef(data.length)

  useEffect(() => {
    if (data.length > prevDataLength.current) {
      const lastItem = data[data.length - 1] as IConfigItem
      const rowElement = tableRef.current?.querySelector(
        `[dnd-itemid="${lastItem.GUID}"]`,
      )
      if (rowElement) {
        rowElement.scrollIntoView({ behavior: "smooth", block: "center" })
      }
    }
    prevDataLength.current = data.length
  }, [data])

  const { t } = useTranslation()

  const sensors = useSensors(
    useSensor(MouseSensor, {}),
    useSensor(TouchSensor, {}),
    useSensor(KeyboardSensor, {})
  )
  

  const handleDragEnd = useCallback((event: DragEndEvent) => {
    const { active, over } = event

    if (active.id !== over?.id) {
      // sort the data items first
      const oldIndex = data.findIndex((item) => (item as IConfigItem).GUID === active?.id);
      const newIndex = data.findIndex((item) => (item as IConfigItem).GUID === over?.id);
      const arrayWithNewOrder = arrayMove(data, oldIndex, newIndex);
      // store them in a local array
      // then set the items

      setItems(arrayWithNewOrder as IConfigItem[]);

      publishOnMessageExchange().publish({
        key: "CommandResortConfigItem",
        payload: {
          items: [ data.find((item) => (item as IConfigItem).GUID === active.id) as IConfigItem ],
          newIndex: newIndex,
        }
      } as CommandResortConfigItem);
    }
  }, [data, setItems])

  return (
    <div className="flex grow flex-col gap-2 overflow-y-auto">
      <div className="p-1">
        <DataTableToolbar table={table} items={data as IConfigItem[]} />
      </div>
        <DndContext
          sensors={sensors}
          collisionDetection={closestCenter}
          modifiers={[restrictToVerticalAxis]}
          onDragEnd={handleDragEnd}
        >
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
                        {row.getVisibleCells().map((cell) => (
                          <TableCell key={cell.id} className="p-1">
                            {flexRender(
                              cell.column.columnDef.cell,
                              cell.getContext(),
                            )}
                          </TableCell>
                        ))}
                      </DndTableRow>
                    )
                  })}
                </SortableContext>
              ) : (
                <TableRow>
                  <TableCell
                    colSpan={columns.length}
                    className="h-24 text-center"
                  >
                    {t("ConfigList.Table.NoResultsFound")}
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
      </div>
        </DndContext>
      <div className="flex justify-start gap-2">
        <Button
          variant={"outline"}
          className="border-pink-600 text-pink-600 hover:bg-pink-600 hover:text-white"
          onClick={() =>
            publish({
              key: "CommandAddConfigItem",
              payload: {
                name: t("ConfigList.Actions.OutputConfigItem.DefaultName"),
                type: "OutputConfig",
              },
            } as CommandAddConfigItem)
          }
        >
          {t("ConfigList.Actions.OutputConfigItem.Add")}
        </Button>
        <Button
          variant={"outline"}
          className="border-teal-600 text-teal-600 hover:bg-teal-600 hover:text-white"
          onClick={() =>
            publish({
              key: "CommandAddConfigItem",
              payload: {
                name: t("ConfigList.Actions.InputConfigItem.DefaultName"),
                type: "InputConfig",
              },
            } as CommandAddConfigItem)
          }
        >
          {t("ConfigList.Actions.InputConfigItem.Add")}
        </Button>
      </div>
    </div>
  )
}
