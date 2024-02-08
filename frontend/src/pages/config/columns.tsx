import { Button } from "@/components/ui/button"
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator } from "@/components/ui/dropdown-menu"
import { Switch } from "@/components/ui/switch"
import { IConfigItem } from "@/types"
import { DropdownMenuTrigger } from "@radix-ui/react-dropdown-menu"
import { ColumnDef } from "@tanstack/react-table"
import { IconDots } from "@tabler/icons-react"

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
    },
    {
        id: "actions",
        cell: ({ row }) => {
            const payment = row.original

            return (
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button variant="ghost" className="h-8 w-8 p-0">
                            <span className="sr-only">Open menu</span>
                            <IconDots className="h-4 w-4" />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                        <DropdownMenuLabel>Actions</DropdownMenuLabel>
                        <DropdownMenuItem>Edit</DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem>Duplicate</DropdownMenuItem>
                        <DropdownMenuItem>Copy</DropdownMenuItem>
                        <DropdownMenuItem>Paste</DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem>Test</DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            )
        },
    }
]