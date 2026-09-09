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
import { useTranslation } from "react-i18next"
import messageExchange from "@/lib/messageExchange"
import Settings from "@/types/settings"
import { ScrollArea } from "../ui/scroll-area"

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

  const [draftSettings, setDraftSettings] = useState<Partial<Settings>>(
    originalSettings || {},
  )

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
        payload: draftSettings,
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
          className="flex h-[90vh] max-h-[860px] w-full max-w-3xl flex-col overflow-x-hidden"
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

            <TabsContent
              value="general"
              className="mt-0 flex min-h-0 flex-1 flex-col"
            >
              <div className="flex flex-row grow">
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
              className="mt-0 flex min-h-0 flex-1 flex-col"
            >
              <div className="flex flex-row grow">
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

          <DialogFooter className="shrink-0 gap-2 sm:gap-0">
            <Button variant="secondary" onClick={handleRequestClose}>
              {t("General.Action.Cancel", "Cancel")}
            </Button>
            <Button onClick={handleSave}>
              {t("MainMenu.File.Save", "Save")}
            </Button>
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

