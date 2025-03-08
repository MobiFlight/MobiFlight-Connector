import ToolTip from "@/components/ToolTip"
import { IConfigItem } from "@/types"
import { IconBan } from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash"
import { useTranslation } from "react-i18next"

interface ConfigItemTableControllerCellProps {
  row: Row<IConfigItem>
}
const ConfigItemTableControllerCell = ({
  row,
}: ConfigItemTableControllerCellProps) => {
  const { t } = useTranslation()

  const label = (row.getValue("ModuleSerial") as string).split("/")[0]
  const serial = (row.getValue("ModuleSerial") as string).split("/")[1]

  return !isEmpty(label) ? (
    <ToolTip content={<span className="text-xs">S/N: {serial}</span>}>
      <div className="hidden w-48 flex-col xl:flex 2xl:w-64">
        <p className="text-md truncate font-semibold">{label}</p>
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
