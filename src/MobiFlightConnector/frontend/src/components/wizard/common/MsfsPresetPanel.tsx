import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import PresetPanel from "@/components/wizard/common/PresetPanel"
import PresetSummary from "@/components/wizard/common/PresetSummary"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import { HubHopPresetDetails } from "@/components/wizard/components/HubHopPresetDetails"
import { Preset } from "@/types/preset"
import { IconExclamationCircle } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

import type { MsfsPresetAdapter } from "@/types/preset"
import { useHubHopPresets } from "@/lib/hooks/useHubHopPresets"

export type MsfsPresetPanelProps<T> = {
  variant: "summary" | "details"
  adapter: MsfsPresetAdapter<T>
  config: T
  onConfigChange?: (config: T) => void
}

const MsfsPresetPanel = <T,>({
  variant,
  adapter,
  config,
  onConfigChange,
}: MsfsPresetPanelProps<T>) => {
  const { data: presets = [], isLoading } = useHubHopPresets("msfs")

  const presetId = adapter.getPresetId(config) ?? null
  const code = adapter.getCode(config) ?? ""
  const preset = presets.find((p) => p.id === presetId) ?? null

  if (variant === "summary") {
    return <PresetSummary label={preset?.label} code={code} />
  }

  return (
    <div className="flex flex-col gap-4">
      <PresetPanel
        sim="msfs"
        mode={adapter.mode}
        isLoading={isLoading}
        selectedPresetId={presetId}
        setSelectedPreset={(selected) =>
          onConfigChange?.(
            adapter.withPreset(config, selected as Preset | null),
          )
        }
      />
      <MsfsPresetDetails
        preset={preset}
        code={code}
        onCodeChange={(newCode) =>
          onConfigChange?.(adapter.withCode(config, newCode))
        }
      />
    </div>
  )
}
export default MsfsPresetPanel

type MsfsPresetDetailsProps = {
  preset: Preset | null
  code: string
  onCodeChange: (code: string) => void
}

const MsfsPresetDetails = ({
  preset,
  code,
  onCodeChange,
}: MsfsPresetDetailsProps) => {
  const { t } = useTranslation()

  return (
    <WizardSection data-testid="preset-code-panel">
      <SectionHeadline
        title={t(
          "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Title",
        )}
        description={t(
          "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Description",
        )}
      />
      <div className="flex flex-col gap-4">
        {preset ? (
          <HubHopPresetDetails preset={preset} variant={"msfs"} />
        ) : (
          code !== "" && (
            <div className="flex flex-row items-center gap-2 rounded-md">
              <IconExclamationCircle className="text-primary fill-background" />
              <div className="text-primary text-sm">
                {t(
                  "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Customized",
                )}
              </div>
            </div>
          )
        )}

        <div className="flex flex-col gap-1">
          <Label htmlFor="code">
            {t("Dialog.InputConfigWizard.InputActions.Common.CodeLabel")}
          </Label>
          <Textarea
            name="code"
            className="dark:focus:bg-input/20 font-mono text-sm whitespace-nowrap dark:bg-transparent"
            placeholder={t(
              "Dialog.InputConfigWizard.InputActions.Common.Preset.Code.Placeholder",
            )}
            value={code}
            onChange={(e) => onCodeChange(e.target.value)}
          />
          <div className="text-muted-foreground text-sm">
            {t(
              "Dialog.InputConfigWizard.InputActions.Common.SupportedPlaceholders",
            )}
          </div>
        </div>
      </div>
    </WizardSection>
  )
}
