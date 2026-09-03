import { useTranslation } from "react-i18next"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Checkbox } from "@/components/ui/checkbox"
import { Label } from "@/components/ui/label"
import { Input } from "@/components/ui/input"
import { Separator } from "@/components/ui/separator"
import ComboBox from "@/components/ComboBox"
import { LogLevel } from "@/types/log"
import Settings from "@/types/settings"

interface GeneralSettingsCardProps {
  values: Partial<Settings>
  onChange: <K extends keyof Settings>(key: K, value: Settings[K]) => void
}

export default function GeneralSettingsCard({
  values,
  onChange,
}: GeneralSettingsCardProps) {
  const { t } = useTranslation()

  const logEnabled = values.LogEnabled ?? false
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
      <CardHeader>
        <CardTitle className="text-xl font-bold">
          {t("Settings.General.Title")}
        </CardTitle>
      </CardHeader>

      <CardContent className="space-y-6">
        {/* Recent Files */}
        <div className="space-y-2">
          <h3 className="text-sm font-semibold">
            {t("Settings.General.RecentFiles.Title")}
          </h3>
          <div className="flex items-center gap-4">
            <Label
              htmlFor="recent-files"
              className="text-sm text-muted-foreground"
            >
              {t("Settings.General.RecentFiles.Description")}
            </Label>
            <Input
              id="recent-files"
              type="number"
              min={0}
              max={20}
              className="w-24"
              value={values.RecentFilesMaxCount ?? 5}
              onChange={(e) =>
                onChange("RecentFilesMaxCount", parseInt(e.target.value) || 0)
              }
            />
          </div>
        </div>

        <Separator />

        {/* Logging */}
        <div className="space-y-3">
          <h3 className="text-sm font-semibold">
            {t("Settings.General.Logging.Title")}
          </h3>
          <div className="flex flex-wrap items-center gap-6">
            <div className="flex items-center space-x-2">
              <Checkbox
                id="logging-enable"
                checked={logEnabled}
                onCheckedChange={(checked) => onChange("LogEnabled", !!checked)}
              />
              <Label htmlFor="logging-enable" className="cursor-pointer">
                {t("Settings.General.Logging.Enabled")}
              </Label>
            </div>

            <div className="flex items-center gap-2">
              <Label
                htmlFor="log-level"
                className={`text-sm ${
                  !logEnabled ? "text-muted-foreground" : ""
                }`}
              >
                {t("Settings.General.Logging.LogLevel")}:
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
                widthClass="w-32"
              />
            </div>

            <div className="flex items-center space-x-2">
              <Checkbox
                id="logging-joystick-axis"
                checked={values.LogJoystickAxis ?? false}
                disabled={!logEnabled}
                onCheckedChange={(checked) =>
                  onChange("LogJoystickAxis", !!checked)
                }
              />
              <Label
                htmlFor="logging-joystick-axis"
                className={`cursor-pointer ${
                  !logEnabled ? "text-muted-foreground" : ""
                }`}
              >
                {t("Settings.General.Logging.LogJoystickAxis")}
              </Label>
            </div>
          </div>
        </div>

        <Separator />

        {/* Beta Versions */}
        <div className="space-y-2">
          <h3 className="text-sm font-semibold">
            {t("Settings.General.BetaVersions.Title")}
          </h3>
          <div className="flex items-center space-x-2">
            <Checkbox
              id="beta-updates"
              checked={values.BetaUpdates ?? false}
              onCheckedChange={(checked) => onChange("BetaUpdates", !!checked)}
            />
            <Label htmlFor="beta-updates" className="cursor-pointer text-sm">
              {t("Settings.General.BetaVersions.Description")}
            </Label>
          </div>
        </div>

        <Separator />

        {/* Community Feedback Program */}
        <div className="space-y-2">
          <h3 className="text-sm font-semibold">
            {t("Settings.General.CommunityFeedback.Title")}
          </h3>
          <div className="flex items-center space-x-2">
            <Checkbox
              id="community-feedback"
              checked={values.CommunityFeedback ?? false}
              onCheckedChange={(checked) =>
                onChange("CommunityFeedback", !!checked)
              }
            />
            <Label
              htmlFor="community-feedback"
              className="cursor-pointer text-sm"
            >
              {t("Settings.General.CommunityFeedback.Description")}
            </Label>
          </div>
        </div>

        <Separator />

        {/* Run options */}
        <div className="space-y-3">
          <h3 className="text-sm font-semibold">
            {t("Settings.General.RunOptions.Title")}
          </h3>
          <div className="space-y-2">
            <div className="flex items-center space-x-2">
              <Checkbox
                id="auto-retrigger"
                checked={values.AutoRetrigger ?? false}
                onCheckedChange={(checked) =>
                  onChange("AutoRetrigger", !!checked)
                }
              />
              <Label htmlFor="auto-retrigger" className="cursor-pointer text-sm">
                {t("Settings.General.RunOptions.AutoRetrigger")}
              </Label>
            </div>
            <div className="flex items-center space-x-2">
              <Checkbox
                id="minimize-on-autorun"
                checked={values.MinimizeOnAutoRun ?? false}
                onCheckedChange={(checked) =>
                  onChange("MinimizeOnAutoRun", !!checked)
                }
              />
              <Label
                htmlFor="minimize-on-autorun"
                className="cursor-pointer text-sm"
              >
                {t("Settings.General.RunOptions.MinimizeOnAutoRun")}
              </Label>
            </div>
          </div>
        </div>

        <Separator />

        {/* HubHop */}
        <div className="space-y-2">
          <h3 className="text-sm font-semibold">
            {t("Settings.General.HubHop.Title")}
          </h3>
          <div className="flex items-center space-x-2">
            <Checkbox
              id="hubhop-auto-check"
              checked={values.HubHopAutoCheck ?? false}
              onCheckedChange={(checked) =>
                onChange("HubHopAutoCheck", !!checked)
              }
            />
            <Label
              htmlFor="hubhop-auto-check"
              className="cursor-pointer text-sm"
            >
              {t("Settings.General.HubHop.AutoCheck")}
            </Label>
          </div>
        </div>

        <Separator />

        {/* Language */}
        <div className="space-y-2">
          <h3 className="text-sm font-semibold">
            {t("Settings.General.Language.Title")}
          </h3>
          <div className="flex flex-col gap-2">
            <div className="flex items-center gap-3">
              <Label className="text-sm text-muted-foreground">
                {t("Settings.General.Language.Description")}
              </Label>
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
            <p className="text-xs text-muted-foreground">
              {t("Settings.General.Language.RestartRequired")}
            </p>
          </div>
        </div>

        <Separator />

        {/* Speed Controls */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {/* Config Execution Speed */}
          <div className="space-y-2 p-3 rounded-md border bg-muted/20">
            <h3 className="text-sm font-semibold">
              {t("Settings.General.ExecutionSpeed.Title")}
            </h3>
            <div className="flex justify-between text-xs text-muted-foreground">
              <span>{t("Settings.General.ExecutionSpeed.Slow")}</span>
              <span>{t("Settings.General.ExecutionSpeed.Fast")}</span>
            </div>
            <input
              type="range"
              min="25"
              max="250"
              step="25"
              value={values.PollInterval ?? 50}
              onChange={(e) => onChange("PollInterval", Number(e.target.value))}
              className="w-full accent-primary cursor-pointer"
            />
            <p className="text-xs text-muted-foreground leading-relaxed">
              {t("Settings.General.ExecutionSpeed.Description")}
            </p>
          </div>

          {/* Test Mode Speed */}
          <div className="space-y-2 p-3 rounded-md border bg-muted/20">
            <h3 className="text-sm font-semibold">
              {t("Settings.General.TestModeSpeed.Title")}
            </h3>
            <div className="flex justify-between text-xs text-muted-foreground">
              <span>{t("Settings.General.TestModeSpeed.Slow")}</span>
              <span>{t("Settings.General.TestModeSpeed.Fast")}</span>
            </div>
            <input
              type="range"
              min="50"
              max="1000"
              step="50"
              value={values.TestTimerInterval ?? 50}
              onChange={(e) =>
                onChange("TestTimerInterval", Number(e.target.value))
              }
              className="w-full accent-primary cursor-pointer"
            />
          </div>
        </div>
      </CardContent>
    </Card>
  )
}

