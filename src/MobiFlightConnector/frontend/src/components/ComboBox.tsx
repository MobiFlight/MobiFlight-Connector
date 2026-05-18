import { Button } from "@/components/ui/button"
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover"
import { cn } from "@/lib/utils"
import { IconChevronDown, IconCheck } from "@tabler/icons-react"
import {
  Command,
  CommandInput,
  CommandList,
  CommandEmpty,
  CommandGroup,
  CommandItem,
} from "@/components/ui/command"
import { useState } from "react"

export type ComboBoxProps<T> = {
  items: T[]
  selected?: T
  getValue: (item: T) => string
  getLabel: (item: T) => string
  isSelected: (item: T, selected?: T) => boolean
  setSelected: (item?: T) => void
  placeholder?: string
  searchPlaceholder?: string
  emptyText?: string
  disabled?: boolean
}

const ComboBox = <T,>({
  items,
  selected,
  getValue,
  getLabel,
  isSelected,
  setSelected,
  placeholder = "Select...",
  searchPlaceholder = "Search...",
  emptyText = "No item found.",
  disabled = false,
}: ComboBoxProps<T>) => {
  const [open, setOpen] = useState(false)
  const selectedValue = selected ? getValue(selected) : ""

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          role="combobox"
          aria-expanded={open}
          className="w-50 justify-between"
          disabled={disabled}
        >
          {selected
            ? getLabel(items.find((item) => isSelected(item, selected)) ?? selected)
            : placeholder}
          <IconChevronDown className="opacity-50" />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-50 p-0">
        <Command>
          <CommandInput placeholder={searchPlaceholder} className="h-9" />
          <CommandList>
            <CommandEmpty>{emptyText}</CommandEmpty>
            <CommandGroup>
              {items.map((item) => (
                <CommandItem
                  key={getValue(item)}
                  value={getValue(item)}
                  onSelect={(currentValue) => {
                    if (currentValue === selectedValue) {
                      setSelected(undefined)
                    } else {
                      const nextSelected = items.find(
                        (nextItem) => getValue(nextItem) === currentValue,
                      )
                      setSelected(nextSelected)
                    }
                    setOpen(false)
                  }}
                >
                  {getLabel(item)}
                  <IconCheck
                    className={cn(
                      "ml-auto",
                      isSelected(item, selected) ? "opacity-100" : "opacity-0",
                    )}
                  />
                </CommandItem>
              ))}
            </CommandGroup>
          </CommandList>
        </Command>
      </PopoverContent>
    </Popover>
  )
}
export default ComboBox
