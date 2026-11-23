import DeviceIcon from "@/components/icons/DeviceIcon"
import { LcdDisplayConfig, OutputConfigItem } from "@/types/config"
import { DeviceReference, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import OutputDeviceComboBox from "@/components/wizard/OutputConfig/OutputDevices/OutputDeviceComboBox"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import { useTranslation } from "react-i18next"
import {
  LcdDisplayConfigEditor,
  LcdEditorView,
} from "@/components/wizard/OutputConfig/OutputDevices/LcdDisplayConfigEditor"
import { useLcdDisplayPreview } from "@/lib/hooks/useLcdDisplayPreview"
import { useState } from "react"

export interface LcdDisplayPanelProps {
  variant: "summary" | "details"
  config: LcdDisplayConfig
  // the whole draft config item, needed by the details variant for the preview
  configItem?: OutputConfigItem
  controller?: Partial<Controller>
  devices?: DeviceReference[]
  onChange?: (config: LcdDisplayConfig) => void
}

const LcdDisplayPanel = ({
  config,
  configItem,
  variant,
  controller,
  devices = [],
  onChange,
}: LcdDisplayPanelProps) => {
  const { t } = useTranslation("wizard")
  const [view, setView] = useState<LcdEditorView>("template")
  const previewLines = useLcdDisplayPreview(configItem, view === "livepreview")

  // We have to derive type information from config (by serial prefix)
  // because controller might not be connected
  const isMobiFlightController =
    configItem?.Controller?.Serial?.startsWith("SN-")

  if (variant == "details") {
    const selectedDevice = devices.find(
      (device) => device.Name === config.Address,
    )

    return (
      <WizardSection>
        <SectionHeadline
          title={t("ConfigWizard.Device.LcdDisplay.Title")}
          description={t("ConfigWizard.Device.LcdDisplay.Description")}
        />
        <div className="flex flex-col gap-2">
          <Label>{t("ConfigWizard.Device.LcdDisplay.Label")}</Label>
          <OutputDeviceComboBox
            devices={devices}
            deviceType="LcdDisplay"
            selectedName={config.Address}
            onChange={(address) =>
              onChange?.({ ...config, Address: address, Name: address })
            }
          />
        </div>
        {isMobiFlightController && (
          <LcdDisplayConfigEditor
            config={config}
            device={selectedDevice}
            configRefs={configItem?.ConfigRefs ?? []}
            configId={configItem?.GUID ?? ""}
            previewLines={previewLines}
            view={view}
            onViewChange={setView}
            onChange={(config) => onChange?.(config)}
          />
        )}
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
            <DeviceIcon variant="LcdDisplay" />{" "}
            {t("ConfigWizard.Device.LcdDisplay.Label")}
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.Device")}</Label>
          <Badge variant="secondary">{config.Name}</Badge>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.Device.LcdDisplay.Lines")}</Label>
          <Badge variant="secondary">{config.Lines?.length ?? 0}</Badge>
        </div>
      </div>
    )
  )
}

export default LcdDisplayPanel
