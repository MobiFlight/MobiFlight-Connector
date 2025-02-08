import DeviceIcon from "@/components/icons/DeviceIcon"
import { IConfigItem, IDeviceConfig } from "@/types/config"
import { DeviceElementType } from "@/types/deviceElements"
import { IconBan } from "@tabler/icons-react"
import { Row } from "@tanstack/react-table"
import { useTranslation } from "react-i18next"

interface ConfigItemTableDeviceCellProps {
  row: Row<IConfigItem>
}
const ConfigItemTableDeviceCell = ({ row }: ConfigItemTableDeviceCellProps) => {
  const { t } = useTranslation()
  const label = (row.getValue("Device") as IDeviceConfig)?.Name ?? "-"
  const type = (row.getValue("Device") as IDeviceConfig)?.Type ?? "-"
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
    <div className="flex flex-row items-center gap-2 md:w-32">
      <div>{icon}</div>
      <div className="hidden w-full flex-col md:flex">
        <p className="text-md truncate font-semibold">{label}</p>
        <p className="truncate text-xs text-muted-foreground">{typeLabel}</p>
      </div>
    </div>
  ) : (
    <div className="item-center flex flex-row gap-2 text-slate-400">
      <IconBan />
      <span>not set</span>
    </div>
  )
}

export default ConfigItemTableDeviceCell
