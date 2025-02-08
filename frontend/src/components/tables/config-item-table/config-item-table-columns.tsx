import { Button } from "@/components/ui/button"
import { IConfigItem } from "@/types"
import { ColumnDef } from "@tanstack/react-table"
import {
  IconArrowsSort,
  IconBan,
  IconBuildingBroadcastTower,
  IconMathSymbols,
} from "@tabler/icons-react"
// import { Badge } from "@/components/ui/badge";
import DeviceIcon from "@/components/icons/DeviceIcon"
import { DeviceElementType } from "@/types/deviceElements"
import { IDeviceConfig } from "@/types/config"
import { isEmpty } from "lodash"
import { useTranslation } from "react-i18next"
import {
  ConfigItemTableActionsCell,
  ConfigItemTableActiveCell,
  ConfigItemTableActiveHeader,
  ConfigItemTableNameCell,
  ConfigItemTableStatusCell,
} from "./items"

export const columns: ColumnDef<IConfigItem>[] = [
  {
    accessorKey: "Active",
    header: ConfigItemTableActiveHeader,
    cell: ConfigItemTableActiveCell,
  },
  {
    accessorKey: "Name",
    size: 1,
    header: ({ column }) => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return (
        <div className="flex w-auto grow items-center gap-4">
          <span>{t("ConfigList.Header.Name")}</span>
          <Button
            className="h-auto w-auto p-1"
            variant="ghost"
            onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
          >
            <IconArrowsSort className="h-4 w-4" />
          </Button>
        </div>
      )
    },
    cell: ConfigItemTableNameCell,
  },
  {
    accessorKey: "ModuleSerial",
    header: () => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return (
        <div className="hidden w-32 lg:block">
          {t("ConfigList.Header.Device")}
        </div>
      )
    },
    cell: ({ row }) => {
      const label = (row.getValue("ModuleSerial") as string).split("/")[0]
      const serial = (row.getValue("ModuleSerial") as string).split("/")[1]
      return !isEmpty(label) ? (
        <div className="hidden w-32 flex-col lg:flex">
          <p className="text-md truncate font-semibold">{label}</p>
          <p className="truncate text-xs text-muted-foreground">{serial}</p>
        </div>
      ) : (
        <span className="item-center hidden flex-row gap-2 text-slate-400 lg:flex">
          <IconBan />
          <span>not set</span>
        </span>
      )
    },
    filterFn: (row, id, value) => {
      return value.includes(row.getValue(id))
    },
  },
  {
    accessorKey: "Device",
    header: () => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return (
        <div className="w-8 truncate md:w-32">
          {t("ConfigList.Header.Component")}
        </div>
      )
    },
    cell: ({ row }) => {
      const label = (row.getValue("Device") as IDeviceConfig)?.Name ?? "-"
      const type = (row.getValue("Device") as IDeviceConfig)?.Type ?? "-"
      const icon = (
        <DeviceIcon
          disabled={!row.getValue("Active") as boolean}
          variant={(type ?? "default") as DeviceElementType}
        />
      )
      return type != "-" ? (
        <div className="flex flex-row items-center gap-2 md:w-32">
          <div>{icon}</div>
          <div className="hidden w-full flex-col md:flex">
            <p className="text-md truncate font-semibold">{label}</p>
            <p className="truncate text-xs text-muted-foreground">{type}</p>
          </div>
        </div>
      ) : (
        <div className="item-center flex flex-row gap-2 text-slate-400">
          <IconBan />
          <span>not set</span>
        </div>
      )
    },
    filterFn: (row, id, value) => {
      return value.includes((row.getValue(id) as IDeviceConfig)?.Name ?? "-")
    },
  },
  {
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
      return value.includes(
        (row.getValue("Device") as IDeviceConfig)?.Type ?? "-",
      )
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
    size: 100,
    accessorKey: "Status",
    header: () => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return (
        <div className="w-26">{t("ConfigList.Header.Status")}</div>
      )
    },
    cell: ConfigItemTableStatusCell,
  },
  {
    size: 100,
    accessorKey: "RawValue",
    header: () => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return (
        <div className="hidden w-16 md:visible md:block lg:w-24">
          {t("ConfigList.Header.RawValue")}
        </div>
      )
    },
    cell: ({ row }) => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      const label = row.getValue("RawValue") as string
      return (
        <div className="text-md hidden w-16 truncate md:visible md:block lg:w-24 xl:w-32">
          {!isEmpty(label) ? (
            label
          ) : (
            <div className="item-center flex flex-row gap-2 text-slate-300">
              <IconBuildingBroadcastTower className="animate-pulse" />
              <span className="truncate">{t("ConfigList.Cell.Waiting")}</span>
            </div>
          )}
        </div>
      )
    },
  },
  {
    size: 100,
    accessorKey: "Value",
    header: () => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return (
        <div className="hidden w-16 md:block lg:w-24">
          {t("ConfigList.Header.FinalValue")}
        </div>
      )
    },
    cell: ({ row }) => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      const label = row.getValue("Value") as string
      return (
        <div className="text-md hidden w-16 truncate md:block lg:w-24 xl:w-32">
          {!isEmpty(label) ? (
            label
          ) : (
            <div className="item-center flex flex-row gap-2 text-slate-300">
              <IconMathSymbols className="animate-pulse" />
              <span className="truncate">{t("ConfigList.Cell.Waiting")}</span>
            </div>
          )}
        </div>
      )
    },
  },
  {
    id: "actions",
    header: () => {
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const { t } = useTranslation()
      return (
        <div className="w-10 truncate sm:w-12">
          {t("ConfigList.Header.Actions")}
        </div>
      )
    },
    cell: ConfigItemTableActionsCell,
  },
]
