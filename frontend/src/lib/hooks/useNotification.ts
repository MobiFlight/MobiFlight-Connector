import { AppMessage, Notification } from "@/types"
import { InterpolationMap } from "i18next"
import { useTranslation } from "react-i18next"
import { useNavigate } from "react-router-dom"
import { ExternalToast } from "sonner"

export const useNotification = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()
  return {
    prepareForToast: (message: AppMessage) => {
      const notification = message.payload as Notification
      const keys: InterpolationMap<string> = {}

      if (typeof notification.Value === "string") {
        keys["Value"] = notification.Value
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