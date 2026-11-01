import DeviceIcon from "@/components/icons/DeviceIcon"
import { LcdDisplayConfig } from "@/types/config"
import { BaseDevice, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import OutputDeviceComboBox from "@/components/wizard/OutputConfig/OutputDevices/OutputDeviceComboBox"

// TODO: use the display size provided by the backend
const LCD_ROWS = 2
const LCD_COLS = 20

export interface LcdDisplayPanelProps {
  variant: "summary" | "details"
  config: LcdDisplayConfig
  controller?: Partial<Controller>
  // only used by the details variant
  devices?: BaseDevice[]
  onChange?: (config: LcdDisplayConfig) => void
}

const LcdDisplayPanel = ({
  config,
  variant,
  controller,
  devices = [],
  onChange,
}: LcdDisplayPanelProps) => {
  if (variant == "details") {
    const lines = Array.from(
      { length: LCD_ROWS },
      (_, index) => config.Lines?.[index] ?? "",
    )

    const updateLine = (index: number, text: string) => {
      const newLines = [...lines]
      newLines[index] = text
      onChange?.({ ...config, Lines: newLines })
    }

    return (
      <Card>
        <CardContent className="flex flex-col gap-4 pt-4">
          <div className="flex flex-col gap-1">
            <div className="text-lg font-semibold">
              Step 2 - LCD Display settings
            </div>
            <div className="text-muted-foreground text-sm">
              Select the display and define the text for each line.
            </div>
          </div>
          <div className="flex flex-col gap-2">
            <Label>Display</Label>
            <OutputDeviceComboBox
              devices={devices}
              deviceType="LcdDisplay"
              selectedName={config.Address}
              onChange={(address) =>
                onChange?.({ ...config, Address: address, Name: address })
              }
            />
          </div>
          <div className="flex flex-col gap-2">
            <Label>Lines</Label>
            {lines.map((line, index) => (
              <Input
                key={index}
                aria-label={`Line ${index + 1}`}
                className="w-auto font-mono"
                size={LCD_COLS}
                maxLength={LCD_COLS}
                value={line}
                onChange={(e) => updateLine(index, e.target.value)}
              />
            ))}
          </div>
        </CardContent>
      </Card>
    )
  }

  return (
    variant == "summary" && (
      <div className="flex grow flex-row items-center justify-between gap-2">
        <div className="flex flex-col gap-1">
          <Label>Controller</Label>
          {controller?.Name}
        </div>
        <div className="flex flex-col gap-1">
          <Label>Display type</Label>
          <span className="flex flex-row gap-2">
            <DeviceIcon variant="LcdDisplay" /> LCD Display
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Device</Label>
          <Badge variant="secondary">{config.Name}</Badge>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Lines</Label>
          <Badge variant="secondary">{config.Lines?.length ?? 0}</Badge>
        </div>
      </div>
    )
  )
}

export default LcdDisplayPanel
