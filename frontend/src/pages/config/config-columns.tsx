import { Button } from "@/components/ui/button"
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator } from "@/components/ui/dropdown-menu"
import { Switch } from "@/components/ui/switch"
import { IConfigItem } from "@/types"
import { DropdownMenuTrigger } from "@radix-ui/react-dropdown-menu"
import { ColumnDef } from "@tanstack/react-table"
import {
    IconArrowsSort
    , IconDots
} from "@tabler/icons-react"

const publishMessage = (message: any) => {
    window.chrome?.webview?.postMessage(message)
}

export const columns: ColumnDef<IConfigItem>[] = [
    {
        accessorKey: "Active",
        header: () => <div className="w-11 text-center">Active</div>,
        cell: ({ row }) => {
            return <Switch className="w-11 dark:bg-gray-800 dark:data-[state=checked]:bg-gray-700" checked={row.getValue("Active") as boolean} />
        },
    },
    {
        accessorKey: "Description",
        cell: ({ row }) => {
            const label = (row.getValue("Description") as String)
            return <div className="grow"><p className="font-semibold">{label}</p></div>
        },
        header: ({ column }) => {
            return (
                <div className="grow"><Button
                    variant="ghost"
                    onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
                >
                    Description
                    <IconArrowsSort className="ml-2 h-4 w-4" />
                </Button>
                </div>
            )
        }
    },
    {
        accessorKey: "Device",
        header: "Device",
        cell: ({ row }) => {
            const label = (row.getValue("Device") as String).split("/")[0]
            const serial = (row.getValue("Device") as String).split("/")[1]
            return <><p className="text-md font-semibold">{label}</p><p className="text-xs text-muted-foreground">{serial}</p></>
        },
        filterFn: (row, id, value) => {
            return value.includes(row.getValue(id))
        },
    },
    {
        accessorKey: "Component",
        header: "Component",
        filterFn: (row, id, value) => {
            return value.includes(row.getValue(id))
        },
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
            const item = row.original

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
                        <DropdownMenuItem onClick={()=>{
                            publishMessage(
                                { 
                                    key: "config.edit", 
                                    payload: item 
                                }
                            )
                        }}>Edit</DropdownMenuItem>
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