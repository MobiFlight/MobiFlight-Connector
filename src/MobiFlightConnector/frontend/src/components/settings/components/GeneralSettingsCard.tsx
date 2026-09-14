import { useTranslation } from "react-i18next"
import { Card, CardContent } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import ComboBox from "@/components/ComboBox"
import { LogLevel } from "@/types/log"
import Settings from "@/types/settings"
import { Switch } from "@/components/ui/switch"
import SettingsRow from "./SettingsRow"
import { Slider } from "@/components/ui/slider"

interface GeneralSettingsCardProps {
  values: Partial<Settings>
  onChange: <K extends keyof Settings>(key: K, value: Settings[K]) => void
}

// TestMode interval steps matching WinForms: index 0 (1000ms/slow) to index 4 (50ms/fast)
const TEST_SPEED_INTERVALS = [1000, 500, 250, 125, 50]

export default function GeneralSettingsCard({
  values,
  onChange,
}: GeneralSettingsCardProps) {
  const { t } = useTranslation()

  const logEnabled = values.LogEnabled ?? true
  const logLevel = values.LogLevel ?? "info"
  const language = values.Language ?? ""

  // Execution Speed: 25ms (Fast / right) to 250ms (Slow / left)
  // Slider position (1 to 10): 1 = 250ms (Slow), 10 = 25ms (Fast)
  const currentPollInterval = values.PollInterval ?? 50
  const executionSpeedSliderValue = Math.max(
    1,
    Math.min(10, 11 - Math.round(currentPollInterval / 25)),
  )

  // Test Mode Speed: index 0 (1000ms / Slow) to index 4 (50ms / Fast)
  const currentTestInterval = values.TestTimerInterval ?? 50
  const testSpeedSliderValue = (() => {
    let closestIndex = 4
    let minDiff = Infinity
    TEST_SPEED_INTERVALS.forEach((val, idx) => {
      const diff = Math.abs(val - currentTestInterval)
      if (diff < minDiff) {
        minDiff = diff
        closestIndex = idx
      }
    })
    return closestIndex
  })()

  const logOptions: { value: LogLevel; label: string }[] = [
    { value: "debug", label: "Debug" },
    { value: "info", label: "Info" },
    { value: "warn", label: "Warn" },
    { value: "error", label: "Error" },
  ]

  const languageOptions = [
    { value: "", label: "System Default" },
    { value: "en-US", label: "English" },
    { value: "de-DE", label: "Deutsch" },
    { value: "es-ES", label: "Español" },
    { value: "fi-FI", label: "Suomi" },
    { value: "pt-PT", label: "Português" },
    { value: "ru-RU", label: "Русский" },
  ]

  return (
    <Card className="w-full">
      <CardContent className="flex flex-col gap-6 p-6">
        {/* Application */}
        <div className="flex flex-col gap-3">
          <h3 className="text-base font-bold">
            {t("Settings.General.Application.Title")}
          </h3>

          <div className="flex flex-col gap-1">
            <SettingsRow label={t("Settings.General.Language.Title")}>
              <ComboBox
                items={languageOptions}
                selected={languageOptions.find((opt) => opt.value === language)}
                getValue={(item) => item.value}
                getLabel={(item) => item.label}
                isSelected={(item, selected) => item.value === selected?.value}
                setSelected={(item) => item && onChange("Language", item.value)}
                widthClass="w-56"
              />
            </SettingsRow>

            <SettingsRow
              label={t("Settings.General.BetaVersions.Description")}
              htmlFor="beta-updates"
            >
              <Switch
                id="beta-updates"
                checked={values.BetaUpdates ?? false}
                onCheckedChange={(checked) =>
                  onChange("BetaUpdates", !!checked)
                }
              />
            </SettingsRow>

            <SettingsRow
              label={t("Settings.General.CommunityFeedback.Description")}
              htmlFor="community-feedback"
            >
              <Switch
                id="community-feedback"
                checked={values.CommunityFeedback ?? false}
                onCheckedChange={(checked) =>
                  onChange("CommunityFeedback", !!checked)
                }
              />
            </SettingsRow>
          </div>
        </div>

        <Separator />

        {/* Logging */}
        <div className="flex flex-col gap-3">
          <h3 className="text-base font-bold">
            {t("Settings.General.Logging.Title")}
          </h3>

          <div className="flex flex-col gap-1">
            <SettingsRow
              label={t("Settings.General.Logging.ShowLogPanel")}
              htmlFor="logging-enable"
            >
              <Switch
                id="logging-enable"
                checked={logEnabled}
                onCheckedChange={(checked) => onChange("LogEnabled", !!checked)}
              />
            </SettingsRow>

            <SettingsRow
              label={t("Settings.General.Logging.LogLevel")}
              htmlFor="log-level"
              disabled={!logEnabled}
            >
              <ComboBox
                items={logOptions}
                selected={logOptions.find(
                  (opt) => opt.value.toLowerCase() === logLevel.toLowerCase(),
                )}
                getValue={(item) => item.value}
                getLabel={(item) => item.label}
                isSelected={(item, selected) =>
                  item.value.toLowerCase() === selected?.value.toLowerCase()
                }
                setSelected={(item) =>
                  item && onChange("LogLevel", item.value as LogLevel)
                }
                disabled={!logEnabled}
                widthClass="w-56"
              />
            </SettingsRow>

            <SettingsRow
              label={t("Settings.General.Logging.LogJoystickAxis")}
              htmlFor="logging-joystick-axis"
              disabled={!logEnabled}
            >
              <Switch
                id="logging-joystick-axis"
                checked={values.LogJoystickAxis ?? false}
                disabled={!logEnabled}
                onCheckedChange={(checked) =>
                  onChange("LogJoystickAxis", !!checked)
                }
              />
            </SettingsRow>
          </div>
        </div>

        <Separator />

        {/* Startup and Run options */}
        <div className="flex flex-col gap-3">
          <h3 className="text-base font-bold">
            {t("Settings.General.StartupAndRunOptions.Title")}
          </h3>

          <div className="flex flex-col gap-1">
            <SettingsRow
              label={t("Settings.General.HubHop.AutoCheck")}
              htmlFor="hubhop-auto-check"
            >
              <Switch
                id="hubhop-auto-check"
                checked={values.HubHopAutoCheck ?? false}
                onCheckedChange={(checked) =>
                  onChange("HubHopAutoCheck", !!checked)
                }
              />
            </SettingsRow>

            <SettingsRow
              label={t("Settings.General.RunOptions.AutoRetrigger")}
              htmlFor="auto-retrigger"
            >
              <Switch
                id="auto-retrigger"
                checked={values.AutoRetrigger ?? false}
                onCheckedChange={(checked) =>
                  onChange("AutoRetrigger", !!checked)
                }
              />
            </SettingsRow>

            <SettingsRow
              label={t("Settings.General.RunOptions.MinimizeOnAutoRun")}
              htmlFor="minimize-on-autorun"
            >
              <Switch
                id="minimize-on-autorun"
                checked={values.MinimizeOnAutoRun ?? false}
                onCheckedChange={(checked) =>
                  onChange("MinimizeOnAutoRun", !!checked)
                }
              />
            </SettingsRow>

            <Separator className="my-2" />

            <div className="hover:bg-muted/70 -mx-2 flex items-center justify-between gap-6 rounded-md p-2 transition-colors">
              <div className="flex flex-1 flex-col gap-0.5">
                <Label className="text-sm font-medium">
                  {t("Settings.General.ExecutionSpeed.Title")}
                </Label>
                <p className="text-muted-foreground text-xs leading-relaxed">
                  {t("Settings.General.ExecutionSpeed.Description")}
                </p>
              </div>
              <div className="flex w-60 shrink-0 flex-col gap-1">
                <div className="text-muted-foreground flex justify-between text-[10px] whitespace-nowrap lowercase">
                  <span className="text-xs">
                    {t("Settings.General.ExecutionSpeed.Slow")}
                  </span>
                  <span className="text-xs">
                    {t("Settings.General.ExecutionSpeed.Fast")}
                  </span>
                </div>
                <Slider
                  min={1}
                  max={10}
                  step={1}
                  value={[executionSpeedSliderValue]}
                  onValueChange={([sliderVal]) => {
                    const pollIntervalMs = (11 - sliderVal) * 25
                    onChange("PollInterval", pollIntervalMs)
                  }}
                  className="w-full"
                />
              </div>
            </div>

            <div className="hover:bg-muted/70 -mx-2 flex items-center justify-between gap-6 rounded-md p-2 transition-colors">
              <Label className="text-sm font-medium">
                {t("Settings.General.TestModeSpeed.Title")}
              </Label>
              <div className="flex w-60 shrink-0 flex-col gap-1">
                <div className="text-muted-foreground flex justify-between text-[10px] whitespace-nowrap lowercase">
                  <span className="text-xs">
                    {t("Settings.General.TestModeSpeed.Slow")}
                  </span>
                  <span className="text-xs">
                    {t("Settings.General.TestModeSpeed.Fast")}
                  </span>
                </div>
                <Slider
                  min={0}
                  max={4}
                  step={1}
                  value={[testSpeedSliderValue]}
                  onValueChange={([idx]) => {
                    const testIntervalMs = TEST_SPEED_INTERVALS[idx] ?? 50
                    onChange("TestTimerInterval", testIntervalMs)
                  }}
                  className="w-full"
                />
              </div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}
