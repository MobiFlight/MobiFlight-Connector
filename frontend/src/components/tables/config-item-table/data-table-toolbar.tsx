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

  const devices = [...new Set(items.map((item) => item.ModuleSerial))].map(
    (serial) => {
      const label = serial?.split("/")[0]
      return {
        label: !isEmpty(label) ? (
          label
        ) : (
          t(`ConfigList.Toolbar.NotSet`)
        ),
        value: serial,
        icon: isEmpty(label) ? IconBan : undefined,
      }
    },
  )

  const components = [
    ...new Set(items.map((item) => item.Device?.Name ?? "-")),
  ].map((component) => ({
    label: component,
    value: component,
  }))

  const types = [...new Set(items.map((item) => item.Device?.Type ?? "-"))].map(
    (type) => {
      const labelRaw = type
      const label = t(`Types.${labelRaw}`)
      return {
        label: label,
        value: type,
      }
    },
  )

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
        {table.getColumn("ModuleSerial") && (
          <DataTableFacetedFilter
            column={table.getColumn("ModuleSerial")}
            title={t("ConfigList.Toolbar.Filter.Device")}
            options={devices}
          />
        )}
        {table.getColumn("Type") && (
          <DataTableFacetedFilter
            column={table.getColumn("Type")}
            title={t("ConfigList.Toolbar.Filter.Type")}
            options={types}
          />
        )}
        {table.getColumn("Device") && (
          <DataTableFacetedFilter
            column={table.getColumn("Device")}
            title={t("ConfigList.Toolbar.Filter.Name")}
            options={components}
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
