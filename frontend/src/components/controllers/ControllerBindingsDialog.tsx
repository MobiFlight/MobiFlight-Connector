import { ControllerBindingFilter } from "@/components/controllers/ControllerBindingDialog/ControllerBindingFilter"
import ControllerBindingItem from "@/components/controllers/ControllerBindingDialog/ControllerBindingItem"
import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { ScrollArea } from "@/components/ui/scroll-area"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import {
  Controller,
  ControllerBinding,
  ControllerBindingStatus,
} from "@/types/controller"
import { IconCheck } from "@tabler/icons-react"
import { useState } from "react"
import { useTranslation } from "react-i18next"

export type ControllerBindingsProps = {
  bindings: ControllerBinding[]
  controllers: Controller[]
  isOpen: boolean
  onOpenChange: (open: boolean) => void
}

const ControllerBindingsDialog = ({
  bindings,
  controllers,
  isOpen,
  onOpenChange,
}: ControllerBindingsProps) => {
  const { t } = useTranslation()
  const [finalBindings, setFinalBindings] =
    useState<ControllerBinding[]>(bindings)
  const { publish } = publishOnMessageExchange()

  // Use the original bindings to determine available states
  // this will ensure that the filter options are consistent
  // even if the user has changed some bindings using the dialog
  const availableStates = [...new Set(bindings.map((b) => b.Status))]

  // Sort bindings by status priority using their original status
  const sortedBindings = [...finalBindings].sort((a, b) => {
    const originalStatusA =
      bindings.find((bind) => bind.OriginalController === a.OriginalController)
        ?.Status ?? a.Status
    const originalStatusB =
      bindings.find((bind) => bind.OriginalController === b.OriginalController)
        ?.Status ?? b.Status
    const priority = {
      RequiresManualBind: 0,
      Missing: 1,
      AutoBind: 2,
      Match: 3,
    }
    return priority[originalStatusA] - priority[originalStatusB]
  })

  const initialFilter =
    sortedBindings.length > 0 ? sortedBindings[0].Status : "all"
  const [filter, setFilter] = useState<ControllerBindingStatus | "all">(
    initialFilter,
  )

  // Filter bindings based on selected filter
  // Use the original status for filter evaluation
  const filteredBindings = sortedBindings.filter((binding) => {
    if (filter === "all") return true

    // Filter based on original status
    const originalStatus =
      bindings.find(
        (bind) => bind.OriginalController === binding.OriginalController,
      )?.Status ?? binding.Status

    return originalStatus === filter
  })

  const handleControllerBindingUpdate = (
    binding: ControllerBinding,
    controller: Controller | null,
  ) => {
    const updatedBindings = finalBindings.map((b) => {
      if (b.OriginalController !== binding.OriginalController) return b

      return {
        ...b,
        BoundController: controller
          ? controller.Name + " / " + controller.Serial
          : null,
        Status: controller ? "Match" : "Missing",
      } as ControllerBinding
    })
    setFinalBindings(updatedBindings)
  }

  const saveChanges = () => {
    publish({
      key: "CommandControllerBindingsUpdate",
      payload: {
        bindings: finalBindings,
      },
    })
    onOpenChange(false)
  }

  const allControllersAreBound = finalBindings.every((binding) =>
    ["Match", "AutoBind"].includes(binding.Status),
  )

  return (
    <Dialog open={isOpen} onOpenChange={onOpenChange}>
      <DialogContent className="vsm:min-h-[75%] vxl:min-h-[60%] flex min-h-[90%] flex-col overflow-y-auto select-none sm:max-w-150 lg:max-w-200 xl:max-w-250">
        <DialogHeader>
          <DialogTitle className="text-2xl">
            {t("Dialog.ControllerBinding.Title")}
          </DialogTitle>
          <DialogDescription className="text-md vsm:block hidden">
            {t("Dialog.ControllerBinding.Description")}
          </DialogDescription>
        </DialogHeader>
        <ControllerBindingFilter
          availableStates={availableStates}
          activeFilter={filter}
          updateFilter={setFilter}
        />
        <div className="flex flex-row justify-between">
          <div className="text-muted-foreground font-semibold">
            {t("Dialog.ControllerBinding.ControllersInProject", {
              count: bindings.length,
            })}
          </div>
          <div className="text-muted-foreground font-semibold">
            {t("Dialog.ControllerBinding.ConnectedControllers", {
              count: controllers.length,
            })}
          </div>
        </div>
        <div className="relative flex grow flex-col gap-2">
          <ScrollArea className="grow">
            <div className="pr-3">
              {
                /* Original Controller Bindings */
                filteredBindings.map((binding) => (
                  <ControllerBindingItem
                    key={binding.OriginalController}
                    controllerBinding={binding}
                    controllers={controllers}
                    onUpdate={handleControllerBindingUpdate}
                  />
                ))
              }
            </div>
          </ScrollArea>
        </div>
        <DialogFooter className="flex flex-row justify-between">
          <div className="flex grow flex-row items-center gap-1">
            {allControllersAreBound && (
              <>
                <IconCheck className="mr-2 inline h-8 w-8 text-green-600" />
                <span className="text-green-600">
                  {t("Dialog.ControllerBinding.AllSet")}
                </span>
              </>
            )}
          </div>
          <div className="flex flex-row gap-2">
            <DialogClose asChild>
              <Button variant="outline" type="button">
                {t("Dialog.General.Close")}
              </Button>
            </DialogClose>
            <Button onClick={saveChanges}>
              {t("Dialog.General.ApplyChanges")}
            </Button>
          </div>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

export default ControllerBindingsDialog
