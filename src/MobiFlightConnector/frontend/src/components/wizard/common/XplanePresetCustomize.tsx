import ComboBox from "@/components/ComboBox"
import Input from "@/components/Input"
import { Label } from "@/components/ui/label"
import { useTranslation } from "react-i18next"
import { ConfigItemType } from "@/types/config"

const INPUT_CODE_TYPE_OPTIONS: ("DataRef" | "Command")[] = [
  "DataRef",
  "Command",
]
const OUTPUT_CODE_TYPE_OPTIONS: "DataRef"[] = ["DataRef"]


type XplanePresetCustomizeProps = {
  mode: ConfigItemType
  codeType: "DataRef" | "Command" | null
  code: string | null
  value?: string | null
  onCodeTypeChange?: (codeType: "DataRef" | "Command") => void
  onCodeChange: (code: string) => void
  onValueChange?: (value: string) => void
}

const XplanePresetCustomize = ({
  mode,
  codeType,
  code,
  onCodeTypeChange,
  onCodeChange,
  onValueChange,
  value,
}: XplanePresetCustomizeProps) => {
  const { t } = useTranslation()

  return (
    <>
      <div className="flex flex-row gap-2">
        <div className="flex flex-col gap-1">
          <Label>
            {t(`Dialog.InputConfigWizard.InputActions.Xplane.InputTypeLabel`)}
          </Label>
          <ComboBox
            items={
              mode === "InputConfigItem"
                ? INPUT_CODE_TYPE_OPTIONS
                : OUTPUT_CODE_TYPE_OPTIONS
            }
            selected={codeType ?? undefined}
            placeholder={t(
              `Dialog.InputConfigWizard.InputActions.Xplane.SelectInputTypePlaceholder`,
            )}
            getLabel={(item) => item}
            getValue={(item) => item}
            isSelected={(item) => item === codeType}
            setSelected={(item) => {
              if (!item) return
              onCodeTypeChange?.(item)
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
            value={code ?? ""}
            onChange={onCodeChange}
            placeholder={t(
              "Dialog.InputConfigWizard.InputActions.Xplane.PathPlaceholder",
            )}
          />
          <div className="text-muted-foreground text-sm">
            {t("Dialog.InputConfigWizard.InputActions.Xplane.PathDescription")}
          </div>
        </div>
      </div>
      {(mode == "InputConfigItem" && codeType === "DataRef") && (
        <div className="flex flex-col gap-1">
          <Label htmlFor="value">
            {t(`Dialog.InputConfigWizard.InputActions.Xplane.ValueLabel`)}
          </Label>
          <Input
            className="font-mono text-sm whitespace-nowrap"
            id="value"
            value={value ?? ""}
            onChange={(value) => onValueChange?.(value)}
            placeholder={t(
              `Dialog.InputConfigWizard.InputActions.Xplane.ValuePlaceholder`,
            )}
          />
          <div className="text-muted-foreground text-sm">
            {t(
              `Dialog.InputConfigWizard.InputActions.Common.SupportedPlaceholders`,
            )}
          </div>
        </div>
      )}
    </>
  )
}

export default XplanePresetCustomize