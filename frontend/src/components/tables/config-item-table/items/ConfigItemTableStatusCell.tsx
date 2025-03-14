import ToolTip from "@/components/ToolTip";
import { IConfigItem } from "@/types";
import { IDictionary, ConfigItemStatusType } from "@/types/config";
import {
  IconAlertSquareRounded,
  IconBuildingBroadcastTower,
  IconPlugConnectedX,
  IconMathSymbols,
  IconFlask,
  IconRouteOff,
} from "@tabler/icons-react";
import { Row } from "@tanstack/react-table";
import { isEmpty } from "lodash-es";
import { useTranslation } from "react-i18next";

interface ConfigItemTableStatusCellProps {
  row: Row<IConfigItem>;
}

type StatusIconProps = {
  condition: boolean;
  title: string;
  IconComponent: React.ElementType;
};

const StatusIcon = ({ condition, title, IconComponent } : StatusIconProps) => (
  <ToolTip content={title}>
    <IconComponent
      role="status"
      aria-disabled={!condition}
      className={!condition ? "stroke-slate-100" : "stroke-red-700"}
    />
  </ToolTip>
);

const ConfigItemTableStatusCell = ({ row }: ConfigItemTableStatusCellProps) => {
  const Status = row.getValue("Status") as IDictionary<string, ConfigItemStatusType>;
  const Precondition = Status && !isEmpty(Status["Precondition"]);
  const Source = Status && !isEmpty(Status["Source"]);
  const Modifier = Status && !isEmpty(Status["Modifier"]);
  const Device = Status && !isEmpty(Status["Device"]);
  const Test = Status && !isEmpty(Status["Test"]);
  const ConfigRef = Status && !isEmpty(Status["ConfigRef"]);

  const { t } = useTranslation();

  return (
    <div className="flex flex-row gap-1">
      <StatusIcon
        condition={Precondition}
        title={
          Precondition
            ? t(`ConfigList.Status.Precondition.${Status["Precondition"]}`)
            : t(`ConfigList.Status.Precondition.normal`)
        }
        IconComponent={IconAlertSquareRounded}
      />
      <StatusIcon
        condition={Source}
        title={
          Source ? t(`ConfigList.Status.Source.${Status["Source"]}`) : "available"
        }
        IconComponent={IconBuildingBroadcastTower}
      />
      <StatusIcon
        condition={Device}
        title={
          Device
            ? t(`ConfigList.Status.Device.NotConnected`)
            : t(`ConfigList.Status.Device.Connected`)
        }
        IconComponent={IconPlugConnectedX}
      />
      <StatusIcon
        condition={Modifier}
        title={
          Modifier
            ? t(`ConfigList.Status.Modifier.Error`)
            : t(`ConfigList.Status.Modifier.OK`)
        }
        IconComponent={IconMathSymbols}
      />
      <StatusIcon
        condition={Test}
        title={
          Test
            ? t(`ConfigList.Status.Test.Executing`)
            : t(`ConfigList.Status.Test.NotExecuting`)
        }
        IconComponent={IconFlask}
      />
      <StatusIcon
        condition={ConfigRef}
        title={
          ConfigRef
            ? t(`ConfigList.Status.ConfigRef.Missing`)
            : t(`ConfigList.Status.ConfigRef.OK`)
        }
        IconComponent={IconRouteOff}
      />
    </div>
  );
};

export default ConfigItemTableStatusCell;