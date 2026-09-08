import { useTranslation } from "react-i18next"
import { Card, CardContent } from "@/components/ui/card"
import { Switch } from "@/components/ui/switch"
import { Label } from "@/components/ui/label"
import { Input } from "@/components/ui/input"
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
        <div className="space-y-4">
          <h3 className="text-base font-bold">
            {t("Settings.Simulator.ProSim.Title")}
          </h3>

          {/* Host & Port */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="prosim-host" className="text-sm font-normal">
                {t("Settings.Simulator.ProSim.Host")}
              </Label>
              <Input
                id="prosim-host"
                type="text"
                value={proSimHost}
                onChange={(value) => onChange("ProSimHost", String(value))}
                placeholder="localhost"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="prosim-port" className="text-sm font-normal">
                {t("Settings.Simulator.ProSim.Port")}
              </Label>
              <Input
                id="prosim-port"
                type="number"
                value={proSimPort}
                onChange={(value) =>
                  onChange("ProSimPort", parseInt(String(value)) || 0)
                }
                placeholder="8080"
              />
            </div>
          </div>

          {/* Auto Connect */}
          <div className="flex items-center justify-between gap-4 py-1">
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

          {/* Max Retry Attempts */}
          <div className="flex items-center justify-between gap-4 py-1">
            <Label
              htmlFor="prosim-max-retry"
              className="text-sm font-normal"
            >
              {t("Settings.Simulator.ProSim.MaxRetryAttempts")}
            </Label>
            <Input
              id="prosim-max-retry"
              type="number"
              min={1}
              max={20}
              className="w-24 text-right"
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
      </CardContent>
    </Card>
  )
}
