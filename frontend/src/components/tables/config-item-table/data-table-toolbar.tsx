import { IconBan, IconFilter, IconToggleLeft, IconTrash, IconX } from "@tabler/icons-react"
import { Table } from "@tanstack/react-table"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
// import { DataTableViewOptions } from "@/app/examples/tasks/components/data-table-view-options"

import { DataTableFacetedFilter } from "./data-table-faceted-filter"
import { IConfigItem } from "@/types"
import { isEmpty } from "lodash-es"
import { useTranslation } from "react-i18next"
import DarkModeToggle from "@/components/DarkModeToggle"
import ToolTip from "@/components/ToolTip"

interface DataTableToolbarProps<TData> {
  table: Table<TData>
  items: IConfigItem[]
  onDeleteSelected?: () => void
  onToggleSelected?: () => void
}

export function DataTableToolbar<TData>({
  table,
  items,
  onDeleteSelected,
  onToggleSelected,
}: DataTableToolbarProps<TData>) {
  const { t } = useTranslation()
  
  const isFiltered = table.getState().columnFilters.length > 0
  const isRowsSelected = table.getSelectedRowModel().rows.length > 0

  const configTypes = [...new Set(items.map((item) => item.Type))].map(
    (type) => {
      const value = type
      const label = t(`Types.${type}`)
      return {
        label: label,
        value: value,
      }
    },
  )

  const controller = [
    ...new Set(items.map((item) => item.ModuleSerial)),
  ]
    .map((serial) => {
      const label = serial?.split("/")[0]
      return {
        label:
          !isEmpty(label) && label != "-"
            ? label
            : t(`ConfigList.Toolbar.NotSet`),
        value: serial,
        icon: isEmpty(label) || label == "-" ? IconBan : undefined,
      }
    })
    .sort((a) => (a.label === t(`ConfigList.Toolbar.NotSet`) ? 1 : -1))

  const deviceTypes = [
    // these are output devices
    ...new Set(
      items
        .filter((item) => item.Type == "OutputConfigItem")
        .map((item) => item.Device?.Type ?? "-"),
    ),
    // these are input devices
    ...new Set(
      items
        .filter((item) => item.Type == "InputConfigItem")
        .map((item) => item.DeviceType ?? "-"),
    ),
  ]
    .map((type) => {
      const label =
        type != "-" ? t(`Types.${type}`) : t(`ConfigList.Toolbar.NotSet`)
      return {
        label: label,
        value: type,
        icon: type == "-" ? IconBan : undefined,
      }
    })
    .sort((a) => (a.label === t(`ConfigList.Toolbar.NotSet`) ? 1 : -1))

  const deviceNames = [
    // these are output devices
    ...new Set(
      items
        .filter((item) => item.Type == "OutputConfigItem")
        .map((item) => item.Device?.Name ?? "-"),
    ),
    // these are input devices
    ...new Set(
      items
        .filter((item) => item.Type == "InputConfigItem")
        .map((item) => item.DeviceName ?? "-"),
    ),
  ]
    .map((device) => {
      const label = device
      return {
        label: label != "-" ? label : t(`ConfigList.Toolbar.NotSet`),
        value: label,
        icon: label == "-" ? IconBan : undefined,
      }
    })
    .sort((a) => (a.label === t(`ConfigList.Toolbar.NotSet`) ? 1 : -1))

  return (
    <div className="flex items-center justify-between">
      <div className="-ml-3 flex flex-1 items-center space-x-2 md:ml-0">
        <IconFilter className="hidden stroke-primary md:flex" />
        <Input
          placeholder={t("ConfigList.Toolbar.Search.Placeholder")}
          value={(table.getColumn("Name")?.getFilterValue() as string) ?? ""}
          onChange={(event) =>
            table.getColumn("Name")?.setFilterValue(event.target.value)
          }
          className="h-8 w-[150px] lg:w-[250px]"
        />
        {table.getColumn("ConfigType") && (
          <DataTableFacetedFilter
            column={table.getColumn("ConfigType")}
            title={t("ConfigList.Toolbar.Filter.ConfigType")}
            options={configTypes}
          />
        )}
        {table.getColumn("ModuleSerial") && (
          <DataTableFacetedFilter
            column={table.getColumn("ModuleSerial")}
            title={t("ConfigList.Toolbar.Filter.Device")}
            options={controller}
          />
        )}
        {table.getColumn("Type") && (
          <DataTableFacetedFilter
            column={table.getColumn("Type")}
            title={t("ConfigList.Toolbar.Filter.Type")}
            options={deviceTypes}
          />
        )}
        {table.getColumn("Device") && (
          <DataTableFacetedFilter
            column={table.getColumn("Device")}
            title={t("ConfigList.Toolbar.Filter.Name")}
            options={deviceNames}
          />
        )}
        {isFiltered && (
          <ToolTip
            content={t("ConfigList.Toolbar.Reset")}
            className="z-[1000] xl:hidden"
          >
            <Button
              variant="ghost"
              onClick={() => table.resetColumnFilters()}
              className="hidden h-8 px-2 lg:flex xl:px-3"
            >
              <span className="hidden xl:flex">
                {t("ConfigList.Toolbar.Reset")}
              </span>
              <IconX className="h-4 w-4 xl:ml-2" />
            </Button>
          </ToolTip>
        )}
        {isRowsSelected && (
          <>
        <Button className="flex flex-row gap-1 items-center py-1 h-8" variant="ghost" onClick={onDeleteSelected}>
          <IconTrash />
          <span>Delete selected rows</span>
        </Button>
        <Button className="flex flex-row gap-1 items-center py-1 h-8" variant="ghost" onClick={onToggleSelected}>
          <IconToggleLeft />
          <span>Toggle selected rows</span>
        </Button>
        </>
        )}
      </div>
      <DarkModeToggle />
    </div>
  )
}
