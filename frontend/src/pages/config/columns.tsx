import { Switch } from "@/components/ui/switch"
import { IConfigItem } from "@/types"
import { ColumnDef } from "@tanstack/react-table"

export const columns: ColumnDef<IConfigItem>[] = [
    {
        accessorKey: "Active",
        header: () => <div className="text-slate-900">Active</div>,
        cell: ({ row }) => {
            return <Switch checked={row.getValue("Active") as boolean} />
          },
    },
    {
        accessorKey: "Description",
        header: "Description",
    },
    {
        accessorKey: "Device",
        header: "Device",
    },
    {
        accessorKey: "Component",
        header: "Component",
    },
    {
        accessorKey: "Type",
        header: "Type",
    },
    {
        accessorKey: "Tags",
        header: "Tags",
    },
    {
        accessorKey: "Status",
        header: "Status",
    },
    {
        accessorKey: "RawValue",
        header: "Raw Value",
    },
    {
        accessorKey: "FinalValue",
        header: "Final Value",
    }
]