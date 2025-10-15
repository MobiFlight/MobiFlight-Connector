import {
  forwardRef,
  useEffect,
  useImperativeHandle,
  useRef,
  useState,
} from "react"
import { Input } from "./ui/input"
import { cn } from "@/lib/utils"

interface InlineEditLabelProps {
  value: string
  onSave: (value: string) => void
  inputClassName?: string
  labelClassName?: string
  disabled?: boolean
}

export interface InlineEditLabelRef {
  startEditing: () => void
}

export const InlineEditLabel = forwardRef<
  InlineEditLabelRef,
  InlineEditLabelProps
>(
  (
    {
      value,
      onSave,
      inputClassName = "",
      labelClassName: spanClassName = "",
      disabled = false,
    },
    ref,
  ) => {
    const [tempValue, setTempValue] = useState(value)
    const [isEditing, setIsEditing] = useState(false)
    const inputRef = useRef<HTMLInputElement>(null)

    // Expose the startEditing method through the ref
    useImperativeHandle(ref, () => ({
      startEditing: () => {
        if (!disabled) {
          setIsEditing(true)
        }
      },
    }))

    // Update temp value when prop value changes
    useEffect(() => {
      setTempValue(value)
    }, [value])

    // Focus and select text when entering edit mode
    useEffect(() => {
      if (isEditing && inputRef.current) {
        setTimeout(() => {
          inputRef?.current?.select()
        }, 200)
      }
    }, [isEditing])
    const handleDoubleClick = (e: React.MouseEvent) => {
      if (!disabled) {
        e.stopPropagation()
        setIsEditing(true)
      }
    }

    const handleSave = () => {
      setIsEditing(false)
      if (tempValue !== value) {
        onSave(tempValue)
      }
    }

    const handleCancel = () => {
      setIsEditing(false)
      setTempValue(value)
    }

    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
      e.stopPropagation()

      if (e.key === "Escape") {
        handleCancel()
      }
      if (e.key === "Enter") {
        if (isEditing) {
          handleSave()
        } else {
          setIsEditing(true)
        }
      }
    }

    return isEditing ? (
      <Input
        ref={inputRef}
        className={cn(
          `border-primary bg-background hover:text-foreground rounded-md border-2 px-2 font-semibold focus-visible:ring-0 focus-visible:ring-transparent focus-visible:ring-offset-0 md:text-base`,
          `field-sizing-content h-8 w-auto`,
          inputClassName,
        )}
        type="text"
        value={tempValue}
        onChange={(e) => setTempValue(e.target.value)}
        onBlur={handleSave}
        onKeyDown={handleKeyDown}
        onDoubleClick={(e) => e.stopPropagation()}
      />
    ) : (
      <span
        role="button"
        onDoubleClick={(e) => handleDoubleClick(e)}
        onKeyDown={handleKeyDown}
        tabIndex={0}
        className={cn(
          `cursor-pointer px-2 font-semibold`,
          `ring-offset-background focus-visible:ring-ring focus-visible:ring-offset-muted focus-visible:ring-2 focus-visible:ring-offset-2 focus-visible:outline-hidden`,
          spanClassName,
        )}
      >
        {value}
      </span>
    )
  },
)

InlineEditLabel.displayName = "InlineEditLabel"
