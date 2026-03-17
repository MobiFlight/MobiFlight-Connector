import {
  ContextMenuContent,
  ContextMenuItem,
  ContextMenuLabel,
  ContextMenuSeparator,
} from "@/components/ui/context-menu"
import {
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
} from "@/components/ui/dropdown-menu"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { useRowInteraction } from "@/lib/hooks/useRowInteraction"
import { IConfigItem } from "@/types"
import { CommandConfigContextMenu } from "@/types/commands"
import {
  IconEdit,
  IconPencil,
  IconTrash,
  IconCopy,
  IconFlask,
} from "@tabler/icons-react"

// Shared items — no Radix Content wrapper, just the items themselves
function MenuItems({
  item,
  Item,
  Label,
  Separator,
}: {
  item: IConfigItem
  Item: typeof DropdownMenuItem | typeof ContextMenuItem
  Label: typeof DropdownMenuLabel | typeof ContextMenuLabel
  Separator: typeof DropdownMenuSeparator | typeof ContextMenuSeparator
}) {
  const { publish } = publishOnMessageExchange()
  const { startNameEdit } = useRowInteraction()

  return (
    <>
      <Label>Actions</Label>
      <Item
        onClick={() =>
          publish({
            key: "CommandConfigContextMenu",
            payload: { action: "edit", item },
          } as CommandConfigContextMenu)
        }
      >
        <div className="flex items-center gap-2 [&_svg]:size-4">
          <IconEdit />
          <span>Edit</span>
        </div>
      </Item>
      <Separator />
      {/* 
          using setTimeout makes sure the context menu closes before starting the edit, 
          otherwise the input won't receive focus sometimes 
       */}
      <Item onClick={() => setTimeout(() => startNameEdit?.(), 0)}>
        <div className="flex items-center gap-2 [&_svg]:size-4">
          <IconPencil />
          <span>Rename</span>
        </div>
      </Item>
      <Item
        onClick={() =>
          publish({
            key: "CommandConfigContextMenu",
            payload: { action: "delete", item },
          } as CommandConfigContextMenu)
        }
      >
        <div className="flex items-center gap-2 [&_svg]:size-4">
          <IconTrash />
          <span>Delete</span>
        </div>
      </Item>
      <Item
        onClick={() =>
          publish({
            key: "CommandConfigContextMenu",
            payload: { action: "duplicate", item },
          } as CommandConfigContextMenu)
        }
      >
        <div className="flex items-center gap-2 [&_svg]:size-4">
          <IconCopy />
          <span>Duplicate</span>
        </div>
      </Item>
      <Separator />
      <Item
        onClick={() =>
          publish({
            key: "CommandConfigContextMenu",
            payload: { action: "test", item },
          } as CommandConfigContextMenu)
        }
      >
        <div className="flex items-center gap-2 [&_svg]:size-4">
          <IconFlask />
          <span>Test</span>
        </div>
      </Item>
    </>
  )
}

export interface ConfigItemRowContextMenuProps {
  item: IConfigItem
  variant?: "dropdown" | "context"
}

const ConfigItemRowContextMenu = ({
  item,
  variant = "dropdown",
}: ConfigItemRowContextMenuProps) => {
  if (variant === "context") {
    return (
      <ContextMenuContent data-testid="config-item-context-menu">
        <MenuItems
          item={item}
          Item={ContextMenuItem}
          Label={ContextMenuLabel}
          Separator={ContextMenuSeparator}
        />
      </ContextMenuContent>
    )
  }

  return (
    <DropdownMenuContent
      align="end"
      data-testid="config-item-context-menu"
      // Prevent setting focus 
      onCloseAutoFocus={(e) => e.preventDefault()}
    >
      <MenuItems
        item={item}
        Item={DropdownMenuItem}
        Label={DropdownMenuLabel}
        Separator={DropdownMenuSeparator}
      />
    </DropdownMenuContent>
  )
}
export default ConfigItemRowContextMenu
