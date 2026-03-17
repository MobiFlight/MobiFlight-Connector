import { IconHourglassEmpty } from "@tabler/icons-react"
import ToolTip from "../ToolTip"
import { useTranslation } from "react-i18next"

const ValueIcon = () => {
  const { t } = useTranslation()
  
  return (
    <div className="flex flex-row justify-center text-slate-100 group-hover/row:text-gray-300/50 group-data-[state=selected]/row:text-slate-300/20 dark:text-slate-500/5 dark:group-hover/row:text-slate-500/10 dark:group-data-[state=selected]/row:text-slate-500/10">
      <ToolTip content={t("ConfigList.Cell.Waiting")}>
        <IconHourglassEmpty className="h-7 w-7" />
      </ToolTip>
    </div>
  )
}

export default ValueIcon
