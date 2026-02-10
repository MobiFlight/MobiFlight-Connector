import { IconMoodConfuzed } from "@tabler/icons-react"
import { FallbackProps } from "react-error-boundary"
import { useTranslation } from "react-i18next"

const ErrorFallback = ({ error }: FallbackProps) => {
  const { t } = useTranslation()
  const errorMessage =
    (error as Error | null | undefined)?.message ??
    error?.toString() ??
    t("General.Error.Unknown")

  return (
    <div
      className="flex h-full w-full flex-col items-center justify-center p-4 text-red-500"
      data-testid="error-fallback"
      role="alert"
      aria-live="assertive"
    >
      <div className="flex flex-row items-center gap-4">
        <IconMoodConfuzed />
        <p className="text-xl font-bold">{t("General.Error.FallbackTitle")}</p>
      </div>
      <pre className="wrap-break-word whitespace-pre-wrap">{errorMessage}</pre>
    </div>
  )
}
export default ErrorFallback
