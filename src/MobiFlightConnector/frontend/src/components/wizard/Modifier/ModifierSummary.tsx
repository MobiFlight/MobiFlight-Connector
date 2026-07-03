import { Badge } from "@/components/ui/badge"
import { Modifier } from "@/types/modifier"
import { useTranslation } from "react-i18next"

type ModifierSummaryProps = {
  rawValue: string | null | undefined
  finalValue: string | null | undefined
  modifiers: Modifier[]
  maxDisplayCount: number
}

const ModifierSummary = ({
  rawValue,
  finalValue,
  modifiers,
  maxDisplayCount,
}: ModifierSummaryProps) => {
  const { t } = useTranslation()
  const rawValueClean = rawValue?.replace("CHANGE =>", "") ?? ""
  return (
    <div className="flex flex-row items-center justify-between gap-2">
      <Badge variant="secondary">{rawValueClean}</Badge>
      <div className="mt-1 h-1 grow border-t border-dashed border-gray-800"></div>
      <div className="flex flex-row flex-wrap gap-2">
        {modifiers.slice(0, maxDisplayCount).map((modifier, index) => (
          <div className="flex flex-row items-center gap-2" key={index}>
            <Badge>{t(`Dialog.Modifiers.Type.${modifier.Type}.Label`)}</Badge>
            <div className="mt-1 h-1 w-16 border-t border-dashed border-gray-800"></div>
          </div>
        ))}
        {modifiers.length > maxDisplayCount && (
          <Badge variant="secondary" className="border-primary">
            {t("Dialog.Modifiers.Summary.More", {
              count: modifiers.length - maxDisplayCount,
            })}
          </Badge>
        )}
        {modifiers.length === 0 && (
          <Badge variant="secondary">
            {t("Dialog.Modifiers.Summary.None")}
          </Badge>
        )}
      </div>
      <div className="mt-1 h-1 grow border-t border-dashed border-gray-800"></div>
      <Badge variant="secondary">{finalValue}</Badge>
    </div>
  )
}
export default ModifierSummary
