import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
} from "@/components/ui/dropdown-menu"
import { Switch } from "@/components/ui/switch"
import { IConfigItem } from "@/types"
import { DropdownMenuTrigger } from "@radix-ui/react-dropdown-menu"
import { ColumnDef } from "@tanstack/react-table"
import {
  IconArrowsSort,
  IconBan,
  IconDots,
  IconPlugConnectedX,
} from "@tabler/icons-react"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
// import { Badge } from "@/components/ui/badge";
import DeviceIcon from "@/components/icons/DeviceIcon"
import { DeviceElementType } from "@/types/deviceElements"
import { IDeviceConfig } from "@/types/config"
import {
  CommandUpdateConfigItem,
  CommandConfigContextMenu,
} from "@/types/messages"
import { isEmpty } from "lodash"

export const columns: ColumnDef<IConfigItem>[] = [
  {
    accessorKey: "Active",
    size: 50,
    header: () => <div className="w-20 text-center">Active</div>,
    cell: ({ row }) => {
      const { publish } = publishOnMessageExchange()
      const item = row.original as IConfigItem

      return (
        <div className="w-20 text-center">
          <Switch
            className="h-5 w-10 dark:bg-gray-800 dark:data-[state=checked]:bg-gray-700"
            checked={row.getValue("Active") as boolean}
            onClick={() => {
              item.Active = !item.Active
              publish({
                key: "CommandUpdateConfigItem",
                payload: { item: item },
              } as CommandUpdateConfigItem)
            }}
          />
        </div>
      )
    },
  },
  {
    accessorKey: "Name",
    size: 1,
    cell: ({ row }) => {
      const label = row.getValue("Name") as string
      return (
        <div>
          <p className="font-semibold">{label}</p>
        </div>
      )
    },
    header: ({ column }) => {
      return (
        <div className="flex items-center gap-4">
          <span>Description</span>
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
  },
  {
    accessorKey: "ModuleSerial",
    header: () => <div className="w-20">Device</div>,
    size: 200,
    cell: ({ row }) => {
      const label = (row.getValue("ModuleSerial") as string).split("/")[0]
      const serial = (row.getValue("ModuleSerial") as string).split("/")[1]
      return (
        <>
          <p className="text-md w-32 truncate font-semibold md:w-64">
            {!isEmpty(label) ? (
              label
            ) : (
              <span className="item-center flex flex-row gap-2 text-slate-400">
                <IconBan />
                <span>not set</span>
              </span>
            )}
          </p>
          <p className="w-32 truncate text-xs text-muted-foreground md:w-64">
            {serial}
          </p>
        </>
      )
    },
    filterFn: (row, id, value) => {
      return value.includes(row.getValue(id))
    },
  },
  {
    size: 200,
    accessorKey: "Device",
    header: "Component",
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
        <div className="flex flex-row items-center gap-2">
          <div>{icon}</div>
          <div className="flex flex-col">
            <p className="text-md font-semibold">{label}</p>
            <p className="text-xs text-muted-foreground">{type}</p>
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
    header: () => <div className="w-20">Component Type</div>,
    cell: ({ row }) => {
      const label = (row.getValue("Device") as IDeviceConfig)?.Type ?? "-"
      return <p className="text-md font-semibold">test {label}</p>
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
  // {
  //   size: 100,
  //   accessorKey: "Status",
  //   header: "Status",
  //   cell: ({ row }) => {
  //     const label = row.getValue("Status") as string;
  //     return label != "" ? (
  //       <p className="text-md font-semibold">{label}</p>
  //     ) : (
  //       <div className="text-xs text-muted-foreground">
  //         <Badge className="dark:border-green-600 dark:text-green-500 bg-green-600">normal</Badge>
  //       </div>
  //     );
  //   },
  // },
  {
    size: 100,
    accessorKey: "RawValue",
    header: () => (
      <div className="w-16 sm:hidden md:visible md:block lg:w-24">
        Raw Value
      </div>
    ),
    cell: ({ row }) => {
      const label = row.getValue("RawValue") as string
      return (
        <div className="text-md w-16 truncate sm:hidden md:visible md:block lg:w-24">
          {!isEmpty(label) ? (
            label
          ) : (
            <div className="item-center flex flex-row gap-2 text-slate-400">
              <IconPlugConnectedX />
              <span>waiting</span>
            </div>
          )}
        </div>
      )
    },
  },
  {
    size: 100,
    accessorKey: "Value",
    header: () => (
      <div className="w-16 sm:hidden md:block lg:w-24">Final Value</div>
    ),
    cell: ({ row }) => {
      const label = row.getValue("Value") as string
      return (
        <div className="text-md w-16 truncate sm:hidden md:block lg:w-24">
          {!isEmpty(label) ? (
            label
          ) : (
            <div className="item-center flex flex-row gap-2 text-slate-400">
              <IconPlugConnectedX />
              <span>waiting</span>
            </div>
          )}
        </div>
      )
    },
  },
  {
    size: 80,
    id: "actions",
    header: "Actions",
    cell: ({ row }) => {
      const item = row.original
      const { publish } = publishOnMessageExchange()

      return (
        <div className="relative">
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="ghost" className="h-8 w-8 p-0">
                <span className="sr-only">Open menu</span>
                <IconDots className="h-4 w-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuLabel>Actions</DropdownMenuLabel>
              <DropdownMenuItem
                onClick={() => {
                  publish({
                    key: "CommandConfigContextMenu",
                    payload: { action: "edit", item: item },
                  } as CommandConfigContextMenu)
                }}
              >
                Edit
              </DropdownMenuItem>
              <DropdownMenuItem
                onClick={() => {
                  publish({
                    key: "CommandConfigContextMenu",
                    payload: { action: "delete", item: item },
                  } as CommandConfigContextMenu)
                }}
              >
                Remove
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                onClick={() => {
                  publish({
                    key: "CommandConfigContextMenu",
                    payload: { action: "duplicate", item: item },
                  } as CommandConfigContextMenu)
                }}
              >
                Duplicate
              </DropdownMenuItem>
              {/* <DropdownMenuItem>Copy</DropdownMenuItem>
              <DropdownMenuItem>Paste</DropdownMenuItem> */}
              <DropdownMenuSeparator />
              <DropdownMenuItem
                onClick={() => {
                  publish({
                    key: "CommandConfigContextMenu",
                    payload: { action: "test", item: item },
                  } as CommandConfigContextMenu)
                }}
              >
                Test
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      )
    },
  },
]
