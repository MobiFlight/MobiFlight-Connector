import { Button } from "@/components/ui/button"
import { Preset } from "@/types"
import { ColumnDef } from "@tanstack/react-table"

export interface PresetColumnsProps {
  onUse: (preset: Preset) => void
}

export const getPresetColumns = ({ onUse }:PresetColumnsProps) : ColumnDef<Preset>[] => [
  {
    accessorKey: "label",
    meta: {
      className: "max-w-[350px]",
    },
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
    accessorKey: "description",
    meta: {
      className: "w-[200px] max-w-[500px]",
    },
    header: () => <div className="w-8">Description</div>,
    cell: ({ row }) => (
      <div>
        <p className="text-xs truncate">{row.original.description}</p>
      </div>
    ),
    filterFn: (row, id, value) => {
      return value.includes(row.getValue(id))
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
  {
    accessorKey: "actions",
    header: () => <div className="w-8">Actions</div>,
    meta: {
      className: "w-[80px]",
    },
    cell: ({ row }) => {
      return (
        <div className="flex flex-row gap-2">
          <Button size={"sm"} onClick={ () => { onUse(row.original) }}>Use</Button>
        </div>
      )
    },
  },
]
