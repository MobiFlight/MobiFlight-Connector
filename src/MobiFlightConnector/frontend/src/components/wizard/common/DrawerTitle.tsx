import { ConfigItemType } from "@/types/config"
import { IconChevronRight } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

const DrawerTitle = ({
  variant,
  configName,
  drawerKey,
}: {
  variant: ConfigItemType
  configName: string
  drawerKey: string
}) => {
  const { t } = useTranslation()
  return (
    <div className="flex flex-row items-center gap-2">
      {variant === "InputConfigItem" ? (
        <div className="h-5 w-2 bg-cfginput"></div>
      ) : (
        <div className="h-5 w-2 bg-cfgoutput"></div>
      )}
      <span className="text-xl">{configName}</span>
      <IconChevronRight className="h-6" />
      <span className="text-xl">{t(drawerKey)}</span>
    </div>
  )
}

export default DrawerTitle
