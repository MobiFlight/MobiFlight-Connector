import { useState } from "react"
import { useAppMessage } from "@/lib/hooks/appMessage"
import messageExchange from "@/lib/messageExchange"
import ConfirmationDialogShutdown from "@/components/ConfirmationDialogShutdown"

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

  const handleCancelShutdown = () => {
    setOpen(false)
  }

  return (
    <ConfirmationDialogShutdown
      open={open}
      onOpenShutdown={setOpen}
      discardChanges={handleDiscardChanges}
      cancel={handleCancelShutdown}
    />
  )
}

export default ConfirmationDialogShutdownHandler
