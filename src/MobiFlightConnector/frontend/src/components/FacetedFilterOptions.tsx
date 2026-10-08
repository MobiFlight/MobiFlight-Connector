import * as React from "react"
import { IconCheck, IconCirclePlus } from "@tabler/icons-react"

import { cn } from "@/lib/utils"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
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
import { Separator } from "@/components/ui/separator"
import { useTranslation } from "react-i18next"

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

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          disabled={disabled}
          variant="outline"
          size="sm"
          className="h-8 border-dashed"
        >
          <IconCirclePlus className="h-4 w-4" />
          {title}
          {selectedValues.size > 0 && (
            <>
              <Separator orientation="vertical" className="h-4" />
              <Badge
                variant="secondary"
                className="rounded-sm px-1 font-normal 2xl:hidden"
              >
                {selectedValues.size}
              </Badge>
              <div className="hidden space-x-1 2xl:flex">
                {selectedValues.size > 2 ? (
                  <Badge
                    variant="secondary"
                    className="rounded-sm px-1 font-normal"
                  >
                    {t("General.FacetedFilter.Selected", {
                      items: selectedValues.size,
                    })}
                  </Badge>
                ) : (
                  options
                    .filter((option) => selectedValues.has(option.value))
                    .map((option) => (
                      <Badge
                        variant="secondary"
                        key={option.value}
                        className="rounded-sm px-1 font-normal"
                      >
                        {option.label}
                      </Badge>
                    ))
                )}
              </div>
            </>
          )}
        </Button>
      </PopoverTrigger>
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
