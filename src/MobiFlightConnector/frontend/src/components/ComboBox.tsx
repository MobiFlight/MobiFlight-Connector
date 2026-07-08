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
import { HTMLAttributes, useState } from "react"
import { useTranslation } from "react-i18next"

export type ComboBoxProps<T> = HTMLAttributes<HTMLElement> & {
  items: T[]
  selected?: T
  getValue: (item: T) => string
  getLabel: (item: T) => string
  isSelected: (item: T, selected?: T) => boolean
  setSelected: (item?: T) => void
  placeholder?: string | null
  searchPlaceholder?: string | null
  noOptionsPlaceholder?: string | null
  disabled?: boolean
  widthClass?: string
  variant?: "default" | "nofilter"
}

const ComboBox = <T,>({
  items,
  selected,
  getValue,
  getLabel,
  isSelected,
  setSelected,
  placeholder,
  searchPlaceholder,
  noOptionsPlaceholder,
  disabled = false,
  widthClass = "w-50",
  variant = "default",
  ...props
}: ComboBoxProps<T>) => {
  const { t } = useTranslation()
  const [open, setOpen] = useState(false)
  
  placeholder ??= t("General.ComboBox.Placeholder")
  searchPlaceholder ??= t("General.ComboBox.SearchPlaceholder")
  noOptionsPlaceholder ??= t("General.ComboBox.NoOptions")
  
  const selectedValue = selected ? getValue(selected) : ""

  const selectedInItems = items.find((item) => isSelected(item, selected))
  const label = selectedInItems
    ? getLabel(selectedInItems)
    : placeholder

  return (
    <Popover open={open} onOpenChange={setOpen} modal={true}>
      <PopoverTrigger asChild>
        <Button
          size="sm"
          variant="outline"
          role="combobox"
          aria-expanded={open}
          className={cn("justify-between h-8", widthClass)}
          disabled={disabled}
          {...props}
        >
          <span className={cn(widthClass, "truncate text-sm text-left")} title={label}>
            {label}
          </span>
          <IconChevronDown className="opacity-50" />
        </Button>
      </PopoverTrigger>
      <PopoverContent className={cn(widthClass, "p-0")}>
        <Command>
          {variant === "default" && (
            <CommandInput placeholder={searchPlaceholder} className="h-9" />
          )}
          <CommandList>
            <CommandEmpty>{noOptionsPlaceholder}</CommandEmpty>
            <CommandGroup>
              {items.map((item) => {
                const itemValue = getValue(item)
                return (
                  <CommandItem
                    key={itemValue}
                    value={`${getLabel(item)}`}
                    onSelect={() => {
                      if (itemValue === selectedValue) {
                        setSelected(undefined)
                      } else {
                        setSelected(item)
                      }
                      setOpen(false)
                    }}
                  >
                    <span className="truncate text-sm">{getLabel(item)}</span>
                    <IconCheck
                      className={cn(
                        "ml-auto",
                        isSelected(item, selected)
                          ? "opacity-100"
                          : "opacity-0",
                      )}
                    />
                  </CommandItem>
                )
              })}
            </CommandGroup>
          </CommandList>
        </Command>
      </PopoverContent>
    </Popover>
  )
}
export default ComboBox
