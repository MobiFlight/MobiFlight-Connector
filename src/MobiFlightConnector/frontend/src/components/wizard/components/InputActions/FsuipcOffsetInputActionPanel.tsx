import { Input } from "@/components/ui/input"
import { FsuipcOffsetInputAction } from "@/types/config"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import ComboBox from "@/components/ComboBox"
import { useState } from "react"
import { useTranslation } from "react-i18next"

export type FsuipcOffsetInputActionPanelProps = {
  config: FsuipcOffsetInputAction | null
  onConfigChange: (config: FsuipcOffsetInputAction) => void
}

const FsuipcSizeOptions = [
  { value: 1, label: "1 Byte" },
  { value: 2, label: "2 Bytes" },
  { value: 4, label: "4 Bytes" },
  { value: 8, label: "8 Bytes" },
]

const defaultConfig: FsuipcOffsetInputAction = {
  Type: "FsuipcOffsetInputAction",
  FSUIPC: {
    OffsetType: "Integer",
    Offset: 0x66C0,
    Size: 1,
    Mask: 0xFF,
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
  config,
  onConfigChange,
}: FsuipcOffsetInputActionPanelProps) => {
  const { t } = useTranslation()
  const currentConfig = config?.FSUIPC ? config : defaultConfig
  const selectedSizeOption = FsuipcSizeOptions.find(
    (option) => option.value === currentConfig.FSUIPC.Size,
  )
  const [mask, setMask] = useState<string | null>(null) // null = not editing

  const formattedMask = currentConfig.FSUIPC.Mask.toString(16)
    .toUpperCase()
    .padStart(currentConfig.FSUIPC.Size * 2, "0")
    .slice(-(currentConfig.FSUIPC.Size * 2))

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-row gap-4">
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
            {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.OffsetLabel")}
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
          />
        </div>
        <div className="flex flex-col gap-1">
          <Label className="text-sm font-medium" htmlFor="mask">
            {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.MaskLabel")}
          </Label>
          <Input
            autoComplete="off"
            id="mask"
            value={mask ?? formattedMask}
            onKeyDown={filterHexInput}
            onFocus={() => setMask(formattedMask)}
            onChange={(e) => setMask(e.target.value.toUpperCase().slice(-currentConfig.FSUIPC.Size * 2))}
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
        <div className="flex flex-row items-center gap-1 pt-6">
          <Label className="text-sm font-medium" htmlFor="bcdMode">
            {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.BcdModeLabel")}
          </Label>
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
        </div>
      </div>
    </div>
  )
}
export default FsuipcOffsetInputActionPanel
