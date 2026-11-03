import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from "@/components/ui/dialog"
import { IconCloud } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export interface SettingsConflictDialogProps {
  isOpen: boolean
  onOpenChange?: (open: boolean) => void
  onKeepLocal: () => void | Promise<void>
  onUseCloud: () => void | Promise<void>
  onUploadLocal: () => void | Promise<void>
  isProcessing?: boolean
}

export function SettingsConflictDialog({
  isOpen,
  onOpenChange,
  onKeepLocal,
  onUseCloud,
  onUploadLocal,
  isProcessing = false,
}: SettingsConflictDialogProps) {
  const { t } = useTranslation()

  return (
    <Dialog open={isOpen} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[480px] p-6 rounded-2xl gap-6">
        <div className="flex items-start gap-4">
          {/* Cloud conflict icon badge */}
          <div className="relative flex h-12 w-12 shrink-0 items-center justify-center rounded-full bg-blue-50 dark:bg-blue-950/40">
            <IconCloud className="h-6 w-6 text-blue-600 dark:text-blue-400" />
            <div
              className="absolute -bottom-0.5 -right-0.5 flex h-4 w-4 items-center justify-center rounded-full bg-red-500 ring-2 ring-background"
              aria-hidden="true"
            >
              <div className="h-0.5 w-2 rounded-full bg-white" />
            </div>
          </div>

          <DialogHeader className="space-y-1.5 text-left">
            <DialogTitle className="text-lg font-bold text-foreground">
              {t("Settings.Conflict.Title", "Settings conflict")}
            </DialogTitle>
            <DialogDescription className="text-sm leading-relaxed text-muted-foreground">
              {t(
                "Settings.Conflict.Description",
                "We found different versions of your settings on this device and in the cloud. Choose which version you want to keep.",
              )}
            </DialogDescription>
          </DialogHeader>
        </div>

        {/* Action cards - 3 options */}
        <div className="flex flex-col gap-3">
          {/* Option 1: Keep local settings */}
          <button
            type="button"
            disabled={isProcessing}
            onClick={onKeepLocal}
            className="w-full rounded-xl border border-border bg-card p-4 text-left transition-all hover:bg-accent/40 hover:border-primary/40 focus:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:opacity-50 disabled:pointer-events-none"
          >
            <div className="text-base font-semibold text-foreground">
              {t("Settings.Conflict.KeepLocal", "Keep local settings")}
            </div>
            <div className="mt-0.5 text-sm text-muted-foreground">
              {t(
                "Settings.Conflict.KeepLocalDesc",
                "Keep settings on this device without syncing with the cloud.",
              )}
            </div>
          </button>

          {/* Option 2: Use cloud settings */}
          <button
            type="button"
            disabled={isProcessing}
            onClick={onUseCloud}
            className="w-full rounded-xl bg-blue-600 p-4 text-left shadow-sm transition-all hover:bg-blue-700 active:scale-[0.99] focus:outline-none focus-visible:ring-2 focus-visible:ring-ring dark:bg-blue-600 dark:hover:bg-blue-700 disabled:opacity-50 disabled:pointer-events-none"
          >
            <div className="text-base font-semibold text-white">
              {t("Settings.Conflict.UseCloud", "Use cloud settings")}
            </div>
            <div className="mt-0.5 text-sm text-blue-100">
              {t(
                "Settings.Conflict.UseCloudDesc",
                "Replace local settings with the version saved in the cloud.",
              )}
            </div>
          </button>

          {/* Option 3: Upload local to cloud */}
          <button
            type="button"
            disabled={isProcessing}
            onClick={onUploadLocal}
            className="w-full rounded-xl border border-border bg-card p-4 text-left transition-all hover:bg-accent/40 hover:border-primary/40 focus:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:opacity-50 disabled:pointer-events-none"
          >
            <div className="text-base font-semibold text-foreground">
              {t("Settings.Conflict.UploadLocal", "Upload local to cloud")}
            </div>
            <div className="mt-0.5 text-sm text-muted-foreground">
              {t(
                "Settings.Conflict.UploadLocalDesc",
                "Overwrite the cloud settings with this device's current version.",
              )}
            </div>
          </button>
        </div>
      </DialogContent>
    </Dialog>
  )
}

export default SettingsConflictDialog
