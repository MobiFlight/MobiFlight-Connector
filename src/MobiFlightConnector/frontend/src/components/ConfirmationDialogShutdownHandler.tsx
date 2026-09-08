import { useState } from "react"
import { useAppMessage } from "@/lib/hooks/appMessage"
import messageExchange from "@/lib/messageExchange"
import ConfirmationDialog from "@/components/ConfirmationDialog"

const ConfirmationDialogShutdownHandler = () => {
  const [open, setOpen] = useState(false)
  const { publish } = messageExchange

  useAppMessage("ShutdownConfirmationRequested", () => {
    setOpen(true)
  })

  const handleDiscardChanges = () => {
    setOpen(false)

    publish({
      key: "CommandShutdown",
      payload: {
        action: "discardChanges",
      },
    })
  }

  const handleCancel = () => {
    setOpen(false)
  }

  return (
    <ConfirmationDialog
      open={open}
      onOpenChange={setOpen}
      discardChanges={handleDiscardChanges}
      cancel={handleCancel}
    />
  )
}

export default ConfirmationDialogShutdownHandler
