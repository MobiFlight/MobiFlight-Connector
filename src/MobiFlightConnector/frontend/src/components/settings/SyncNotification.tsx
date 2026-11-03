import { useTranslation } from "react-i18next"
import {
  IconCloudCheck,
  IconCloudUpload,
  IconCloudOff,
  IconX,
} from "@tabler/icons-react"
import { toast as sonnerToast } from "sonner"

export type SyncNotificationType =
  | "restored"
  | "synced"
  | "saved"
  | "updated"
  | "failed_upload"
  | "failed_fetch"

interface SyncNotificationProps {
  type: SyncNotificationType
  onClose?: () => void
}

export const SyncNotification = ({ type, onClose }: SyncNotificationProps) => {
  const { t } = useTranslation()
  const isError = type.startsWith("failed")

  const renderIcon = () => {
    if (isError) {
      return <IconCloudOff className="h-6 w-6 text-red-500" />
    }
    if (type === "restored" || type === "updated") {
      return <IconCloudCheck className="h-6 w-6 text-blue-500" />
    }
    return <IconCloudUpload className="h-6 w-6 text-blue-500" />
  }

  return (
    <div
      className={`relative flex items-center gap-4 rounded-2xl border-2 bg-white p-4 shadow-lg dark:bg-zinc-900 w-[420px] max-w-full ${
        isError ? "border-red-500" : "border-blue-500"
      }`}
    >
      <div
        className={`flex h-12 w-12 shrink-0 items-center justify-center rounded-full ${
          isError ? "bg-red-50 dark:bg-red-950" : "bg-blue-50 dark:bg-blue-950"
        }`}
      >
        {renderIcon()}
      </div>

      <div className="flex-1 min-w-0 pr-2">
        <h4 className="font-semibold text-gray-900 dark:text-white text-sm">
          {t(`Settings.Sync.Notifications.${type}.Title`)}
        </h4>
        <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5 leading-snug">
          {t(`Settings.Sync.Notifications.${type}.Description`)}
        </p>
      </div>

      <button
        onClick={onClose}
        className="shrink-0 text-gray-400 hover:text-gray-600 dark:hover:text-gray-200 transition-colors p-1"
        aria-label="Close"
      >
        <IconX className="h-5 w-5" />
      </button>
    </div>
  )
}

export const showSyncNotification = (type: SyncNotificationType) => {
  return sonnerToast.custom((id) => (
    <SyncNotification type={type} onClose={() => sonnerToast.dismiss(id)} />
  ))
}

export default SyncNotification
