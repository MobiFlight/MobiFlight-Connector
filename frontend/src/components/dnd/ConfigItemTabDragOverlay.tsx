import { IConfigItem } from "@/types"
import { Alert, AlertTitle } from "../ui/alert"
import { Badge } from "../ui/badge"
import { cn } from "@/lib/utils"
import { useTranslation } from "react-i18next"

export type ConfigItemTabDragOverlayProps = {
  items?: IConfigItem[]
  className?: string
}

const ConfigItemTabDragOverlay = (props: ConfigItemTabDragOverlayProps) => {
  const { items, className } = props
  const { t } = useTranslation()
  
  return (
    <Alert
      className={cn(
        `flex w-48 items-center justify-between px-4 py-3 shadow-xl`,
        className,
      )}
    >
      <AlertTitle>{t("ConfigList.Table.Drag.MovingItems")}</AlertTitle>
      <Badge className="text-xs py-1">{items?.length}</Badge>
    </Alert>
  )
}

export default ConfigItemTabDragOverlay
