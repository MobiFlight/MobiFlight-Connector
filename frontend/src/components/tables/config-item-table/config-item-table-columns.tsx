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
  IconAlertSquareRounded,
  IconArrowsSort,
  IconBan,
  IconBuildingBroadcastTower,
  IconCircleCheck,
  IconDots,
  IconEdit,
  IconFlask,
  IconMathSymbols,
  IconPlugConnectedX,
  IconRouteOff,
  IconX,
} from "@tabler/icons-react"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
// import { Badge } from "@/components/ui/badge";
import DeviceIcon from "@/components/icons/DeviceIcon"
import { DeviceElementType } from "@/types/deviceElements"
import {
  ConfigItemStatusType,
  IDeviceConfig,
  IDictionary,
} from "@/types/config"
import {
  CommandUpdateConfigItem,
  CommandConfigContextMenu,
} from "@/types/commands"
import { isEmpty } from "lodash"
import { useCallback, useEffect, useState } from "react"
import { Input } from "@/components/ui/input"

export const columns: ColumnDef<IConfigItem>[] = [
  {
    accessorKey: "Active",
    header: () => <div className="w-20 text-center select-none">Active</div>,
    cell: ({ row }) => {
      const { publish } = publishOnMessageExchange()
      const item = row.original as IConfigItem

      return (
        <div className="w-20 text-center select-none">
          <Switch
            className="dark:bg-gray-800 dark:data-[state=checked]:bg-gray-700"
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
    header: ({ column }) => {
      return (
        <div className="flex w-auto grow items-center gap-4 select-none">
          <span>Name / Description</span>
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
    cell: ({ row }) => {
      const { publish } = publishOnMessageExchange()
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const [isEditing, setIsEditing] = useState(false)
      // eslint-disable-next-line react-hooks/rules-of-hooks
      const [label, setLabel] = useState(row.getValue("Name") as string)
      const realLabel = row.getValue("Name") as string

      const toggleEdit = () => {
        setIsEditing(!isEditing)
      }

      const moduleName =
        (row.getValue("ModuleSerial") as string).split("/")[0] ?? "not set"
      const deviceName = (row.getValue("Device") as IDeviceConfig)?.Name ?? "-"

      // eslint-disable-next-line react-hooks/rules-of-hooks
      const saveChanges = useCallback(() => {
        const item = row.original as IConfigItem
        item.Name = label
        console.log(item)
        publish({
          key: "CommandUpdateConfigItem",
          payload: { item: item },
        } as CommandUpdateConfigItem)
      }, [label, row, publish])

      // eslint-disable-next-line react-hooks/rules-of-hooks
      useEffect(() => {
        setLabel(realLabel)
      }, [realLabel])

      return (
        <div className="group flex w-auto cursor-pointer flex-row items-center gap-1 select-none">
          {!isEditing ? (
            <div className="flex flex-col">
              <div className="flex flex-row items-center gap-1">
                <p className="max-w-60 truncate font-semibold px-0">{label}</p>
                <IconEdit
                  role="button"
                  aria-label="Edit"
                  onClick={toggleEdit}
                  className="ml-2 opacity-0 transition-opacity delay-300 ease-in group-hover:opacity-100 group-hover:delay-100 group-hover:ease-out"
                />
              </div>
              <p className="w-60 truncate text-xs text-slate-500 md:hidden">
                {moduleName} - {deviceName}
              </p>
            </div>
          ) : (
            <div className="flex flex-row items-center gap-1">
              <Input
                type="text"
                value={label}
                className="text-sm h-6 md:h-8 px-2"
                onChange={(e) => setLabel(e.target.value)}
              />
              <IconCircleCheck
                className="stroke-green-700"
                role="button"
                aria-label="Save"
                onClick={() => {
                  saveChanges()
                  toggleEdit()
                }}
              />
              <IconX
                role="button"
                aria-label="Discard"
                onClick={() => {
                  setLabel(row.getValue("Name") as string)
                  toggleEdit()
                }}
              />
            </div>
          )}
        </div>
      )
    },
  },
  {
    accessorKey: "ModuleSerial",
    header: () => <div className="hidden w-32 lg:block select-none">Device</div>,
    cell: ({ row }) => {
      const label = (row.getValue("ModuleSerial") as string).split("/")[0]
      const serial = (row.getValue("ModuleSerial") as string).split("/")[1]
      return !isEmpty(label) ? (
        <div className="hidden w-32 flex-col lg:flex select-none">
          <p className="text-md truncate font-semibold">{label}</p>
          <p className="truncate text-xs text-muted-foreground">{serial}</p>
        </div>
      ) : (
        <span className="item-center hidden flex-row gap-2 text-slate-400 lg:flex select-none">
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
    header: () => <div className="w-8 truncate md:w-32 select-none">Component</div>,
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
        <div className="flex flex-row items-center gap-2 md:w-32 select-none">
          <div>{icon}</div>
          <div className="hidden w-full flex-col md:flex">
            <p className="text-md truncate font-semibold">{label}</p>
            <p className="truncate text-xs text-muted-foreground">{type}</p>
          </div>
        </div>
      ) : (
        <div className="item-center flex flex-row gap-2 text-slate-400 select-none">
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
    header: () => <div className="w-20 select-none">Component Type</div>,
    cell: ({ row }) => {
      const label = (row.getValue("Device") as IDeviceConfig)?.Type ?? "-"
      return <p className="text-md font-semibold select-none">{label}</p>
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
    header: () => <div className="w-26 select-none">Status</div>,
    cell: ({ row }) => {
      const Status = row.getValue("Status") as IDictionary<
        string,
        ConfigItemStatusType
      >
      const Precondition = Status && !isEmpty(Status["Precondition"])
      const Source = Status && !isEmpty(Status["Source"])
      const Modifier = Status && !isEmpty(Status["Modifier"])
      const Device = Status && !isEmpty(Status["Device"])
      const Test = Status && !isEmpty(Status["Test"])
      const ConfigRef = Status && !isEmpty(Status["ConfigRef"])

      return (
        <div className="flex w-28 flex-row gap-0 select-none">
          <IconAlertSquareRounded
            role="status"
            aria-disabled={!Precondition}
            className={!Precondition ? "stroke-slate-100" : "stroke-red-700"}
          >
            normal
          </IconAlertSquareRounded>
          <IconBuildingBroadcastTower
            role="status"
            aria-disabled={!Source}
            className={!Source ? "stroke-slate-100" : "stroke-red-700"}
          >
            normal
          </IconBuildingBroadcastTower>
          <IconPlugConnectedX
            role="status"
            aria-disabled={!Device}
            className={!Device ? "stroke-slate-100" : "stroke-red-700"}
          >
            normal
          </IconPlugConnectedX>
          <IconMathSymbols
            aria-disabled={!Modifier}
            role="status"
            className={!Modifier ? "stroke-slate-100" : "stroke-red-700"}
          >
            normal
          </IconMathSymbols>
          <IconFlask
            aria-disabled={!Test}
            role="status"
            className={!Test ? "stroke-slate-100" : "stroke-red-700"}
          >
            normal
          </IconFlask>
          <IconRouteOff
            aria-disabled={!ConfigRef}
            role="status"
            className={!ConfigRef ? "stroke-slate-100" : "stroke-red-700"}
          >
            normal
          </IconRouteOff>
        </div>
      )
    },
  },
  {
    size: 100,
    accessorKey: "RawValue",
    header: () => (
      <div className="hidden w-16 md:visible md:block lg:w-24 select-none">Raw Value</div>
    ),
    cell: ({ row }) => {
      const label = row.getValue("RawValue") as string
      return (
        <div className="text-md hidden w-16 truncate md:visible md:block lg:w-24 select-none">
          {!isEmpty(label) ? (
            label
          ) : (
            <div className="item-center flex flex-row gap-2 text-slate-300">
              <IconBuildingBroadcastTower className="animate-pulse" />
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
      <div className="hidden w-16 md:block lg:w-24 select-none">Final Value</div>
    ),
    cell: ({ row }) => {
      const label = row.getValue("Value") as string
      return (
        <div className="text-md hidden w-16 truncate md:block lg:w-24 select-none">
          {!isEmpty(label) ? (
            label
          ) : (
            <div className="item-center flex flex-row gap-2 text-slate-300">
              <IconMathSymbols className="animate-pulse" />
              <span>waiting</span>
            </div>
          )}
        </div>
      )
    },
  },
  {
    id: "actions",
    header: () => <div className="w-10 truncate sm:w-12 select-none">Actions</div>,
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
