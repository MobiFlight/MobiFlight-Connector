import StatusIcon from "@/components/icons/StatusIcon"
import ToolTip from "@/components/ToolTip"
import { Badge } from "@/components/ui/badge"
import { IConfigItem } from "@/types"
import { IDictionary, ConfigItemStatusType } from "@/types/config"
import {
  IconBuildingBroadcastTower,
  IconHourglassEmpty,
} from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash-es"
import React from "react"
import { useTranslation } from "react-i18next"

interface ConfigItemTableRawValueCellProps {
  row: Row<IConfigItem>
}

const ConfigItemTableRawValueCell = React.memo(({
  row,
}: ConfigItemTableRawValueCellProps) => {
  const item = row.original as IConfigItem
  const Status = row.getValue("Status") as IDictionary<
    string,
    ConfigItemStatusType
  >
  const Source = Status && !isEmpty(Status["Source"])

  const { t } = useTranslation()
  const label = row.getValue("RawValue") as string

  return (
    <div className="text-md truncate">
      {!isEmpty(label) && !Source ? (
        item.Type == "InputConfigItem" ? (
          <div className="flex flex-row justify-center">
            <Badge variant="secondary">{label.replace("CHANGE =>", "")}</Badge>
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
        <div className="flex flex-row justify-center text-slate-200">
          <ToolTip content={t("ConfigList.Cell.Waiting")}>
            <IconHourglassEmpty className="h-7 w-7"/>
          </ToolTip>
        </div>
      )}
    </div>
  )
})

export default ConfigItemTableRawValueCell
