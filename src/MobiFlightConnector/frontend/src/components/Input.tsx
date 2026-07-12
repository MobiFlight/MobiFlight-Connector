import {
  useDraftCommitInput,
  ValidationResult,
} from "@/lib/hooks/useDraftCommitInput"
import { Input as InputComponent } from "@/components/ui/input"
import { cn } from "@/lib/utils"

type InputProps<T> = Omit<React.ComponentProps<"input">, "onChange" | "onBlur" | "value"> & {
  value: T
  format?: (value: T) => string
  validateOnCommit?: (draft: string) => ValidationResult<T>
  onChange: (value: T) => void
  onBlur?: (event: React.FocusEvent<HTMLInputElement>) => void
}

const Input = <T,>({
  className,
  value,
  format,
  validateOnCommit,
  onChange,
  onBlur,
  ...props
}: InputProps<T>) => {
  const draft = useDraftCommitInput({
    originalValue: value,
    format: format,
    validateOnCommit: validateOnCommit,
    onCommit: onChange,
  })
  return (
    <InputComponent
      value={draft.draftValue}
      onChange={draft.inputProps.onChange}
      onBlur={(event) => {
        draft.inputProps.onBlur()
        if (onBlur) {
          onBlur(event)
        }
      }}
      className={cn(className)}
      {...props}
    />
  )
}

export default Input
