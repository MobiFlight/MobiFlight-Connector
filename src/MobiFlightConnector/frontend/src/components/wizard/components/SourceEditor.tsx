import { Card, CardContent } from "@/components/ui/card"
import MsfsPresetPanel from "@/components/wizard/components/InputActions/MsfsPresetPanel"
import MsfsPresetDetails from "@/components/wizard/components/MsfsPresetDetails"
import SourceTypeComboBox from "@/components/wizard/components/SourceTypeComboBox"
import { fetchHubHopPresets, SourceTypeOptions } from "@/lib/configWizard"
import { SimConnectSource, Source } from "@/types/config"
import { useQuery } from "@tanstack/react-query"
import { useTranslation } from "react-i18next"

export interface SourceEditorProps {
  source: Source | null
  onSourceChange: (source: Source | null) => void
}
const SourceEditor = ({ source, onSourceChange }: SourceEditorProps) => {
  const { t } = useTranslation()
  const selectedSourceType = source
    ? SourceTypeOptions.find((option) => option.value === source.Type)
    : undefined

  const { data: presets = [], isLoading } = useQuery({
    queryKey: ["msfs-presets"],
    queryFn: () => fetchHubHopPresets("msfs"),
    // presets don't change at runtime; HubHopState drives invalidation
    staleTime: Infinity,
  })

  const preset =
    presets.find(
      (p) => p.id === (source as SimConnectSource)?.SimConnectValue.UUID,
    ) ?? null

  return (
    <div className="flex flex-col gap-4" data-testid="source-editor">
      <div className="flex flex-col gap-1">
        <div className="text-lg font-semibold">
          {t("Dialog.ConfigWizard.Source.Title")}
        </div>
        <div className="text-muted-foreground text-sm">
          {t("Dialog.ConfigWizard.Source.Description")}
        </div>
      </div>
      <Card>
        <CardContent className="pt-4">
          <div className="flex flex-col gap-4">
            <div className="flex flex-row items-end justify-between">
              <SourceTypeComboBox
                selectedSourceType={selectedSourceType}
                setSelectedSourceType={(option) => {
                  onSourceChange(option ? { Type: option.value } as Source : null)
                }}
              />
            </div>
          </div>
        </CardContent>
      </Card>
      {selectedSourceType?.value === "SimConnectSource" && (
        <>
          <MsfsPresetPanel
            variant="output"
            isLoading={isLoading}
            selectedPresetId={preset?.id ?? null}
            setSelectedPreset={() => {}}
          />
          <MsfsPresetDetails
            command={(source as SimConnectSource)?.SimConnectValue?.Value ?? ""}
            preset={preset}
            onCommandChange={(command) => {
              onSourceChange({
                ...source,
                SimConnectValue: {
                  ...((source as SimConnectSource)?.SimConnectValue ?? {}),
                  Value: command,
                },
              } as SimConnectSource)  
            }}
          />
        </>
      )}
      {selectedSourceType?.value === "VariableSource" && (
        <div>Variable options</div>
      )}
      {selectedSourceType?.value === "FsuipcSource" && (
        <div>Fsuipc options</div>
      )}
      {selectedSourceType?.value === "ProSimSource" && (
        <div>ProSim options</div>
      )}
      {selectedSourceType?.value === "XplaneSource" && (
        <div>Xplane options</div>
      )}
    </div>
  )
}
export default SourceEditor
