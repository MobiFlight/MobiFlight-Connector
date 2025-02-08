import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandConfigContextMenu } from "@/types/commands"
import { IConfigItem } from "@/types/config"
import { IconCopy, IconDots, IconEdit, IconFlask, IconTrash } from "@tabler/icons-react"

import { Row } from "@tanstack/react-table"

interface ConfigItemTableActionsCellProps {
  row: Row<IConfigItem>
}

const ConfigItemTableActionsCell = ({
  row,
}: ConfigItemTableActionsCellProps) => {
  const item = row.original
  const { publish } = publishOnMessageExchange()

  return (
    <div className="flex items-center">
      <Button
        variant="outline"
        className="h-8 w-8 rounded-r-none border-r-0 p-0"
        onClick={() => {
          publish({
            key: "CommandConfigContextMenu",
            payload: { action: "edit", item: item },
          } as CommandConfigContextMenu)
        }}
      >
        <IconEdit
        />
      </Button>
      <div className="relative">
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="outline" className="h-8 w-8 rounded-l-none p-0">
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
              <div className="flex items-center gap-2">
              <IconEdit></IconEdit>
              <span>Edit</span>
            </div>
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem
              onClick={() => {
                publish({
                  key: "CommandConfigContextMenu",
                  payload: { action: "delete", item: item },
                } as CommandConfigContextMenu)
              }}
            ><div className="flex items-center gap-2">
              <IconTrash></IconTrash>
              <span>Delete</span>
            </div>
              
            </DropdownMenuItem>
            <DropdownMenuItem
              onClick={() => {
                publish({
                  key: "CommandConfigContextMenu",
                  payload: { action: "duplicate", item: item },
                } as CommandConfigContextMenu)
              }}
            >
              <div className="flex items-center gap-2">
              <IconCopy></IconCopy>
              <span>Duplicate</span>
            </div>
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
              <div className="flex items-center gap-2">
              <IconFlask></IconFlask>
              <span>Test</span>
            </div>
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </div>
  )
}

export default ConfigItemTableActionsCell
