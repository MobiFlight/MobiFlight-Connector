import { ProjectInfo } from "@/types/project"
import { useLocation, useNavigate } from "react-router"

export type ProjectModalOptions = {
  mode: "create" | "edit"
  project?: ProjectInfo
}

export function useProjectModal() {
  const navigate = useNavigate()
  const location = useLocation()

  const showOverlay = (options: ProjectModalOptions) => {
    console.log("Showing project modal with options:", options)
    const route = options.mode === "create" ? "/project/new" : `/project/edit`
    navigate(route, { state: { backgroundLocation: location, ...options } })
  }

  const showStandalone = (options: ProjectModalOptions) => {
    const route = options.mode === "create" ? "/project/new" : `/project/edit`
    navigate(route, { state: { ...options } })
  }

  return { showOverlay, showStandalone }
}