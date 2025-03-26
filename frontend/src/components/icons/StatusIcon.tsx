import { ConfigItemStatusType } from "@/types/config"
import ToolTip from "../ToolTip"

type StatusIconProps = {
  status: ConfigItemStatusType
  condition: boolean
  title: string
  IconComponent: React.ElementType
}

const StatusIcon = ({ status, condition, title, IconComponent }: StatusIconProps) => (
  <ToolTip content={title}>
    <IconComponent
      role="status"
      aria-label={status}
      aria-disabled={!condition}
      className={!condition ? "stroke-slate-100" : "stroke-red-700"}
    />
  </ToolTip>
)

export default StatusIcon