import {
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
} from "@/components/ui/dropdown-menu"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { IConfigItem } from "@/types"
import { CommandConfigContextMenu } from "@/types/commands"
import {
  IconEdit,
  IconPencil,
  IconTrash,
  IconCopy,
  IconFlask,
} from "@tabler/icons-react"

export interface ConfigItemRowContextMenuProps {
  item: IConfigItem
  startNameEdit?: () => void
}

const ConfigItemRowContextMenu = ({
  item,
  startNameEdit,
}: ConfigItemRowContextMenuProps) => {
  const { publish } = publishOnMessageExchange()

  return (
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
          startNameEdit?.()
        }}
      >
        <div className="flex items-center gap-2">
          <IconPencil />
          <span>Rename</span>
        </div>
      </DropdownMenuItem>
      <DropdownMenuItem
        onClick={() => {
          publish({
            key: "CommandConfigContextMenu",
            payload: { action: "delete", item: item },
          } as CommandConfigContextMenu)
        }}
      >
        <div className="flex items-center gap-2">
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
  )
}
export default ConfigItemRowContextMenu
