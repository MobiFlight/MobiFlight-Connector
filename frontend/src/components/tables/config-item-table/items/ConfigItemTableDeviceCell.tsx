import DeviceIcon from "@/components/icons/DeviceIcon"
import ToolTip from "@/components/ToolTip"
import { IConfigItem, IDeviceConfig } from "@/types/config"
import { DeviceElementType } from "@/types/deviceElements"
import { IconBan, IconX } from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash-es"
import { useTranslation } from "react-i18next"
import StackedIcons from "@/components/icons/StackedIcons"
import React from "react"
import { useControllerDefinitionsStore } from "@/stores/definitionStore"
import { mapJoystickNameToLabel } from "@/types/definitions"

interface ConfigItemTableDeviceCellProps {
  row: Row<IConfigItem>
}
const ConfigItemTableDeviceCell = React.memo(
  ({ row }: ConfigItemTableDeviceCellProps) => {
    const { t } = useTranslation()
    const { JoystickDefinitions } = useControllerDefinitionsStore()
    const item = row.original as IConfigItem
    const Status = item.Status
    const Device = Status && !isEmpty(Status["Device"])

    const instanceName = item.ModuleSerial.split("/")[0].trim() ?? "not set"

    const JoystickDefinition = JoystickDefinitions.find(i => i.InstanceName == instanceName)
    
    
    const deviceName = (item.Device as IDeviceConfig)?.Name ?? "-"
    const type = (item.Device as IDeviceConfig)?.Type ?? "-"
    
    const icon = (
      <DeviceIcon
        disabled={!item.Active}
        variant={(type ?? "default") as DeviceElementType}
      />
    )
    const mappedLabel = JoystickDefinition != null ? mapJoystickNameToLabel(JoystickDefinition, deviceName) ?? deviceName : deviceName
    const statusIcon = Device ? (
      <StackedIcons
        bottomIcon={icon}
        topIcon={
          <IconX aria-label="Device" role="status" aria-disabled="false" />
        }
      />
    ) : (
      icon
    )
    const typeLabel = t(
      `Types.${type?.replace("MobiFlight.OutputConfigItem", "").replace("MobiFlight.InputConfigItem", "")}`,
    )

    const tooltipLabel = Device
      ? t(`ConfigList.Status.Device.${Status["Device"]}`)
      : typeLabel

    return type != "-" ? (
      <ToolTip content={tooltipLabel}>
        <div className="flex flex-row items-center gap-2">
          {statusIcon}
          <span className="text-md hidden truncate lg:inline">
            {mappedLabel}
          </span>
        </div>
      </ToolTip>
    ) : (
      <ToolTip content={t("ConfigList.Cell.Device.not set")}>
        <div className="flex flex-row items-center gap-2 text-slate-400">
          <IconBan />
          <span className="hidden lg:inline">not set</span>
        </div>
      </ToolTip>
    )
  },
)

export default ConfigItemTableDeviceCell
