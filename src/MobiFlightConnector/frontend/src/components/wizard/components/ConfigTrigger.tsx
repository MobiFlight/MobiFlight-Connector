import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command"
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover"
import { cn } from "@/lib/utils"
import { useControllerStore } from "@/stores/controllerStore"
import { useControllerDefinitionsStore } from "@/stores/definitionStore"
import { IConfigItem } from "@/types/config"
import { Controller } from "@/types/controller"
import { IconCheck, IconChevronDown } from "@tabler/icons-react"
import { useState } from "react"

export type ConfigTriggerProps = {
  configItem: IConfigItem
  setConfigItem: (item: IConfigItem) => void
}
const ConfigTrigger = ({ configItem, setConfigItem }: ConfigTriggerProps) => {
  const { controllers } = useControllerStore()
  const { BoardDefinitions, JoystickDefinitions, MidiControllerDefinitions } =
    useControllerDefinitionsStore()

  const [selectedController, setSelectedController] = useState<
    Partial<Controller> | undefined
  >(configItem.Controller)

  const [open, setOpen] = useState(false)
  const [value, setValue] = useState("")

  return (
    <Card>
      <CardHeader>
        <CardTitle>Define trigger</CardTitle>
        <CardDescription>
          The trigger defines the conditions or events that will activate this
          configuration.
        </CardDescription>
      </CardHeader>
      <CardContent className="flex flex-row gap-4">
        <Popover open={open} onOpenChange={setOpen}>
          <PopoverTrigger asChild>
            <Button
              variant="outline"
              role="combobox"
              aria-expanded={open}
              className="w-50 justify-between"
            >
              {selectedController
                ? controllers.find((controller) => controller.Serial === selectedController?.Serial)
                    ?.Name
                : "Select controller..."}
              <IconChevronDown className="opacity-50" />
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-50 p-0">
            <Command>
              <CommandInput
                placeholder="Search controller..."
                className="h-9"
              />
              <CommandList>
                <CommandEmpty>No controller found.</CommandEmpty>
                <CommandGroup>
                  {controllers.map((controller) => (
                    <CommandItem
                      key={controller.Serial}
                      value={controller.Serial}
                      onSelect={(currentValue) => {
                        setValue(currentValue === value ? "" : currentValue)
                        setOpen(false)
                      }}
                    >
                      {controller.Name}
                      <IconCheck
                        className={cn(
                          "ml-auto",
                          value === controller.Serial
                            ? "opacity-100"
                            : "opacity-0",
                        )}
                      />
                    </CommandItem>
                  ))}
                </CommandGroup>
              </CommandList>
            </Command>
          </PopoverContent>
        </Popover>
        <Button variant="outline" className="flex-1">
          Scan for input
        </Button>
      </CardContent>
    </Card>
  )
}
export default ConfigTrigger
