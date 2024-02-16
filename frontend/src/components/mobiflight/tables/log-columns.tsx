import { Button } from "@/components/ui/button"
import { ILogMessage } from "@/types"
import { ColumnDef } from "@tanstack/react-table"
import { IconAlertCircle, IconArrowsSort } from "@tabler/icons-react"
import { cn } from "@/lib/utils"
import i18next from "i18next"

const determineLogLevelColor = (level: string) => {
    level = level.toLowerCase()
    return level === "error" ? "text-red-600" : level==="warn" ? "text-yellow-600": level==="info" ? "text-blue-600" : "text-gray-400"
}

export const columns: ColumnDef<ILogMessage>[] = [
    {
        accessorKey: "Severity",
        size: 75,
        header: () => <div className="w-8">Level</div>,
        cell: ({ row }) => {
            const alertClassName = determineLogLevelColor(row.getValue("Severity"))
            return (<div className="w-8 justify-center flex flex-row"><IconAlertCircle className={cn(alertClassName, "dark:bg-gray-800 dark:data-[state=checked]:bg-gray-700")} /></div>)
        },
        filterFn: (row, id, value) => {
            return value.includes(row.getValue(id))
        },
    },
    {
        accessorKey: "Timestamp",
        size: 150,
        cell: ({ row }) => {
            const label = new Date(row.getValue("Timestamp") as string).toLocaleTimeString(i18next.language, { hour: '2-digit', minute: '2-digit', second: '2-digit' })
            return <div className="w-64"><p className="font-semibold">{label}</p></div>
        },
        header: ({ column }) => {
            return (
                <div className="w-64"><Button
                    variant="ghost"
                    onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
                >
                    Time
                    <IconArrowsSort className="opacity-0 group-hover/header:opacity-100 ml-2 h-4 w-4 animate-in animate-out duration-300 " />
                </Button>
                </div>
            )
        }
    },
    {
        size: 1,
        accessorKey: "Message",
        header: ()=> <div className="grow">Messages</div>,
        cell: ({ row }) => {
            return <div className="grow"><p className="text-md font-semibold">{row.getValue("Message")}</p></div>
        }
    },
]