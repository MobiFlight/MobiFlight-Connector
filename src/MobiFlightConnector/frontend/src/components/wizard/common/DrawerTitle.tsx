import { ConfigItemType } from "@/types/config"
import { IconChevronRight } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

const DrawerTitle = ({
  mode,
  configName,
  drawerKey,
}: {
  mode: ConfigItemType
  configName: string
  drawerKey: string
}) => {
  const { t } = useTranslation("wizard")
  return (
    <div className="flex flex-row items-center gap-2">
      {mode === "InputConfigItem" ? (
        <div className="h-5 w-2 bg-cfginput"></div>
      ) : (
        <div className="h-5 w-2 bg-cfgoutput"></div>
      )}
      <span className="text-xl">{configName}</span>
      <IconChevronRight className="w-5 -mx-1 mt-0.5" />
      <span className="text-xl">{t(drawerKey)}</span>
    </div>
  )
}

export default DrawerTitle
