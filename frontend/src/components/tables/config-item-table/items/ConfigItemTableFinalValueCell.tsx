import ToolTip from "@/components/ToolTip"
import { Badge } from "@/components/ui/badge"
import { IConfigItem } from "@/types"
import { IDictionary, ConfigItemStatusType } from "@/types/config"
import {
  IconHourglassEmpty,
  IconMathSymbols,
} from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash-es"
import { useTranslation } from "react-i18next"

interface ConfigItemTableFinalValueCellProps {
  row: Row<IConfigItem>
}

type StatusIconProps = {
  condition: boolean
  title: string
  IconComponent: React.ElementType
}

const StatusIcon = ({ condition, title, IconComponent }: StatusIconProps) => (
  <ToolTip content={title}>
    <IconComponent
      role="status"
      aria-disabled={!condition}
      className={!condition ? "stroke-slate-100" : "stroke-red-700"}
    />
  </ToolTip>
)

const ConfigItemTableFinalValueCell = ({
  row,
}: ConfigItemTableFinalValueCellProps) => {
  const item = row.original as IConfigItem
  const Status = row.getValue("Status") as IDictionary<
    string,
    ConfigItemStatusType
  >
  const Modifier = Status && !isEmpty(Status["Modifier"])

  const { t } = useTranslation()
  const label = row.getValue("Value") as string

  return (
    <div className="text-md truncate">
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
        <div className="flex flex-row justify-center text-slate-200">
          <ToolTip content={t("ConfigList.Cell.Waiting")}>
            <IconHourglassEmpty />
          </ToolTip>
        </div>
      )}
    </div>
  )
}

export default ConfigItemTableFinalValueCell
