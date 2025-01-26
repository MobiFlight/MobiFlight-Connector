import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
} from "@/components/ui/dropdown-menu";
import { Switch } from "@/components/ui/switch";
import { IConfigItem } from "@/types";
import { DropdownMenuTrigger } from "@radix-ui/react-dropdown-menu";
import { ColumnDef } from "@tanstack/react-table";
import { IconArrowsSort, IconDots } from "@tabler/icons-react";
import { publishOnMessageExchange } from "@/lib/hooks/appMessage";
// import { Badge } from "@/components/ui/badge";
import DeviceIcon from "@/components/icons/DeviceIcon";
import { DeviceElementType } from "@/types/deviceElements";
import { IDeviceConfig } from "@/types/config";

export const columns: ColumnDef<IConfigItem>[] = [
  {
    accessorKey: "Active",
    size: 75,
    header: () => (
      <div className="text-center">Active</div>
    ),
    cell: ({ row }) => {
      return (
        <div className="text-center">
          <Switch
            className="dark:bg-gray-800 dark:data-[state=checked]:bg-gray-700"
            checked={row.getValue("Active") as boolean}
          />
        </div>
      );
    },
  },
  {
    accessorKey: "Name",
    size: 1,
    cell: ({ row }) => {
      const label = row.getValue("Name") as string;
      return (
        <div>
          <p className="font-semibold">{label}</p>
        </div>
      );
    },
    header: ({ column }) => {
      return (
        <div className="flex items-center gap-4">
          <span>Description</span>
          <Button
            className="p-1 h-auto w-auto"
            variant="ghost"
            onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}>
            <IconArrowsSort className="h-4 w-4" />
          </Button>
        </div>
      );
    },
  },
  {
    accessorKey: "ModuleSerial",
    header: () => <div className="w-20">Device</div>,
    size: 200,
    cell: ({ row }) => {
      const label = (row.getValue("ModuleSerial") as string).split("/")[0];
      const serial = (row.getValue("ModuleSerial") as string).split("/")[1];
      return (
        <>
          <p className="text-md font-semibold">{label}</p>
          <p className="text-xs text-muted-foreground">{serial}</p>
        </>
      );
    },
    filterFn: (row, id, value) => {
      return value.includes(row.getValue(id));
    },
  },
  {
    size: 200,
    accessorKey: "Device",
    header: "Component",
    cell: ({ row }) => {
      const label = (row.getValue("Device") as IDeviceConfig)?.Name ?? "-";
      const type =  (row.getValue("Device") as IDeviceConfig)?.Type ?? "-";
      const icon = (
        <DeviceIcon variant={(type ?? "default") as DeviceElementType} />
      );
      return (
        <div className="flex flex-row items-center gap-2">
          <div>{icon}</div>
          <div className="flex flex-col">
            <p className="text-md font-semibold">{label}</p>
            <p className="text-xs text-muted-foreground">{type}</p>
          </div>
        </div>
      );
    },
    filterFn: (row, id, value) => {
      return value.includes((row.getValue(id)as IDeviceConfig).Name);
    },
  },
  {
    size: 80,
    accessorKey: "Type",
    header: () => <div className="w-20">Type</div>,
    cell: ({ row }) => {
      const label = row.getValue("Type") as string;
      return <p className="text-md font-semibold">{label}</p>;
    },
    filterFn: (row, id, value) => {
      return value.includes(row.getValue(id));
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
    header: "Raw Value",
    cell: ({ row }) => {
      const label = row.getValue("RawValue") as string;
      return (
        <div className="text-md truncate">{label}</div>
      )
    },
  },
  {
    size: 100,
    accessorKey: "ModifiedValue",
    header: "Final Value",
    cell: ({ row }) => {
      const label = row.getValue("ModifiedValue") as string;
      return (
        <div className="text-md truncate">{label}</div>
      )
    },
  },
  {
    size: 80,
    id: "actions",
    header: "Actions",
    cell: ({ row }) => {
      const item = row.original;
      const { publish } = publishOnMessageExchange();

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
                  publish({ key: "config.edit", payload: item });
                }}
              >
                Edit
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem>Duplicate</DropdownMenuItem>
              <DropdownMenuItem>Copy</DropdownMenuItem>
              <DropdownMenuItem>Paste</DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem>Test</DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      );
    },
  },
];
