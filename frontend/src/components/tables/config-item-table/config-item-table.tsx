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
  useSensor,
  useSensors,
  DragEndEvent,
  MouseSensor,
  TouchSensor,
  DragStartEvent,
  Active,
} from "@dnd-kit/core"

import {
  restrictToParentElement,
  restrictToVerticalAxis,
} from "@dnd-kit/modifiers"

import { useCallback, useEffect, useRef, useState } from "react"
import { DataTableToolbar } from "./data-table-toolbar"
import { IConfigItem } from "@/types"
import { Button } from "@/components/ui/button"
import { publishOnMessageExchange, useAppMessage } from "@/lib/hooks/appMessage"
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
import { snapToCursor } from "@/lib/dnd-kit/snap-to-cursor"
import { Toaster } from "@/components/ui/sonner"
import { useTheme } from "@/lib/hooks/useTheme"
import { toast } from "@/components/ui/ToastWrapper"

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
  // useReactTable does not work with React Compiler https://github.com/TanStack/table/issues/5567
  // eslint-disable-next-line react-hooks/react-compiler
  "use no memo"

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
  const addedItem = useRef(false)

  // the useCallback hook is necessary so that playwright tests work correctly
  const handleProjectMessage = useCallback(() => {
    console.log("Project message received, resetting filters")
    table.resetColumnFilters()
  }, [table])

  useAppMessage("Project", handleProjectMessage)

  useEffect(() => {
    console.log("added item", addedItem.current)
  }, [addedItem])

  useEffect(() => {
    if (addedItem.current && data.length === prevDataLength.current + 1) {
      addedItem.current = false
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
      } else {
        toast({
          title: t("ConfigList.Notification.NewConfigNotVisible.Message") ,
          description: t("ConfigList.Notification.NewConfigNotVisible.Description"),
          id: "reset-filter",
          options: {
            duration: 15000
          },
          button: {
            label: t("ConfigList.Notification.NewConfigNotVisible.Action"),
            onClick: () => {
              table.resetColumnFilters()
              setTimeout(() => {
                const row = table
                  .getRowModel()
                  .rows.find((r) => r.id === lastItem.GUID)
                if (row) {
                  const rowElement = tableRef.current?.querySelector(
                    `[dnd-itemid="${lastItem.GUID}"]`,
                  )
                  rowElement?.scrollIntoView({
                    behavior: "smooth",
                    block: "center",
                  })
                  table.setRowSelection({ [row.id]: true })
                }
              }, 500)
            },
          },
        })
      }

      publish({
        key: "CommandConfigContextMenu",
        payload: { action: "edit", item: lastItem },
      } as CommandConfigContextMenu)
    }
    prevDataLength.current = data.length
  }, [publish, table, data])

  const { t } = useTranslation()
  const [dragItem, setDragItem] = useState<Active | undefined>(undefined)

  const sensors = useSensors(
    useSensor(MouseSensor, {}),
    useSensor(TouchSensor, {}),
    // useSensor(KeyboardSensor, {}),
  )

  const handleDragStart = useCallback(
    (event: DragStartEvent) => {
      const { active } = event
      setDragItem(active)

      const draggedRow = table
        .getRowModel()
        .rows.find((row) => row.id === active.id)
      if (!draggedRow) return
      if (!draggedRow.getIsSelected()) {
        table.setRowSelection({ [active.id]: true })
      }
    },
    [setDragItem, table],
  )

  const handleDragEnd = useCallback(
    (event: DragEndEvent) => {
      const { active, over } = event

      setDragItem(undefined)

      if (!over?.id || !active.id) return

      // we didn't really move anything
      if (active.id === over.id) return

      // Get all selected row GUIDs, or just the dragged one if nothing selected
      const selectedRows = table.getSelectedRowModel().rows
      const selectedIds = selectedRows.map(
        (row) => (row.original as IConfigItem).GUID,
      )
      const originalIndex = (data as IConfigItem[]).findIndex(
        (item) => item.GUID === active.id,
      )

      // Remove dragged items from data
      let newData = (data as IConfigItem[]).filter(
        (item) => !selectedIds.includes(item.GUID),
      )
      // Find drop index
      const newIndex = newData.findIndex((item) => item.GUID === over.id)

      // we determine drag direction
      const dragDirectionOffset = newIndex >= originalIndex ? 1 : 0

      const draggedData = (data as IConfigItem[]).filter((item) =>
        selectedIds.includes(item.GUID),
      )

      // Insert dragged items at drop index
      newData = [
        ...newData.slice(0, newIndex + dragDirectionOffset),
        ...draggedData,
        ...newData.slice(newIndex + dragDirectionOffset),
      ]

      setItems(newData)

      publishOnMessageExchange().publish({
        key: "CommandResortConfigItem",
        payload: {
          items: draggedData,
          newIndex: newIndex + dragDirectionOffset,
        },
      } as CommandResortConfigItem)
    },
    [data, setItems, table],
  )

  const handleAddOutputConfig = useCallback(() => {
    addedItem.current = true
    publish({
      key: "CommandAddConfigItem",
      payload: {
        name: t("ConfigList.Actions.OutputConfigItem.DefaultName"),
        type: "OutputConfig",
      },
    } as CommandAddConfigItem)
  }, [publish, t])

  const handleAddInputConfig = useCallback(() => {
    addedItem.current = true
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

  const { theme } = useTheme()

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
          <Toaster
            position="top-center"
            theme={theme}
            className="flex w-full justify-center ![--width:540px] xl:![--width:800px]"
          />
          {table.getRowModel().rows?.length ? (
            <DndContext
              sensors={sensors}
              collisionDetection={closestCenter}
              modifiers={[
                snapToCursor,
                restrictToVerticalAxis,
                restrictToParentElement,
              ]}
              onDragEnd={handleDragEnd}
              onDragStart={handleDragStart}
            >
              <div
                className="border-primary flex flex-col overflow-y-auto rounded-lg border"
                ref={parentRef}
              >
                <Table ref={tableRef} className="table-fixed">
                  <ConfigItemTableHeader
                    headerGroups={table.getHeaderGroups()}
                  />
                  <ConfigItemTableBody
                    table={table}
                    dragItemId={dragItem?.id as string}
                    onDeleteSelected={deleteSelected}
                    onToggleSelected={toggleSelected}
                  />
                </Table>
              </div>
            </DndContext>
          ) : (
            <div className="border-primary flex flex-col gap-2 rounded-lg border-2 border-solid pb-6">
              <div className="bg-primary mb-4 h-12"></div>
              <div className="text-center" role="alert">
                {t("ConfigList.Table.NoResultsMatchingFilter")}
              </div>
              <div className="flex justify-center">
                <ToolTip
                  content={t("ConfigList.Toolbar.Reset")}
                  className="z-1000 xl:hidden"
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
        <div className="border-primary flex flex-col gap-2 rounded-lg border-2 border-solid">
          <div className="bg-primary h-12"></div>
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
