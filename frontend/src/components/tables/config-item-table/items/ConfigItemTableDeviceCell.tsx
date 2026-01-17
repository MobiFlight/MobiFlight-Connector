import DeviceIcon from "@/components/icons/DeviceIcon"
import ToolTip from "@/components/ToolTip"
import { IConfigItem, IDeviceConfig } from "@/types/config"
import { DeviceElementType } from "@/types/deviceElements"
import { IconBan, IconX } from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash-es"
import { useTranslation } from "react-i18next"
import StackedIcons from "@/components/icons/StackedIcons"
import { useControllerDefinitionsStore } from "@/stores/definitionStore"
import { mapJoystickDeviceNameToLabel } from "@/types/definitions"

interface ConfigItemTableDeviceCellProps {
  row: Row<IConfigItem>
}
function ConfigItemTableDeviceCell({
  row,
}: ConfigItemTableDeviceCellProps) {
    const { t } = useTranslation()
    const { JoystickDefinitions, MidiControllerDefinitions } = useControllerDefinitionsStore()
    const item = row.original as IConfigItem
    const Status = item.Status
    const Device = Status && !isEmpty(Status["Device"])

    const controllerName = item.ModuleSerial?.split(" / ")[0] ?? "not set"

    const joystickDefinition = JoystickDefinitions.find(
      (i) => i.InstanceName == controllerName,
    )

    const midiControllerDefinition = MidiControllerDefinitions.find(
      (i) => i.InstanceName == controllerName,
    )

    const deviceName =
      (item.Device as IDeviceConfig)?.Name ??
      (!isEmpty(item.DeviceName) ? item.DeviceName : "-")

    const deviceType =
      (item.Device as IDeviceConfig)?.Type ??
      (!isEmpty(item.DeviceType) ? item.DeviceType : "-")

    const icon = (
      <DeviceIcon
        disabled={!item.Active}
        variant={(deviceType ?? "default") as DeviceElementType}
      />
    )
    const mappedLabel = 
      joystickDefinition != null
        ? (mapJoystickDeviceNameToLabel(joystickDefinition, deviceName) ?? deviceName)
        : 
      midiControllerDefinition != null 
      ? (midiControllerDefinition.ProcessedLabels?.[deviceName] ?? deviceName)
      : deviceName


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
      `Types.${deviceType?.replace("MobiFlight.OutputConfigItem", "").replace("MobiFlight.InputConfigItem", "")}`,
    )

    const tooltipLabel = Device
      ? t(`ConfigList.Status.Device.${Status["Device"]}`)
      : typeLabel

    return deviceType != "-" ? (
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
}

export default ConfigItemTableDeviceCell
