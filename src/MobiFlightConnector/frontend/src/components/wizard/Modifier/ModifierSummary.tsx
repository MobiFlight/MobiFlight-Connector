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
  const showMoreCount = modifiers.length > maxDisplayCount

  return (
    <div className="flex flex-row items-end justify-between gap-2">
      <div className="flex flex-col items-center gap-2">
        <div className="text-sm font-medium">{t("ConfigList.Header.RawValue")}</div>
        <Badge variant="secondary">{rawValueClean}</Badge>
      </div>

      <div className="mt-1 h-3 grow border-t border-dashed border-gray-800"></div>

      <div className="flex flex-row flex-wrap gap-2">
        {modifiers.slice(0, maxDisplayCount).map((modifier, index) => {
          const isLast =
            index === modifiers.length - 1 || index === maxDisplayCount - 1
          return (
            <div className="flex flex-row items-center gap-2" key={index}>
              <Badge variant="secondary">{t(`Dialog.Modifiers.Type.${modifier.Type}.Label`)}</Badge>
              {!isLast && (
                <div className="mt-1 h-1 w-16 border-t border-dashed border-gray-800"></div>
              )}
            </div>
          )
        })}
      </div>
      {showMoreCount && (
        <div className="mt-1 h-3 w-16 border-t border-dashed border-gray-800"></div>
      )}
      {showMoreCount && (
        <Badge variant="secondary" className="border-primary">
          {t("Dialog.Modifiers.Summary.More", {
            count: modifiers.length - maxDisplayCount,
          })}
        </Badge>
      )}

      <div className="mt-1 h-3 grow border-t border-dashed border-gray-800"></div>

      <div className="flex flex-col items-center gap-2">
        <div className="text-sm font-medium">{t("ConfigList.Header.FinalValue")}</div>
        <Badge variant="secondary">{finalValue}</Badge>
      </div>
    </div>
  )
}
export default ModifierSummary
