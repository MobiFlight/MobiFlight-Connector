import { IConfigItem } from "@/types"

export type ConfigItemTabDragOverlayProps = {
  items?: IConfigItem[]
}

const ConfigItemTabDragOverlay = (props: ConfigItemTabDragOverlayProps) => {
  const { items } = props
  return (
    <div>Moving {items?.length} items</div>
  )
}

export default ConfigItemTabDragOverlay