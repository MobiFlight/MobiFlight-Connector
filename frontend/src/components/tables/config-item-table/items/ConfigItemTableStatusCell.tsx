import ToolTip from "@/components/ToolTip";
import { IConfigItem } from "@/types";
import { IDictionary, ConfigItemStatusType } from "@/types/config";
import {
  IconAlertSquareRounded,
  IconPlugConnectedX,
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
      className={!condition ? "stroke-slate-100" : "stroke-blue-700"}
    />
  </ToolTip>
);

const ConfigItemTableStatusCell = ({ row }: ConfigItemTableStatusCellProps) => {
  const Status = row.getValue("Status") as IDictionary<string, ConfigItemStatusType>;
  const Precondition = Status && !isEmpty(Status["Precondition"]);
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