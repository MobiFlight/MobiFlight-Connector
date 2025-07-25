import { useState, useEffect } from "react"
import { IconCheck, IconDeviceFloppy } from "@tabler/icons-react"
import { TwoStateIcon } from "@/components/icons/TwoStateIcon"
import { Button } from "@/components/ui/button"
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip"
import { cn } from "@/lib/utils"

interface AnimatedSaveButtonProps {
  /**
   * Whether there are unsaved changes to save
   */
  hasChanged: boolean
  /**
   * Function to call when save is triggered
   */
  onSave: () => void
  /**
   * Tooltip text when save is available
   */
  saveTooltip?: string
  /**
   * Tooltip text when save was successful
   */
  successTooltip?: string
  /**
   * Tooltip text when no changes to save
   */
  noChangesTooltip?: string
  /**
   * Duration to show success state in milliseconds (default: 2000)
   */
  successDuration?: number
  /**
   * Size of the icons (default: 24)
   */
  size?: string | number
  /**
   * Custom className for the button container
   */
  className?: string
}

type SaveState = "inactive" | "active" | "success"

export const AnimatedSaveButton = ({
  hasChanged,
  onSave,
  saveTooltip,
  successTooltip,
  noChangesTooltip,
  successDuration = 2000,
  size = 24,
  className,
}: AnimatedSaveButtonProps) => {
  const [saveState, setSaveState] = useState<SaveState>("inactive")

  // Update state based on hasChanged prop
  useEffect(() => {
    if (hasChanged && saveState !== "success") {
      setSaveState("active")
    } else if (!hasChanged && saveState !== "success") {
      setSaveState("inactive")
    }
  }, [hasChanged, saveState])

  const handleSave = () => {
    if (saveState === "active") {
      setSaveState("success")
      onSave()

      // Reset to inactive after success duration
      setTimeout(() => {
        setSaveState("inactive")
      }, successDuration)
    }
  }

  const isEnabled = saveState !== "inactive"
  const showSuccess = saveState === "success"

  const buttonContent = (
    <Button
      variant="ghost"
      className={cn("h-8 gap-1 p-1 [&_svg]:size-6", className)}
      onClick={handleSave}
      disabled={!isEnabled}
    >
      <TwoStateIcon
        state={showSuccess}
        primaryIcon={IconDeviceFloppy}
        secondaryIcon={IconCheck}
        size={size}
        primaryClassName={hasChanged ? "stroke-primary" : ""}
        secondaryClassName={showSuccess ? "stroke-green-600" : ""}
      />
    </Button>
  )

  return (
    <TooltipProvider>
      <Tooltip>
        <TooltipTrigger asChild>
          <div className="inline-flex">{buttonContent}</div>
        </TooltipTrigger>
        <TooltipContent>
          <p>{isEnabled ? showSuccess ? successTooltip : saveTooltip : noChangesTooltip}</p>
        </TooltipContent>
      </Tooltip>
    </TooltipProvider>
  )
}

export default AnimatedSaveButton
