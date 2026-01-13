import ControllerBindings from "@/components/controllers/ControllerBindings"
// import { useProjectStore } from "@/stores/projectStore"
import { Controller, ControllerBinding } from "@/types/controller"

const ControllerBindingsModal = () => {
  // const { project } = useProjectStore()
  const bindings = [
    {
      "BoundController": "miniCOCKPIT miniFCU/ SN-E98-277",
      "Status": "Match",
      "OriginalController": "miniCOCKPIT miniFCU/ SN-E98-277"
    },
    {
      "BoundController": "ProtoBoard-v2/ SN-5FC-1CF",
      "Status": "AutoBind",
      "OriginalController": "ProtoBoard-v2/ SN-3F1-FDD"
    },
    {
      "BoundController": "Alpha Flight Controls / JS-b0875190-3b89-11ed-8007-444553540000",
      "Status": "RequiresManualBind",
      "OriginalController": "Alpha Flight Controls / JS-b0875190-3b89-11ed-8007-444553540000"
    },
    {
      "BoundController": null,
      "Status": "Missing",
      "OriginalController": "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000"
    },
    {
      "BoundController": null,
      "Status": "Missing",
      "OriginalController": "Behringer X-Touch Mini / MI-b0875190-3b89-11ed-8007-444553540001"
    },
    {
      "BoundController": "Generic Gamepad / JS-b0875190-3b89-11ed-8007-444553540002",
      "Status": "Match",
      "OriginalController": "Generic Gamepad / JS-b0875190-3b89-11ed-8007-444553540002"
    },
    {
      "BoundController": "Octavi / JS-b0875190-3b89-11ed-8007-44455354003",
      "Status": "Match",
      "OriginalController": "Octavi / JS-b0875190-3b89-11ed-8007-44455354003"
    }  
  ] as ControllerBinding[]
  
  const connectedControllers : Controller[] = [
    { Name: "Prototyping Board", Serial: "SN-5FC-1CF", Vendor: "MobiFlight", ProductId: "", VendorId: "", Type: "MobiFlight", Connected: true, ImageUrl: null, certified: false },
    { Name: "Alpha Controls Yoke", Serial: "JS-12345678", Vendor: "Honeycomb", ProductId: "", VendorId: "", Type: "Joystick", Connected: true, ImageUrl: null, certified: false },
    { Name: "Bravo Controls Throttle", Serial: "JS-87654321", Vendor: "Honeycomb", ProductId: "", VendorId: "", Type: "Joystick", Connected: true, ImageUrl: null, certified: false },
    { Name: "Throttle B", Serial: "67890", Vendor: "Korg", ProductId: "", VendorId: "", Type: "Midi", Connected: true, ImageUrl: null, certified: false },
    { Name: "miniCOCKPIT miniFCU", Serial: "SN-E98-277", Vendor: "miniCockpit", ProductId: "", VendorId: "", Type: "Joystick", Connected: true, ImageUrl: null, certified: false },
    { Name: "Generic Gamepad", Serial: "JS-b0875190-3b89-11ed-8007-444553540002", Vendor: "Microsoft", ProductId: "", VendorId: "", Type: "Joystick", Connected: true, ImageUrl: null, certified: false },
    { Name: "Octavi", Serial: "JS-b0875190-3b89-11ed-8007-44455354003", Vendor: "Octavi", ProductId: "", VendorId: "", Type: "Joystick", Connected: true, ImageUrl: null, certified: false },
  ]

  return (
    <div>
      <ControllerBindings
        bindings={bindings}
        controllers={connectedControllers}
        isOpen={true}
        onOpenChange={() => {}}
      />
    </div>
  )
}
export default ControllerBindingsModal
