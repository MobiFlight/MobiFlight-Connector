import * as React from "react"

import { cn } from "@/lib/utils"
import { isNativeTextContextMenuTarget } from "@/lib/contextMenuPolicy"

const Input = React.forwardRef<HTMLInputElement, React.ComponentProps<"input">>(
  ({ className, type, onContextMenu, ...props }, ref) => {
    const handleContextMenu: React.MouseEventHandler<HTMLInputElement> = (
      event,
    ) => {
      onContextMenu?.(event)

      if (isNativeTextContextMenuTarget(event.currentTarget)) {
        event.stopPropagation()
      }
    }

    return (
      <input
        type={type}
        autoComplete="off"
        className={cn(
          "border-input bg-background ring-offset-background file:text-foreground placeholder:text-muted-foreground focus-visible:ring-ring flex h-8 w-full rounded-md border px-2 py-1 text-base file:border-0 file:bg-transparent file:text-sm file:font-medium focus-visible:ring-2 focus-visible:ring-offset-2 focus-visible:outline-hidden disabled:cursor-not-allowed disabled:opacity-50 md:text-sm",
          className,
        )}
        ref={ref}
        {...props}
        onContextMenu={handleContextMenu}
      />
    )
  },
)
Input.displayName = "Input"

export { Input }
