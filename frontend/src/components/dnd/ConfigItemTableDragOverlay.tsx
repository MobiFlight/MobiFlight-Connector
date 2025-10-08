import { cn } from "@/lib/utils"
import { IConfigItem } from "@/types"
import { Badge } from "@/components/ui/badge"

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
        `bg-background border min-w-[200px] rounded p-3 opacity-95 shadow-md`,
        className,
      )}
    >
      <div className="flex flex-row items-center gap-2">
        <div className="text-md truncate pl-24">{nameText}</div>
        {isMultiDrag && (
          <Badge className="text-xs">
            {itemCount}
          </Badge>
        )}
      </div>
    </div>
  )
}

export default ConfigItemTableDragOverlay
