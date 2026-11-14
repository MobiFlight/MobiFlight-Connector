import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import { HubHopPresetDetails } from "@/components/wizard/common/HubHopPresetDetails"
import { XplanePreset, Preset } from "@/types/preset"
import { IconExclamationCircle } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"
import XplanePresetCustomize from "@/components/wizard/common/XplanePresetCustomize"
import MsfsPresetCustomize from "@/components/wizard/common/MsfsPresetCustomize"

export interface PresetDetailsProps {
  sim: "xplane" | "msfs"
  mode: "input" | "output"
  preset: XplanePreset | Preset | null
  code: string | null
  value?: string | null
  codeType?: "DataRef" | "Command" | null
  onCodeChange: (code: string) => void
  onCodeTypeChange?: (codeType: "DataRef" | "Command") => void
  onValueChange?: (value: string) => void
}

const PresetDetails = ({
  sim,
  preset,
  mode,
  code,
  codeType,
  value,
  onCodeChange,
  onValueChange,
  onCodeTypeChange,
}: PresetDetailsProps) => {
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
          <HubHopPresetDetails preset={preset} variant={sim} />
        ) : (
          (code ?? "") !== "" && (
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
      </div>
      {sim === "xplane" && (
        <XplanePresetCustomize
          mode={mode}
          code={code}
          codeType={codeType!}
          onCodeTypeChange={onCodeTypeChange}
          onCodeChange={onCodeChange}
          onValueChange={onValueChange}
          value={value}
        />
      )}
      {sim === "msfs" && (
        <MsfsPresetCustomize
          code={code}
          onCodeChange={onCodeChange}
        />
      )}
    </WizardSection>
  )
}

export default PresetDetails
