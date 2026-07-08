import ComboBox from "@/components/ComboBox"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"
import EventIdPresetsPanel from "@/components/wizard/components/InputActions/EventIdPresetsPanel"
import { EventIdInputAction, PmdgEventIdInputAction } from "@/types/config"
import { useTranslation } from "react-i18next"

export type EventIdInputActionPanelProps = {
  variant: "summary" | "details"
  options: "default" | "pmdg"
  config: EventIdInputAction | PmdgEventIdInputAction | null
  onConfigChange: (config: EventIdInputAction | PmdgEventIdInputAction) => void
}

type MouseParam = {
  Label: string
  Value: string
}

const EventIdInputActionPanel = ({
  variant,
  options,
  config,
  onConfigChange,
}: EventIdInputActionPanelProps) => {
  const { t } = useTranslation()
  const mouseParams = [
    { Label: "MOUSE_FLAG_RIGHTSINGLE", Value: "2147483648" },
    { Label: "MOUSE_FLAG_MIDDLESINGLE", Value: "1073741824" },
    { Label: "MOUSE_FLAG_LEFTSINGLE", Value: "536870912" },
    { Label: "MOUSE_FLAG_RIGHTDOUBLE", Value: "268435456" },
    { Label: "MOUSE_FLAG_MIDDLEDOUBLE", Value: "134217728" },
    { Label: "MOUSE_FLAG_LEFTDOUBLE", Value: "67108864" },
    { Label: "MOUSE_FLAG_RIGHTDRAG", Value: "33554432" },
    { Label: "MOUSE_FLAG_MIDDLEDRAG", Value: "16777216" },
    { Label: "MOUSE_FLAG_LEFTDRAG", Value: "8388608" },
    { Label: "MOUSE_FLAG_MOVE", Value: "4194304" },
    { Label: "MOUSE_FLAG_DOWN_REPEAT", Value: "2097152" },
    { Label: "MOUSE_FLAG_RIGHTRELEASE", Value: "524288" },
    { Label: "MOUSE_FLAG_MIDDLERELEASE", Value: "262144" },
    { Label: "MOUSE_FLAG_LEFTRELEASE", Value: "131072" },
    { Label: "MOUSE_FLAG_WHEEL_FLIP", Value: "65563" },
    { Label: "MOUSE_FLAG_WHEEL_SKIP", Value: "32768" },
    { Label: "MOUSE_FLAG_WHEEL_UP", Value: "16384" },
    { Label: "MOUSE_FLAG_WHEEL_DOWN", Value: "8192" },
  ] as MouseParam[]

  const isCustomParam =
    !mouseParams.some((item) => item.Value === config?.Param) ||
    config?.Param === "" ||
    config?.Param === undefined

  if (variant === "summary") {
    const label =
      options === "pmdg"
        ? t("Dialog.InputConfigWizard.InputActions.EventId.MouseParamLabel")
        : t("Dialog.InputConfigWizard.InputActions.EventId.CustomParamLabel")

    const param =
      options === "pmdg"
        ? (mouseParams.find((item) => item.Value === config?.Param)?.Label ??
          config?.Param)
        : config?.Param

    return (
      <div className="flex grow flex-row items-center gap-8">
        <div className="flex w-1/3 flex-col gap-1">
          <Label htmlFor="eventid">
            {t("Dialog.InputConfigWizard.InputActions.EventId.EventIdLabel")}:
          </Label>
          <div className="text-sm">{config?.EventId}</div>
        </div>
        <div className="flex grow flex-col gap-1">
          <Label htmlFor="param">{label}:</Label>
          <CodeValueLabel>{param}</CodeValueLabel>
        </div>
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-4">
      {options === "pmdg" && (
        <div className="flex flex-col gap-2">
          <div className="text-sm font-semibold">
            {t(
              "Dialog.InputConfigWizard.InputActions.EventId.PmdgAircraftLabel",
            )}
          </div>
          <RadioGroup
            defaultValue="B737"
            className="flex flex-row"
            value={(config as PmdgEventIdInputAction).AircraftType}
            onValueChange={(value) =>
              onConfigChange({
                ...(config as PmdgEventIdInputAction),
                AircraftType: value,
              } as PmdgEventIdInputAction)
            }
          >
            <div className="flex items-center gap-3">
              <RadioGroupItem value="B737" id="b737" />
              <Label htmlFor="B737">B737</Label>
            </div>
            <div className="flex items-center gap-3">
              <RadioGroupItem value="B747" id="b747" />
              <Label htmlFor="B747">B747</Label>
            </div>
            <div className="flex items-center gap-3">
              <RadioGroupItem value="B777" id="b777" />
              <Label htmlFor="B777">B777</Label>
            </div>
          </RadioGroup>
        </div>
      )}
      <EventIdPresetsPanel
        variant={options}
        aircraft={(config as PmdgEventIdInputAction).AircraftType}
        selectedPresetId={config?.EventId ?? null}
        setSelectedPreset={(preset) =>
          onConfigChange({
            ...config,
            EventId: preset ? preset.eventId : null,
            // reset param when changing preset
            Param: "0",
          } as EventIdInputAction | PmdgEventIdInputAction)
        }
      />
      <div className="flex w-100 flex-col gap-2">
        <Label htmlFor="eventId">
          {t("Dialog.InputConfigWizard.InputActions.EventId.EventIdLabel")}
        </Label>
        <Input
          id="eventId"
          value={config?.EventId ?? ""}
          onChange={(e) =>
            onConfigChange({ ...config, EventId: e.target.value } as
              | EventIdInputAction
              | PmdgEventIdInputAction)
          }
        />
      </div>
      {(options === "default" || isCustomParam) && (
        <div className="flex w-100 flex-col gap-2">
          <Label htmlFor="customParam">
            {t(
              "Dialog.InputConfigWizard.InputActions.EventId.CustomParamLabel",
            )}
          </Label>
          <Input
            className="font-mono text-sm whitespace-nowrap"
            id="customParam"
            value={config?.Param ?? ""}
            onChange={(e) =>
              onConfigChange({ ...config, Param: e.target.value } as
                | EventIdInputAction
                | PmdgEventIdInputAction)
            }
          />
        </div>
      )}
      {options === "pmdg" && (
        <div className="flex flex-col gap-2">
          <Label htmlFor="mouseParam">
            {t("Dialog.InputConfigWizard.InputActions.EventId.MouseParamLabel")}
          </Label>
          <ComboBox
            placeholder={t(
              "Dialog.InputConfigWizard.InputActions.EventId.SelectMouseParamPlaceholder",
            )}
            items={mouseParams}
            getLabel={(item) => item.Label}
            getValue={(item) => item.Value}
            isSelected={(item) => item.Value === config?.Param}
            selected={mouseParams.find((item) => item.Value === config?.Param)}
            setSelected={(item) =>
              onConfigChange({
                ...config,
                Param: item ? item.Value : "",
              } as EventIdInputAction | PmdgEventIdInputAction)
            }
            widthClass="w-100"
          />
        </div>
      )}
    </div>
  )
}
export default EventIdInputActionPanel
