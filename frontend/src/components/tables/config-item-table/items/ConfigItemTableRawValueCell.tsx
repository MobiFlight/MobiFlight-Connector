import StatusIcon from "@/components/icons/StatusIcon"
import ValueIcon from "@/components/icons/ValueIcon"
import { Badge } from "@/components/ui/badge"
import { IConfigItem } from "@/types"
import {
  IconBuildingBroadcastTower,
} from "@tabler/icons-react"
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
      <div className="text-md truncate" title="RawValue">
        {!isEmpty(label) && !Source ? (
          item.Type == "InputConfigItem" ? (
            <div className="flex flex-row justify-center">
              <Badge variant="secondary">
                {label?.replace("CHANGE =>", "")}
              </Badge>
            </div>
          ) : (
            <div className="text-sm">
              <>{label}</>
            </div>
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
