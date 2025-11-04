import { HTMLAttributes } from "react"
import { Badge } from "../ui/badge"
import { cn } from "@/lib/utils"
import { IConfigItem } from "@/types"

export type ConfigItemTableDragOverlayProps = HTMLAttributes<HTMLDivElement> & {
  items: IConfigItem[]
}

const ConfigItemTableDragOverlay = (props: ConfigItemTableDragOverlayProps) => {
  const { items, className, ...htmlProps } = props
  const itemCount = items.length
  return (
    <div
      className={cn(
        "border-border bg-background relative flex items-center justify-start gap-0 rounded-sm border p-0",
        // Add stacked effect for multiple items using pseudo-elements
        itemCount > 1 && [
          "before:bg-background before:border-border before:absolute before:inset-0 before:-z-10 before:translate-y-1 before:rounded-sm before:border",
          itemCount > 2 &&
            "after:bg-background after:border-border after:absolute after:inset-0 after:-z-20 after:translate-y-1.5 after:rounded-sm after:border",
        ],
        itemCount === 1 ? "shadow-xl" : "after:shadow-xl",
        className,
      )}
      {...htmlProps}
    >
      <div className="flex w-24 items-center justify-center py-2 pl-5">
        <Badge className="px-4 text-xs">{itemCount}</Badge>
      </div>
      <span className="grow p-3 py-2 font-medium">
        {
          itemCount === 1 ? 
          items[0].Name 
          : 
          `You are moving ${itemCount} item(s)`
        }
      </span>
      <div className="text-muted-foreground bg-muted flex w-44 items-center gap-2 p-2 lg:w-136 xl:w-152 2xl:w-1/3">
        {/* <IconExclamationCircle className="stroke-muted-foreground/80 h-6 w-6" />
        You can drag & drop items here in the list or move them to other profile
        tabs. */}
        &nbsp;
      </div>
      <div className="bg-muted hidden w-44 p-2 lg:block 2xl:w-108">&nbsp;</div>
    </div>
  )
}

export default ConfigItemTableDragOverlay
