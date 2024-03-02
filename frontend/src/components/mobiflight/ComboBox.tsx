import * as React from "react";
import { IconCheck, IconChevronDown } from "@tabler/icons-react";

import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
} from "@/components/ui/command";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { ScrollArea } from "../ui/scroll-area";

interface ComboBoxProps {
  className?: string;
  options: {
    value: string;
    label: string;
  }[];
  value: string;
  onSelect: (value: string) => void;
  showFilter?: boolean;
}

export function ComboBox(props: ComboBoxProps) {
  const className = props.className;
  const options = props.options;
  const value = props.value;
  const onSelect = props.onSelect;
  const [open, setOpen] = React.useState(false);
  const showFilter = props.showFilter ?? false;

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          role="combobox"
          aria-expanded={open}
          className={cn("w-[200px] justify-between", className)}
        >
          {value
            ? options.find((option) => option.value === value)?.label
            : "Select level..."}
          <IconChevronDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
        </Button>
      </PopoverTrigger>
      <PopoverContent className={cn("w-[200px] p-0", className)}>
        <Command>
          {showFilter && <CommandInput placeholder="Select level..." />}
          <CommandEmpty>No framework found.</CommandEmpty>
          <ScrollArea className="max-h-[300px]">
          <CommandGroup>
            {options.map((option) => (
              <CommandItem
                role="option"
                key={option.value}
                value={option.value}
                onSelect={() => {
                  onSelect(option.value);
                  setOpen(false);
                }}
              >
                <IconCheck
                  className={cn(
                    "mr-2 h-4 w-4",
                    value === option.value ? "opacity-100" : "opacity-0"
                  )}
                />
                {option.label}
              </CommandItem>
            ))}
          </CommandGroup>
          </ScrollArea>
        </Command>
      </PopoverContent>
    </Popover>
  );
}
