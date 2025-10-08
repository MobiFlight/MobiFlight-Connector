import { IConfigItem } from "@/types"
import { Alert, AlertTitle } from "../ui/alert"
import { Badge } from "../ui/badge"
import { cn } from "@/lib/utils"

export type ConfigItemTabDragOverlayProps = {
  items?: IConfigItem[]
  className?: string
}

const ConfigItemTabDragOverlay = (props: ConfigItemTabDragOverlayProps) => {
  const { items, className } = props
  return (
    <Alert
      className={cn(
        `flex w-48 items-center justify-between px-4 py-3 shadow-xl`,
        className,
      )}
    >
      <AlertTitle>Moving items</AlertTitle>
      <Badge className="text-xs">{items?.length}</Badge>
    </Alert>
  )
}

export default ConfigItemTabDragOverlay
