import ControllerBindingsDialog from "@/components/controllers/ControllerBindingsDialog"
import { useControllerStore } from "@/stores/controllerStore"
import { useProjectStore } from "@/stores/projectStore"
import { useState } from "react"
import { useNavigate } from "react-router"

const ControllerBindingsModal = () => {
  const { project } = useProjectStore()
  const { controllers } = useControllerStore()
  const navigate = useNavigate()
  const bindings = project?.ControllerBindings || []
  const [open, setOpen] = useState(true)

  return (
    <ControllerBindingsDialog
      bindings={bindings}
      controllers={controllers}
      isOpen={open}
      onOpenChange={(open: boolean) => {
        setOpen(open)
        navigate(-1)
      }}
    />
  )
}
export default ControllerBindingsModal
