import { toast } from "@/components/ui/ToastWrapper"
import i18n from "i18next"
import {
  IconCloudCheck,
  IconCloudUpload,
  IconCloudOff,
} from "@tabler/icons-react"

export type SyncNotificationType =
  | "restored"
  | "synced"
  | "saved"
  | "updated"
  | "failed_upload"
  | "failed_fetch"

const renderIcon = (type: SyncNotificationType) => {
  if (type.startsWith("failed")) {
    return (
      <IconCloudOff
        className="h-5 w-5 text-destructive shrink-0"
        aria-hidden="true"
      />
    )
  }
  if (type === "synced") {
    return (
      <IconCloudUpload
        className="h-5 w-5 text-primary shrink-0"
        aria-hidden="true"
      />
    )
  }
  return (
    <IconCloudCheck
      className="h-5 w-5 text-primary shrink-0"
      aria-hidden="true"
    />
  )
}

export const showSyncNotification = (type: SyncNotificationType) => {
  return toast({
    id: `sync-${type}`,
    title: (
      <span className="flex items-center gap-2">
        {renderIcon(type)}
        <span>{i18n.t(`Settings.Sync.Notifications.${type}.Title`)}</span>
      </span>
    ),
    description: i18n.t(`Settings.Sync.Notifications.${type}.Description`),
  })
}

export default showSyncNotification
