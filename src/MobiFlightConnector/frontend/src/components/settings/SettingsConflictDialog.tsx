import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
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

  const handleOpenChange = (open: boolean) => {
    if (!open) {
      onKeepLocal()
    }
    onOpenChange?.(open)
  }

  return (
    <Dialog open={isOpen} onOpenChange={handleOpenChange}>
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
          {/* Option 1: Use local settings */}
          <Button
            type="button"
            variant="default"
            disabled={isProcessing}
            onClick={onUploadLocal}
            className="h-auto w-full flex-col items-start justify-start p-4 text-left whitespace-normal rounded-xl"
          >
            <div className="text-base font-semibold">
              {t("Settings.Conflict.UseLocal", "Use local settings")}
            </div>
            <div className="mt-0.5 text-sm font-normal text-primary-foreground/90">
              {t(
                "Settings.Conflict.UseLocalDesc",
                "Save them to the cloud, replacing the cloud version.",
              )}
            </div>
          </Button>

          {/* Option 2: Use cloud settings */}
          <Button
            type="button"
            variant="outline"
            disabled={isProcessing}
            onClick={onUseCloud}
            className="h-auto w-full flex-col items-start justify-start p-4 text-left whitespace-normal rounded-xl"
          >
            <div className="text-base font-semibold text-foreground">
              {t("Settings.Conflict.UseCloud", "Use cloud settings")}
            </div>
            <div className="mt-0.5 text-sm font-normal text-muted-foreground">
              {t(
                "Settings.Conflict.UseCloudDesc",
                "Replace the local settings with the cloud version.",
              )}
            </div>
          </Button>

          {/* Option 3: Turn off cloud sync */}
          <Button
            type="button"
            variant="ghost"
            disabled={isProcessing}
            onClick={onKeepLocal}
            className="h-auto w-full flex-col items-start justify-start p-4 text-left whitespace-normal rounded-xl border border-border"
          >
            <div className="text-base font-semibold text-foreground">
              {t("Settings.Conflict.TurnOffSync", "Turn off cloud sync")}
            </div>
            <div className="mt-0.5 text-sm font-normal text-muted-foreground">
              {t(
                "Settings.Conflict.TurnOffSyncDesc",
                "Keep your local settings and don't update the cloud version.",
              )}
            </div>
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  )
}

export default SettingsConflictDialog
