import { toast } from "@/components/ui/ToastWrapper"
import i18n from "i18next"

export type SyncNotificationType =
  | "restored"
  | "synced"
  | "saved"
  | "updated"
  | "failed_upload"
  | "failed_fetch"

export const showSyncNotification = (type: SyncNotificationType) => {
  return toast({
    id: `sync-${type}`,
    title: i18n.t(`Settings.Sync.Notifications.${type}.Title`),
    description: i18n.t(`Settings.Sync.Notifications.${type}.Description`),
  })
}

export default showSyncNotification
