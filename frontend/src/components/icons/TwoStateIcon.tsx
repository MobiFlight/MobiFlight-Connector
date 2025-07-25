import { cn } from "@/lib/utils"

interface TwoStateIconProps extends Omit<React.HTMLAttributes<HTMLDivElement>, "onClick"> {
  /**
   * Current state - true shows secondaryIcon, false shows primaryIcon
   */
  state: boolean
  /**
   * Primary icon component (shown when state is false)
   */
  primaryIcon: React.ComponentType<{ className?: string; size?: string | number }>
  /**
   * Secondary icon component (shown when state is true)
   */
  secondaryIcon: React.ComponentType<{ className?: string; size?: string | number }>
  /**
   * Function called when primary icon is clicked
   */
  onPrimaryClick?: () => void
  /**
   * Function called when secondary icon is clicked
   */
  onSecondaryClick?: () => void
  /**
   * Duration of the transition animation in milliseconds (default: 300)
   */
  duration?: number
  /**
   * Custom className for primary icon
   */
  primaryClassName?: string
  /**
   * Custom className for secondary icon
   */
  secondaryClassName?: string
  /**
   * Size of the icons (default: 24)
   */
  size?: string | number
}

export const TwoStateIcon = ({
  state,
  primaryIcon: PrimaryIcon,
  secondaryIcon: SecondaryIcon,
  onPrimaryClick,
  onSecondaryClick,
  duration = 300,
  primaryClassName = "",
  secondaryClassName = "",
  size = 24,
  className,
  ...props
}: TwoStateIconProps) => {
  const handleClick = () => {
    if (state && onSecondaryClick) {
      onSecondaryClick()
    } else if (!state && onPrimaryClick) {
      onPrimaryClick()
    }
  }

  return (
    <div
      className={cn("relative inline-flex cursor-pointer", className)}
      onClick={handleClick}
      {...props}
    >
      <SecondaryIcon
        size={size}
        className={cn(
          `absolute inset-0 transition-all duration-${duration} ${
            state
              ? "rotate-0 scale-100 opacity-100"
              : "rotate-180 scale-0 opacity-0"
          }`,
          secondaryClassName
        )}
      />
      <PrimaryIcon
        size={size}
        className={cn(
          `transition-all duration-${duration} ${
            state
              ? "-rotate-90 scale-0 opacity-0"
              : "rotate-0 scale-100 opacity-100"
          }`,
          primaryClassName
        )}
      />
    </div>
  )
}

export default TwoStateIcon
