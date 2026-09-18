import * as React from "react"
import { IconCheck, IconCirclePlus } from "@tabler/icons-react"
import { Column } from "@tanstack/react-table"

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

interface DataTableFacetedFilterProps<TData, TValue> {
  disabled?: boolean
  /** Table column to filter. Leave out when `values` drives the filter. */
  column?: Column<TData, TValue>
  title?: string
  options: {
    /** Shown in the menu and on the trigger; a node allows coloured text. */
    label: React.ReactNode
    value: string
    icon?: React.ComponentType<{ className?: string }>
  }[]
  /** Controlled selection, for callers that do not filter a table column. */
  values?: string[]
  onValuesChange?: (values: string[]) => void
  /** How many items each option matches, shown on the right of each row. */
  facets?: Map<string, number>
  /** Keep the clear row in place, disabled, while nothing is selected. */
  keepClearVisible?: boolean
}

export function DataTableFacetedFilter<TData, TValue>({
  disabled = false,
  column,
  title,
  options,
  values,
  onValuesChange,
  facets: facetsProp,
  keepClearVisible = false,
}: DataTableFacetedFilterProps<TData, TValue>) {
  // TODO: Fix this to work with React Compiler, changes done to table columns does not trigger a re-render
  "use no memo"

  const facets = facetsProp ?? column?.getFacetedUniqueValues()
  const selectedValues = new Set(
    values ?? (column?.getFilterValue() as string[]),
  )

  const setSelectedValues = (next: string[]) => {
    if (onValuesChange) {
      onValuesChange(next)
      return
    }
    column?.setFilterValue(next.length ? next : undefined)
  }

  const { t } = useTranslation()

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button disabled={disabled} variant="outline" size="sm" className="h-8 border-dashed">
          <IconCirclePlus className="h-4 w-4" />
          {title}
          {selectedValues?.size > 0 && (
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
                    {t("ConfigList.Toolbar.Filter.Selected", {
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
              {t("ConfigList.Toolbar.Filter.NoResultsFound")}
            </CommandEmpty>
            <CommandGroup>
              {options.map((option) => {
                const isSelected = selectedValues.has(option.value)
                return (
                  <CommandItem
                    className="gap-1 px-2"
                    key={option.value}
                    onSelect={() => {
                      if (isSelected) {
                        selectedValues.delete(option.value)
                      } else {
                        selectedValues.add(option.value)
                      }
                      setSelectedValues(Array.from(selectedValues))
                    }}
                  >
                    <div
                      className={cn(
                        "mr-2 flex h-4 w-4 items-center justify-center rounded-sm border border-primary",
                        isSelected
                          ? "bg-primary text-primary-foreground"
                          : "opacity-50 [&_svg]:invisible",
                      )}
                    >
                      <IconCheck className={cn("h-4 w-4")} />
                    </div>
                    {option.icon && (
                      <div>
                      <option.icon className="mr-0 h-4 w-4 text-muted-foreground" />
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
                    onSelect={() => setSelectedValues([])}
                    className="justify-center text-center"
                  >
                    {t("ConfigList.Toolbar.Filter.Clear")}
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
