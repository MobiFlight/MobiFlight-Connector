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

import { useCallback, useEffect, useMemo, useRef, useState } from "react"
import { DataTableToolbar } from "./data-table-toolbar"
import { IConfigItem } from "@/types"
import { Button } from "@/components/ui/button"
import { publishOnMessageExchange, useAppMessage } from "@/lib/hooks/appMessage"
import {
  CommandAddConfigItem,
  CommandConfigContextMenu,
} from "@/types/commands"
import { useTranslation } from "react-i18next"
import ConfigItemTableHeader from "./items/ConfigItemTableHeader"
import ConfigItemTableBody from "./items/ConfigItemTableBody"
import ToolTip from "@/components/ToolTip"
import { IconX } from "@tabler/icons-react"
import { Toaster } from "@/components/ui/sonner"
import { useTheme } from "@/lib/hooks/useTheme"
import { toast } from "@/components/ui/ToastWrapper"
import { useConfigItemDragContext } from "@/lib/hooks/useConfigItemDragContext"
import ConfigItemNoResultsDroppable from "./items/ConfigItemNoResultsDroppable"
import { useDroppable } from "@dnd-kit/core"

interface DataTableProps<TData, TValue> {
  columns: ColumnDef<TData, TValue>[]
  data: TData[]
  dragItemId?: string // Add this prop to receive drag state from parent
}

export function ConfigItemTable<TValue>({
  columns,
  data,
  dragItemId,
}: DataTableProps<IConfigItem, TValue>) {
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

  const { setNodeRef: setTableBodyRef } = useDroppable({
    id: "config-item-table-body",
    data: { type: "table" },
  })

  const { setNodeRef: setTableHeaderRef } = useDroppable({
    id: "config-item-table-header",
    data: { type: "header" },
  })
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
  const tableBodyRef = useRef<HTMLTableSectionElement>(null)
  const prevDataLength = useRef(data.length)
  const addedItem = useRef(false)
  const showInvisibleToastOnDialogClose = useRef<string | null>(null)

  const { setTable, setTableContainerRef, dragState } =
    useConfigItemDragContext()
  // Register this table with the drag context
  useEffect(() => {
    setTable(table)

    // Clean up when component unmounts
    return () => setTable(null)
  }, [table, setTable])

  // We need this indirection just so that
  // we can store the ref in our Context correctly
  //
  // This way it is guaranteed that the ref
  // is set before we use it in the DragDropProvider
  const handleTableBodyRef = useCallback(
    (node: HTMLTableSectionElement | null) => {
      tableBodyRef.current = node
      if (node) {
        setTableBodyRef(node)
        setTableContainerRef(node)
      }
    },
    [setTableContainerRef, setTableBodyRef],
  )

  // the useCallback hook is necessary so that playwright tests work correctly
  const handleProjectMessage = useCallback(() => {
    console.log("Project message received, resetting filters")
    table.resetColumnFilters()
  }, [table])

  useAppMessage("Project", handleProjectMessage)
  useAppMessage("OverlayState", (message) => {
    const isClosing =
      (message.payload as { Visible: boolean }).Visible === false
    if (!isClosing) return

    if (showInvisibleToastOnDialogClose.current == null) return

    const lastGuid = showInvisibleToastOnDialogClose.current
    showInvisibleToastOnDialogClose.current = null

    toast({
      title: t("ConfigList.Notification.NewConfigNotVisible.Message"),
      description: t("ConfigList.Notification.NewConfigNotVisible.Description"),
      id: "reset-filter",
      options: {
        duration: 5000,
      },
      button: {
        label: t("ConfigList.Notification.NewConfigNotVisible.Action"),
        onClick: () => {
          table.resetColumnFilters()
          setTimeout(() => {
            const row = table.getRowModel().rows.find((r) => r.id === lastGuid)
            if (row) {
              const rowElement = tableRef.current?.querySelector(
                `[dnd-itemid="${lastGuid}"]`,
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
  })

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
        // If the newly added item's row element is not found,
        // it means the item is not visible due to active filters.
        // Store its GUID so that when the dialog closes,
        // we can show a toast notification to inform the user
        // and offer to reset the filters.
        showInvisibleToastOnDialogClose.current = lastItem.GUID
      }
      publish({
        key: "CommandConfigContextMenu",
        payload: { action: "edit", item: lastItem },
      } as CommandConfigContextMenu)
    }
    prevDataLength.current = data.length
  }, [publish, table, data])

  const { t } = useTranslation()

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

  const showTable = useMemo(() => {
    if (!(dragState?.ui.isDragging ?? false)) {
      return data.length > 0
    }

    return (
      table.getRowModel().rows?.length -
        (dragState?.items.draggedItems.length ?? 0) >
      0
    )
  }, [
    data.length,
    dragState?.items.draggedItems.length,
    dragState?.ui.isDragging,
    table,
  ])

  return (
    <div className="flex grow flex-col gap-2 overflow-y-auto">
      <div className="flex grow flex-col gap-2 overflow-y-auto">
        <div className="p-1">
          <DataTableToolbar
            disabled={!showTable}
            table={table}
            items={data as IConfigItem[]}
            onDeleteSelected={deleteSelected}
            onToggleSelected={toggleSelected}
            onClearSelected={() => table.setRowSelection({})}
          />
        </div>
        <Toaster
          position="bottom-right"
          theme={theme}
          className="flex w-full justify-center ![--width:540px] xl:![--width:800px]"
        />
        {showTable ? (
          table.getRowModel().rows?.length ? (
            <div className="border-primary flex flex-col overflow-y-auto rounded-lg border">
              <Table ref={tableRef} className="table-fixed">
                <ConfigItemTableHeader
                  ref={setTableHeaderRef}
                  headerGroups={table.getHeaderGroups()}
                />
                <ConfigItemTableBody
                  ref={handleTableBodyRef}
                  table={table}
                  dragItemId={dragItemId}
                  onDeleteSelected={deleteSelected}
                  onToggleSelected={toggleSelected}
                />
              </Table>
            </div>
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
          )
        ) : (
          <ConfigItemNoResultsDroppable />
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
    </div>
  )
}
