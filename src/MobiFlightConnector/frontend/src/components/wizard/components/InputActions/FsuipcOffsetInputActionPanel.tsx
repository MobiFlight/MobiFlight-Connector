import { Input } from "@/components/ui/input"
import { FsuipcOffsetInputAction } from "@/types/config"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import ComboBox from "@/components/ComboBox"
import { useState } from "react"
import { useTranslation } from "react-i18next"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"

export type FsuipcOffsetInputActionPanelProps = {
  variant: "summary" | "details"
  config: FsuipcOffsetInputAction | null
  onConfigChange: (config: FsuipcOffsetInputAction) => void
}

const FsuipcSizeOptions = [
  { value: 1, label: "1 Byte" },
  { value: 2, label: "2 Bytes" },
  { value: 4, label: "4 Bytes" },
  { value: 8, label: "8 Bytes" },
]

const FSUIPC_TYPE_INTEGER = 0
const FSUIPC_TYPE_FLOAT = 1
const FSUIPC_TYPE_STRING = 2

const FsuipcTypeOptions = [
  { value: FSUIPC_TYPE_INTEGER, label: "Integer" },
  { value: FSUIPC_TYPE_FLOAT, label: "Float" },
  { value: FSUIPC_TYPE_STRING, label: "String" },
]

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

const filterHexInput = (e: React.KeyboardEvent<HTMLInputElement>) => {
  // Allow only hex characters and control keys
  if (
    !/[0-9a-fA-F]/.test(e.key) &&
    !["Backspace", "Delete", "ArrowLeft", "ArrowRight", "Tab"].includes(e.key)
  ) {
    e.preventDefault()
  }
}

const FsuipcOffsetInputActionPanel = ({
  variant,
  config,
  onConfigChange,
}: FsuipcOffsetInputActionPanelProps) => {
  const { t } = useTranslation()
  const currentConfig = config?.FSUIPC ? config : defaultConfig
  const selectedSizeOption = FsuipcSizeOptions.find(
    (option) => option.value === currentConfig.FSUIPC.Size,
  )
  const selectedTypeOption = FsuipcTypeOptions.find(
    (option) => option.value === currentConfig.FSUIPC.OffsetType,
  )
  const [mask, setMask] = useState<string | null>(null) // null = not editing

  const formattedMask = currentConfig.FSUIPC.Mask.toString(16)
    .toUpperCase()
    .padStart(currentConfig.FSUIPC.Size * 2, "0")
    .slice(-(currentConfig.FSUIPC.Size * 2))

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center gap-8">
        <div className="flex flex-col gap-1">
          <Label htmlFor="size">
            {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.SizeLabel")}
          </Label>
          <div className="text-sm">{currentConfig.FSUIPC.Size.toString()}</div>
        </div>
        <div className="flex flex-col gap-1">
          <Label htmlFor="offset">
            {t(
              "Dialog.InputConfigWizard.InputActions.FsuipcOffset.OffsetLabel",
            )}
          </Label>
          <div className="text-sm">
            {currentConfig.FSUIPC.Offset.toString(16)
              .toUpperCase()
              .padStart(4, "0")}
          </div>
        </div>
        <div className="flex flex-col gap-1">
          <Label htmlFor="mask">
            {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.MaskLabel")}
          </Label>
          <div className="text-sm">
            {currentConfig.FSUIPC.Mask.toString(16)
              .toUpperCase()
              .padStart(4, "0")}
          </div>
        </div>
        <div className="flex flex-col gap-1">
          <Label htmlFor="bcdMode">
            {t(
              "Dialog.InputConfigWizard.InputActions.FsuipcOffset.BcdModeLabel",
            )}
          </Label>
          <div id="bcdMode" className="text-sm">
            {currentConfig.FSUIPC.BcdMode ? "True" : "False"}
          </div>
        </div>
        <div className="flex grow flex-col gap-1">
          <Label htmlFor="value">
            {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.ValueLabel")}
          </Label>
          <CodeValueLabel>
            {currentConfig.Value}
          </CodeValueLabel>
        </div>
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-row gap-4">
        <div className="flex flex-col gap-1">
          <Label className="text-sm font-medium" htmlFor="type">
            {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.TypeLabel")}
          </Label>
          <ComboBox
            items={FsuipcTypeOptions}
            selected={selectedTypeOption}
            getValue={(option) => option.value.toString()}
            getLabel={(option) => option.label}
            isSelected={(option, selected) => option.value === selected?.value}
            setSelected={(option) =>
              onConfigChange({
                ...currentConfig,
                FSUIPC: {
                  ...currentConfig.FSUIPC,
                  OffsetType: option?.value ?? FSUIPC_TYPE_INTEGER,
                  // if switching to string, set size to 255
                  Size:
                    option?.value === FSUIPC_TYPE_STRING
                      ? 255
                      : currentConfig.FSUIPC.Size,
                },
              } as FsuipcOffsetInputAction)
            }
            variant="nofilter"
            widthClass="w-32"
          />
        </div>
        <div className="flex flex-col gap-1">
          <Label className="text-sm font-medium" htmlFor="size">
            {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.SizeLabel")}
          </Label>
          <ComboBox
            items={FsuipcSizeOptions}
            selected={selectedSizeOption}
            getValue={(option) => option.value.toString()}
            getLabel={(option) => option.label}
            isSelected={(option, selected) => option.value === selected?.value}
            setSelected={(option) =>
              onConfigChange({
                ...currentConfig,
                FSUIPC: {
                  ...currentConfig.FSUIPC,
                  Size: option?.value ?? 1,
                },
              } as FsuipcOffsetInputAction)
            }
            variant="nofilter"
            widthClass="w-32"
          />
        </div>
        <div className="flex flex-col gap-1">
          <Label className="text-sm font-medium" htmlFor="offset">
            {t(
              "Dialog.InputConfigWizard.InputActions.FsuipcOffset.OffsetLabel",
            )}
          </Label>
          <Input
            onKeyDown={filterHexInput}
            id="offset"
            value={currentConfig.FSUIPC.Offset.toString(16)
              .toUpperCase()
              .padStart(4, "0")
              .slice(-4)}
            onChange={(e) =>
              onConfigChange({
                ...currentConfig,
                FSUIPC: {
                  ...currentConfig.FSUIPC,
                  Offset: parseInt(e.target.value, 16),
                },
              } as FsuipcOffsetInputAction)
            }
            className="w-32"
          />
        </div>
        {currentConfig.FSUIPC.OffsetType === FSUIPC_TYPE_INTEGER && (
          <div className="flex flex-col gap-1">
            <Label className="text-sm font-medium" htmlFor="mask">
              {t(
                "Dialog.InputConfigWizard.InputActions.FsuipcOffset.MaskLabel",
              )}
            </Label>
            <Input
              className="field-sizing-content"
              id="mask"
              value={mask ?? formattedMask}
              onKeyDown={filterHexInput}
              onFocus={() => setMask(formattedMask)}
              onChange={(e) =>
                setMask(
                  e.target.value
                    .toUpperCase()
                    .slice(-currentConfig.FSUIPC.Size * 2),
                )
              }
              onBlur={() => {
                const newMask = (mask ?? formattedMask)
                  .toUpperCase()
                  .padStart(currentConfig.FSUIPC.Size * 2, "0")
                  .slice(-(currentConfig.FSUIPC.Size * 2))
                setMask(null)
                onConfigChange({
                  ...currentConfig,
                  FSUIPC: {
                    ...currentConfig.FSUIPC,
                    Mask: parseInt(newMask, 16),
                  },
                } as FsuipcOffsetInputAction)
              }}
            />
          </div>
        )}
        {currentConfig.FSUIPC.OffsetType === FSUIPC_TYPE_INTEGER && (
          <div className="flex flex-row items-center gap-2 pt-5">
            <Switch
              id="bcdMode"
              checked={currentConfig.FSUIPC.BcdMode}
              onCheckedChange={(e) =>
                onConfigChange({
                  ...currentConfig,
                  FSUIPC: { ...currentConfig.FSUIPC, BcdMode: e },
                } as FsuipcOffsetInputAction)
              }
            />
            <Label className="text-sm font-medium" htmlFor="bcdMode">
              {t(
                "Dialog.InputConfigWizard.InputActions.FsuipcOffset.BcdModeLabel",
              )}
            </Label>
          </div>
        )}
      </div>
      <div className="flex flex-col gap-2">
        <Label htmlFor="value">
          {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.ValueLabel")}
        </Label>
        <Input
          className="font-mono text-sm whitespace-nowrap"
          id="value"
          value={config?.Value ?? ""}
          onChange={(e) =>
            onConfigChange({
              ...(config as FsuipcOffsetInputAction),
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
  )
}
export default FsuipcOffsetInputActionPanel
