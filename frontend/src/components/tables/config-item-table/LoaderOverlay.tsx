import {
  Dialog,
  DialogDescription,
  DialogOverlay,
  DialogPortal,
  DialogTitle,
} from "@/components/ui/dialog"

import * as DialogPrimitive from "@radix-ui/react-dialog"
import { IconLoader2 as LoaderIcon } from "@tabler/icons-react"

type LoaderOverlayProps = {
  message: string
  open: boolean
  onOpenChange?: (open: boolean) => void
}

const LoaderOverlay = ({
  message,
  open,
  onOpenChange,
}: LoaderOverlayProps) => {
  const handleOpenChange = onOpenChange ? onOpenChange : () => {}
  return (
    <>
      <Dialog open={open} onOpenChange={handleOpenChange}>
        <DialogPortal>
          <DialogOverlay
            data-testid="loader-overlay"
            onClick={() => {
              handleOpenChange(false)
            }}
          />
          <DialogTitle>Loader Overlay</DialogTitle>
          <DialogDescription>Overlay as long as Config Wizard is displayed</DialogDescription>
          <DialogPrimitive.Content className="fixed top-[50%] left-[50%] z-51 grid w-full max-w-lg translate-x-[-50%] translate-y-[-50%] gap-4 focus:outline-none">
            <div className="flex h-full w-full flex-col items-center justify-center gap-2">
              <LoaderIcon className="text-background dark:text-foreground animate-spin" />
              <p className="text-background dark:text-foreground">
                {message}
              </p>
            </div>
          </DialogPrimitive.Content>
        </DialogPortal>
      </Dialog>
    </>
  )
}
export default LoaderOverlay
