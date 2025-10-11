import { useConfigItemDragContext } from "@/lib/hooks/useConfigItemDragContext"
import { cn } from "@/lib/utils"
import { useDroppable } from "@dnd-kit/core"
import { useTranslation } from "react-i18next"

const ConfigItemNoResultsDroppable = () => {
  const { t } = useTranslation()
  const { dragState } = useConfigItemDragContext()

  const { setNodeRef, isOver } = useDroppable({
    id: "no-results-droppable",
    data: {
      type: "placeholder",
    },
  })
  return (
    <div
      ref={setNodeRef}
      className="border-primary flex flex-col rounded-lg border-2 border-solid"
    >
      <div className="bg-primary h-12"></div>
      {!dragState?.ui.isDragging ? (
        <div className="p-4 text-center" role="alert">
          {t("ConfigList.Table.NoResultsFound")}
        </div>
      ) : (
        <div className="px-1 py-1 text-center" role="alert">
          <div
            className={cn(
              "text-md rounded-sm border-2 border-dashed p-1 font-medium",
              isOver && "bg-accent",
            )}
          >
            {t("ConfigList.Table.DropHereToAdd")}
          </div>
        </div>
      )}
    </div>
  )
}

export default ConfigItemNoResultsDroppable
