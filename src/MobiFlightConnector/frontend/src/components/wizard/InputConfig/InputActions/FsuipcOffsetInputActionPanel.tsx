import { Input } from "@/components/ui/input"
import { FsuipcOffsetInputAction } from "@/types/config"
import { Label } from "@/components/ui/label"
import { useTranslation } from "react-i18next"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import {
  FSUIPC_TYPE_INTEGER,
  FsuipcOffsetPanel,
  FsuipcOffsetSummary,
} from "@/components/wizard/common/FsuipcOffsetPanel"

export type FsuipcOffsetInputActionPanelProps = {
  variant: "summary" | "details"
  config: FsuipcOffsetInputAction | null
  onConfigChange: (config: FsuipcOffsetInputAction) => void
}

const defaultConfig: FsuipcOffsetInputAction = {
  Type: "FsuipcOffsetInputAction",
  FSUIPC: {
    OffsetType: FSUIPC_TYPE_INTEGER,
    Offset: 0x66c0,
    Size: 1,
    Mask: 0xff,
    BcdMode: false,
  },
  Modifiers: [],
  Value: "",
}

const FsuipcOffsetInputActionPanel = ({
  variant,
  config,
  onConfigChange,
}: FsuipcOffsetInputActionPanelProps) => {
  const { t } = useTranslation()
  const currentConfig = config?.FSUIPC ? config : defaultConfig

  if (variant === "summary") {
    return (
      <FsuipcOffsetSummary
        mode="InputConfigItem"
        config={currentConfig.FSUIPC}
        value={currentConfig.Value}
      />
    )
  }

  return (
    <WizardSection>
      <SectionHeadline
        title={t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.Title")}
        description={t(
          "Dialog.InputConfigWizard.InputActions.FsuipcOffset.Description",
        )}
      />
      <div className="flex flex-col gap-4">
        <FsuipcOffsetPanel
          fsuipc={currentConfig.FSUIPC}
          onConfigChange={(fsuipc) =>
            onConfigChange({
              ...(currentConfig as FsuipcOffsetInputAction),
              FSUIPC: fsuipc,
            })
          }
        />

        <div className="flex flex-col gap-2">
          <Label htmlFor="value">
            {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.ValueLabel")}
          </Label>
          <Input
            className="font-mono text-sm whitespace-nowrap"
            id="value"
            value={currentConfig?.Value ?? ""}
            onChange={(e) =>
              onConfigChange({
                ...(currentConfig as FsuipcOffsetInputAction),
                Value: e.target.value,
              })
            }
            placeholder={t(
              "Dialog.InputConfigWizard.InputActions.FsuipcOffset.ValuePlaceholder",
            )}
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
export default FsuipcOffsetInputActionPanel
