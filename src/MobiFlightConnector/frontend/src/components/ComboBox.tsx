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
import { useEffect, useState } from "react"

export type ComboBoxProps<T> = {
  items: T[]
  selected: T
  getValue: (item: T) => string
  getLabel: (item: T) => string
  isSelected: (item: T, selected: T) => boolean
  setSelected: (item: T) => void
}

const ComboBox = <T,>({
  items,
  selected,
  getValue,
  getLabel,
  isSelected,
  setSelected,
}: ComboBoxProps<T>) => {
  const [open, setOpen] = useState(false)
  const [value, setValue] = useState(getValue(selected))

  useEffect(() => {
    setSelected(items.find((item) => getValue(item) === value)!)
  }, [value, items, setSelected, getValue])

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          role="combobox"
          aria-expanded={open}
          className="w-50 justify-between"
        >
          {selected
            ? getLabel(items.find((item) => isSelected(item, selected))!)
            : "Select controller..."}
          <IconChevronDown className="opacity-50" />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-50 p-0">
        <Command>
          <CommandInput placeholder="Search controller..." className="h-9" />
          <CommandList>
            <CommandEmpty>No controller found.</CommandEmpty>
            <CommandGroup>
              {items.map((item) => (
                <CommandItem
                  key={getValue(item)}
                  value={getValue(item)}
                  onSelect={(currentValue) => {
                    setValue(currentValue === value ? "" : currentValue)
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
