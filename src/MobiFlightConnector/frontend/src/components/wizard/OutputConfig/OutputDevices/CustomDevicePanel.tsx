import DeviceIcon from "@/components/icons/DeviceIcon"
import { CustomDeviceConfig } from "@/types/config"
import { BaseDevice, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import ComboBox from "@/components/ComboBox"
import OutputDeviceComboBox from "@/components/wizard/OutputConfig/OutputDevices/OutputDeviceComboBox"

type MessageType = { id: number; label: string; description: string }

// TODO: use the message types of the device definition provided by the backend
const messageTypes: MessageType[] = [
  { id: 0, label: "Set value", description: "$ will be shown on the display" },
  { id: 1, label: "Set text", description: "$ as string will be displayed" },
  {
    id: 2,
    label: "Clear display",
    description: "$ = 1 will clear the display",
  },
]

export interface CustomDevicePanelProps {
  variant: "summary" | "details"
  config: CustomDeviceConfig
  controller?: Partial<Controller>
  // only used by the details variant
  devices?: BaseDevice[]
  onChange?: (config: CustomDeviceConfig) => void
}

const CustomDevicePanel = ({
  config,
  variant,
  controller,
  devices = [],
  onChange,
}: CustomDevicePanelProps) => {
  if (variant == "details") {
    const selectedMessageType = messageTypes.find(
      (messageType) => messageType.id === config.MessageType,
    )

    return (
      <Card>
        <CardContent className="flex flex-col gap-4 pt-4">
          <div className="flex flex-col gap-1">
            <div className="text-lg font-semibold">
              Step 2 - Custom Device settings
            </div>
            <div className="text-muted-foreground text-sm">
              Select the device and the message that receives the value.
            </div>
          </div>
          <div className="flex flex-row gap-4">
            <div className="flex flex-col gap-2">
              <Label>Device</Label>
              <OutputDeviceComboBox
                devices={devices}
                deviceType="CustomDevice"
                selectedName={config.CustomName}
                onChange={(name) =>
                  onChange?.({ ...config, CustomName: name, Name: name })
                }
              />
            </div>
            {config.CustomType && (
              <div className="flex flex-col gap-2">
                <Label>Type</Label>
                <Badge variant="secondary">{config.CustomType}</Badge>
              </div>
            )}
          </div>
          <div className="flex flex-col gap-2">
            <Label>Message type</Label>
            <ComboBox
              widthClass="w-56"
              align="start"
              items={messageTypes}
              getLabel={(messageType) => messageType.label}
              getValue={(messageType) => `${messageType.id}`}
              isSelected={(messageType, selected) =>
                messageType.id === selected?.id
              }
              selected={selectedMessageType}
              setSelected={(messageType) =>
                messageType &&
                onChange?.({ ...config, MessageType: messageType.id })
              }
            />
            {selectedMessageType && (
              <div className="text-muted-foreground text-sm">
                {selectedMessageType.description}
              </div>
            )}
          </div>
          <div className="flex flex-col gap-2">
            <Label htmlFor="custom-device-value">Value</Label>
            <Input
              id="custom-device-value"
              className="w-56"
              placeholder="$"
              value={config.Value}
              onChange={(e) => onChange?.({ ...config, Value: e.target.value })}
            />
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
            <DeviceIcon variant="CustomDevice" /> Custom Device
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Device</Label>
          <Badge variant="secondary">{config.CustomName || config.Name}</Badge>
        </div>
        {config.CustomType && (
          <div className="flex flex-col gap-1">
            <Label>Type</Label>
            <Badge variant="secondary">{config.CustomType}</Badge>
          </div>
        )}
      </div>
    )
  )
}

export default CustomDevicePanel
