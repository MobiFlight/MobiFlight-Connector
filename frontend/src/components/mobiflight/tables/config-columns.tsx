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
import { publishOnMessageExchange } from "@/lib/hooks";
import { Badge } from "@/components/ui/badge";
import DeviceIcon from "../icons/DeviceIcon";
import { DeviceElementType } from "@/types/deviceElements";
import { useLocation, useNavigate, useParams } from "react-router-dom";

export const columns: ColumnDef<IConfigItem>[] = [
  {
    accessorKey: "Active",
    meta: {
      className: "w-[80px]",
    },
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
    meta: {
      className: "hidden xl:table-cell",
    },
    cell: ({ row }) => {
      const label = row.getValue("Description") as string;
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
    meta: {
      className: "w-[200px]",
    },
    accessorKey: "Device",
    header: () => <div>Device</div>,
    cell: ({ row }) => {
      const label = (row.getValue("Device") as string).split("/")[0];
      const serial = (row.getValue("Device") as string).split("/")[1];
      return (
        <>
          <p className="text-md font-semibold truncate">{label}</p>
          <p className="text-xs text-muted-foreground truncate">{serial}</p>
        </>
      );
    },
    filterFn: (row, id, value) => {
      return value.includes(row.getValue(id));
    },
  },
  {
    meta: {
      className: "w-[200px]",
    },
    accessorKey: "Component",
    header: "Component",
    cell: ({ row }) => {
      const label = row.getValue("Component") as string;
      const type = row.getValue("Type") as string;
      const icon = (
        <DeviceIcon variant={(type ?? "default") as DeviceElementType} />
      );
      return (
        <div className="flex flex-row items-center gap-2">
          <div>{icon}</div>
          <div className="flex flex-col">
            <p className="text-md font-semibold truncate">{label}</p>
            <p className="text-xs text-muted-foreground truncate">{type}</p>
          </div>
        </div>
      );
    },
    filterFn: (row, id, value) => {
      return value.includes(row.getValue(id));
    },
  },
  {
    meta: {
      className: "w-20",
    },
    accessorKey: "Type",
    header: () => <div>Type</div>,
    cell: ({ row }) => {
      const label = row.getValue("Type") as string;
      return <p className="text-md font-semibold">{label}</p>;
    },
    filterFn: (row, id, value) => {
      return value.includes(row.getValue(id));
    },
  },
  {
    meta: { 
      className: "hidden 2xl:table-cell w-[150px] 3xl:w-[250px]"
    },
    accessorKey: "Tags",
    header: () => <div>Tags</div>,
    cell: ({ row }) => {
      const label = row.getValue("Tags") as string;
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
    meta: {
      className: "w-[120px]",
    },
    accessorKey: "Status",
    header: "Status",
    cell: ({ row }) => {
      const label = row.getValue("Status") as string;
      return label != "" ? (
        <p className="text-md font-semibold">{label}</p>
      ) : (
        <div className="text-xs text-muted-foreground">
          <Badge className="dark:border-green-600 dark:text-green-500 bg-green-600">normal</Badge>
        </div>
      );
    },
  },
  {
    meta: {
      className: "w-[120px]",
    },
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
    meta: {
      className: "w-[120px]",
    },
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
    meta: {
      className: "w-[80px]",
    },
    id: "actions",
    cell: ({ row }) => {
      const item = row.original;
      const params = useParams()
      const navigate = useNavigate()

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
                  const navigateTo = `/projects/${params.id}/configs/${item.GUID}`
                  console.log(navigateTo)
                  navigate(navigateTo)
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
