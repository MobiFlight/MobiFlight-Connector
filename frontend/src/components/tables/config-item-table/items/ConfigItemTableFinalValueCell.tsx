import StatusIcon from "@/components/icons/StatusIcon"
import ValueIcon from "@/components/icons/ValueIcon"
import ToolTip from "@/components/ToolTip"
import { Badge } from "@/components/ui/badge"
import { IConfigItem } from "@/types"
import { IconMathSymbols } from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash-es"
import { useTranslation } from "react-i18next"

interface ConfigItemTableFinalValueCellProps {
  row: Row<IConfigItem>
}

function ConfigItemTableFinalValueCell({
  row,
}: ConfigItemTableFinalValueCellProps) {
  const item = row.original as IConfigItem
  const Status = item.Status
  const Modifier = Status && !isEmpty(Status["Modifier"])

  const { t } = useTranslation()
  const label = item.Value

  return (
    <div className="text-md" data-testid="final-value">
      {!isEmpty(label) && !Modifier ? (
        item.Type == "InputConfigItem" ? (
          <ToolTip content={label}>
            <div className="flex flex-row justify-center">
              <Badge variant="secondary" className="truncate">
                {label}
              </Badge>
            </div>
          </ToolTip>
        ) : (
          <ToolTip content={label}>
            <div className="truncate px-2 text-sm">{label}</div>
          </ToolTip>
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
}

export default ConfigItemTableFinalValueCell
