import { Button } from "@/components/ui/button"
import { useProjectModal } from "@/lib/hooks/useProjectModal"
import { IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export const ProjectCreateButton = () => {
  const { showOverlay } = useProjectModal()
  const { t } = useTranslation()
  
  return (
    <Button
      className="[&_svg]:size-6"
      onClick={() => showOverlay({ mode: "create" })}
    >
      <IconPlus /> {t("Project.Label")}
    </Button>
  )
}
