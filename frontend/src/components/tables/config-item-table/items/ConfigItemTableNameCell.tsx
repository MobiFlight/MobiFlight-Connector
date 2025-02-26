import { Input } from "@/components/ui/input"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandUpdateConfigItem } from "@/types/commands"
import { IConfigItem, IDeviceConfig } from "@/types/config"
import { IconCircleCheck, IconEdit, IconX } from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { useCallback, useEffect, useState } from "react"

interface ConfigItemTableNameCellProps {
  row: Row<IConfigItem>
}
const ConfigItemTableNameCell = ({ row }: ConfigItemTableNameCellProps) => {
  const { publish } = publishOnMessageExchange()
  const [isEditing, setIsEditing] = useState(false)
  const [label, setLabel] = useState(row.getValue("Name") as string)
  const realLabel = row.getValue("Name") as string

  const toggleEdit = () => {
    setIsEditing(!isEditing)
  }

  const moduleName =
    (row.getValue("ModuleSerial") as string).split("/")[0] ?? "not set"
  const deviceName = (row.getValue("Device") as IDeviceConfig)?.Name ?? "-"

  const saveChanges = useCallback(() => {
    const item = row.original as IConfigItem
    item.Name = label
    console.log(item)
    publish({
      key: "CommandUpdateConfigItem",
      payload: { item: item },
    } as CommandUpdateConfigItem)
  }, [label, row, publish])

  useEffect(() => {
    setLabel(realLabel)
  }, [realLabel])

  return (
    <div className="group flex w-auto cursor-pointer flex-row items-center gap-1">
      {!isEditing ? (
        <div className="flex flex-col">
          <div className="flex flex-row items-center gap-1">
            <p className="max-w-52 truncate px-0 font-semibold">{label}</p>
            <IconEdit
              role="button"
              aria-label="Edit"
              onClick={toggleEdit}
              className="ml-2 opacity-0 transition-opacity delay-300 ease-in group-hover:opacity-100 group-hover:delay-100 group-hover:ease-out"
            />
          </div>
          <p className="w-60 truncate text-xs text-muted-foreground xl:hidden">
            {moduleName} - {deviceName}
          </p>
        </div>
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
          <IconX
            role="button"
            aria-label="Discard"
            onClick={() => {
              setLabel(row.getValue("Name") as string)
              toggleEdit()
            }}
          />
        </div>
      )}
    </div>
  )
}

export default ConfigItemTableNameCell
