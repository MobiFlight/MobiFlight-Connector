import { useEffect, useState } from "react"

export type ValidationResult<T> =
  | { ok: true; value: T; displayValue?: string }
  | { ok: false; error?: string; displayValue?: string }

export function useDraftCommitInput<T>({
  originalValue,
  format = (value: T) => String(value),
  validateOnCommit,
  onCommit,
}: {
  originalValue: T
  format?: (value: T) => string
  validateOnCommit: (draft: string) => ValidationResult<T>
  onCommit: (value: T) => void
}) {
  const formattedOriginal = format(originalValue)
  const [draftValue, setDraftValue] = useState(formattedOriginal)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    setDraftValue(formattedOriginal)
  }, [formattedOriginal])

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setDraftValue(e.target.value)
    setError(null)
  }

  const handleBlur = () => {
    const result = validateOnCommit(draftValue)

    if (result.ok) {
      setError(null)
      
      if (result.value === originalValue) return

      setTimeout(() => {
        onCommit(result.value)
      }, 0)
    } else {
      setError(result.error ?? "Invalid value")
      setDraftValue(format(originalValue))
    }
  }

  return {
    error,
    draftValue,
    inputProps: {
      value: draftValue,
      onChange: handleChange,
      onBlur: handleBlur,
    },
  }
}
