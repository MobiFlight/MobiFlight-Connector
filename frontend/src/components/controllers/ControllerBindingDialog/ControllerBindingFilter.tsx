import { Button } from "@/components/ui/button"
import { ControllerBindingStatus } from "@/types/controller"

export type ControllerBindingFilterProps = {
  activeFilter: ControllerBindingStatus | "all"
  updateFilter: (filter: ControllerBindingStatus | "all") => void
}

export const ControllerBindingFilter = ({
  activeFilter,
  updateFilter,
}: ControllerBindingFilterProps) => {

  const handleFilterChange = (filter: ControllerBindingStatus | "all") => {
    updateFilter(filter)
  }

  const options = [
    { label: "All", value: "all" },
    { label: "Manual", value: "ManualRebindRequired" as ControllerBindingStatus },
    { label: "Missing", value: "Missing" as ControllerBindingStatus },
    { label: "Auto bind", value: "AutoBind" as ControllerBindingStatus },
    { label: "Match", value: "Match" as ControllerBindingStatus },
  ]

  return (
    <div className="flex flex-row items-center gap-2 pb-2">
      {options.map((option) => (
        <Button
          key={option.value}
          className="h-8"
          variant={activeFilter === option.value ? "default" : "outline"}
          onClick={() => handleFilterChange(option.value as ControllerBindingStatus | "all")}
        >
          {option.label}
        </Button>
      ))}
    </div>
  )
}
