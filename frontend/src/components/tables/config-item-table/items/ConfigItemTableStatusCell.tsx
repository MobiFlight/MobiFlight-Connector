import StatusIcon from "@/components/icons/StatusIcon"
import { IConfigItem } from "@/types"
import { IDictionary, ConfigItemStatusType } from "@/types/config"
import {
  IconAlertSquareRounded,
  IconFlask,
  IconRouteOff,
} from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash-es"
import React from "react"
import { useTranslation } from "react-i18next"

interface ConfigItemTableStatusCellProps {
  row: Row<IConfigItem>
}

const ConfigItemTableStatusCell = React.memo(
  ({ row }: ConfigItemTableStatusCellProps) => {
    const Status = row.getValue("Status") as IDictionary<
      string,
      ConfigItemStatusType
    >
    const Precondition = Status && !isEmpty(Status["Precondition"])
    const Test = Status && !isEmpty(Status["Test"])
    const ConfigRef = Status && !isEmpty(Status["ConfigRef"])

    const { t } = useTranslation()

    return (
      <div className="flex flex-row gap-1">
        <StatusIcon
          status="Precondition"
          condition={Precondition}
          title={
            Precondition
              ? t(`ConfigList.Status.Precondition.${Status["Precondition"]}`)
              : t(`ConfigList.Status.Precondition.normal`)
          }
          IconComponent={IconAlertSquareRounded}
        />
        <StatusIcon
          status="Test"
          condition={Test}
          title={
            Test
              ? t(`ConfigList.Status.Test.Executing`)
              : t(`ConfigList.Status.Test.NotExecuting`)
          }
          IconComponent={IconFlask}
        />
        <StatusIcon
          status="ConfigRef"
          condition={ConfigRef}
          title={
            ConfigRef
              ? t(`ConfigList.Status.ConfigRef.Missing`)
              : t(`ConfigList.Status.ConfigRef.OK`)
          }
          IconComponent={IconRouteOff}
        />
      </div>
    )
  },
)

export default ConfigItemTableStatusCell
