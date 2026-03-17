import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandConfigContextMenu } from "@/types/commands"
import { IConfigItem } from "@/types/config"
import { IconDots, IconEdit } from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import ConfigItemRowContextMenu from "@/components/ConfigItemRowContextMenu"

interface ConfigItemTableActionsCellProps {
  row: Row<IConfigItem>
}

function ConfigItemTableActionsCell({
  row,
}: ConfigItemTableActionsCellProps) {
  const item = row.original
  const { publish } = publishOnMessageExchange()
  
  return (
    <div className="flex justify-center">
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
      <div className="relative" onContextMenu={(e) => { e.preventDefault() }}>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="outline" className="h-8 w-8 rounded-l-none p-0">
              <span className="sr-only">Open menu</span>
              <IconDots className="h-4 w-4" />
            </Button>
          </DropdownMenuTrigger>
          <ConfigItemRowContextMenu item={item} />
        </DropdownMenu>
      </div>
    </div>
  )
}

export default ConfigItemTableActionsCell
