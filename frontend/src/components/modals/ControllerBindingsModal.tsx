import ControllerBindings from "@/components/controllers/ControllerBindings"
import { useControllerStore } from "@/stores/controllerStore"
import { useProjectStore } from "@/stores/projectStore"

const ControllerBindingsModal = () => {
  const { project } = useProjectStore()
  const { controller } = useControllerStore()
  const bindings = project?.ControllerBindings || []

  return (
    <div>
      <ControllerBindings
        bindings={bindings}
        controllers={controller}
        isOpen={true}
        onOpenChange={() => {}}
      />
    </div>
  )
}
export default ControllerBindingsModal
