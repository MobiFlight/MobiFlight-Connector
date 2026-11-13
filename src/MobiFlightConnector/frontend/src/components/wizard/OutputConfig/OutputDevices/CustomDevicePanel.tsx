import DeviceIcon from "@/components/icons/DeviceIcon"
import { CustomDeviceConfig } from "@/types/config"
import { DeviceReference, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Input } from "@/components/ui/input"
import ComboBox from "@/components/ComboBox"
import OutputDeviceComboBox from "@/components/wizard/OutputConfig/OutputDevices/OutputDeviceComboBox"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import { useTranslation } from "react-i18next"

export interface CustomDevicePanelProps {
  variant: "summary" | "details"
  config: CustomDeviceConfig
  controller?: Partial<Controller>
  // only used by the details variant
  devices?: DeviceReference[]
  onChange?: (config: CustomDeviceConfig) => void
}

const CustomDevicePanel = ({
  config,
  variant,
  controller,
  devices = [],
  onChange,
}: CustomDevicePanelProps) => {
  const { t } = useTranslation("wizard")
  if (variant == "details") {
    const selectedDevice = devices.find(
      (device) => device.Name === config.CustomName
    )
    // Labels and descriptions come from the custom device definition.
    // They are not translation keys, so never pass them through t().
    const messageTypes = selectedDevice?.Capabilities?.MessageTypes ?? []
    const selectedMessageType = messageTypes.find(
      (messageType) => messageType.Id === config.MessageType,
    )

    return (
      <WizardSection>
        <SectionHeadline
          title={t("ConfigWizard.Device.CustomDevice.Title")}
          description={t("ConfigWizard.Device.CustomDevice.Description")}
        />
        <div className="flex flex-row gap-4">
          <div className="flex flex-col gap-2">
            <Label>{t("ConfigWizard.General.Device")}</Label>
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
              <Label>{t("ConfigWizard.Device.CustomDevice.Type")}</Label>
              <Badge variant="secondary">{config.CustomType}</Badge>
            </div>
          )}
        </div>
        <div className="flex flex-col gap-2">
          <Label>{t("ConfigWizard.Device.CustomDevice.MessageType")}</Label>
          <ComboBox
            widthClass="w-56"
            align="start"
            items={messageTypes}
            getLabel={(messageType) => messageType.Label}
            getValue={(messageType) => `${messageType.Id}`}
            isSelected={(messageType, selected) =>
              messageType.Id === selected?.Id
            }
            selected={selectedMessageType}
            setSelected={(messageType) =>
              messageType &&
              onChange?.({ ...config, MessageType: messageType.Id })
            }
          />
          {selectedMessageType && (
            <div className="text-muted-foreground text-sm">
              {selectedMessageType.Description}
            </div>
          )}
        </div>
        <div className="flex flex-col gap-2">
          <Label htmlFor="custom-device-value">
            {t("ConfigWizard.Device.CustomDevice.Value")}
          </Label>
          <Input
            id="custom-device-value"
            className="w-56"
            placeholder="$"
            value={config.Value}
            onChange={(e) => onChange?.({ ...config, Value: e.target.value })}
          />
        </div>
      </WizardSection>
    )
  }

  return (
    variant == "summary" && (
      <div className="flex grow flex-row items-center justify-between gap-2">
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.Controller")}</Label>
          {controller?.Name}
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.DisplayType")}</Label>
          <span className="flex flex-row gap-2">
            <DeviceIcon variant="CustomDevice" />{" "}
            {t("ConfigWizard.Device.CustomDevice.Label")}
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.Device")}</Label>
          <Badge variant="secondary">{config.CustomName || config.Name}</Badge>
        </div>
        {config.CustomType && (
          <div className="flex flex-col gap-1">
            <Label>{t("ConfigWizard.Device.CustomDevice.Type")}</Label>
            <Badge variant="secondary">{config.CustomType}</Badge>
          </div>
        )}
      </div>
    )
  )
}

export default CustomDevicePanel
