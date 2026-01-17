import ControllerBindingsDialog from "@/components/controllers/ControllerBindingsDialog"
import { useControllerStore } from "@/stores/controllerStore"
import { useProjectStore } from "@/stores/projectStore"
import { useNavigate } from "react-router"

const ControllerBindingsModal = () => {
  const { project } = useProjectStore()
  const { controllers } = useControllerStore()
  const navigate = useNavigate()
  const close = () => navigate(-1)
  const bindings = project?.ControllerBindings || []

  return (
    <ControllerBindingsDialog
      bindings={bindings}
      controllers={controllers}
      isOpen
      onOpenChange={(open: boolean) => {
        if (!open) close()
      }}
    />
  )
}
export default ControllerBindingsModal
