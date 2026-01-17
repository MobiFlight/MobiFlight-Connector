import { Button } from "@/components/ui/button"
import { ControllerBindingStatus } from "@/types/controller"
import { useTranslation } from "react-i18next"

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
  const { t } = useTranslation()
  const handleFilterChange = (filter: ControllerBindingStatus | "all") => {
    updateFilter(filter)
  }

  const options = [
    {
      label: t("Dialog.ControllerBinding.Filter.all"),
      value: "all",
      enabled: true,
    },
    {
      label: t("Dialog.ControllerBinding.Filter.manual"),
      value: "RequiresManualBind" as ControllerBindingStatus,
      enabled: availableStates.includes("RequiresManualBind"),
    },
    {
      label: t("Dialog.ControllerBinding.Filter.missing"),
      value: "Missing" as ControllerBindingStatus,
      enabled: availableStates.includes("Missing"),
    },
    {
      label: t("Dialog.ControllerBinding.Filter.autobind"),
      value: "AutoBind" as ControllerBindingStatus,
      enabled: availableStates.includes("AutoBind"),
    },
    {
      label: t("Dialog.ControllerBinding.Filter.match"),
      value: "Match" as ControllerBindingStatus,
      enabled: availableStates.includes("Match"),
    },
  ]

  return (
    <div className="flex flex-row items-center gap-2 pb-2">
      {options.map((option) => (
        <Button
          key={option.value}
          className="h-8"
          variant={activeFilter === option.value ? "default" : "outline"}
          onClick={() =>
            handleFilterChange(option.value as ControllerBindingStatus | "all")
          }
          disabled={!option.enabled}
        >
          {option.label}
        </Button>
      ))}
    </div>
  )
}
