import { useEffect, useState } from "react"
import { IconCheck, IconDeviceFloppy } from "@tabler/icons-react"
import { Button } from "./button"
import { Tooltip, TooltipContent, TooltipTrigger } from "./tooltip"
import { cn } from "@/lib/utils"

interface AnimatedSaveButtonProps
  extends Omit<
    React.ButtonHTMLAttributes<HTMLButtonElement>,
    "onClick" | "disabled"
  > {
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
  /**
   * Tooltip content to show on hover
   */
  tooltip?: string | React.ReactNode
}

export const AnimatedSaveButton = ({
  hasChanges,
  onSave,
  successDuration = 2000,
  className,
  disabled,
  saveIcon: SaveIcon = IconDeviceFloppy,
  successIcon: SuccessIcon = IconCheck,
  tooltip,
  ...props
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

  const button = (
    <Button
      variant="ghost"
      className={cn("h-8 gap-1 px-1 py-1 [&_svg]:size-6", className)}
      disabled={isButtonDisabled}
      onClick={onSave}
      {...props}
    >
      <div className="relative">
        <SuccessIcon
          className={`absolute inset-0 stroke-green-500 transition-all duration-300 ${
            showSuccess
              ? "rotate-0 scale-100 opacity-100"
              : "rotate-180 scale-0 opacity-0"
          }`}
        />
        <SaveIcon
          className={`transition-all duration-300 ${
            showSuccess
              ? "-rotate-90 scale-0 opacity-0"
              : hasChanges
                ? "scale-100 stroke-primary opacity-100"
                : "scale-100 stroke-muted-foreground opacity-100"
          }`}
        />
      </div>
    </Button>
  )

  // Always wrap in a container for consistent behavior
  const container = <div className="inline-flex">{button}</div>

  if (!tooltip) {
    return container
  }

  return (
    <Tooltip>
      <TooltipTrigger asChild>{container}</TooltipTrigger>
      <TooltipContent align="start" side="bottom" className="rounded-none">
        {typeof tooltip === "string" ? (
          <div className="text-sm">{tooltip}</div>
        ) : (
          tooltip
        )}
      </TooltipContent>
    </Tooltip>
  )
}

export default AnimatedSaveButton
