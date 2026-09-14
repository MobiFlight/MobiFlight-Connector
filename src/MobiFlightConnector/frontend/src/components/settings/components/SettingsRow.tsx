import * as React from "react"
import { Label } from "@/components/ui/label"
import { cn } from "@/lib/utils"

export interface SettingsRowProps {
  label: React.ReactNode
  description?: React.ReactNode
  htmlFor?: string
  disabled?: boolean
  className?: string
  children: React.ReactNode
}

export default function SettingsRow({
  label,
  description,
  htmlFor,
  disabled = false,
  className,
  children,
}: SettingsRowProps) {
  return (
    <div
      className={cn(
        "-mx-2 flex items-center justify-between gap-4 rounded-md p-2 transition-colors hover:bg-muted/70",
        disabled && "opacity-60",
        className,
      )}
    >
      <div className="flex flex-col gap-0.5">
        <Label
          htmlFor={htmlFor}
          className={cn(
            "text-sm font-normal",
            htmlFor && "cursor-pointer",
            disabled && "text-muted-foreground",
          )}
        >
          {label}
        </Label>
        {description && (
          <p className="text-xs text-muted-foreground leading-relaxed">
            {description}
          </p>
        )}
      </div>
      {children}
    </div>
  )
}
