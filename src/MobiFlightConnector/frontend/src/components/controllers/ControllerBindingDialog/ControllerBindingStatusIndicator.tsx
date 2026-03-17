import { cn } from "@/lib/utils"
import { ControllerBindingStatus } from "@/types/controller"
import { IconCircleCheck, IconCircleDashed } from "@tabler/icons-react"

export type ControllerBindingStatusIndicatorProps = {
  isBound: boolean
  status: ControllerBindingStatus
}

const ControllerBindingStatusIndicator = ({
  isBound,
  status,
}: ControllerBindingStatusIndicatorProps) => {
  const variant = {
    Match: "text-green-500",
    AutoBind: "text-blue-500",
    Missing: "",
    RequiresManualBind: "text-green-500",
  }

  return (
    <div
      className="flex flex-row items-center gap-0"
      title={
        isBound ? `Controller is bound (${status})` : "Controller is not bound"
      }
    >
      <div className="border-muted-foreground/50 h-1 w-6 border-b" />
      {isBound ? (
        <IconCircleCheck className={cn(`h-8 w-8`, variant[status])} />
      ) : (
        <IconCircleDashed className="stroke-muted-foreground/50 h-8 w-8" />
      )}
      <div className="border-muted-foreground/50 h-1 w-6 border-b" />
    </div>
  )
}
export default ControllerBindingStatusIndicator
