import {
  InlineEditLabel,
  InlineEditLabelRef,
} from "@/components/InlineEditLabel"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandUpdateConfigItem } from "@/types/commands"
import { IConfigItem } from "@/types/config"
import { Row } from "@tanstack/react-table"
import { useRef, useEffect } from "react"
import { useRowInteraction } from "@/lib/hooks/useRowInteraction"

interface ConfigItemTableNameCellProps {
  row: Row<IConfigItem>
}
function ConfigItemTableNameCell({ row }: ConfigItemTableNameCellProps) {
  const { publish } = publishOnMessageExchange()
  const { registerNameEdit } = useRowInteraction()
  const inlineEditRef = useRef<InlineEditLabelRef>(null)

  const item = row.original as IConfigItem
  const label = item.Name

  // Register the edit function with the context
  useEffect(() => {
    if (inlineEditRef.current) {
      registerNameEdit(() => {
        inlineEditRef.current?.startEditing()
      })
    }
  }, [registerNameEdit])

  const saveChanges = (label: string) => {
    const updatedItem = { ...item, Name: label }
    publish({
      key: "CommandUpdateConfigItem",
      payload: { item: updatedItem },
    } as CommandUpdateConfigItem)
  }

  return (
    <InlineEditLabel
          ref={inlineEditRef}
          labelClassName="truncate group-[.is-first-drag-item]/row:hidden py-1"
          inputClassName="py-0"
          value={label}
          onSave={saveChanges}
        />
  )
}

export default ConfigItemTableNameCell
