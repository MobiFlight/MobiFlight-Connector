import { Button } from "@/components/ui/button"
import { ILogMessage } from "@/types"
import { ColumnDef } from "@tanstack/react-table"
import { IconAlertCircle, IconArrowsSort } from "@tabler/icons-react"
import { cn } from "@/lib/utils"

const determineLogLevelColor = (level: string) => {
    level = level.toLowerCase()
    return level === "error" ? "text-red-600" : level==="warn" ? "text-yellow-600": level==="info" ? "text-blue-600" : "text-gray-400"
}

export const columns: ColumnDef<ILogMessage>[] = [
    {
        accessorKey: "Severity",
        header: () => <div>Level</div>,
        cell: ({ row }) => {
            const alertClassName = determineLogLevelColor(row.getValue("Severity"))
            return (<div className="justify-center flex flex-row"><IconAlertCircle className={cn(alertClassName, "dark:bg-gray-800 dark:data-[state=checked]:bg-gray-700")} /></div>)
        },
        filterFn: (row, id, value) => {
            return value.includes(row.getValue(id))
        },
    },
    {
        accessorKey: "Timestamp",
        cell: ({ row }) => {
            const label = (row.getValue("Timestamp") as String)
            return <><p className="font-semibold">{label}</p></>
        },
        header: ({ column }) => {
            return (
                <Button
                    variant="ghost"
                    onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
                >
                    Time
                    <IconArrowsSort className="opacity-0 group-hover/header:opacity-100 ml-2 h-4 w-4 animate-in animate-out duration-300 " />
                </Button>
            )
        }
    },
    {
        accessorKey: "Message",
        header: "Message",
        cell: ({ row }) => {
            return <><p className="text-md font-semibold">{row.getValue("Message")}</p></>
        }
    },
]