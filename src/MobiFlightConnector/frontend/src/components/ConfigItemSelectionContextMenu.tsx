import {
  ContextMenuContent,
  ContextMenuItem,
  ContextMenuLabel,
  ContextMenuSeparator,
} from "@/components/ui/context-menu"
import { IconToggleLeft, IconTrash, IconX } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export interface ConfigItemSelectionContextMenuProps {
  selectedCount: number
  onDeleteSelected?: () => void
  onToggleSelected?: () => void
  onClearSelected: () => void
}

const ConfigItemSelectionContextMenu = ({
  selectedCount,
  onDeleteSelected,
  onToggleSelected,
  onClearSelected,
}: ConfigItemSelectionContextMenuProps) => {
  const { t } = useTranslation()

  return (
    <ContextMenuContent
      data-testid="config-item-selection-context-menu"
      // The menu is a React child of the table row and would
      // otherwise trigger its selection handling on click
      onClick={(e) => e.stopPropagation()}
      // Keep the table's keyboard shortcuts, e.g. Delete,
      // from acting on the selection while the menu is open
      onKeyDown={(e) => e.stopPropagation()}
    >
      <ContextMenuLabel>
        {t("ConfigList.Toolbar.SelectedRows.Commands.Label", {
          count: selectedCount,
        })}
      </ContextMenuLabel>
      <ContextMenuItem onClick={onDeleteSelected}>
        <div className="flex items-center gap-2 [&_svg]:size-4">
          <IconTrash />
          <span>{t("ConfigList.Toolbar.SelectedRows.Commands.Delete")}</span>
        </div>
      </ContextMenuItem>
      <ContextMenuItem onClick={onToggleSelected}>
        <div className="flex items-center gap-2 [&_svg]:size-4">
          <IconToggleLeft />
          <span>{t("ConfigList.Toolbar.SelectedRows.Commands.Toggle")}</span>
        </div>
      </ContextMenuItem>
      <ContextMenuSeparator />
      <ContextMenuItem onClick={onClearSelected}>
        <div className="flex items-center gap-2 [&_svg]:size-4">
          <IconX />
          <span>{t("ConfigList.Toolbar.SelectedRows.Commands.Clear")}</span>
        </div>
      </ContextMenuItem>
    </ContextMenuContent>
  )
}

export default ConfigItemSelectionContextMenu
