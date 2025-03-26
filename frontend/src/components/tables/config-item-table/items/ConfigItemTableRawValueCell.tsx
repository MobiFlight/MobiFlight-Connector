import ToolTip from "@/components/ToolTip"
import { IConfigItem } from "@/types"
import { IDictionary, ConfigItemStatusType } from "@/types/config"
import {
  IconBuildingBroadcastTower,
  IconHourglassEmpty,
} from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash-es"
import { useTranslation } from "react-i18next"

interface ConfigItemTableRawValueCellProps {
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

const ConfigItemTableRawValueCell = ({
  row,
}: ConfigItemTableRawValueCellProps) => {
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
        label
      ) : Source ? (
        <div className="flex flex-row justify-center">
        <StatusIcon
                condition={Source}
                title={
                  Source ? t(`ConfigList.Status.Source.${Status["Source"]}`) : "available"
                }
                IconComponent={IconBuildingBroadcastTower}
              />
        </div>
      ) : (
        <div className="flex flex-row justify-center text-slate-300">
          <IconHourglassEmpty />
        </div>
      )}
    </div>
  )
}

export default ConfigItemTableRawValueCell
