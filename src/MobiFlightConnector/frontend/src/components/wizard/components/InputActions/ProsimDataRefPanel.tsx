import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { cn } from "@/lib/utils"
import {
  ProSimDataRefDefinition,
  useProSimDataRefStore,
} from "@/stores/prosimDataRefStore"
import { useState } from "react"
import { useTranslation } from "react-i18next"

export type ProSimDataRefPanelProps = {
  variant: "input" | "output"
  selectedPath: string | null
  onPresetChange: (preset: ProSimDataRefDefinition) => void
}

const ProSimDataRefPanel = ({
  variant,
  selectedPath,
  onPresetChange,
}: ProSimDataRefPanelProps) => {
  const { t } = useTranslation()
  const { dataRefs } = useProSimDataRefStore()
  const { publish } = publishOnMessageExchange()
  const selectedPreset = selectedPath ? dataRefs[selectedPath] : null

  const [filter, setFilter] = useState({
    search: "",
  })

  const filteredPresets = Object.values(dataRefs).filter((preset) => {
    if (variant === "input" && !preset.CanWrite) return false
    if (variant === "output" && !preset.CanRead) return false
    if (filter.search && !preset.Description.toLowerCase().includes(filter.search.toLowerCase())) return false
    return true
  })

  const refreshPresets = () => {
    publish({
      key: "CommandRefreshPresets",
      payload: {
        type: "prosim",
      },
    })
  }

  return (
    <div>
      {filteredPresets.length > 0 ? (
        <div className="flex flex-col gap-4">
          <Input
            placeholder={t("Dialog.InputConfigWizard.InputActions.ProSimDataRef.FilterPresetsPlaceholder")}
            value={filter.search}
            onChange={(e) =>
              setFilter((prev) => ({ ...prev, search: e.target.value }))
            }
          />
          <div className="relative flex max-h-64 flex-0 flex-col overflow-x-hidden overflow-y-auto pr-3">
            {filteredPresets.map((preset, index) => {
              const isSelected = preset.Name === selectedPreset?.Name
              return (
                <div
                  key={index}
                  onClick={() => onPresetChange(preset)}
                  className={cn(
                    "hover:bg-secondary flex cursor-pointer flex-row items-center gap-4 rounded p-2",
                    isSelected ? "bg-secondary" : "",
                  )}
                >
                  <div className="w-1/2 text-sm font-semibold">
                    {preset.Description}
                  </div>
                  <div className="text-muted-foreground w-1/4 truncate text-sm">
                    {preset.Name}
                  </div>
                  <div className="text-muted-foreground w-1/4 truncate text-xs">
                    {preset.DataType}
                  </div>
                </div>
              )
            })}
          </div>
        </div>
      ) : (
        <div className="flex flex-col gap-2">
          <div className="text-muted-foreground text-sm">
            {t("Dialog.InputConfigWizard.InputActions.ProSimDataRef.NoPresetsAvailable")}
          </div>
          <Button variant="outline" onClick={() => refreshPresets()}>
            {t("Dialog.InputConfigWizard.InputActions.ProSimDataRef.RefreshPresets")}
          </Button>
        </div>
      )}
      <div></div>
    </div>
  )
}
export default ProSimDataRefPanel
