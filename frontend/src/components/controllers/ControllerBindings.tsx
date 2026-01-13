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
import { Controller, ControllerBinding } from "@/types/controller"
import { IconCheck } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export type ControllerBindingsProps = {
  bindings: ControllerBinding[]
  controllers: Controller[]
  isOpen: boolean
  onOpenChange: (open: boolean) => void
}

const ControllerBindings = ({
  bindings,
  controllers,
  isOpen,
  onOpenChange,
}: ControllerBindingsProps) => {
  const { t } = useTranslation()

  const sortedBindings = bindings.sort((a, b) => {
    const priority = {
      RequiresManualBind: 0,
      Missing: 1,
      AutoBind: 2,
      Match: 3,
    }
    return priority[a.Status] - priority[b.Status]
  })

  return (
    <Dialog open={isOpen} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[90vh] overflow-y-auto sm:max-w-150 lg:max-w-200 xl:max-w-250">
        <DialogHeader>
          <DialogTitle className="text-2xl">
            {t("ControllerBinding.Title")}
          </DialogTitle>
          <DialogDescription className="text-md">
            {t("ControllerBinding.Description")}
          </DialogDescription>
        </DialogHeader>
        <div className="flex flex-col gap-2">
          <div className="flex flex-row items-center gap-2 pb-2">
            <Button className="h-8" variant={"default"}>
              All
            </Button>
            <Button className="h-8" variant={"outline"}>
              Manual rebind
            </Button>
            <Button className="h-8" variant={"outline"}>
              Missing
            </Button>
            <Button className="h-8" variant={"outline"}>
              Auto bind
            </Button>
            <Button className="h-8" variant={"outline"}>
              Match
            </Button>
          </div>
          <div className="flex flex-col pt-2">
            <div className="flex flex-row justify-between">
              <div className="text-muted-foreground font-semibold">Original controller</div>
              <div className="text-muted-foreground font-semibold">Connected controller</div>
            </div>
            {
              /* Original Controller Biundings */
              sortedBindings.map((binding, index) => (
                <ControllerBindingItem
                  key={index}
                  controllerBinding={binding}
                  controllers={controllers}
                />
              ))
            }
          </div>
        </div>
        <DialogFooter className="flex flex-row justify-between">
          <div className="grow">
            <IconCheck className="mr-2 inline h-8 w-8 text-green-600" />
            <span className="text-green-600">All set! Your profile is completely configured.</span>
          </div>
          <div className="flex flex-row gap-2">
            <DialogClose asChild>
              <Button variant="outline" type="button">
                Close
              </Button>
            </DialogClose>
            <Button>Apply & Save</Button>
          </div>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

export default ControllerBindings
