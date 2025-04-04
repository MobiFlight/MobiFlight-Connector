import React from "react"
import { useTranslation } from "react-i18next"

const ConfigItemTableActiveHeader = React.memo(() => {
  const { t } = useTranslation()
  return <div className="text-center">{t("ConfigList.Header.Active")}</div>
})

export default ConfigItemTableActiveHeader
