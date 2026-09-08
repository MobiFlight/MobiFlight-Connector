import { useTranslation } from "react-i18next"
import { Card, CardContent } from "@/components/ui/card"
import { Switch } from "@/components/ui/switch"
import { Label } from "@/components/ui/label"
import Input from "@/components/Input"
import Settings from "@/types/settings"

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
      <CardContent className="space-y-6 p-6">
        <div className="space-y-3">
          <h3 className="text-base font-bold">
            {t("Settings.Simulator.ProSim.Title")}
          </h3>

          <div className="space-y-1">
            {/* Auto Connect */}
            <div className="hover:bg-muted/40 -mx-2 flex items-center justify-between gap-4 rounded-md p-2 transition-colors">
              <Label
                htmlFor="prosim-auto-connect"
                className="cursor-pointer text-sm font-normal"
              >
                {t("Settings.Simulator.ProSim.AutoConnect")}
              </Label>
              <Switch
                id="prosim-auto-connect"
                checked={proSimAutoConnect}
                onCheckedChange={(checked) =>
                  onChange("ProSimAutoConnectEnabled", !!checked)
                }
              />
            </div>

            {/* Connection Settings: Host & Port */}
            <div className="hover:bg-muted/40 -mx-2 flex items-center justify-between gap-4 rounded-md p-2 transition-colors">
              <Label htmlFor="prosim-host" className="text-sm font-normal">
                {t("Settings.Simulator.ProSim.ConnectionSettings")}
              </Label>
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
            </div>

            {/* Max Retry Attempts */}
            <div className="hover:bg-muted/40 -mx-2 flex items-center justify-between gap-4 rounded-md p-2 transition-colors">
              <Label htmlFor="prosim-max-retry" className="text-sm font-normal">
                {t("Settings.Simulator.ProSim.MaxRetryAttempts")}
              </Label>
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
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}
