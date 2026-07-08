import ComboBox from "@/components/ComboBox"
import { Input } from "@/components/ui/input"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { publishOnMessageExchange, useAppMessage } from "@/lib/hooks/appMessage"
import { useVJoyControllerStore } from "@/stores/controllerStore"
import { VJoyInputAction } from "@/types/config"
import { VJoyDefinitionsUpdate } from "@/types/messages"
import { Switch } from "@/components/ui/switch"
import { Label } from "@/components/ui/label"
import { useTranslation } from "react-i18next"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"

export type VJoyInputActionPanelProps = {
  variant: "summary" | "details"
  config: VJoyInputAction | null
  setConfig: (item: VJoyInputAction) => void
}

const VJoyInputActionPanel = ({
  variant,
  config,
  setConfig,
}: VJoyInputActionPanelProps) => {
  const { t } = useTranslation()
  const { publish } = publishOnMessageExchange()
  const { vJoyDefinitions, setVJoyDefinitions } = useVJoyControllerStore()

  useAppMessage("VJoyDefinitionsUpdate", (message) => {
    const { Definitions } = message.payload as VJoyDefinitionsUpdate
    setVJoyDefinitions(Definitions)
  })

  if (vJoyDefinitions.length === 0) {
    publish({
      key: "CommandRefreshPresets",
      payload: {
        type: "vjoy",
      },
    })
  }

  const vJoyOptions = vJoyDefinitions.map((def) => ({
    label: t("Dialog.InputConfigWizard.InputActions.VJoy.ControllerLabel", {
      index: def.Id,
    }),
    value: def.Id,
  }))

  const selectedDevice = vJoyDefinitions.find(
    (def) => def.Id === config?.vJoyID,
  )

  const selectedDeviceOption = vJoyOptions.find((def) => {
    return def.value === config?.vJoyID
  })

  const axisOptions = selectedDevice
    ? Object.keys(selectedDevice.Axis).filter(
        (key) => (selectedDevice.Axis as Record<string, boolean>)[key],
      )
    : []

  const buttonOptions = selectedDevice
    ? Array.from({ length: selectedDevice.Buttons }, (_, i) => i + 1)
    : []

  const activeTab = config?.axisString ? "axis" : "button"

  const controllerLabel = config?.vJoyID
    ? t("Dialog.InputConfigWizard.InputActions.VJoy.ControllerLabel", {
        index: config?.vJoyID,
      })
    : t("Dialog.InputConfigWizard.InputActions.VJoy.None")

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center justify-between gap-8">
        <div className="flex flex-col gap-1">
          <Label>
            {t("Dialog.InputConfigWizard.InputActions.VJoy.Controller")}:
          </Label>
          <div className="text-sm">{controllerLabel}</div>
        </div>
        <div className="flex flex-col gap-1">
          <Label>
            {t("Dialog.InputConfigWizard.InputActions.VJoy.Device")}:
          </Label>
          {activeTab === "button" && (
            <div className="text-sm">Button {config?.buttonNr}</div>
          )}
          {activeTab === "axis" && (
            <div className="text-sm">Axis {config?.axisString}</div>
          )}
        </div>
        {activeTab === "button" && (
          <div className="flex flex-col gap-1">
            <Label>
              {t("Dialog.InputConfigWizard.InputActions.VJoy.ButtonStateLabel")}
            </Label>
            <div className="text-sm">
              {config?.buttonComand
                ? t("Dialog.InputConfigWizard.InputActions.VJoy.Pressed")
                : t("Dialog.InputConfigWizard.InputActions.VJoy.Released")}
            </div>
          </div>
        )}
        {activeTab === "axis" && (
          <div className="flex flex-col gap-1">
            <Label>
              {t("Dialog.InputConfigWizard.InputActions.VJoy.AxisValueLabel")}
            </Label>
            <CodeValueLabel>{config?.sendValue ?? "-"}</CodeValueLabel>
          </div>
        )}
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col gap-2">
        <Label htmlFor="joystick">
          {t("Dialog.InputConfigWizard.InputActions.VJoy.SelectDeviceLabel")}
        </Label>
        <ComboBox
          placeholder={t(
            "Dialog.InputConfigWizard.InputActions.VJoy.SelectDevicePlaceholder",
          )}
          items={vJoyOptions}
          getLabel={(item) => item.label}
          getValue={(item) => item.value.toString()}
          isSelected={(item) => item.value === selectedDeviceOption?.value}
          selected={selectedDeviceOption}
          setSelected={(item) =>
            setConfig({
              ...(config ?? {}),
              vJoyID: item ? Number(item.value) : undefined,
            } as VJoyInputAction)
          }
          widthClass="w-100"
          variant="nofilter"
        />
      </div>
      <Tabs
        defaultValue={activeTab}
        onValueChange={(e) => {
          if (e === "button") {
            // we are switching type to button, unset AXIS
            setConfig({
              ...(config ?? {}),
              axisString: "",
            } as VJoyInputAction)
          } else {
            // we are switching type to axis, set buttonNr to -1
            setConfig({
              ...(config ?? {}),
              buttonNr: -1,
            } as VJoyInputAction)
          }
        }}
      >
        <TabsList>
          <TabsTrigger key="button" value="button">
            {t("Dialog.InputConfigWizard.InputActions.VJoy.ButtonTab")}
          </TabsTrigger>
          <TabsTrigger key="axis" value="axis">
            {t("Dialog.InputConfigWizard.InputActions.VJoy.AxisTab")}
          </TabsTrigger>
        </TabsList>
        <TabsContent key="button" value="button">
          <div className="flex flex-col gap-4 pt-2">
            <div className="flex flex-col gap-2">
              <Label htmlFor="buttonNr">
                {t(
                  "Dialog.InputConfigWizard.InputActions.VJoy.ButtonNumberLabel",
                )}
              </Label>
              <ComboBox
                placeholder={t(
                  "Dialog.InputConfigWizard.InputActions.VJoy.SelectButtonPlaceholder",
                )}
                items={buttonOptions}
                getLabel={(item) => `Button ${item}`}
                getValue={(item) => item.toString()}
                isSelected={(item) => item === config?.buttonNr}
                selected={buttonOptions.find(
                  (item) => item === config?.buttonNr,
                )}
                setSelected={(item) =>
                  setConfig({
                    ...(config ?? {}),
                    buttonNr: item ? Number(item) : undefined,
                  } as VJoyInputAction)
                }
                variant="nofilter"
              />
            </div>
            <div className="flex flex-col gap-2">
              <Label htmlFor="buttonCommand">
                {t(
                  "Dialog.InputConfigWizard.InputActions.VJoy.ButtonStateLabel",
                )}
              </Label>
              <div className="flex flex-row items-center gap-2">
                <Switch
                  id="buttonCommand"
                  checked={config?.buttonComand ?? false}
                  onCheckedChange={(checked) =>
                    setConfig({
                      ...(config ?? {}),
                      buttonComand: checked,
                    } as VJoyInputAction)
                  }
                />
                <span
                  className="text-sm"
                  data-testid="vjoy-button-command-state"
                >
                  {config?.buttonComand
                    ? t("Dialog.InputConfigWizard.InputActions.VJoy.Pressed")
                    : t("Dialog.InputConfigWizard.InputActions.VJoy.Released")}
                </span>
              </div>
            </div>
          </div>
        </TabsContent>
        <TabsContent key="axis" value="axis">
          <div className="flex flex-col gap-4 pt-2">
            <ComboBox
              placeholder={t(
                "Dialog.InputConfigWizard.InputActions.VJoy.SelectAxisPlaceholder",
              )}
              items={axisOptions}
              getLabel={(item) => item}
              getValue={(item) => item}
              isSelected={(item) => item === config?.axisString}
              selected={axisOptions.find((item) => item === config?.axisString)}
              setSelected={(item) =>
                setConfig({
                  ...(config ?? {}),
                  axisString: item ? item : undefined,
                } as VJoyInputAction)
              }
              variant="nofilter"
            />
            <div className="flex flex-col gap-2">
              <Label htmlFor="axisValue">
                {t("Dialog.InputConfigWizard.InputActions.VJoy.AxisValueLabel")}
              </Label>
              <Input
                className="font-mono text-sm whitespace-nowrap"
                id="axisValue"
                value={config?.sendValue ?? "1"}
                onChange={(e) =>
                  setConfig({
                    ...(config ?? {}),
                    sendValue: e.target.value,
                  } as VJoyInputAction)
                }
              />
            </div>
          </div>
        </TabsContent>
      </Tabs>
    </div>
  )
}
export default VJoyInputActionPanel
