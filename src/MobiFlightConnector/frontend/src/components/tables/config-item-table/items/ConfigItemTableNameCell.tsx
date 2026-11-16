import {
  InlineEditLabel,
  InlineEditLabelRef,
} from "@/components/InlineEditLabel"
import messageExchange from "@/lib/messageExchange"
import { CommandUpdateConfigItem } from "@/types/commands"
import { IConfigItem } from "@/types/config"
import { Row } from "@tanstack/react-table"
import { useRef, useEffect } from "react"
import { useContext } from "react"
import { ConfigItemTableContext } from "../ConfigItemTableContext"

interface ConfigItemTableNameCellProps {
  row: Row<IConfigItem>
}
function ConfigItemTableNameCell({ row }: ConfigItemTableNameCellProps) {
  const { publish } = messageExchange
  const tableContext = useContext(ConfigItemTableContext)
  const inlineEditRef = useRef<InlineEditLabelRef>(null)

  const item = row.original as IConfigItem
  const label = item.Name

  useEffect(() => {
    const startEditing = () => inlineEditRef.current?.startEditing()
    tableContext?.registerNameEdit(item.GUID, startEditing)

    return () => tableContext?.registerNameEdit(item.GUID, null)
  }, [item.GUID, tableContext])

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
      labelClassName="max-w-full inline-block truncate group-[.is-first-drag-item]/row:hidden py-1"
      inputClassName="py-0"
      value={label}
      onSave={saveChanges}
    />
  )
}

export default ConfigItemTableNameCell
