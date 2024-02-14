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
import { useMessageExchange } from "@/lib/hooks"

export const columns: ColumnDef<IConfigItem>[] = [
    {
        accessorKey: "Active",
        header: () => <div className="text-center">Active</div>,
        cell: ({ row }) => {
            return <div className="text-center"><Switch className="dark:bg-gray-800 dark:data-[state=checked]:bg-gray-700" checked={row.getValue("Active") as boolean} /></div>
        },
    },
    {
        accessorKey: "Description",
        cell: ({ row }) => {
            const label = (row.getValue("Description") as String)
            return <div><p className="font-semibold">{label}</p></div>
        },
        header: ({ column }) => {
            return (
                <div className="border-2"><Button
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
        header: () => <div className="w-20">Device</div>,
        size: 80,
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
        cell: ({ row }) => {
            const label = (row.getValue("Component") as String)
            const type = (row.getValue("Type") as String)
            return <><p className="text-md font-semibold">{label}</p><p className="text-xs text-muted-foreground">{type}</p></>
        },
        filterFn: (row, id, value) => {
            return value.includes(row.getValue(id))
        },
    },
    {
        accessorKey: "Type",
        header: () => <div className="w-20">Type</div>,
        cell: ({ row }) => {
            const label = (row.getValue("Type") as String)
            return <p className="text-md font-semibold">{label}</p>
        },
        filterFn: (row, id, value) => {
            return value.includes(row.getValue(id))
        },
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
        accessorKey: "ModifiedValue",
        header: "Final Value",
    },
    {
        id: "actions",
        cell: ({ row }) => {
            const item = row.original
            const { publish } = useMessageExchange()
            
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
                            <DropdownMenuItem onClick={()=>{
                                publish({ key: "config.edit", payload: item })
                            }}>Edit</DropdownMenuItem>
                            <DropdownMenuSeparator />
                            <DropdownMenuItem>Duplicate</DropdownMenuItem>
                            <DropdownMenuItem>Copy</DropdownMenuItem>
                            <DropdownMenuItem>Paste</DropdownMenuItem>
                            <DropdownMenuSeparator />
                            <DropdownMenuItem>Test</DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                </div>
            )
        },
    }
]