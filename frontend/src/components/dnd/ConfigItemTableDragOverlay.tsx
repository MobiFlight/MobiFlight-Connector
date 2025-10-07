import { IConfigItem } from "@/types"

export type ConfigItemTableDragOverlayProps = {
  firstItem: IConfigItem | null
  itemCount: number
}

const ConfigItemTableDragOverlay = ({ firstItem, itemCount }: ConfigItemTableDragOverlayProps) => {
  return (
    <div className="bg-background border border-border rounded shadow-lg p-2 min-w-[200px] opacity-95">
        <div className="text-sm font-medium truncate">
          {firstItem?.Name || firstItem?.GUID}
        </div>
        {itemCount > 1 && (
          <div className="text-xs text-muted-foreground">
            +{itemCount - 1} more items
          </div>
        )}
      </div>
  )
}

export default ConfigItemTableDragOverlay