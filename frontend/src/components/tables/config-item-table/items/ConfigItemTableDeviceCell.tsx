import DeviceIcon from "@/components/icons/DeviceIcon"
import ToolTip from "@/components/ToolTip"
import { IConfigItem, IDeviceConfig } from "@/types/config"
import { DeviceElementType } from "@/types/deviceElements"
import { IconBan, IconX } from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash-es"
import { useTranslation } from "react-i18next"
import { IDictionary, ConfigItemStatusType } from "@/types/config";
import StackedIcons from "@/components/icons/StackedIcons"

interface ConfigItemTableDeviceCellProps {
  row: Row<IConfigItem>
}
const ConfigItemTableDeviceCell = ({ row }: ConfigItemTableDeviceCellProps) => {
  const { t } = useTranslation()
  const item = row.original as IConfigItem
  const Status = row.getValue("Status") as IDictionary<string, ConfigItemStatusType>;
  const Device = Status && !isEmpty(Status["Device"]);

  const deviceLabel =
    (item.Device as IDeviceConfig)?.Name ??
    (!isEmpty(item.DeviceName) ? item.DeviceName : "-")
  const type =
    (item.Device as IDeviceConfig)?.Type ??
    (!isEmpty(item.DeviceType) ? item.DeviceType : "-")
  const icon = (
    <DeviceIcon
      
      disabled={!row.getValue("Active") as boolean}
      variant={(type ?? "default") as DeviceElementType}
    />
  )

  const statusIcon = Device ? (<StackedIcons bottomIcon={icon} topIcon={<IconX aria-label="Device" role="status" aria-disabled="false" />} />) : icon
  const typeLabel = t(
    `Types.${type?.replace("MobiFlight.OutputConfigItem", "").replace("MobiFlight.InputConfigItem", "")}`,
  )

  const tooltipLabel = Device ? t(`ConfigList.Status.Device.${Status["Device"]}`) : typeLabel

  return type != "-" ? (
    <ToolTip content={tooltipLabel}>
      <div className="flex flex-row items-center gap-2">
        {statusIcon}
        <div className="hidden flex-col lg:flex">
          <p className="text-md truncate">{deviceLabel}</p>
        </div>
      </div>
    </ToolTip>
  ) : (
    <ToolTip content={t("ConfigList.Cell.Device.not set")}>
    <div className="item-center flex flex-row gap-2 text-slate-400 w-full">
      <IconBan />
      <span className="hidden lg:inline">not set</span>
    </div>
    </ToolTip>
  )
}

export default ConfigItemTableDeviceCell
