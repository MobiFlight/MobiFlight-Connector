import { Preset } from "@/types"
import { ColumnDef } from "@tanstack/react-table"

export const columns: ColumnDef<Preset>[] = [
  {
    accessorKey: "label",
    header: () => <div className="w-8">Preset</div>,
    cell: ({ row }) => {
      const label = (row.original as Preset).label
      const aircraft = (row.original as Preset).aircraft
      const system = (row.original as Preset).system
      const vendor = (row.original as Preset).vendor
      return (
        <>
          <p className="text-md truncate font-semibold">{label}</p>
          <p className="truncate text-xs text-muted-foreground">
            {vendor} &gt; {aircraft} &gt; {system}
          </p>
        </>
      )
    },
    filterFn: (row, id, value) => {
      return (
        (row.getValue(id) as string)
          .toUpperCase()
          .indexOf(value.toUpperCase()) > -1
      )
    },
  },
  {
    accessorKey: "vendor",
    header: () => <div className="w-8">Vendor</div>,
    filterFn: (row, id, value) => {
      return value.includes(row.getValue(id))
    },
  },
  {
    accessorKey: "aircraft",
    header: () => <div className="w-8">Aircraft</div>,
    filterFn: (row, id, value) => {
      return value.includes(row.getValue(id))
    },
  },
  {
    accessorKey: "system",
    header: () => <div className="w-8">System</div>,
    filterFn: (row, id, value) => {
      return value.includes(row.getValue(id))
    },
  },
]
