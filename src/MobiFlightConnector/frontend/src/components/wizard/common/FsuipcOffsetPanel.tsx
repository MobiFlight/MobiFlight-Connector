import { Input } from "@/components/ui/input"
import { ConfigItemType, FsuipcOffset } from "@/types/config"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import ComboBox from "@/components/ComboBox"
import { useState } from "react"
import { useTranslation } from "react-i18next"
import CodeValueLabel from "@/components/wizard/common/CodeValueLabel"
import { Badge } from "@/components/ui/badge"

const FsuipcSizeOptions = [
  { value: 1, label: "1 Byte" },
  { value: 2, label: "2 Bytes" },
  { value: 4, label: "4 Bytes" },
  { value: 8, label: "8 Bytes" },
]

export const FSUIPC_TYPE_INTEGER = 0
const FSUIPC_TYPE_FLOAT = 1
const FSUIPC_TYPE_STRING = 2

const FsuipcTypeOptions = [
  { value: FSUIPC_TYPE_INTEGER, label: "Integer" },
  { value: FSUIPC_TYPE_FLOAT, label: "Float" },
  { value: FSUIPC_TYPE_STRING, label: "String" },
]

const filterHexInput = (e: React.KeyboardEvent<HTMLInputElement>) => {
  // Allow only hex characters and control keys
  if (
    !/[0-9a-fA-F]/.test(e.key) &&
    !["Backspace", "Delete", "ArrowLeft", "ArrowRight", "Tab"].includes(e.key)
  ) {
    e.preventDefault()
  }
}

export interface FsuipcOffsetSummaryProps {
  mode: ConfigItemType
  config: FsuipcOffset
  value?: string
}
export const FsuipcOffsetSummary = ({
  mode,
  config,
  value,
}: FsuipcOffsetSummaryProps) => {
  const { t } = useTranslation()
  return (
    <div className="flex grow flex-row items-center gap-8">
      <div className="flex flex-col gap-1">
        <Label htmlFor="size">
          {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.SizeLabel")}
        </Label>
        <Badge variant="secondary" className="w-fit">
          {config.Size.toString()}
        </Badge>
      </div>
      <div className="flex flex-col gap-1">
        <Label htmlFor="offset">
          {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.OffsetLabel")}
        </Label>
        <Badge variant="secondary" className="w-fit">
          {config.Offset.toString(16).toUpperCase().padStart(4, "0")}
        </Badge>
      </div>
      <div className="flex flex-col gap-1">
        <Label htmlFor="mask">
          {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.MaskLabel")}
        </Label>
        <Badge variant="secondary" className="w-fit">
          {config.Mask.toString(16).toUpperCase().padStart(4, "0")}
        </Badge>
      </div>
      <div className="flex flex-col gap-1">
        <Label htmlFor="bcdMode">
          {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.BcdModeLabel")}
        </Label>
        <Badge variant="secondary" className="w-fit" id="bcdMode">
          {config.BcdMode ? "True" : "False"}
        </Badge>
      </div>
      {mode === "InputConfigItem" && (
        <div className="flex flex-col gap-1">
          <Label htmlFor="value">
            {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.ValueLabel")}
          </Label>
          <CodeValueLabel>{value ?? " "}</CodeValueLabel>
        </div>
      )}
    </div>
  )
}

export const FsuipcOffsetPanel = ({
  fsuipc,
  onConfigChange,
}: {
  fsuipc: FsuipcOffset
  onConfigChange: (fsuipc: FsuipcOffset) => void
}) => {
  const { t } = useTranslation()

  const selectedTypeOption = FsuipcTypeOptions.find(
    (option) => option.value === fsuipc.OffsetType,
  )
  const selectedSizeOption = FsuipcSizeOptions.find(
    (option) => option.value === fsuipc.Size,
  )

  const [mask, setMask] = useState<string | null>(null) // null = not editing

  const formattedMask = fsuipc.Mask.toString(16)
    .toUpperCase()
    .padStart(fsuipc.Size * 2, "0")
    .slice(-(fsuipc.Size * 2))

  return (
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
              ...fsuipc,
              OffsetType: option?.value ?? FSUIPC_TYPE_INTEGER,
              // if switching to string, set size to 255
              Size: option?.value === FSUIPC_TYPE_STRING ? 255 : fsuipc.Size,
            } as FsuipcOffset)
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
              ...fsuipc,
              Size: option?.value ?? 1,
            } as FsuipcOffset)
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
          value={fsuipc.Offset.toString(16)
            .toUpperCase()
            .padStart(4, "0")
            .slice(-4)}
          onChange={(e) =>
            onConfigChange({
              ...fsuipc,
              Offset: parseInt(e.target.value, 16),
            } as FsuipcOffset)
          }
          className="w-32"
        />
      </div>
      {fsuipc.OffsetType === FSUIPC_TYPE_INTEGER && (
        <div className="flex flex-col gap-1">
          <Label className="text-sm font-medium" htmlFor="mask">
            {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.MaskLabel")}
          </Label>
          <Input
            className="field-sizing-content"
            id="mask"
            value={mask ?? formattedMask}
            onKeyDown={filterHexInput}
            onFocus={() => setMask(formattedMask)}
            onChange={(e) =>
              setMask(e.target.value.toUpperCase().slice(-fsuipc.Size * 2))
            }
            onBlur={() => {
              const newMask = (mask ?? formattedMask)
                .toUpperCase()
                .padStart(fsuipc.Size * 2, "0")
                .slice(-(fsuipc.Size * 2))
              setMask(null)
              onConfigChange({
                ...fsuipc,
                Mask: parseInt(newMask, 16),
              } as FsuipcOffset)
            }}
          />
        </div>
      )}
      {fsuipc.OffsetType === FSUIPC_TYPE_INTEGER && (
        <div className="flex flex-row items-center gap-2 pt-5">
          <Switch
            id="bcdMode"
            checked={fsuipc.BcdMode}
            onCheckedChange={(e) =>
              onConfigChange({
                ...fsuipc,
                BcdMode: e,
              } as FsuipcOffset)
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
  )
}
