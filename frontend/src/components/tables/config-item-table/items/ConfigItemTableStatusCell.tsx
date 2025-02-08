import { IConfigItem } from "@/types"
import { IDictionary, ConfigItemStatusType } from "@/types/config"
import {
  IconAlertSquareRounded,
  IconBuildingBroadcastTower,
  IconPlugConnectedX,
  IconMathSymbols,
  IconFlask,
  IconRouteOff,
} from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash-es"

interface ConfigItemTableStatusCellProps {
  row: Row<IConfigItem>
}

const ConfigItemTableStatusCell = ({ row }: ConfigItemTableStatusCellProps) => {
  const Status = row.getValue("Status") as IDictionary<
    string,
    ConfigItemStatusType
  >
  const Precondition = Status && !isEmpty(Status["Precondition"])
  const Source = Status && !isEmpty(Status["Source"])
  const Modifier = Status && !isEmpty(Status["Modifier"])
  const Device = Status && !isEmpty(Status["Device"])
  const Test = Status && !isEmpty(Status["Test"])
  const ConfigRef = Status && !isEmpty(Status["ConfigRef"])

  return (
    <div className="flex w-28 flex-row gap-0">
      <IconAlertSquareRounded
        role="status"
        aria-disabled={!Precondition}
        className={!Precondition ? "stroke-slate-100" : "stroke-red-700"}
      >
        normal
      </IconAlertSquareRounded>
      <IconBuildingBroadcastTower
        role="status"
        aria-disabled={!Source}
        className={!Source ? "stroke-slate-100" : "stroke-red-700"}
      >
        normal
      </IconBuildingBroadcastTower>
      <IconPlugConnectedX
        role="status"
        aria-disabled={!Device}
        className={!Device ? "stroke-slate-100" : "stroke-red-700"}
      >
        normal
      </IconPlugConnectedX>
      <IconMathSymbols
        aria-disabled={!Modifier}
        role="status"
        className={!Modifier ? "stroke-slate-100" : "stroke-red-700"}
      >
        normal
      </IconMathSymbols>
      <IconFlask
        aria-disabled={!Test}
        role="status"
        className={!Test ? "stroke-slate-100" : "stroke-red-700"}
      >
        normal
      </IconFlask>
      <IconRouteOff
        aria-disabled={!ConfigRef}
        role="status"
        className={!ConfigRef ? "stroke-slate-100" : "stroke-red-700"}
      >
        normal
      </IconRouteOff>
    </div>
  )
}

export default ConfigItemTableStatusCell
