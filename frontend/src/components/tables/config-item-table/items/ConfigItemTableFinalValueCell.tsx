import StatusIcon from "@/components/icons/StatusIcon"
import ValueIcon from "@/components/icons/ValueIcon"
import { Badge } from "@/components/ui/badge"
import { IConfigItem } from "@/types"
import { IconMathSymbols } from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash-es"
import React from "react"
import { useTranslation } from "react-i18next"

interface ConfigItemTableFinalValueCellProps {
  row: Row<IConfigItem>
}

const ConfigItemTableFinalValueCell = React.memo(
  ({ row }: ConfigItemTableFinalValueCellProps) => {
    const item = row.original as IConfigItem
    const Status = item.Status
    const Modifier = Status && !isEmpty(Status["Modifier"])

    const { t } = useTranslation()
    const label = item.Value

    return (
      <div className="text-md truncate" title="Value">
        {!isEmpty(label) && !Modifier ? (
          item.Type == "InputConfigItem" ? (
            <div className="flex flex-row justify-center">
              <Badge variant="secondary">{label}</Badge>
            </div>
          ) : (
            <div className="text-sm">
              <>{label}</>
            </div>
          )
        ) : Modifier ? (
          <div className="flex flex-row justify-center">
            <StatusIcon
              status="Modifier"
              condition={Modifier}
              title={
                Modifier
                  ? t(`ConfigList.Status.Modifier.Error`)
                  : t(`ConfigList.Status.Modifier.OK`)
              }
              IconComponent={IconMathSymbols}
            />
          </div>
        ) : (
          <ValueIcon />
        )}
      </div>
    )
  },
)

export default ConfigItemTableFinalValueCell
