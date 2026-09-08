import { useTranslation } from "react-i18next"
import { Card, CardContent } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import { Input } from "@/components/ui/input"
import { Separator } from "@/components/ui/separator"
import ComboBox from "@/components/ComboBox"
import { LogLevel } from "@/types/log"
import Settings from "@/types/settings"
import { Switch } from "@/components/ui/switch"

interface GeneralSettingsCardProps {
  values: Partial<Settings>
  onChange: <K extends keyof Settings>(key: K, value: Settings[K]) => void
}

export default function GeneralSettingsCard({
  values,
  onChange,
}: GeneralSettingsCardProps) {
  const { t } = useTranslation()

  const logEnabled = values.LogEnabled ?? true
  const logLevel = values.LogLevel ?? "info"
  const language = values.Language ?? ""

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
      <CardContent className="space-y-6 p-6">
        {/* Application */}
        <div className="space-y-3">
          <h3 className="text-base font-bold">
            {t("Settings.General.Application.Title")}
          </h3>

          <div className="space-y-1">
            <div className="flex items-center justify-between gap-4 -mx-2 rounded-md p-2 transition-colors hover:bg-muted/70">
              <div className="space-y-0.5">
                <Label className="text-sm font-normal">
                  {t("Settings.General.Language.Title")}
                </Label>
              </div>
              <ComboBox
                items={languageOptions}
                selected={languageOptions.find((opt) => opt.value === language)}
                getValue={(item) => item.value}
                getLabel={(item) => item.label}
                isSelected={(item, selected) => item.value === selected?.value}
                setSelected={(item) => item && onChange("Language", item.value)}
                widthClass="w-56"
              />
            </div>

            <div className="flex items-center justify-between gap-4 -mx-2 rounded-md p-2 transition-colors hover:bg-muted/70">
              <Label
                htmlFor="beta-updates"
                className="cursor-pointer text-sm font-normal"
              >
                {t("Settings.General.BetaVersions.Description")}
              </Label>
              <Switch
                id="beta-updates"
                checked={values.BetaUpdates ?? false}
                onCheckedChange={(checked) =>
                  onChange("BetaUpdates", !!checked)
                }
              />
            </div>

            <div className="flex items-center justify-between gap-4 -mx-2 rounded-md p-2 transition-colors hover:bg-muted/70">
              <Label
                htmlFor="community-feedback"
                className="cursor-pointer text-sm font-normal"
              >
                {t("Settings.General.CommunityFeedback.Description")}
              </Label>
              <Switch
                id="community-feedback"
                checked={values.CommunityFeedback ?? false}
                onCheckedChange={(checked) =>
                  onChange("CommunityFeedback", !!checked)
                }
              />
            </div>
          </div>
        </div>

        <Separator />

        {/* Logging */}
        <div className="space-y-3">
          <h3 className="text-base font-bold">
            {t("Settings.General.Logging.Title")}
          </h3>

          <div className="space-y-1">
            <div className="flex items-center justify-between gap-4 -mx-2 rounded-md p-2 transition-colors hover:bg-muted/70">
              <Label
                htmlFor="logging-enable"
                className="cursor-pointer text-sm font-normal"
              >
                {t("Settings.General.Logging.ShowLogPanel")}
              </Label>
              <Switch
                id="logging-enable"
                checked={logEnabled}
                onCheckedChange={(checked) => onChange("LogEnabled", !!checked)}
              />
            </div>

            <div className="flex items-center justify-between gap-4 -mx-2 rounded-md p-2 transition-colors hover:bg-muted/70">
              <Label
                htmlFor="log-level"
                className={`text-sm font-normal ${
                  !logEnabled ? "text-muted-foreground" : ""
                }`}
              >
                {t("Settings.General.Logging.LogLevel")}
              </Label>
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
            </div>

            <div className="flex items-center justify-between gap-4 -mx-2 rounded-md p-2 transition-colors hover:bg-muted/70">
              <Label
                htmlFor="logging-joystick-axis"
                className={`cursor-pointer text-sm font-normal ${
                  !logEnabled ? "text-muted-foreground" : ""
                }`}
              >
                {t("Settings.General.Logging.LogJoystickAxis")}
              </Label>
              <Switch
                id="logging-joystick-axis"
                checked={values.LogJoystickAxis ?? false}
                disabled={!logEnabled}
                onCheckedChange={(checked) =>
                  onChange("LogJoystickAxis", !!checked)
                }
              />
            </div>
          </div>
        </div>

        <Separator />

        {/* Startup and Run options */}
        <div className="space-y-3">
          <h3 className="text-base font-bold">
            {t("Settings.General.StartupAndRunOptions.Title")}
          </h3>

          <div className="space-y-1">
            <div className="flex items-center justify-between gap-4 -mx-2 rounded-md p-2 transition-colors hover:bg-muted/70">
              <Label
                htmlFor="hubhop-auto-check"
                className="cursor-pointer text-sm font-normal"
              >
                {t("Settings.General.HubHop.AutoCheck")}
              </Label>
              <Switch
                id="hubhop-auto-check"
                checked={values.HubHopAutoCheck ?? false}
                onCheckedChange={(checked) =>
                  onChange("HubHopAutoCheck", !!checked)
                }
              />
            </div>

            <div className="flex items-center justify-between gap-4 -mx-2 rounded-md p-2 transition-colors hover:bg-muted/70">
              <Label
                htmlFor="auto-retrigger"
                className="cursor-pointer text-sm font-normal"
              >
                {t("Settings.General.RunOptions.AutoRetrigger")}
              </Label>
              <Switch
                id="auto-retrigger"
                checked={values.AutoRetrigger ?? false}
                onCheckedChange={(checked) =>
                  onChange("AutoRetrigger", !!checked)
                }
              />
            </div>

            <div className="flex items-center justify-between gap-4 -mx-2 rounded-md p-2 transition-colors hover:bg-muted/70">
              <Label
                htmlFor="minimize-on-autorun"
                className="cursor-pointer text-sm font-normal"
              >
                {t("Settings.General.RunOptions.MinimizeOnAutoRun")}
              </Label>
              <Switch
                id="minimize-on-autorun"
                checked={values.MinimizeOnAutoRun ?? false}
                onCheckedChange={(checked) =>
                  onChange("MinimizeOnAutoRun", !!checked)
                }
              />
            </div>

            <div className="flex items-center justify-between gap-6 -mx-2 rounded-md p-2 transition-colors hover:bg-muted/70">
              <div className="flex-1 space-y-0.5">
                <Label className="text-sm font-medium">
                  {t("Settings.General.ExecutionSpeed.Title")}
                </Label>
                <p className="text-xs text-muted-foreground leading-relaxed">
                  {t("Settings.General.ExecutionSpeed.Description")}
                </p>
              </div>
              <div className="w-60 shrink-0 space-y-1">
                <div className="flex justify-between text-[10px] text-muted-foreground lowercase whitespace-nowrap">
                  <span>{t("Settings.General.ExecutionSpeed.Slow")}</span>
                  <span>{t("Settings.General.ExecutionSpeed.Fast")}</span>
                </div>
                <Input
                  type="range"
                  min="25"
                  max="250"
                  step="25"
                  value={values.PollInterval ?? 50}
                  onChange={(e) =>
                    onChange("PollInterval", Number(e.target.value))
                  }
                  className="w-full cursor-pointer accent-primary"
                />
              </div>
            </div>

            <div className="flex items-center justify-between gap-6 -mx-2 rounded-md p-2 transition-colors hover:bg-muted/70">
              <Label className="text-sm font-medium">
                {t("Settings.General.TestModeSpeed.Title")}
              </Label>
              <div className="w-60 shrink-0 space-y-1">
                <div className="flex justify-between text-[10px] text-muted-foreground lowercase whitespace-nowrap">
                  <span>{t("Settings.General.TestModeSpeed.Slow")}</span>
                  <span>{t("Settings.General.TestModeSpeed.Fast")}</span>
                </div>
                <Input
                  type="range"
                  min="50"
                  max="1000"
                  step="50"
                  value={values.TestTimerInterval ?? 50}
                  onChange={(e) =>
                    onChange("TestTimerInterval", Number(e.target.value))
                  }
                  className="w-full cursor-pointer accent-primary"
                />
              </div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}

