import { useTranslation } from "react-i18next"
import { Card, CardContent } from "@/components/ui/card"
import { Switch } from "@/components/ui/switch"
import Input from "@/components/Input"
import Settings from "@/types/settings"
import SettingsRow from "./SettingsRow"

interface SimulatorSettingsCardProps {
  values: Partial<Settings>
  onChange: <K extends keyof Settings>(key: K, value: Settings[K]) => void
}

export default function SimulatorSettingsCard({
  values,
  onChange,
}: SimulatorSettingsCardProps) {
  const { t } = useTranslation()

  const proSimHost = values.ProSimHost ?? "localhost"
  const proSimPort = values.ProSimPort ?? 8080
  const proSimAutoConnect = values.ProSimAutoConnectEnabled ?? false
  const proSimMaxRetry = values.ProSimMaxRetryAttempts ?? 5

  return (
    <Card className="w-full">
      <CardContent className="flex flex-col gap-6 p-6">
        <div className="flex flex-col gap-3">
          <h3 className="text-base font-bold">
            {t("Settings.Simulator.ProSim.Title")}
          </h3>

          <div className="flex flex-col gap-1">
            {/* Auto Connect */}
            <SettingsRow
              label={t("Settings.Simulator.ProSim.AutoConnect")}
              htmlFor="prosim-auto-connect"
            >
              <Switch
                id="prosim-auto-connect"
                checked={proSimAutoConnect}
                onCheckedChange={(checked) =>
                  onChange("ProSimAutoConnectEnabled", !!checked)
                }
              />
            </SettingsRow>

            {/* Connection Settings: Host & Port */}
            <SettingsRow
              label={t("Settings.Simulator.ProSim.ConnectionSettings")}
              htmlFor="prosim-host"
            >
              <div className="flex items-center gap-2">
                <Input
                  id="prosim-host"
                  type="text"
                  value={proSimHost}
                  onChange={(value) => onChange("ProSimHost", String(value))}
                  placeholder={t("Settings.Simulator.ProSim.Host", "Host")}
                  className="w-32"
                />
                <span className="text-muted-foreground text-sm">:</span>
                <Input
                  id="prosim-port"
                  type="number"
                  value={proSimPort}
                  onChange={(value) =>
                    onChange("ProSimPort", parseInt(String(value)) || 0)
                  }
                  placeholder={t("Settings.Simulator.ProSim.Port", "Port")}
                  className="w-20"
                />
              </div>
            </SettingsRow>

            {/* Max Retry Attempts */}
            <SettingsRow
              label={t("Settings.Simulator.ProSim.MaxRetryAttempts")}
              htmlFor="prosim-max-retry"
            >
              <Input
                id="prosim-max-retry"
                type="number"
                min={1}
                max={20}
                className="w-20"
                value={proSimMaxRetry}
                onChange={(value) =>
                  onChange(
                    "ProSimMaxRetryAttempts",
                    parseInt(String(value)) || 1,
                  )
                }
              />
            </SettingsRow>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}
