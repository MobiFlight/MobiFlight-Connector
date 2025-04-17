import {
  ColumnDef,
  ColumnFiltersState,
  SortingState,
  VisibilityState,
  getCoreRowModel,
  getFacetedRowModel,
  getFacetedUniqueValues,
  getFilteredRowModel,
  getSortedRowModel,
  useReactTable,
} from "@tanstack/react-table"

import { Table } from "@/components/ui/table"

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

import { arrayMove } from "@dnd-kit/sortable"

import { restrictToVerticalAxis } from "@dnd-kit/modifiers"

import { useCallback, useEffect, useRef, useState } from "react"
import { DataTableToolbar } from "./data-table-toolbar"
import { IConfigItem } from "@/types"
import { Button } from "@/components/ui/button"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import {
  CommandAddConfigItem,
  CommandConfigContextMenu,
  CommandResortConfigItem,
} from "@/types/commands"
import { useTranslation } from "react-i18next"
import ConfigItemTableHeader from "./items/ConfigItemTableHeader"
import ConfigItemTableBody from "./items/ConfigItemTableBody"
import ToolTip from "@/components/ToolTip"
import { IconX } from "@tabler/icons-react"

interface DataTableProps<TData, TValue> {
  columns: ColumnDef<TData, TValue>[]
  data: TData[]
  setItems: (items: IConfigItem[]) => void
}

export function ConfigItemTable<TData, TValue>({
  columns,
  data,
  setItems,
}: DataTableProps<TData, TValue>) {
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [columnVisibility, setColumnVisibility] = useState<VisibilityState>({
    Type: false,
    ConfigType: false,
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

  const parentRef = useRef<HTMLDivElement>(null)
  // const { rows } = table.getRowModel()
  // Virtualization setup
  // const virtualizer = useVirtualizer({
  //   count: rows.length,
  //   getScrollElement: () => parentRef.current,
  //   estimateSize: () => 50,
  //   overscan: 5
  // });

  const { publish } = publishOnMessageExchange()
  const tableRef = useRef<HTMLTableElement>(null)
  const prevDataLength = useRef(data.length)

  useEffect(() => {
    if (data.length === prevDataLength.current + 1) {
      const lastItem = data[data.length - 1] as IConfigItem
      const rowElement = tableRef.current?.querySelector(
        `[dnd-itemid="${lastItem.GUID}"]`,
      )
      if (rowElement) {
        rowElement.scrollIntoView({ behavior: "smooth", block: "center" })
        const row = table.getRowModel().rows.find((r) => r.id === lastItem.GUID)
        if (row) {
          table.setRowSelection({ [row.id]: true })
        }
        publish({
          key: "CommandConfigContextMenu",
          payload: { action: "edit", item: lastItem },
        } as CommandConfigContextMenu)
      }
    }
    prevDataLength.current = data.length
  }, [publish, table, data])

  const { t } = useTranslation()

  const sensors = useSensors(
    useSensor(MouseSensor, {}),
    useSensor(TouchSensor, {}),
    useSensor(KeyboardSensor, {}),
  )

  const handleDragEnd = useCallback(
    (event: DragEndEvent) => {
      const { active, over } = event

      if (active.id !== over?.id) {
        // sort the data items first
        const oldIndex = data.findIndex(
          (item) => (item as IConfigItem).GUID === active?.id,
        )
        const newIndex = data.findIndex(
          (item) => (item as IConfigItem).GUID === over?.id,
        )
        const arrayWithNewOrder = arrayMove(data, oldIndex, newIndex)
        // store them in a local array
        // then set the items

        setItems(arrayWithNewOrder as IConfigItem[])

        publishOnMessageExchange().publish({
          key: "CommandResortConfigItem",
          payload: {
            items: [
              data.find(
                (item) => (item as IConfigItem).GUID === active.id,
              ) as IConfigItem,
            ],
            newIndex: newIndex,
          },
        } as CommandResortConfigItem)
      }
    },
    [data, setItems],
  )

  const handleAddOutputConfig = useCallback(() => {
    publish({
      key: "CommandAddConfigItem",
      payload: {
        name: t("ConfigList.Actions.OutputConfigItem.DefaultName"),
        type: "OutputConfig",
      },
    } as CommandAddConfigItem)
  }, [publish, t])

  const handleAddInputConfig = useCallback(() => {
    publish({
      key: "CommandAddConfigItem",
      payload: {
        name: t("ConfigList.Actions.InputConfigItem.DefaultName"),
        type: "InputConfig",
      },
    } as CommandAddConfigItem)
  }, [publish, t])

  const deleteSelected = useCallback(() => {
    const action = "delete"
    const selectedConfigs = table.getSelectedRowModel().rows.map((row) => {
      return row.original as IConfigItem
    })
    if (selectedConfigs.length === 0) return

    publish({
      key: "CommandConfigBulkAction",
      payload: {
        action: action,
        items: selectedConfigs,
      },
    })
    // Clear selection after deletion
    table.setRowSelection({})
  }, [table, publish])

  const toggleSelected = useCallback(() => {
    const action = "toggle"
    const selectedConfigs = table.getSelectedRowModel().rows.map((row) => {
      return row.original as IConfigItem
    })
    if (selectedConfigs.length === 0) return

    publish({
      key: "CommandConfigBulkAction",
      payload: {
        action: action,
        items: selectedConfigs,
      },
    })
  }, [table, publish])

  return (
    <div className="flex grow flex-col gap-2 overflow-y-auto">
      {data.length > 0 ? (
        <div className="flex grow flex-col gap-2 overflow-y-auto">
          <div className="p-1">
            <DataTableToolbar
              table={table}
              items={data as IConfigItem[]}
              onDeleteSelected={deleteSelected}
              onToggleSelected={toggleSelected}
              onClearSelected={() => table.setRowSelection({})}
            />
          </div>
          {table.getRowModel().rows?.length ? (
            <DndContext
              sensors={sensors}
              collisionDetection={closestCenter}
              modifiers={[restrictToVerticalAxis]}
              onDragEnd={handleDragEnd}
            >
              <div
                className="flex flex-col overflow-y-auto rounded-lg border border-primary"
                ref={parentRef}
              >
                <Table ref={tableRef} className="table-fixed">
                  <ConfigItemTableHeader
                    headerGroups={table.getHeaderGroups()}
                  />
                  <ConfigItemTableBody
                    table={table}
                    onDeleteSelected={deleteSelected}
                    onToggleSelected={toggleSelected}
                  />
                </Table>
              </div>
            </DndContext>
          ) : (
            <div className="flex flex-col gap-2 rounded-lg border-2 border-solid border-primary pb-6">
              <div className="mb-4 h-12 bg-primary"></div>
              <div className="text-center" role="alert">
                {t("ConfigList.Table.NoResultsMatchingFilter")}
              </div>
              <div className="flex justify-center">
                <ToolTip
                  content={t("ConfigList.Toolbar.Reset")}
                  className="z-[1000] xl:hidden"
                >
                  <Button
                    onClick={() => table.resetColumnFilters()}
                    variant={"ghost"}
                  >
                    <span className="flex">
                      {t("ConfigList.Toolbar.Reset")}
                    </span>
                    <IconX className="h-4 w-4 xl:ml-2" />
                  </Button>
                </ToolTip>
              </div>
            </div>
          )}
        </div>
      ) : (
        <div className="flex flex-col gap-2 rounded-lg border-2 border-solid border-primary">
          <div className="h-12 bg-primary"></div>
          <div className="p-4 pb-6 text-center" role="alert">
            {t("ConfigList.Table.NoResultsFound")}
          </div>
        </div>
      )}
      <div className="flex justify-start gap-2">
        <Button
          variant={"outline"}
          className="border-pink-600 text-pink-600 hover:bg-pink-600 hover:text-white"
          onClick={handleAddOutputConfig}
        >
          {t("ConfigList.Actions.OutputConfigItem.Add")}
        </Button>
        <Button
          variant={"outline"}
          className="border-teal-600 text-teal-600 hover:bg-teal-600 hover:text-white"
          onClick={handleAddInputConfig}
        >
          {t("ConfigList.Actions.InputConfigItem.Add")}
        </Button>
      </div>
    </div>
  )
}
