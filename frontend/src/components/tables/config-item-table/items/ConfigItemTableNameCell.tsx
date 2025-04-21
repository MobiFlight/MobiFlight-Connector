import ToolTip from "@/components/ToolTip"
import { Input } from "@/components/ui/input"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandUpdateConfigItem } from "@/types/commands"
import { IConfigItem } from "@/types/config"
import { IconCircleCheck, IconCircleX, IconEdit } from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import React from "react"
import { useCallback, useEffect, useState } from "react"
import { useTranslation } from "react-i18next"

interface ConfigItemTableNameCellProps {
  row: Row<IConfigItem>
}
const ConfigItemTableNameCell = React.memo(({ row }: ConfigItemTableNameCellProps) => {
  const { t } = useTranslation()

  const { publish } = publishOnMessageExchange()
  const [isEditing, setIsEditing] = useState(false)
  
  const item = row.original as IConfigItem
  const realLabel = item.Name
  const typeLabel = t(`Types.${item.Type}`)
  
  const [label, setLabel] = useState(item.Name)

  const toggleEdit = () => {
    setIsEditing(!isEditing)
  }

  const moduleName =
    (item.ModuleSerial).split("/")[0] ?? "not set"
  const deviceName = (item.Device)?.Name ?? "-"

  const saveChanges = useCallback(() => {
    item.Name = label
    publish({
      key: "CommandUpdateConfigItem",
      payload: { item: item },
    } as CommandUpdateConfigItem)
  }, [label, item, publish])

  useEffect(() => {
    setLabel(realLabel)
  }, [realLabel])

  const selectedRows = row.getVisibleCells()[0].getContext().table.getSelectedRowModel().rows.length
  const dragLabel = selectedRows > 1 ? t("ConfigList.Cell.Drag.Multiple", { count: selectedRows}) : label

  return (
    <div className="group flex cursor-pointer flex-row items-center gap-1">
      {!isEditing ? (
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
            <p className="px-0 font-semibold truncate group-[.is-first-drag-item]/row:hidden">{label}</p>
            <p className="px-0 font-semibold truncate hidden group-[.is-first-drag-item]/row:block">{dragLabel}</p>
            <IconEdit
              role="button"
              aria-label="Edit"
              onClick={toggleEdit}
              className="min-w-10 ml-2 opacity-0 transition-opacity delay-300 ease-in group-hover:opacity-100 group-hover:delay-100 group-hover:ease-out"
            />
          </div>
        </ToolTip>
      ) : (
        <div
          className="flex flex-row items-center gap-1"
          onKeyDown={(e) => e.key === "Enter" && saveChanges() && toggleEdit()}
        >
          <Input
            type="text"
            value={label}
            className="m-0 h-6 px-2 text-sm lg:h-8"
            onChange={(e) => setLabel(e.target.value)}
            onKeyDown={(e) =>
              e.key === "Enter" && (saveChanges(), toggleEdit())
            }
            autoFocus
          />
          <IconCircleCheck
            className="stroke-green-700"
            role="button"
            aria-label="Save"
            onClick={() => {
              saveChanges()
              toggleEdit()
            }}
          />
          <IconCircleX
            className="stroke-red-700"
            role="button"
            aria-label="Discard"
            onClick={() => {
              setLabel(item.Name)
              toggleEdit()
            }}
          />
        </div>
      )}
    </div>
  )
})

export default ConfigItemTableNameCell
