import ComboBox from "@/components/ComboBox"
import { Label } from "@/components/ui/label"
import { parsePresets } from "@/lib/configWizard"
import { useQuery } from "@tanstack/react-query"
import { useTranslation } from "react-i18next"

export type EventIdPresetsPanelProps = {
  variant: "default" | "pmdg"
  aircraft?: "B737" | "B747" | "B777"
  selectedPresetId: string | null
  setSelectedPreset: (preset: { name: string; eventId: string } | null) => void
}

const presetUrls = {
  default: "/presets/presets_eventids.cip",
  pmdg: {
    B737: "/presets/presets_eventids_pmdg_737.cip",
    B747: "/presets/presets_eventids_pmdg_747.cip",
    B777: "/presets/presets_eventids_pmdg_777.cip",
  },
}

const EventIdPresetsPanel = ({
  variant,
  aircraft,
  selectedPresetId,
  setSelectedPreset,
}: EventIdPresetsPanelProps) => {
  const { t } = useTranslation()
  const presetUrl =
    variant === "default"
      ? presetUrls.default
      : presetUrls.pmdg[aircraft || "B737"]
  // In MsfsPresetPanel (or a dedicated hook)
  const { data: presets = [] /*, isLoading */ } = useQuery({
    queryKey: [`presets-${variant}-${aircraft}`],
    queryFn: () =>
      fetch(presetUrl)
        .then((r) => r.text())
        .then((content) => parsePresets(content)) as Promise<
        { name: string; eventId: string; description: string }[]
      >,
    staleTime: Infinity, // presets don't change at runtime; HubHopState drives invalidation
  })

  const selectedPreset = presets.find(
    (item) => item.eventId === selectedPresetId?.toString(),
  )
  
  return (
    <div className="flex flex-col gap-2">
      <Label>{t("Dialog.InputConfigWizard.InputActions.EventIdPresets.SelectPresetLabel")}</Label>
      <ComboBox
        selected={selectedPreset}
        placeholder={t("Dialog.InputConfigWizard.InputActions.EventIdPresets.SelectPresetPlaceholder")}
        getLabel={(item) => item.name}
        getValue={(item) => item.eventId}
        items={presets}
        isSelected={(item) => item.eventId === selectedPreset?.eventId}
        setSelected={(item) => setSelectedPreset(item ? item : null)}
        widthClass="w-100"
      />
    </div>
  )
}

export default EventIdPresetsPanel
