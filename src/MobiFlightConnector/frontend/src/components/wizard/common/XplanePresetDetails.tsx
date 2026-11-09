import ComboBox from "@/components/ComboBox"
import Input from "@/components/Input"
import { Label } from "@/components/ui/label"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import { HubHopPresetDetails } from "@/components/wizard/components/HubHopPresetDetails"
import { XplaneInputAction } from "@/types/config"
import { XplanePreset } from "@/types/preset"
import { IconExclamationCircle } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export interface XplanePresetDetailsProps {
  variant: "input" | "output"
  preset: XplanePreset | null
  config: XplaneInputAction | null
  onConfigChange: (config: XplaneInputAction) => void
}

const INPUT_CODE_TYPE_OPTIONS: ("DataRef" | "Command")[] = [
  "DataRef",
  "Command",
]
const OUTPUT_CODE_TYPE_OPTIONS: "DataRef"[] = ["DataRef"]

const XplanePresetDetails = ({
  preset,
  variant,
  config,
  onConfigChange,
}: XplanePresetDetailsProps) => {
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
          <HubHopPresetDetails preset={preset} variant="xplane" />
        ) : (
          (config?.Path ?? "") !== "" && (
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
        <div className="flex flex-row gap-2">
          <div className="flex flex-col gap-1">
            <Label>
              {t("Dialog.InputConfigWizard.InputActions.Xplane.InputTypeLabel")}
            </Label>
            <ComboBox
              items={
                variant === "input"
                  ? INPUT_CODE_TYPE_OPTIONS
                  : OUTPUT_CODE_TYPE_OPTIONS
              }
              selected={
                (config?.InputType as "DataRef" | "Command") ?? undefined
              }
              placeholder={t(
                "Dialog.InputConfigWizard.InputActions.Xplane.SelectInputTypePlaceholder",
              )}
              getLabel={(item) => item}
              getValue={(item) => item}
              isSelected={(item) => item === config?.InputType}
              setSelected={(item) => {
                if (!item) return
                onConfigChange({
                  ...(config as XplaneInputAction),
                  InputType: item,
                })
              }}
              variant="nofilter"
              widthClass="w-32"
            />
          </div>
          <div className="flex flex-col gap-1">
            <Label htmlFor="path">
              {t("Dialog.InputConfigWizard.InputActions.Xplane.PathLabel")}
            </Label>
            <Input
              id="path"
              className="font-mono text-sm whitespace-nowrap"
              value={config?.Path ?? ""}
              onChange={(e) =>
                onConfigChange({
                  ...(config as XplaneInputAction),
                  Path: e,
                })
              }
              placeholder={t(
                "Dialog.InputConfigWizard.InputActions.Xplane.PathPlaceholder",
              )}
            />
            <div className="text-muted-foreground text-sm">
              {t(
                "Dialog.InputConfigWizard.InputActions.Xplane.PathDescription",
              )}
            </div>
          </div>
        </div>
        {variant == "input" && config?.InputType === "DataRef" && (
          <div className="flex flex-col gap-1">
            <Label htmlFor="value">
              {t("Dialog.InputConfigWizard.InputActions.Xplane.ValueLabel")}
            </Label>
            <Input
              className="font-mono text-sm whitespace-nowrap"
              id="value"
              value={config?.Expression ?? ""}
              onChange={(e) =>
                onConfigChange({
                  ...(config as XplaneInputAction),
                  Expression: e,
                })
              }
              placeholder={t(
                "Dialog.InputConfigWizard.InputActions.Xplane.ValuePlaceholder",
              )}
            />
            <div className="text-muted-foreground text-sm">
              {t(
                "Dialog.InputConfigWizard.InputActions.Common.SupportedPlaceholders",
              )}
            </div>
          </div>
        )}
      </div>
    </WizardSection>
  )
}
export default XplanePresetDetails
