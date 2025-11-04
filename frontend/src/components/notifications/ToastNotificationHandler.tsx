import { publishOnMessageExchange, useAppMessage } from "@/lib/hooks/appMessage"
import { Notification } from "@/types/messages"
import { toast } from "@/components/ui/ToastWrapper"
import { CommandMainMenu } from "@/types/commands"
import HubHopUpdateToast from "./HubHopUpdateToast"
import { useTranslation } from "react-i18next"

export const ToastNotificationHandler = () => {
  const { publish } = publishOnMessageExchange()
  const { t } = useTranslation()

  useAppMessage("Notification", (message) => {
    const notification = message.payload as Notification
    const controllerType = notification.Context?.Type ?? "Board"

    switch (notification.Event) {
      case "MissingControllerDetected":
        toast({
          id: "missing-controllers-detected",
          title: "Missing Controllers Detected",
          description: `Some ${controllerType} controllers used in this profile are currently not connected.`,
          button: {
            label: `Reassign ${controllerType}`,
            onClick: () => {
              publish({
                key: "CommandMainMenu",
                payload: { action: "extras.serials" },
              } as CommandMainMenu)
            },
          },
        })
        break
    }
  })

  useAppMessage("HubHopState", (message) => {
    const status = message.payload
    if (status.ShouldUpdate && status.Result === "Pending") {
      toast({
        id: "hubhop-auto-update",
        title: t("General.HubHopUpdate.Title"),
        description:
          t("General.HubHopUpdate.Description", { days: 7 }),
        button: {
          label: "Update Now",
          onClick: () => {
            publish({
              key: "CommandMainMenu",
              payload: { action: "extras.hubhop.download" },
            } as CommandMainMenu)
          },
        },
      })
    }

    if (status.ShouldUpdate && status.Result === "InProgress" && status.UpdateProgress === 0) {
      toast({
        id: "hubhop-auto-update",
        title: t("General.HubHopUpdate.Title.Downloading"),
        description: <HubHopUpdateToast timeout={2000} />,
        options: {
          duration: Infinity, // Keep it open until completed
        }
      })
    }
  })

  // This component doesn't render anything visible
  return null
}