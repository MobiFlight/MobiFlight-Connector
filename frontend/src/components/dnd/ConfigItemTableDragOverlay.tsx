import { cn } from "@/lib/utils"
import { IConfigItem } from "@/types"
import { Badge } from "lucide-react"

export type ConfigItemTableDragOverlayProps = {
  firstItem: IConfigItem | null
  itemCount: number
  className?: string
}

const ConfigItemTableDragOverlay = ({
  firstItem,
  itemCount,
  className,
}: ConfigItemTableDragOverlayProps) => {

  const isMultiDrag = itemCount > 1
  const nameText =
    !isMultiDrag ? firstItem?.Name : "You are moving multiple items"


  return (
    <div
      className={cn(
        `bg-background border-border min-w-[200px] rounded border p-3 opacity-95 shadow-lg`,
        className,
      )}
    >
      <div className="flex flex-row items-center gap-2">
        <div className="text-md truncate pl-24 font-medium">{nameText}</div>
        {isMultiDrag && (
          <Badge className="rounded bg-muted px-2 py-1 text-xs font-medium tabular-nums">
            {itemCount} items
          </Badge>
        )}
      </div>
    </div>
  )
}

export default ConfigItemTableDragOverlay
