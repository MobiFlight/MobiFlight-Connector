import { Button } from '@/components/ui/button'
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator, DropdownMenuTrigger } from '@/components/ui/dropdown-menu'
import { publishOnMessageExchange } from '@/lib/hooks/appMessage'
import { CommandConfigContextMenu } from '@/types/commands'
import { IConfigItem } from '@/types/config'
import { IconDots } from '@tabler/icons-react'

import { Row } from "@tanstack/react-table"

interface ConfigItemTableActionsCellProps {
  row: Row<IConfigItem>
}

const ConfigItemTableActionsCell = ({ row } : ConfigItemTableActionsCellProps) => {
  const item = row.original
  const { publish } = publishOnMessageExchange()

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
          <DropdownMenuItem
            onClick={() => {
              publish({
                key: "CommandConfigContextMenu",
                payload: { action: "edit", item: item },
              } as CommandConfigContextMenu)
            }}
          >
            Edit
          </DropdownMenuItem>
          <DropdownMenuItem
            onClick={() => {
              publish({
                key: "CommandConfigContextMenu",
                payload: { action: "delete", item: item },
              } as CommandConfigContextMenu)
            }}
          >
            Remove
          </DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem
            onClick={() => {
              publish({
                key: "CommandConfigContextMenu",
                payload: { action: "duplicate", item: item },
              } as CommandConfigContextMenu)
            }}
          >
            Duplicate
          </DropdownMenuItem>
          {/* <DropdownMenuItem>Copy</DropdownMenuItem>
          <DropdownMenuItem>Paste</DropdownMenuItem> */}
          <DropdownMenuSeparator />
          <DropdownMenuItem
            onClick={() => {
              publish({
                key: "CommandConfigContextMenu",
                payload: { action: "test", item: item },
              } as CommandConfigContextMenu)
            }}
          >
            Test
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  )
}

export default ConfigItemTableActionsCell