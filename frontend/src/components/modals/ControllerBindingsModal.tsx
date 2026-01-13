import ControllerBindingsDialog from "@/components/controllers/ControllerBindingsDialog"
import { useControllerStore } from "@/stores/controllerStore"
import { useProjectStore } from "@/stores/projectStore"
import { useNavigate } from "react-router"

const ControllerBindingsModal = () => {
  const { project } = useProjectStore()
  const { controller } = useControllerStore()
  const navigate = useNavigate()
  const close = () => navigate(-1)
  const bindings = project?.ControllerBindings || []

  return (
    <ControllerBindingsDialog
      bindings={bindings}
      controllers={controller}
      isOpen
      onOpenChange={(open: boolean) => {
        if (!open) close()
      }}
    />
  )
}
export default ControllerBindingsModal
