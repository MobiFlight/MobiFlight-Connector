import { InlineEditLabel, InlineEditLabelRef } from "@/components/InlineEditLabel"
import ToolTip from "@/components/ToolTip"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandUpdateConfigItem } from "@/types/commands"
import { IConfigItem } from "@/types/config"
import { Row } from "@tanstack/react-table"
import { useTranslation } from "react-i18next"
import { useRef, useEffect } from "react"
import { useRowInteraction } from "@/lib/hooks/useRowInteraction"

interface ConfigItemTableNameCellProps {
  row: Row<IConfigItem>
}
function ConfigItemTableNameCell({ row }: ConfigItemTableNameCellProps) {
  const { t } = useTranslation()
  const { publish } = publishOnMessageExchange()
  const { registerNameEdit } = useRowInteraction()
  const inlineEditRef = useRef<InlineEditLabelRef>(null)
  
  const item = row.original as IConfigItem
  const typeLabel = t(`Types.${item.Type}`)

  const label = item.Name

  const moduleName =
    (item.ModuleSerial).split("/")[0] ?? "not set"
  const deviceName = (item.Device)?.Name ?? "-"

  // Register the edit function with the context
  useEffect(() => {
    if (inlineEditRef.current) {
      registerNameEdit(() => {
        inlineEditRef.current?.startEditing()
      })
    }
  }, [registerNameEdit])

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
              ref={inlineEditRef}
              labelClassName="truncate group-[.is-first-drag-item]/row:hidden"
              value={label}
              onSave={saveChanges}
            />
          </div>
        </ToolTip>
    </div>
  )
}

export default ConfigItemTableNameCell
