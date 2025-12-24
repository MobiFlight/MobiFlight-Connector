import StatusIcon from "@/components/icons/StatusIcon"
import ValueIcon from "@/components/icons/ValueIcon"
import ToolTip from "@/components/ToolTip"
import { Badge } from "@/components/ui/badge"
import { IConfigItem } from "@/types"
import { IconBuildingBroadcastTower } from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash-es"
import { useTranslation } from "react-i18next"

interface ConfigItemTableRawValueCellProps {
  row: Row<IConfigItem>
}

function ConfigItemTableRawValueCell({
  row,
}: ConfigItemTableRawValueCellProps) {
  const item = row.original as IConfigItem

  const Status = item.Status
  const Source = Status && !isEmpty(Status["Source"])

  const { t } = useTranslation()
  const label = item.RawValue

  return (
    <div className="text-md" data-testid="raw-value">
      {!isEmpty(label) && !Source ? (
        item.Type == "InputConfigItem" ? (
          <ToolTip content={label}>
            <div className="flex flex-row justify-center">
              <Badge variant="secondary" className="truncate">
                {label?.replace("CHANGE =>", "")}
              </Badge>
            </div>
          </ToolTip>
        ) : (
          <ToolTip content={label}>
            <div className="truncate px-2 text-sm">{label}</div>
          </ToolTip>
        )
      ) : Source ? (
        <div className="flex flex-row justify-center">
          <StatusIcon
            status="Source"
            condition={Source}
            title={
              Source
                ? t(`ConfigList.Status.Source.${Status["Source"]}`)
                : "available"
            }
            IconComponent={IconBuildingBroadcastTower}
          />
        </div>
      ) : (
        <ValueIcon />
      )}
    </div>
  )
}

export default ConfigItemTableRawValueCell
