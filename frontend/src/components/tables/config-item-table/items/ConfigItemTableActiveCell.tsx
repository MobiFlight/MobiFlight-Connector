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
    <div className='flex items-center'>
      <div {...attributes} {...listeners} className='text-gray-500 cursor-move ml-2 opacity-10 transition-opacity delay-100 ease-in group-hover/row:opacity-100 group-hover/row:delay-100 group-hover/row:ease-out'>
      <IconGripVertical className='stroke-2 fill-slate-500' />
      </div>
    <div className="w-20 text-center">
      <Switch
        className="dark:bg-gray-800 dark:data-[state=checked]:bg-gray-700"
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