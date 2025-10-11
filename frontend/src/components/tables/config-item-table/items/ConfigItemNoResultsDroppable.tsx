import { useConfigItemDragContext } from "@/lib/hooks/useConfigItemDragContext"
import { useDroppable } from "@dnd-kit/core"
import { useTranslation } from "react-i18next"

const ConfigItemNoResultsDroppable = () => {
  const { t } = useTranslation()
  const { dragState } = useConfigItemDragContext()

  const { setNodeRef } = useDroppable({
    id: "no-results-droppable",
    data: {
      type: "empty-config"
    },
  })
  return (
    <div
      ref={setNodeRef}
      className="border-primary flex flex-col gap-2 rounded-lg border-2 border-solid"
    >
      <div className="bg-primary h-12"></div>
      {!dragState?.ui.isDragging ? (
        <div className="p-4 text-center" role="alert">
          {t("ConfigList.Table.NoResultsFound")}
        </div>
      ) : (
        <div className="p-4 text-center" role="alert">
          {t("ConfigList.Table.DropHereToAdd")}
        </div>
      )}
    </div>
  )
}

export default ConfigItemNoResultsDroppable
