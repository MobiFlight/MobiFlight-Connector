import { AppMessageKey, IDeviceElement, IDeviceItem, Notification } from "@/types"
import { AppMessage, FrontendMessageType } from "@/types/messages"
import { InterpolationMap } from "i18next"
import { useEffect } from "react"
import { useTranslation } from "react-i18next"
import { useNavigate, useOutletContext } from "react-router-dom"
import { ExternalToast } from "sonner"

export const publishOnMessageExchange = () => ({
  publish: (message: FrontendMessageType) => {
    console.log(`Publishing FrontendMessage -> ${message.key}`)
    window.chrome?.webview?.postMessage(message)
  },
})
// create a useAppMessage hook that listens for messages
// the paramaters are the AppMessageKey and the onReceiveMessage callback
// the callback is called when a message is received
export const useAppMessage = (
  key: AppMessageKey,
  onReceiveMessage: (message: AppMessage) => void
) => {
  useEffect(() => {
    const onReveiveMessageHandler = (event: Event) => {
      try {
        const appMessage = JSON.parse(
          (event as MessageEvent).data
        ) as AppMessage
        if (appMessage.key === key) {
          onReceiveMessage(appMessage)
        }
      } catch (error) {
        console.error("Error parsing message", error)
      }
    }

    // add an event listener for the message key
    window.chrome?.webview?.addEventListener("message", onReveiveMessageHandler)
    return () => {
      // remove the event listener when the component is unmounted
      window.chrome?.webview?.removeEventListener(
        "message",
        onReveiveMessageHandler
      )
    }
  }, [key, onReceiveMessage])
}

export type DeviceDetailContext = {
  device: IDeviceItem;
  element: IDeviceElement;
  updateDevice: (device: IDeviceItem) => void;
};

export function useDeviceDetailPageContext() {
  return useOutletContext<DeviceDetailContext>();
}

export function useNotification() {
  const navigate = useNavigate()
  const { t } = useTranslation()
  return {
    prepareForToast: (message: AppMessage) => {
      const notification = message.payload as Notification
      const keys: InterpolationMap<string> = {}

      if (typeof notification.Value === "string") {
        keys["Data"] = notification.Value
      } else {
        Object.keys(notification.Value ?? {}).forEach(key => {
          keys[key] = notification.Value[key]?.toString()
        })
      }

      const title = t(`notification.${notification.Type}.title`, keys)
      const options: ExternalToast = {
        duration: 6000,	
        description: t(`notification.${notification.Type}.description`, keys)
      }

      const hasAction = notification.Action && notification.Action !== ""
      if (hasAction) {
        options.action = {
          label: t(`notification.${notification.Type}.action.label`),
          onClick: () => {
            console.log(
              `Navigating to ${t(`notification.${notification.Type}.action.navigate`)}`
            )
            navigate(t(`notification.${notification.Type}.action.navigate`, keys))
          }
        }
      }

      return { title: title, options: options }
    }
  }
}
