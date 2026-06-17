import ComboBox from "@/components/ComboBox"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { parsePresets } from "@/lib/configWizard"
import { JeehellInputAction } from "@/types/config"
import { useQuery } from "@tanstack/react-query"
import { useTranslation } from "react-i18next"

export type JeehellInputActionPanelProps = {
  variant: "summary" | "details"
  config: JeehellInputAction | null
  onConfigChange: (config: JeehellInputAction) => void
}

const JeehellInputActionPanel = ({
  variant,
  config,
  onConfigChange
}: JeehellInputActionPanelProps) => {
  const { t } = useTranslation()
  // In MsfsPresetPanel (or a dedicated hook)
  const presetUrl = "/presets/presets_jeehell.cip"
  const { data: presets = [] /*, isLoading */ } = useQuery({
    queryKey: [`presets-jeehell`],
    queryFn: () =>
      fetch(presetUrl)
        .then((r) => r.text())
        .then((content) => parsePresets(content)) as Promise<
        { name: string; eventId: string; description: string }[]
      >,
    staleTime: Infinity, // presets don't change at runtime; HubHopState drives invalidation
  })

  const selectedPreset = presets.find((item) => item.eventId === config?.EventId?.toString())

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-8">
        <div className="flex w-1/3 flex-col gap-1">
          <Label htmlFor="mouseParam">
            {t("Dialog.InputConfigWizard.InputActions.Jeehell.FunctionLabel")}:
          </Label>
          <div>{selectedPreset?.name}</div>
        </div>
        <div className="flex grow flex-col gap-1">
          <Label htmlFor="param">
            {t("Dialog.InputConfigWizard.InputActions.Jeehell.ValueLabel")}:
          </Label>
          <div
            id="param"
            className="bg-accent/20 rounded px-2 py-1 font-mono text-sm whitespace-pre-wrap"
          >
            {config?.Param}
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col gap-2">
        <Label htmlFor="mouseParam">{t("Dialog.InputConfigWizard.InputActions.Jeehell.FunctionLabel")}</Label>
        <ComboBox
          placeholder={t("Dialog.InputConfigWizard.InputActions.Jeehell.SelectFunctionPlaceholder")}
          items={presets}
          getLabel={(item) => item.name}
          getValue={(item) => item.eventId}
          isSelected={(item) => item.eventId === config?.EventId?.toString()}
          selected={selectedPreset}
          setSelected={(item) =>
            onConfigChange({
              ...config,
              EventId: item ? item.eventId : "",
            } as JeehellInputAction)
          }
          widthClass="w-100"
        />
        <p className="text-sm text-muted-foreground">{selectedPreset?.description}</p>
      </div>
      <div className="flex w-100 flex-col gap-2">
        <Label htmlFor="value">{t("Dialog.InputConfigWizard.InputActions.Jeehell.ValueLabel")}</Label>
        <Input
          id="value"
          value={config?.Param ?? ""}
          onChange={(e) =>
            onConfigChange({
              ...config,
              Param: e.target.value,
            } as JeehellInputAction)
          }
        />
      </div>
    </div>
  )
}
export default JeehellInputActionPanel
