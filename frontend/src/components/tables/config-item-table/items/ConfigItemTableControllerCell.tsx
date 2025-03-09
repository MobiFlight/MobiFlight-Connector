import ToolTip from "@/components/ToolTip"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { IConfigItem } from "@/types"
import { CommandConfigContextMenu } from "@/types/commands"
import { IconBan, IconExternalLink } from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash"
import { useCallback } from "react"
import { useTranslation } from "react-i18next"

interface ConfigItemTableControllerCellProps {
  row: Row<IConfigItem>
}
const ConfigItemTableControllerCell = ({
  row,
}: ConfigItemTableControllerCellProps) => {
  const { t } = useTranslation()
  const { publish } = publishOnMessageExchange()

  const label = (row.getValue("ModuleSerial") as string).split("/")[0]
  const serial = (row.getValue("ModuleSerial") as string).split("/")[1]

  const openControllerSettings = useCallback(() => {
    const item = row.original as IConfigItem
    publish({
      key: "CommandConfigContextMenu",
      payload: { action: "settings", item: item },
    } as CommandConfigContextMenu)
  }, [row, publish])

  return !isEmpty(label) ? (
    <ToolTip content={<span className="text-xs">S/N: {serial}</span>}>
      <div className="3xl:w-72 group hidden w-48 flex-row items-center xl:flex 2xl:w-64">
        <p className="text-md truncate font-normal">{label}</p>
        <IconExternalLink
          role="link"
          aria-label="Edit"
          onClick={openControllerSettings}
          className="ml-2 opacity-0 transition-opacity delay-300 ease-in group-hover:opacity-100 group-hover:delay-100 group-hover:ease-out"
        />
      </div>
    </ToolTip>
  ) : (
    <ToolTip content={t("ConfigList.Cell.Controller.not set")}>
      <span className="item-center hidden flex-row gap-2 text-slate-400 xl:flex">
        <IconBan />
        <span>not set</span>
      </span>
    </ToolTip>
  )
}

export default ConfigItemTableControllerCell
