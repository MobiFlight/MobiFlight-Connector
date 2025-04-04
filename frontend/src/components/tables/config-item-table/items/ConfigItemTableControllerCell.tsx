import ToolTip from "@/components/ToolTip"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { IConfigItem } from "@/types"
import { CommandConfigContextMenu } from "@/types/commands"
import { IconBan, IconExternalLink } from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash"
import React from "react"
import { useCallback } from "react"
import { useTranslation } from "react-i18next"

interface ConfigItemTableControllerCellProps {
  row: Row<IConfigItem>
}
const ConfigItemTableControllerCell = React.memo(({
  row,
}: ConfigItemTableControllerCellProps) => {
  const { t } = useTranslation()
  const { publish } = publishOnMessageExchange()
  const item = row.original as IConfigItem

  const [ label, serial ] = item.ModuleSerial.split("/")
    
  const openControllerSettings = useCallback(() => {
    publish({
      key: "CommandConfigContextMenu",
      payload: { action: "settings", item: item },
    } as CommandConfigContextMenu)
  }, [publish, item])

  return !isEmpty(label) ? (
    <div className="group flex flex-row items-center">
      <ToolTip
        content={
          <div className="flex flex-col">
            <span className="text-xs">{label}</span>
            <span className="text-xs">{serial}</span>
          </div>
        }
      >
        <p className="text-md truncate font-normal">{label}</p>
      </ToolTip>
      <ToolTip
        content={
          <span className="text-xs">
            {t("ConfigList.Cell.Controller.openInSettings")}
          </span>
        }
      >
        <IconExternalLink
          role="link"
          aria-label="Edit"
          onClick={openControllerSettings}
          className="ml-2 min-w-7 cursor-pointer opacity-0 transition-opacity delay-300 ease-in group-hover:opacity-100 group-hover:delay-100 group-hover:ease-out"
        />
      </ToolTip>
    </div>
  ) : (
    <ToolTip content={t("ConfigList.Cell.Controller.not set")}>
      <span className="item-center hidden flex-row gap-2 text-slate-400 lg:flex">
        <IconBan />
        <span className="hidden lg:inline">not set</span>
      </span>
    </ToolTip>
  )
})

export default ConfigItemTableControllerCell
