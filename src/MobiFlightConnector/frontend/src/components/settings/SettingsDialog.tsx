import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  DialogDescription,
} from "@/components/ui/dialog"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import SimulatorSettingsCard from "./components/SimulatorSettingsCard"
import GeneralSettingsCard from "./components/GeneralSettingsCard"
import { Button } from "@/components/ui/button"
import { useSettingsStore } from "@/stores/settingsStore"
import { useState } from "react"
import { Trans, useTranslation } from "react-i18next"
import messageExchange from "@/lib/messageExchange"
import Settings from "@/types/settings"
import { ScrollArea } from "../ui/scroll-area"
import { Badge } from "@/components/ui/badge"
import { useAuth } from "react-oidc-context"
import { uploadCloudSettings } from "@/lib/cloudSettings"
import useCloudSettingsSync from "@/lib/hooks/useCloudSettingsSync";
import { showSyncNotification } from "@/components/settings/SyncNotification";
import { IconCloud, IconCloudOff, IconCheck } from "@tabler/icons-react";

export type SettingsDialogProps = {
  isOpen: boolean
  onOpenChange: (open: boolean) => void
}

export default function SettingsDialog({
  isOpen,
  onOpenChange,
}: SettingsDialogProps) {
  const { t } = useTranslation()
  const originalSettings = useSettingsStore((state) => state.settings)
  const setSettings = useSettingsStore((state) => state.setSettings)

  const auth = useAuth()
  const [prevOriginalSettings, setPrevOriginalSettings] =
    useState(originalSettings)
  const [draftSettings, setDraftSettings] = useState<Partial<Settings>>(
    originalSettings || {},
  )

  if (originalSettings !== prevOriginalSettings) {
    setPrevOriginalSettings(originalSettings)
    setDraftSettings(originalSettings || {})
  }

  const updateSetting = <K extends keyof Settings>(
    key: K,
    value: Settings[K],
  ) => {
    setDraftSettings((prev) => ({
      ...prev,
      [key]: value,
    }))
  }
  const { triggerSyncCheck } = useCloudSettingsSync()
  const handleSave = async () => {
    const isEnablingSync =
      draftSettings.CloudSettingsSyncEnabled &&
      !originalSettings?.CloudSettingsSyncEnabled &&
      auth.isAuthenticated

    if (draftSettings) {
      setSettings(draftSettings as Settings)
      messageExchange.publish({
        key: "CommandUpdateSettings",
        payload: {
          Settings: draftSettings,
        },
      })

      if (isEnablingSync) {
        triggerSyncCheck(draftSettings)
      } else if (auth.isAuthenticated && draftSettings.CloudSettingsSyncEnabled) {
        uploadCloudSettings(auth, {
          schemaVersion: 1,
          content: draftSettings,
        })
          .then(() => {
            showSyncNotification("saved")
          })
          .catch((err: unknown) => {
            console.error(
              "[SettingsDialog] Failed to upload settings to cloud on save:",
              err,
            )
            showSyncNotification("failed_upload")
          })
      }
    }
    onOpenChange(false)
  }

  const [showDiscardConfirm, setShowDiscardConfirm] = useState(false)

  const hasUnsavedChanges = () => {
    if (!originalSettings) return Object.keys(draftSettings).length > 0
    return JSON.stringify(draftSettings) !== JSON.stringify(originalSettings)
  }

  const renderSyncStatus = () => {
    if (!auth.isAuthenticated) {
      return (
        <div className="flex items-center gap-2 text-xs text-muted-foreground select-none">
          <IconCloudOff className="h-5 w-5 text-muted-foreground/70" />
          <span>{t("Settings.Sync.Status.SignInToSync", "Sign in to sync settings")}</span>
        </div>
      )
    }

    if (!draftSettings.CloudSettingsSyncEnabled) {
      return (
        <div className="flex items-center gap-2 text-xs text-muted-foreground select-none">
          <IconCloudOff className="h-5 w-5 text-muted-foreground/70" />
          <span>{t("Settings.Sync.Status.SyncPaused", "Cloud sync paused")}</span>
        </div>
      )
    }

    return (
      <div className="flex items-center gap-2 text-xs text-muted-foreground select-none">
        <div className="relative flex items-center justify-center">
          <IconCloud className="h-5 w-5 text-blue-500" />
          <IconCheck className="h-3 w-3 text-emerald-500 absolute -bottom-0.5 -right-1 stroke-[3]" />
        </div>
        <span className="text-gray-700 dark:text-gray-300 font-medium">
          {t("Settings.Sync.Status.Synced", "Settings synced")}
        </span>
      </div>
    )
  }

  const handleRequestClose = () => {
    if (hasUnsavedChanges()) {
      setShowDiscardConfirm(true)
    } else {
      onOpenChange(false)
    }
  }

  return (
    <>
      <Dialog
        open={isOpen}
        onOpenChange={(open) => {
          if (!open) {
            handleRequestClose()
          }
        }}
      >
        <DialogContent
          className="flex h-[90vh] max-h-255 w-full max-w-3xl flex-col overflow-x-hidden"
          onPointerDownOutside={(e) => {
            if (hasUnsavedChanges()) {
              e.preventDefault()
              setShowDiscardConfirm(true)
            }
          }}
          onEscapeKeyDown={(e) => {
            if (hasUnsavedChanges()) {
              e.preventDefault()
              setShowDiscardConfirm(true)
            }
          }}
        >
          <DialogHeader>
            <DialogTitle>{t("MainMenu.Extras.Settings")}</DialogTitle>
          </DialogHeader>

          <Tabs defaultValue="general" className="flex min-h-0 flex-1 flex-col">
            <TabsList className="w-full shrink-0">
              <TabsTrigger value="general" className="w-1/2">
                {t("Settings.General.Title")}
              </TabsTrigger>
              <TabsTrigger value="simulator" className="w-1/2">
                {t("Settings.Simulator.Title")}
              </TabsTrigger>
            </TabsList>

            <div className="text-muted-foreground mt-4 flex flex-row items-center gap-1 px-1 text-sm font-normal">
              <Badge variant="default" className="border-primary border">
                {t("General.NewFeature")}
              </Badge>
              <Trans
                i18nKey="Settings.ManageControllerNote"
                shouldUnescape={true}
                values={{
                  menuPath: `${t("MainMenu.Extras.Label")} > ${t("MainMenu.Extras.ManageControllers")}`,
                }}
                components={{
                  strong: <strong className="text-sm font-semibold" />,
                }}
              />
            </div>

            <TabsContent
              value="general"
              className="mt-4 flex min-h-0 flex-1 flex-col"
            >
              <div className="flex grow flex-row">
                <ScrollArea className="grow">
                  <div className="pr-3">
                    <GeneralSettingsCard
                      values={draftSettings}
                      onChange={updateSetting}
                    />
                  </div>
                </ScrollArea>
              </div>
            </TabsContent>

            <TabsContent
              value="simulator"
              className="mt-4 flex min-h-0 flex-1 flex-col"
            >
              <div className="flex grow flex-row">
                <ScrollArea className="grow">
                  <div className="pr-3">
                    <SimulatorSettingsCard
                      values={draftSettings}
                      onChange={updateSetting}
                    />
                  </div>
                </ScrollArea>
              </div>
            </TabsContent>
          </Tabs>

          <DialogFooter className="shrink-0 flex flex-row items-center justify-between sm:justify-between w-full">
            {renderSyncStatus()}
            <div className="flex items-center gap-2">
              <Button variant="secondary" onClick={handleRequestClose}>
                {t("Dialog.General.Cancel")}
              </Button>
              <Button onClick={handleSave}>
                {t("MainMenu.File.Save", "Save")}
              </Button>
            </div>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={showDiscardConfirm} onOpenChange={setShowDiscardConfirm}>
        <DialogContent className="sm:max-w-[425px]">
          <DialogHeader>
            <DialogTitle>{t("Settings.Discard.Title")}</DialogTitle>
            <DialogDescription>
              {t("Settings.Discard.Description")}
            </DialogDescription>
          </DialogHeader>

          <DialogFooter className="gap-2 sm:gap-0">
            <Button
              variant="secondary"
              onClick={() => setShowDiscardConfirm(false)}
            >
              {t("Settings.Discard.KeepEditing")}
            </Button>
            <Button
              variant="destructive"
              onClick={() => {
                setShowDiscardConfirm(false)
                onOpenChange(false)
              }}
            >
              {t("Settings.Discard.DiscardChanges")}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  )
}
