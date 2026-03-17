import { ConfigItemStatusType } from "@/types/config"
import ToolTip from "../ToolTip"

type StatusIconProps = {
  status: ConfigItemStatusType
  condition: boolean
  title: string
  IconComponent: React.ElementType
}

const StatusIcon = ({
  status,
  condition,
  title,
  IconComponent,
}: StatusIconProps) => (
  <ToolTip content={title}>
    <IconComponent
      role="status"
      aria-label={status}
      aria-disabled={!condition}
      className={
        !condition
          ? "stroke-slate-100 group-hover/row:stroke-gray-300/50 group-data-[state=selected]/row:stroke-slate-300/20 dark:stroke-slate-500/5 dark:group-hover/row:stroke-slate-500/10 dark:group-data-[state=selected]/row:stroke-slate-500/10"
          : "stroke-purple-800"
      }
    />
  </ToolTip>
)

export default StatusIcon
