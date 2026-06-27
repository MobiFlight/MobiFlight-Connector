import { IModifier } from "@/types/modifier"
import { useTranslation } from "react-i18next"

type ModifierSummaryProps = {
  modifiers: IModifier[]
  maxDisplayCount: number
}

const ModifierSummary = ({
  modifiers,
  maxDisplayCount,
}: ModifierSummaryProps) => {
  const { t } = useTranslation()
  return <div>{
    modifiers.slice(0, maxDisplayCount).map((modifier, index) => (
      <div key={index}>
        {t(`Dialog.Modifiers.Type.${modifier.Type}.Label`)}
      </div>
    ))
  }</div>
}
export default ModifierSummary
