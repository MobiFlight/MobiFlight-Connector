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
    
    switch (notification.Event) {
      case "ControllerAutoBindSuccessful": {
        const controllerName =
          notification?.Context?.Controllers ?? "Unknown Controller"
        toast({
          id: "autobind-controllers-successful",
          title: t("Notifications.ControllerAutoBindSuccessful.Title"),
          description: t("Notifications.ControllerAutoBindSuccessful.Description", { controllerName }),
        })
        break
      }

      case "ControllerManualBindRequired": {
        const controllerName =
          notification?.Context?.Controllers ?? "Unknown Controller"
        toast({
          id: "manual-binding-required",
          title: t("Notifications.ControllerManualBindRequired.Title"),
          description: t("Notifications.ControllerManualBindRequired.Description", { controllerName }),
          button: {
            label: t("Notifications.ControllerManualBindRequired.Action"),
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

      case "ProjectFileExtensionMigrated":
        toast({
          id: "file-extension-migrated",
          title: "Your project just got better!",
          description: `We have automatically migrated your project to use the new "Project" extension. You don't have to do anything. All safe and sound!`,
        })
        break

      case "SimConnectionLost": {
        const simType = notification?.Context?.SimType ?? "the simulator"
        toast({
          id: "sim-connection-lost",
          title: t("Notifications.SimConnectionLost.Title"),
          description: t("Notifications.SimConnectionLost.Description", { simType }),
        })
        break
      }

      case "SimStopped":
        toast({
          id: "sim-stopped",
          title: t("Notifications.SimStopped.Title"),
          description: t("Notifications.SimStopped.Description"),
        })
        break

      case "TestModeException": {
        const errorMessage = notification?.Context?.ErrorMessage ?? "An error occurred"
        toast({
          id: "test-mode-exception",
          title: t("Notifications.TestModeException.Title"),
          description: t("Notifications.TestModeException.Description", { errorMessage }),
        })
        break
      }

      default:
        console.error("Unhandled notification event:", notification.Event)
        break
    }
  })

  useAppMessage("HubHopState", (message) => {
    const status = message.payload
    if (status.ShouldUpdate && status.Result === "Pending") {
      toast({
        id: "hubhop-auto-update",
        title: t("General.HubHopUpdate.Title"),
        description: t("General.HubHopUpdate.Description", { days: 7 }),
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

    if (
      status.ShouldUpdate &&
      status.Result === "InProgress" &&
      status.UpdateProgress === 0
    ) {
      toast({
        id: "hubhop-auto-update",
        title: t("General.HubHopUpdate.Title.Downloading"),
        description: <HubHopUpdateToast timeout={2000} />,
        options: {
          duration: Infinity, // Keep it open until completed
        },
      })
    }
  })

  // This component doesn't render anything visible
  return null
}
