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
import { useMessageExchange } from "@/lib/hooks";
import { Badge } from "@/components/ui/badge";
import DeviceIcon, { DeviceIconVariant } from "../icons/DeviceIcon";

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
    accessorKey: "Description",
    size: 1,
    cell: ({ row }) => {
      const label = row.getValue("Description") as String;
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
    accessorKey: "Device",
    header: () => <div className="w-20">Device</div>,
    size: 200,
    cell: ({ row }) => {
      const label = (row.getValue("Device") as String).split("/")[0];
      const serial = (row.getValue("Device") as String).split("/")[1];
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
    accessorKey: "Component",
    header: "Component",
    cell: ({ row }) => {
      const label = row.getValue("Component") as String;
      const type = row.getValue("Type") as String;
      const icon = (
        <DeviceIcon variant={(type ?? "default") as DeviceIconVariant} />
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
      return value.includes(row.getValue(id));
    },
  },
  {
    size: 80,
    accessorKey: "Type",
    header: () => <div className="w-20">Type</div>,
    cell: ({ row }) => {
      const label = row.getValue("Type") as String;
      return <p className="text-md font-semibold">{label}</p>;
    },
    filterFn: (row, id, value) => {
      return value.includes(row.getValue(id));
    },
  },
  {
    size: 300,
    accessorKey: "Tags",
    header: "Tags",
    cell: ({ row }) => {
      const label = row.getValue("Tags") as String;
      return label != "" ? (
        <p className="text-md font-semibold">{label}</p>
      ) : (
        <div className="text-xs text-muted-foreground">
          <Badge>Default</Badge>
        </div>
      );
    },
  },
  {
    size: 100,
    accessorKey: "Status",
    header: "Status",
    cell: ({ row }) => {
      const label = row.getValue("Status") as String;
      return label != "" ? (
        <p className="text-md font-semibold">{label}</p>
      ) : (
        <p className="text-xs text-muted-foreground">
          <Badge className="dark:border-green-600 dark:text-green-500 bg-green-600">normal</Badge>
        </p>
      );
    },
  },
  {
    size: 100,
    accessorKey: "RawValue",
    header: "Raw Value",
  },
  {
    size: 100,
    accessorKey: "ModifiedValue",
    header: "Final Value",
  },
  {
    size: 80,
    id: "actions",
    cell: ({ row }) => {
      const item = row.original;
      const { publish } = useMessageExchange();

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
