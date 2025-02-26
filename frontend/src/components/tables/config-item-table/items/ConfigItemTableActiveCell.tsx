import { Switch } from '@/components/ui/switch'
import { publishOnMessageExchange } from '@/lib/hooks/appMessage'
import { CommandUpdateConfigItem } from '@/types/commands'
import { IConfigItem } from '@/types/config'
import { useSortable } from '@dnd-kit/sortable'
import { IconGripVertical } from '@tabler/icons-react'

import { Row } from "@tanstack/react-table"

interface ConfigItemTableActiveCellProps {
  row: Row<IConfigItem>
}

const ConfigItemTableActiveCell = ({ row } : ConfigItemTableActiveCellProps) => {
  const { publish } = publishOnMessageExchange()
  const item = row.original as IConfigItem

  const {
      attributes,
      listeners,
    } = useSortable({ id: item.GUID })

  return (
    <div className='flex items-start'>
      <div {...attributes} {...listeners} className='px-1 text-gray-500 cursor-move opacity-10 transition-opacity delay-100 ease-in group-hover/row:opacity-100 group-hover/row:delay-100 group-hover/row:ease-out'>
      <IconGripVertical className='stroke-2 fill-slate-500' />
      </div>
    <div className="w-12 text-center">
      <Switch
        checked={row.getValue("Active") as boolean}
        onClick={() => {
          item.Active = !item.Active
          publish({
            key: "CommandUpdateConfigItem",
            payload: { item: item },
          } as CommandUpdateConfigItem)
        }}
      />
    </div>
    </div>
  )
}

export default ConfigItemTableActiveCell