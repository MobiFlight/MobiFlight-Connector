import ControllerIcon from "@/components/project/ControllerIcon"
import { Button } from "@/components/ui/button"
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover"
import { Controller, ControllerBinding } from "@/types/controller"
import { IconSelector } from "@tabler/icons-react"
import {
  Command,
  CommandInput,
  CommandList,
  CommandEmpty,
  CommandGroup,
  CommandItem,
} from "@/components/ui/command"
import { useState } from "react"
import ControllerBindingStatusIndicator from "@/components/controllers/ControllerBindingDialog/ControllerBindingStatusIndicator"
import { cn } from "@/lib/utils"

export type ControllerIconWithLabelProps = {
  serial: string
  status?: ControllerBinding["Status"] | undefined
}

const ControllerIconWithLabel = ({
  serial,
  status,
  className,
}: ControllerIconWithLabelProps & React.HTMLAttributes<HTMLDivElement>) => {
  const [controllerLabel, controllerSerial] = serial
    ?.split("/")
    ?.map((s) => s.trim()) ?? [null, null]
  return (
    <div
      className={cn(
        `flex flex-row items-center gap-2 overflow-hidden text-left`,
        className,
      )}
    >
      <ControllerIcon className="shrink-0" serial={serial} status={status} />
      <div className="overflow-hidden">
        <div className="truncate font-semibold">{controllerLabel}</div>
        <div className="text-muted-foreground truncate text-sm">
          {controllerSerial}
        </div>
      </div>
    </div>
  )
}

export type ControllerBindingProps = {
  controllerBinding: ControllerBinding
  controllers: Controller[]
  onUpdate: (binding: ControllerBinding, controller: Controller | null) => void
}

const ControllerBindingItem = ({
  controllerBinding,
  controllers,
  onUpdate,
}: ControllerBindingProps) => {
  const [, serial] = controllerBinding?.BoundController?.split("/")?.map((s) =>
    s.trim(),
  ) ?? [null, null]

  const boundController = serial
    ? controllers.find((controller) => controller.Serial.includes(serial))
    : null

  const [open, setOpen] = useState(false)
  const [selectedSerial, setSelectedSerial] = useState(boundController?.Serial)

  const selectedBoundController = controllers.find(
    (controller) => controller.Serial === selectedSerial,
  )

  return (
    <div
      className="grid grid-cols-[1fr_auto_1fr] items-center border-b border-solid py-2"
      data-testid="controller-binding-item"
    >
      <div className="overflow-hidden" data-testid="original-controller">
        <ControllerIconWithLabel
          serial={controllerBinding.OriginalController || ""}
          status={controllerBinding.Status}
        />
      </div>
      <ControllerBindingStatusIndicator
        isBound={!!selectedBoundController}
        status={controllerBinding.Status}
      />
      <div className="overflow-hidden">
        <Popover open={open} onOpenChange={setOpen} modal={true}>
          <PopoverTrigger asChild>
            <Button
              variant="outline"
              role="combobox"
              aria-expanded={open}
              className="flex h-14 w-full flex-row justify-between overflow-hidden"
              data-testid="bound-controller"
            >
              {selectedBoundController ? (
                <ControllerIconWithLabel
                  serial={`${selectedBoundController.Name} / ${selectedBoundController.Serial}`}
                />
              ) : (
                <span className="text-left">Select a controller</span>
              )}

              <IconSelector className="ml-2 h-4 w-4 flex-none shrink-0 opacity-50" />
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-107 p-0">
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
                        const itemSelected = currentValue !== selectedSerial
                        onUpdate(
                          controllerBinding,
                          itemSelected ? controller : null,
                        )
                        setSelectedSerial(itemSelected ? currentValue : "")
                        setOpen(false)
                      }}
                      className={
                        selectedSerial === controller.Serial
                          ? "bg-accent/50"
                          : ""
                      }
                    >
                      <ControllerIconWithLabel
                        serial={`${controller.Name} / ${controller.Serial}`}
                      />
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
