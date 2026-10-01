import * as React from "react"
import { IconCheck, IconCirclePlus } from "@tabler/icons-react"

import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import ToolTip from "@/components/ToolTip"
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandSeparator,
} from "@/components/ui/command"
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover"
import { useTranslation } from "react-i18next"

/** How many picked values the trigger names before it falls back to "+N". */
const SHOWN_VALUES = 2

export type FacetedFilterOption = {
  /** Shown in the menu and on the trigger; a node allows coloured text. */
  label: React.ReactNode
  value: string
  icon?: React.ComponentType<{ className?: string }>
}

export type FacetedFilterOptionsProps = {
  options: FacetedFilterOption[]
  /** The picked values; an empty array means nothing is filtered out. */
  values: string[]
  onValuesChange: (values: string[]) => void
  /** How many items each option matches, shown on the right of each row. */
  facets?: Map<string, number>
  title?: string
  disabled?: boolean
  /** Keep the clear row in place, disabled, while nothing is selected. */
  keepClearVisible?: boolean
  /** The trigger has a fixed width so picking values never shifts the layout; override it here. */
  className?: string
}

/**
 * A multi-select filter menu. It knows nothing about where its options come
 * from, so a table column and a plain list work the same way. For a TanStack
 * Table column, use DataTableFacetedFilter, which wraps this component.
 */
const FacetedFilterOptions = ({
  options,
  values,
  onValuesChange,
  facets,
  title,
  disabled = false,
  keepClearVisible = false,
  className,
}: FacetedFilterOptionsProps) => {
  const { t } = useTranslation()

  const selectedValues = new Set(values)

  const toggleValue = (value: string) => {
    if (selectedValues.has(value)) {
      selectedValues.delete(value)
    } else {
      selectedValues.add(value)
    }
    onValuesChange(Array.from(selectedValues))
  }

  const selectedOptions = options.filter((option) =>
    selectedValues.has(option.value),
  )
  // the first few picked values stand in for the title, the rest become "+N";
  // the width is fixed so the neighbours never move
  const shownOptions = selectedOptions.slice(0, SHOWN_VALUES)
  const hiddenCount = selectedOptions.length - shownOptions.length

  const trigger = (
    <PopoverTrigger asChild>
      <Button
        disabled={disabled}
        variant="outline"
        size="sm"
        className={cn("h-8 w-40 justify-start border-dashed", className)}
      >
        <IconCirclePlus className="h-4 w-4 shrink-0" />
        {/* keeps the accessible name stable while the values replace the title */}
        {selectedOptions.length > 0 && (
          <span className="sr-only">{title}: </span>
        )}
        <span className="min-w-0 flex-1 truncate text-left">
          {selectedOptions.length === 0
            ? title
            : shownOptions.map((option, index) => (
                <React.Fragment key={option.value}>
                  {index > 0 && ", "}
                  {option.label}
                </React.Fragment>
              ))}
        </span>
        {hiddenCount > 0 && (
          <span className="text-muted-foreground shrink-0 text-xs">
            +{hiddenCount}
          </span>
        )}
      </Button>
    </PopoverTrigger>
  )

  return (
    <Popover>
      {selectedOptions.length > 0 ? (
        <ToolTip
          content={
            <div className="flex flex-col gap-0.5 text-sm">
              <div className="text-muted-foreground">{title}</div>
              {selectedOptions.map((option) => (
                <div key={option.value}>{option.label}</div>
              ))}
            </div>
          }
        >
          {trigger}
        </ToolTip>
      ) : (
        trigger
      )}
      <PopoverContent className="w-[240px] p-0" align="start">
        <Command>
          <CommandInput placeholder={title} />
          <CommandList>
            <CommandEmpty>
              {t("General.FacetedFilter.NoResultsFound")}
            </CommandEmpty>
            <CommandGroup>
              {options.map((option) => {
                const isSelected = selectedValues.has(option.value)
                return (
                  <CommandItem
                    className="gap-1 px-2"
                    key={option.value}
                    onSelect={() => toggleValue(option.value)}
                  >
                    <div
                      className={cn(
                        "border-primary mr-2 flex h-4 w-4 items-center justify-center rounded-sm border",
                        isSelected
                          ? "bg-primary text-primary-foreground"
                          : "opacity-50 [&_svg]:invisible",
                      )}
                    >
                      <IconCheck className={cn("h-4 w-4")} />
                    </div>
                    {option.icon && (
                      <div>
                        <option.icon className="text-muted-foreground mr-0 h-4 w-4" />
                      </div>
                    )}
                    <span className="truncate">{option.label}</span>
                    {facets?.get(option.value) && (
                      <span className="ml-auto flex h-4 w-4 items-center justify-center font-mono text-xs">
                        {facets.get(option.value)}
                      </span>
                    )}
                  </CommandItem>
                )
              })}
            </CommandGroup>
            {(selectedValues.size > 0 || keepClearVisible) && (
              <>
                <CommandSeparator />
                <CommandGroup>
                  <CommandItem
                    disabled={selectedValues.size === 0}
                    onSelect={() => onValuesChange([])}
                    className="justify-center text-center"
                  >
                    {t("General.FacetedFilter.Clear")}
                  </CommandItem>
                </CommandGroup>
              </>
            )}
          </CommandList>
        </Command>
      </PopoverContent>
    </Popover>
  )
}

export default FacetedFilterOptions
