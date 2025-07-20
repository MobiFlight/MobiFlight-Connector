import { useEffect, useState } from "react"
import { IconCheck, IconDeviceFloppy } from "@tabler/icons-react"
import { Button } from "./button"
import { cn } from "@/lib/utils"

interface AnimatedSaveButtonProps {
  /**
   * Whether there are unsaved changes
   */
  hasChanges: boolean
  /**
   * Function to call when the save button is clicked
   */
  onSave: () => void
  /**
   * Duration in milliseconds to show the success state (default: 2000)
   */
  successDuration?: number
  /**
   * Additional CSS classes for the button
   */
  className?: string
  /**
   * Whether the button is disabled
   */
  disabled?: boolean
  /**
   * Icon to show for the inactive/active states (default: IconDeviceFloppy)
   */
  saveIcon?: React.ComponentType<{ className?: string }>
  /**
   * Icon to show for the success state (default: IconCheck)
   */
  successIcon?: React.ComponentType<{ className?: string }>
}

export const AnimatedSaveButton = ({
  hasChanges,
  onSave,
  successDuration = 2000,
  className,
  disabled,
  saveIcon: SaveIcon = IconDeviceFloppy,
  successIcon: SuccessIcon = IconCheck,
}: AnimatedSaveButtonProps) => {
  const [showSuccess, setShowSuccess] = useState(false)
  const [prevHasChanges, setPrevHasChanges] = useState(hasChanges)

  // Detect when hasChanges transitions from true to false (successful save)
  useEffect(() => {
    if (prevHasChanges && !hasChanges) {
      setShowSuccess(true)
      const timer = setTimeout(() => {
        setShowSuccess(false)
      }, successDuration)
      return () => clearTimeout(timer)
    }
    setPrevHasChanges(hasChanges)
  }, [hasChanges, prevHasChanges, successDuration])

  const isButtonDisabled = disabled || (!hasChanges && !showSuccess)

  return (
    <Button
      variant="ghost"
      className={cn("h-8 gap-1 px-1 py-1 [&_svg]:size-6", className)}
      disabled={isButtonDisabled}
      onClick={onSave}
    >
      <div className="relative">
        <SaveIcon
          className={`transition-opacity duration-300 ${
            showSuccess
              ? "opacity-0"
              : hasChanges
              ? "stroke-primary opacity-100"
              : "stroke-muted-foreground opacity-100"
          }`}
        />
        <SuccessIcon
          className={`absolute inset-0 stroke-green-500 transition-opacity duration-300 ${
            showSuccess ? "opacity-100" : "opacity-0"
          }`}
        />
      </div>
    </Button>
  )
}
