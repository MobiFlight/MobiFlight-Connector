import DeviceIcon from "@/components/icons/DeviceIcon"
import ToolTip from "@/components/ToolTip"
import { IConfigItem, IDeviceConfig } from "@/types/config"
import { DeviceElementType } from "@/types/deviceElements"
import { IconBan } from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { isEmpty } from "lodash-es"
import { useTranslation } from "react-i18next"

interface ConfigItemTableDeviceCellProps {
  row: Row<IConfigItem>
}
const ConfigItemTableDeviceCell = ({ row }: ConfigItemTableDeviceCellProps) => {
  const { t } = useTranslation()
  const item = row.original as IConfigItem
  const label =
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

  const typeLabel = t(
    `Types.${type?.replace("MobiFlight.OutputConfigItem", "").replace("MobiFlight.InputConfigItem", "")}`,
  )
  return type != "-" ? (
    <ToolTip content={typeLabel}>
      <div className="flex flex-row items-center gap-2">
        {icon}
        <div className="hidden flex-col lg:flex">
          <p className="text-md truncate">{label}</p>
        </div>
      </div>
    </ToolTip>
  ) : (
    <ToolTip content={t("ConfigList.Cell.Device.not set")}>
    <div className="item-center flex flex-row gap-2 text-slate-400">
      <IconBan />
      <span className="hidden lg:inline">not set</span>
    </div>
    </ToolTip>
  )
}

export default ConfigItemTableDeviceCell
