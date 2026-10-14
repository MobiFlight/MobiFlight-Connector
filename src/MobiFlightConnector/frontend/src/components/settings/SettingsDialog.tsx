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
import {
  fetchCloudSettings,
  uploadCloudSettings,
} from "@/lib/cloudSettings"
import isEqual from "lodash-es/isEqual"

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
  const [isFetchingCloud, setIsFetchingCloud] = useState(false)
  const [isUploadingCloud, setIsUploadingCloud] = useState(false)
  const handleGetCloudSettings = async () => {
    try {
      setIsFetchingCloud(true)
      console.log("[SettingsDialog] Fetching cloud settings...")
      const cloudData = await fetchCloudSettings(auth)
      if (cloudData && cloudData.content) {
        console.log("[SettingsDialog] Cloud settings received:", cloudData)
        if (isEqual(draftSettings, cloudData.content)) {
          console.log("[SettingsDialog] Local settings are already up to date with cloud.")
        } else {
          console.log("[SettingsDialog] Difference detected. Applying cloud settings immediately...")
          const newSettings = cloudData.content as Settings
          setDraftSettings(newSettings)
          setPrevOriginalSettings(newSettings)
          setSettings(newSettings)
          messageExchange.publish({
            key: "CommandUpdateSettings",
            payload: {
              Settings: newSettings,
            },
          })
          console.log("[SettingsDialog] Cloud settings applied successfully.")
        }
      } else {
        console.log("[SettingsDialog] No cloud settings found.")
      }
    } catch (err: unknown) {
      console.error("[SettingsDialog] Failed to fetch cloud settings:", err)
    } finally {
      setIsFetchingCloud(false)
    }
  }

  const handleUploadCloudSettings = async () => {
    try {
      setIsUploadingCloud(true)
      console.log("[SettingsDialog] Uploading cloud settings...")
      const uploadPayload = {
        schemaVersion: 1,
        content: draftSettings,
      }
      const saved = await uploadCloudSettings(auth, uploadPayload)
      console.log("[SettingsDialog] Cloud settings uploaded successfully:", saved)
    } catch (err: unknown) {
      console.error("[SettingsDialog] Failed to upload cloud settings:", err)
    } finally {
      setIsUploadingCloud(false)
    }
  }

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

  const handleSave = () => {
    if (draftSettings) {
      setSettings(draftSettings as Settings)
      messageExchange.publish({
        key: "CommandUpdateSettings",
        payload: {
          Settings: draftSettings,
        },
      })
    }
    onOpenChange(false)
  }
  const [showDiscardConfirm, setShowDiscardConfirm] = useState(false)

  const hasUnsavedChanges = () => {
    if (!originalSettings) return Object.keys(draftSettings).length > 0
    return JSON.stringify(draftSettings) !== JSON.stringify(originalSettings)
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

          <DialogFooter className="shrink-0 gap-2 sm:gap-0 sm:justify-between">
            <div className="flex gap-2">
              <Button
                type="button"
                variant="outline"
                disabled={isFetchingCloud}
                onClick={handleGetCloudSettings}
              >
                {isFetchingCloud ? "Downloading..." : "Get Cloud Settings"}
              </Button>
              <Button
                type="button"
                variant="outline"
                disabled={isUploadingCloud}
                onClick={handleUploadCloudSettings}
              >
                {isUploadingCloud ? "Uploading..." : "Upload Cloud Settings"}
              </Button>
            </div>
            <div className="flex gap-2">
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
