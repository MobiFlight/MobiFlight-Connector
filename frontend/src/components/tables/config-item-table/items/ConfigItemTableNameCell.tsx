import { InlineEditLabel } from "@/components/InlineEditLabel"
import ToolTip from "@/components/ToolTip"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandUpdateConfigItem } from "@/types/commands"
import { IConfigItem } from "@/types/config"
import { Row } from "@tanstack/react-table"
import { useTranslation } from "react-i18next"

interface ConfigItemTableNameCellProps {
  row: Row<IConfigItem>
}
function ConfigItemTableNameCell({ row }: ConfigItemTableNameCellProps) {
  const { t } = useTranslation()

  const { publish } = publishOnMessageExchange()
  
  const item = row.original as IConfigItem
  const typeLabel = t(`Types.${item.Type}`)

  const label = item.Name

  const moduleName =
    (item.ModuleSerial).split("/")[0] ?? "not set"
  const deviceName = (item.Device)?.Name ?? "-"

  const saveChanges = (label:string) => {
    const updatedItem = { ...item, Name: label }
    publish({
      key: "CommandUpdateConfigItem",
      payload: { item: updatedItem },
    } as CommandUpdateConfigItem)
  }

  const selectedRows = row.getVisibleCells()[0].getContext().table.getSelectedRowModel().rows.length
  const dragLabel = selectedRows > 1 ? t("ConfigList.Cell.Drag.Multiple", { count: selectedRows}) : label

  return (
    <div className="group flex cursor-pointer flex-row items-center gap-1">
      <ToolTip
          content={
            <div className="flex flex-col gap-1">
              <p className="text-xs font-semibold">{typeLabel}</p>
              <p className="truncate text-xs text-muted-foreground xl:hidden">
                {moduleName} - {deviceName}
              </p>
            </div>
          }
        >
          <div className="flex flex-row items-center gap-0 w-full">
            <p className="px-0 font-semibold truncate hidden group-[.is-first-drag-item]/row:block">{dragLabel}</p>
            <InlineEditLabel
              labelClassName="group-[.is-first-drag-item]/row:hidden"
              value={label}
              onSave={saveChanges}
              placeholder={t("ConfigList.Cell.Name.Placeholder") || "Unnamed Item"}
            />
          </div>
        </ToolTip>
    </div>
  )
}

export default ConfigItemTableNameCell
