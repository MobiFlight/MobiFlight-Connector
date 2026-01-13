import { useLocation, useNavigate } from "react-router-dom"
import type { ProjectInfo } from "@/types/project"
import ProjectForm from "@/components/project/ProjectForm"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"

export default function ProjectFormModal() {
  const navigate = useNavigate()
  const close = () => navigate(-1)
  const location = useLocation()

  const project = location.state?.project as ProjectInfo | { Name: "" } as ProjectInfo
  const isEdit = location.state?.mode === "edit"
  const { publish } = publishOnMessageExchange()

  return (
    <ProjectForm
      project={project}
      isOpen
      onOpenChange={(open: boolean) => {
        if (!open) close()
      }}
      onSave={async (values) => {
        publish({
          key: "CommandMainMenu",
          payload: {
            action: isEdit ? "project.edit" : "file.new",
            options: {
              project: values
            }
          }
        })
        if(!isEdit) {
          navigate(`/config`)
          return
        }        
        close()
      }}
    />
  )
}