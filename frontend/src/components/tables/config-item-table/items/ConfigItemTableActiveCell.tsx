import { Switch } from '@/components/ui/switch'
import { publishOnMessageExchange } from '@/lib/hooks/appMessage'
import { CommandUpdateConfigItem } from '@/types/commands'
import { IConfigItem } from '@/types/config'

import { Row } from "@tanstack/react-table"

interface ConfigItemTableActiveCellProps {
  row: Row<IConfigItem>
}

const ConfigItemTableActiveCell = ({ row } : ConfigItemTableActiveCellProps) => {
  const { publish } = publishOnMessageExchange()
  const item = row.original as IConfigItem

  return (
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
  )
}

export default ConfigItemTableActiveCell