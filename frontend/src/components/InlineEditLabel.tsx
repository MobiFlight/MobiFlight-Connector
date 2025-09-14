import { forwardRef, useEffect, useImperativeHandle, useRef, useState } from "react"
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

export const InlineEditLabel = forwardRef<InlineEditLabelRef, InlineEditLabelProps>(({
  value,
  onSave,
  inputClassName = "",
  labelClassName: spanClassName = "",
  disabled = false,
}, ref) => {
  const [tempValue, setTempValue] = useState(value)
  const [isEditing, setIsEditing] = useState(false)
  const inputRef = useRef<HTMLInputElement>(null)

  // Expose the startEditing method through the ref
  useImperativeHandle(ref, () => ({
    startEditing: () => {
      if (!disabled) {
        setIsEditing(true)
      }
    }
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

  const handleClick = (e: React.MouseEvent) => {
    if (!disabled) {
      e.stopPropagation()
      setIsEditing(true)
    }
  }

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
      handleSave()
    }
  }

  return isEditing ? (
    <Input
      ref={inputRef}
      className={cn(
        `rounded-none text-md md:text-md flex h-6 border-none p-1 focus-visible:ring-offset-0 focus-visible:ring-transparent focus-visible:ring-0 bg-transparent`,
        `field-sizing-content`,
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
      onClick={(e) => handleClick(e)}
      onDoubleClick={(e) => handleDoubleClick(e)}
      className={cn(
        `cursor-pointer px-2 font-semibold`,
        spanClassName,
      )}
    >
      {value}
    </span>
  )
})

InlineEditLabel.displayName = "InlineEditLabel"
