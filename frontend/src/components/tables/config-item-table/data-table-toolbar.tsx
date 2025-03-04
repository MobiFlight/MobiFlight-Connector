import { IconBan, IconFilter, IconX } from "@tabler/icons-react"
import { Table } from "@tanstack/react-table"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
// import { DataTableViewOptions } from "@/app/examples/tasks/components/data-table-view-options"

import { DataTableFacetedFilter } from "./data-table-faceted-filter"
import { IConfigItem } from "@/types"
import { isEmpty } from "lodash-es"
import { useTranslation } from "react-i18next"
import DarkModeToggle from "@/components/DarkModeToggle"

interface DataTableToolbarProps<TData> {
  table: Table<TData>
  items: IConfigItem[]
}

export function DataTableToolbar<TData>({
  table,
  items,
}: DataTableToolbarProps<TData>) {
  const { t } = useTranslation()

  const isFiltered = table.getState().columnFilters.length > 0

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

  const controller = [...new Set(items.map((item) => item.ModuleSerial))]
    .map((serial) => {
      const label = serial?.split("/")[0]
      return {
        label: !isEmpty(label) && label !="-" ? label : t(`ConfigList.Toolbar.NotSet`),
        value: serial,
        icon: isEmpty(label) || label =="-" ? IconBan : undefined,
      }
    })
    .sort((a) => (a.label === t(`ConfigList.Toolbar.NotSet`) ? 1 : -1))

  const deviceTypes = [...new Set(items.map((item) => item.Device?.Type ?? "-"))].map(
    (type) => {
      const label = (type!="-")
        ? t(`Types.${type}`)
        : t(`ConfigList.Toolbar.NotSet`)
      return {
        label: label,
        value: type,
        icon: (type=="-") ? IconBan : undefined,
      }
    },
  ).sort((a) => (a.label === t(`ConfigList.Toolbar.NotSet`) ? 1 : -1))

  const deviceNames = [...new Set(items.map((item) => item.Device?.Name ?? "-"))].map(
    (device) => {
      const label = device
      return {
        label: (label!="-") ? label : t(`ConfigList.Toolbar.NotSet`),
        value: label,
        icon: (label=="-") ? IconBan : undefined,
      }
    },
  ).sort((a) => (a.label === t(`ConfigList.Toolbar.NotSet`) ? 1 : -1))

  return (
    <div className="flex items-center justify-between">
      <div className="flex flex-1 items-center space-x-2">
        <IconFilter className="stroke-primary" />
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
          <Button
            variant="ghost"
            onClick={() => table.resetColumnFilters()}
            className="h-8 px-2 lg:px-3"
          >
            {t("ConfigList.Toolbar.Reset")}
            <IconX className="ml-2 h-4 w-4" />
          </Button>
        )}
      </div>
      <DarkModeToggle />
    </div>
  )
}
