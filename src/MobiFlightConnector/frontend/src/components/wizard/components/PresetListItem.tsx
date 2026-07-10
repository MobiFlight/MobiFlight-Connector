import { cn } from "@/lib/utils"
import { Preset, XplanePreset } from "@/types/preset"
import { forwardRef } from "react"

type PresetListItemProps = {
  key: string
  preset: Preset | XplanePreset
  isSelected: boolean
  setSelectedPreset: (preset: Preset | XplanePreset | null) => void
}

export const PresetListItem = forwardRef<
  HTMLDivElement,
  PresetListItemProps
>(
  (
    {
      key,
      preset,
      isSelected,
      setSelectedPreset,
    }: PresetListItemProps,
    ref,
  ) => {
    return (
      <div
        ref={ref}
        role="listitem"
        key={key}
        className={cn(
          "bg-background hover:bg-accent/50 border-background flex cursor-pointer flex-row justify-between gap-2 rounded-md border-2 px-2 pt-0.5 pb-1.5",
          isSelected && "bg-primary/20 border-primary border-2",
        )}
        onClick={() => setSelectedPreset(preset)}
      >
        <div className="flex flex-col">
          <div className="text-sm font-semibold">{preset.label}</div>
          <div className="text-muted-foreground text-xs/3">
            {preset.description ?? "-"}
          </div>
        </div>
        <div className="flex flex-col justify-items-end px-2">
          <div className="text-right text-sm/5 font-semibold">
            {preset.system}
          </div>
          <div className="text-muted-foreground text-right text-xs/3">
            {preset.aircraft}
          </div>
        </div>
      </div>
    )
  },
)
