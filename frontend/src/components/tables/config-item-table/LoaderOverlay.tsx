import {
  Dialog,
  DialogOverlay,
  DialogPortal,  
} from "@/components/ui/dialog"

import * as DialogPrimitive from "@radix-ui/react-dialog"
import { LoaderIcon } from "lucide-react"

type LoaderOverlayProps = {
  open: boolean
  onOpenChange?: (open: boolean) => void
}

const LoaderOverlay = (props: LoaderOverlayProps) => {
  const onOpenChange = props.onOpenChange ? props.onOpenChange : () => {}

  return (
    <>
      <Dialog open={props.open} onOpenChange={onOpenChange}>
        <DialogPortal>
          <DialogOverlay
            onClick={() => {
              onOpenChange(false)
            }}
          />
          <DialogPrimitive.Content className="fixed left-[50%] top-[50%] z-51 grid w-full max-w-lg translate-x-[-50%] translate-y-[-50%] gap-4">
            <div className="h-full w-full flex items-center justify-center flex-col gap-2">
              <LoaderIcon className="animate-spin text-background" />
              <p className="text-background">Showing wizard...</p>
            </div>
          </DialogPrimitive.Content>
        </DialogPortal>
      </Dialog>
    </>
  )
}
export default LoaderOverlay
