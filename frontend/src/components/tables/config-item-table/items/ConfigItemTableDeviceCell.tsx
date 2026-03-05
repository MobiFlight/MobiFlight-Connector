import DeviceIcon from "@/components/icons/DeviceIcon"
import ToolTip from "@/components/ToolTip"
import {
  ExtendedDeviceConfig,
  IConfigItem,
  IDeviceConfig,
} from "@/types/config"
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

function DetermineDeviceName(item: IConfigItem): string[] {
  const deviceType = (item.Device as IDeviceConfig)?.Type ?? item.DeviceType
  const deviceName = (item.Device as IDeviceConfig)?.Name ?? item.DeviceName
  
  if (!deviceName || isEmpty(deviceName)) {
    if (deviceType === "InputAction") {
      return ["Input Action"]
    }
    return ["-"]
  }
  
  const deviceNames = deviceName.split("|").map((name) => name.trim())
  return deviceNames
}

function DetermineSubIndices(item: IConfigItem): string[] {
  const extendedDevice = item.Device as ExtendedDeviceConfig
  const subIndex =
    extendedDevice?.SubIndex != null ? `${extendedDevice.SubIndex}` : null
  const firstName =
    ((item.Device as IDeviceConfig)?.Name ?? item.DeviceName)?.split("|")[0] ??
    ""

  const pins = extendedDevice?.Pin?.split("|")
  const pinLabels =
    item.Device?.Type == "ShiftRegister"
      ? pins?.map((pin) => pin.match(/\d+/)?.[0] ?? "")
      : (pins?.length ?? 0) > 0
        ? pins?.filter((pin) => pin != firstName)
        : []
  return [...(pinLabels ?? []), ...(subIndex ? [subIndex] : [])].filter(
    (index) => index != null,
  )
}

function ConfigItemTableDeviceCell({ row }: ConfigItemTableDeviceCellProps) {
  const { t } = useTranslation()
  const { JoystickDefinitions, MidiControllerDefinitions } =
    useControllerDefinitionsStore()
  const item = row.original as IConfigItem
  const Status = item.Status
  const Device = Status && !isEmpty(Status["Device"])

  const controllerName = item.Controller?.Name ?? ""

  const joystickDefinition = JoystickDefinitions.find(
    (i) => i.InstanceName == controllerName,
  )

  const midiControllerDefinition = MidiControllerDefinitions.find(
    (i) => i.InstanceName == controllerName,
  )

  const deviceNames = DetermineDeviceName(item)
  const deviceName = deviceNames[0]
  const subIndices = DetermineSubIndices(item)

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
      ? (mapJoystickDeviceNameToLabel(joystickDefinition, deviceName) ??
        deviceName)
      : midiControllerDefinition != null
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
        <div className="hidden flex-col lg:flex truncate">
          <div className="max-w-full inline-block truncate" data-testid="device-name">{mappedLabel}</div>
          {subIndices.length > 0 && (
            <div className="flex flex-row items-center gap-2 truncate text-xs text-slate-400" data-testid="device-sub-index">
              {subIndices.filter((index) => index != null).join(", ")}
            </div>
          )}
        </div>
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
