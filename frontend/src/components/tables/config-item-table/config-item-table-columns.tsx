import { IConfigItem } from "@/types"
import { ColumnDef } from "@tanstack/react-table"
import { IDeviceConfig } from "@/types/config"
import { isEmpty } from "lodash"
import { useTranslation } from "react-i18next"
import {
  ConfigItemTableActionsCell,
  ConfigItemTableActiveCell,
  ConfigItemTableActiveHeader,
  ConfigItemTableNameCell,
  ConfigItemTableStatusCell,
  ConfigItemTableControllerCell,
  ConfigItemTableDeviceCell,
  ConfigItemTableRawValueCell,
} from "./items"
import ConfigItemTableFinalValueCell from "./items/ConfigItemTableFinalValueCell"

export const columns: ColumnDef<IConfigItem>[] = [
  {
    meta: {
      className: "w-24",
    },
    accessorKey: "Active",
    header: ConfigItemTableActiveHeader,
    cell: ConfigItemTableActiveCell,
  },
  {
    accessorKey: "Name",
    size: 1,
    header: () => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return (
        <div className="flex items-center gap-4">
          <span>{t("ConfigList.Header.Name")}</span>
          {/* <Button
            className="h-auto w-auto p-1"
            variant="ghost"
            onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
          >
            <IconArrowsSort className="h-4 w-4" />
          </Button> */}
        </div>
      )
    },
    cell: ConfigItemTableNameCell,
  },
  {
    accessorKey: "ConfigType",
    size: 1,
    cell: ({ row }) => {
      return <p> {(row.original as IConfigItem).Type} </p>
    },
    filterFn: (row, _, value) => {
      return value.includes(row.original.Type)
    },
  },
  {
    meta: {
      className: "hidden w-44 3xl:w-72 lg:table-cell",
    },
    accessorKey: "ModuleSerial",
    header: () => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return <div className="text-center">{t("ConfigList.Header.Device")}</div>
    },
    cell: ConfigItemTableControllerCell,
    filterFn: (row, id, value) => {
      return value.includes(row.getValue(id))
    },
  },
  {
    meta: {
      className: "w-12 lg:w-44",
    },
    accessorKey: "Device",
    header: () => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return <div className="text-center truncate">{t("ConfigList.Header.Component")}</div>
    },
    cell: ConfigItemTableDeviceCell,
    filterFn: (row, _, value) => {
      const item = row.original as IConfigItem
      const name =
        (item.Device as IDeviceConfig)?.Name ??
        (!isEmpty(item.DeviceName) ? item.DeviceName : "-")
      return value.includes(name)
    },
  },
  {
    // Invisible, see ConfigItemTable columnVisibility
    meta: {
      className: "hidden",
    },
    size: 80,
    accessorKey: "Type",
    header: () => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return (
        <div className="w-20">
          Component Type{t("ConfigList.Header.Component")}
        </div>
      )
    },
    cell: ({ row }) => {
      const label = (row.getValue("Device") as IDeviceConfig)?.Type ?? "-"
      return <p className="text-md font-semibold">{label}</p>
    },
    filterFn: (row, _, value) => {
      const item = row.original as IConfigItem
      const type =
        (item.Device as IDeviceConfig)?.Type ??
        (!isEmpty(item.DeviceType) ? item.DeviceType : "-")
      return value.includes(type)
    },
  },
  // {
  //   size: 300,
  //   accessorKey: "Tags",
  //   header: "Tags",
  //   cell: ({ row }) => {
  //     const label = row.getValue("Tags") as string;
  //     return label != "" ? (
  //       <p className="text-md font-semibold">{label}</p>
  //     ) : (
  //       <div className="text-xs text-muted-foreground">
  //         <Badge>Default</Badge>
  //       </div>
  //     );
  //   },
  // },
  {
    meta: {
      className: "w-24",
    },
    size: 100,
    accessorKey: "Status",
    header: () => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return <div className="text-center">{t("ConfigList.Header.Status")}</div>
    },
    cell: ConfigItemTableStatusCell,
  },
  {
    meta: {
      className: "w-16 lg:w-24 xl:w-32",
    },
    accessorKey: "RawValue",
    header: () => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return <div className="text-center">{t("ConfigList.Header.RawValue")}</div>
    },
    cell: ConfigItemTableRawValueCell,
  },
  {
    meta: {
      className: "w-16 lg:w-24 xl:w-32",
    },
    accessorKey: "Value",
    header: () => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return <div className="text-center">{t("ConfigList.Header.FinalValue")}</div>
    },
    cell: ConfigItemTableFinalValueCell,
  },
  {
    meta: {
      className: "w-20",
    },
    id: "actions",
    header: () => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return <div className="text-center truncate">{t("ConfigList.Header.Actions")}</div>
    },
    cell: ConfigItemTableActionsCell,
  },
]
