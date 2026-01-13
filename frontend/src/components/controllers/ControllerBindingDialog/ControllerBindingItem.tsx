import ControllerIcon from "@/components/project/ControllerIcon"
import { Button } from "@/components/ui/button"
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover"
import { cn } from "@/lib/utils"
import { Controller, ControllerBinding } from "@/types/controller"
import {
  IconCheck,
  IconCircleCheck,
  IconCircleDashed,
  IconSelector,
} from "@tabler/icons-react"
import {
  Command,
  CommandInput,
  CommandList,
  CommandEmpty,
  CommandGroup,
  CommandItem,
} from "@/components/ui/command"
import { useEffect, useState } from "react"

const ControllerIconWithLabel = ({
  serial,
  status,
}: {
  serial: string
  status: ControllerBinding["Status"]
}) => {
  const [controllerLabel, controllerSerial] = serial
    ?.split("/")
    ?.map((s) => s.trim()) ?? [null, null]
  return (
    <div className="flex flex-row items-center gap-2 text-left">
      <ControllerIcon
        className="transition-all ease-in-out"
        serial={serial}
        status={status}
      />
      <div className="flex flex-col">
        <div className="font-semibold">{controllerLabel}</div>
        <div className="text-muted-foreground text-sm">{controllerSerial}</div>
      </div>
    </div>
  )
}

export type ControllerBindingProps = {
  controllerBinding: ControllerBinding
  controllers: Controller[]
}

const ControllerBindingItem = ({
  controllerBinding,
  controllers,
}: ControllerBindingProps) => {
  const [, serial] = controllerBinding?.BoundController?.split("/")?.map((s) =>
    s.trim(),
  ) ?? [null, null]

  const boundController = controllers.find((controller) =>
    controller.Serial.includes(serial),
  )

  const [open, setOpen] = useState(false)
  const [value, setValue] = useState(boundController?.Serial)

  console.log("boundController", boundController)
  useEffect(() => {
    console.log("value: ", value)
  }, [value])

  return (
    <div className="flex flex-row items-center gap-2 border-b border-solid py-2">
      <div className="flex flex-1/2 flex-row gap-4">
        <ControllerIconWithLabel
          serial={controllerBinding.OriginalController || ""}
          status={controllerBinding.Status}
        />
      </div>
      <div className="flex flex-row items-center gap-0">
        <div className="h-1 w-6 border-b border-muted-foreground/50" />
        {boundController ? (
          <IconCircleCheck className="h-8 w-8 text-green-500" />
        ) : (
          <IconCircleDashed className="h-8 w-8 stroke-muted-foreground/50" />
        )}
        <div className="h-1 w-6 border-b border-muted-foreground/50" />
      </div>

      <div className="flex flex-1/2 flex-row">
        <Popover open={open} onOpenChange={setOpen}>
          <PopoverTrigger asChild>
            <Button
              variant="outline"
              role="combobox"
              aria-expanded={open}
              className="flex h-14 w-full flex-row justify-between"
            >
              {boundController ? (
                <ControllerIconWithLabel
                  serial={
                    boundController
                      ? boundController.Name + "/" + boundController.Serial
                      : ""
                  }
                  status={
                    boundController ? controllerBinding.Status : "Missing"
                  }
                />
              ) : (
                "Select a controller"
              )}

              <IconSelector className="ml-2 h-4 w-4 shrink-0 opacity-50" />
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-full p-0">
            <Command>
              <CommandInput placeholder="Search controller..." />
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
                      <IconCheck
                        className={cn(
                          "mr-2 h-4 w-4",
                          value === controller.Serial
                            ? "opacity-100"
                            : "opacity-0",
                        )}
                      />
                      {controller.Name}
                    </CommandItem>
                  ))}
                </CommandGroup>
              </CommandList>
            </Command>
          </PopoverContent>
        </Popover>
      </div>
    </div>
  )
}

export default ControllerBindingItem
