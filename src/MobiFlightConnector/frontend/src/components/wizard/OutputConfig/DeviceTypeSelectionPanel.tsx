import ComboBox from "@/components/ComboBox"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import { ControllerComboBox } from "@/components/wizard/common/ConfigTrigger"
import { Controller, OutputDeviceType } from "@/types/controller"
import { useTranslation } from "react-i18next"

export type DeviceTypeSelectionPanelProps = {
  controllers: Partial<Controller>[]
  selectedController?: Partial<Controller>
  onControllerChange: (controller: Partial<Controller> | undefined) => void
  deviceTypes: OutputDeviceType[]
  selectedDeviceType?: OutputDeviceType
  onDeviceTypeChange: (deviceType: OutputDeviceType | null) => void
}

const DeviceTypeSelectionPanel = ({
  controllers,
  selectedController,
  onControllerChange,
  deviceTypes,
  selectedDeviceType,
  onDeviceTypeChange,
}: DeviceTypeSelectionPanelProps) => {
  const { t } = useTranslation("wizard")

  return (
    <div className="flex flex-col gap-4">
      <SectionHeadline
        title={t("ConfigWizard.DeviceType.Title")}
        description={t("ConfigWizard.DeviceType.Description")}
      />
      <div className="flex flex-row items-center gap-4">
        <div className="flex flex-col gap-2">
          <ControllerComboBox
            completeControllers={controllers}
            selectedController={selectedController}
            onChange={onControllerChange}
          />
        </div>
        <div className="flex flex-col gap-2">
          <ComboBox
            items={deviceTypes}
            getLabel={(deviceType) =>
              t(`ConfigWizard.DeviceType.Options.${deviceType}.label`)
            }
            getValue={(deviceType) => deviceType}
            selected={selectedDeviceType}
            isSelected={(deviceType, selected) => deviceType === selected}
            setSelected={(deviceType) => onDeviceTypeChange(deviceType ?? null)}
          />
        </div>
      </div>
    </div>
  )
}
export default DeviceTypeSelectionPanel
