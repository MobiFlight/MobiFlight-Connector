import { Button } from "@/components/ui/button"
import { ControllerBindingStatus } from "@/types/controller"

export type ControllerBindingFilterProps = {
  availableStates: ControllerBindingStatus[]
  activeFilter: ControllerBindingStatus | "all"
  updateFilter: (filter: ControllerBindingStatus | "all") => void
}

export const ControllerBindingFilter = ({
  availableStates,
  activeFilter,
  updateFilter,
}: ControllerBindingFilterProps) => {

  const handleFilterChange = (filter: ControllerBindingStatus | "all") => {
    updateFilter(filter)
  }

  const options = [
    { label: "All", value: "all", enabled : true },
    { label: "Manual", value: "RequiresManualBind" as ControllerBindingStatus, enabled: availableStates.includes("RequiresManualBind") },
    { label: "Missing", value: "Missing" as ControllerBindingStatus, enabled: availableStates.includes("Missing") },
    { label: "Auto bind", value: "AutoBind" as ControllerBindingStatus, enabled: availableStates.includes("AutoBind") },
    { label: "Match", value: "Match" as ControllerBindingStatus, enabled: availableStates.includes("Match") },
  ]

  return (
    <div className="flex flex-row items-center gap-2 pb-2">
      {options.map((option) => (
        <Button
          key={option.value}
          className="h-8"
          variant={activeFilter === option.value ? "default" : "outline"}
          onClick={() => handleFilterChange(option.value as ControllerBindingStatus | "all")}
          disabled={!option.enabled}
        >
          {option.label}
        </Button>
      ))}
    </div>
  )
}
