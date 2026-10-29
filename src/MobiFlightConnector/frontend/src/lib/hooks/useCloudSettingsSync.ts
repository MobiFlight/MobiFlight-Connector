import { useEffect, useRef, useState, useCallback } from "react"
import { useAuth } from "react-oidc-context"
import { fetchCloudSettings, uploadCloudSettings } from "@/lib/cloudSettings"
import messageExchange from "@/lib/messageExchange"
import Settings from "@/types/settings"
import { useSettingsStore } from "@/stores/settingsStore"
import isEqual from "lodash-es/isEqual"
import { showSyncNotification } from "@/components/settings/SyncNotification"

export const useCloudSettingsSync = () => {
  const auth = useAuth()
  const settings = useSettingsStore((state) => state.settings)
  const setSettings = useSettingsStore((state) => state.setSettings)

  const isConflictOpen = useSettingsStore((state) => state.isConflictOpen)
  const setIsConflictOpen = useSettingsStore((state) => state.setIsConflictOpen)
  const cloudSettings = useSettingsStore((state) => state.cloudSettings)
  const setCloudSettings = useSettingsStore((state) => state.setCloudSettings)

  const [isProcessing, setIsProcessing] = useState(false)

  const hasCheckedRef = useRef(false)
  const prevUserIdRef = useRef<string | undefined>(undefined)

  const userId =
    auth.user?.profile?.sub ||
    (auth.user?.profile as Record<string, unknown> | undefined)?.oid as string | undefined ||
    auth.user?.id_token

  const isLoggedIn =
    !auth.isLoading && auth.isAuthenticated && Boolean(auth.user?.id_token)

  // Reset sync check flag when user logs out or switches accounts
  useEffect(() => {
    if (!isLoggedIn) {
      hasCheckedRef.current = false
      prevUserIdRef.current = undefined
      setIsConflictOpen(false)
      setCloudSettings(null)
    } else if (prevUserIdRef.current !== userId) {
      hasCheckedRef.current = false
      prevUserIdRef.current = userId
    }
  }, [isLoggedIn, userId, setIsConflictOpen, setCloudSettings])

  // Trigger sync check (can be called manually from SettingsDialog or on login)
  const triggerSyncCheck = useCallback(
    async (targetSettings?: Partial<Settings>) => {
      const current = (targetSettings || settings) as Settings | null
      if (!current || !isLoggedIn) return

      try {
        console.log("[useCloudSettingsSync] Checking cloud settings...")
        const cloudData = await fetchCloudSettings(auth)

        // Case 1: No cloud settings exist yet (New user / initial upload)
        if (
          !cloudData ||
          !cloudData.content ||
          Object.keys(cloudData.content).length === 0
        ) {
          console.log(
            "[useCloudSettingsSync] No cloud settings found. Performing initial upload...",
          )
          await uploadCloudSettings(auth, {
            schemaVersion: 1,
            content: current,
          })
          console.log(
            "[useCloudSettingsSync] Initial settings uploaded to cloud successfully.",
          )
          showSyncNotification("saved")
          return
        }

        // Case 2: Local settings are already identical to cloud settings
        const isMatch = isEqual(current, cloudData.content)
        if (isMatch) {
          console.log(
            "[useCloudSettingsSync] Local settings are already up to date with cloud.",
          )
          return
        }

        // Case 3: Conflict detected -> prompt user with 3 options
        console.log(
          "[useCloudSettingsSync] Settings conflict detected. Prompting user...",
        )
        setCloudSettings(cloudData.content)
        setIsConflictOpen(true)
      } catch (err: unknown) {
        console.error(
          "[useCloudSettingsSync] Error during cloud settings sync check:",
          err,
        )
        showSyncNotification("failed_fetch")
      }
    },
    [auth, isLoggedIn, settings, setCloudSettings, setIsConflictOpen],
  )

  // Perform background sync check on login
  useEffect(() => {
    // Requirements: user must be logged in, not checked yet, and settings must be loaded
    if (!isLoggedIn || hasCheckedRef.current || !settings) {
      return
    }

    // Do not sync if cloud sync is disabled
    if (!settings.CloudSettingsSyncEnabled) {
      return
    }

    hasCheckedRef.current = true
    triggerSyncCheck(settings)
  }, [isLoggedIn, settings, triggerSyncCheck])

  // Option 1: Keep local settings without syncing -> disable CloudSettingsSyncEnabled
  const handleKeepLocal = useCallback(() => {
    console.log("[useCloudSettingsSync] User chose to keep local settings.")
    if (settings) {
      const updatedSettings: Settings = {
        ...settings,
        CloudSettingsSyncEnabled: false,
      }
      setSettings(updatedSettings)
      messageExchange.publish({
        key: "CommandUpdateSettings",
        payload: {
          Settings: updatedSettings,
        },
      })
    }
    setIsConflictOpen(false)
    setCloudSettings(null)
  }, [settings, setSettings, setIsConflictOpen, setCloudSettings])

  // Option 2: Use cloud settings (replace local with cloud)
  const handleUseCloud = useCallback(() => {
    if (!cloudSettings || !settings) return
    console.log(
      "[useCloudSettingsSync] User chose to use cloud settings. Applying...",
    )
    const newSettings: Settings = { ...settings, ...cloudSettings }
    setSettings(newSettings)
    messageExchange.publish({
      key: "CommandUpdateSettings",
      payload: {
        Settings: newSettings,
      },
    })
    console.log("[useCloudSettingsSync] Cloud settings applied locally.")
    setIsConflictOpen(false)
    setCloudSettings(null)
    showSyncNotification("restored")
  }, [cloudSettings, settings, setSettings, setIsConflictOpen, setCloudSettings])

  // Option 3: Upload local to cloud (overwrite cloud settings)
  const handleUploadLocal = useCallback(async () => {
    if (!settings) return
    try {
      setIsProcessing(true)
      console.log(
        "[useCloudSettingsSync] User chose to overwrite cloud with local settings. Uploading...",
      )
      await uploadCloudSettings(auth, {
        schemaVersion: 1,
        content: settings,
      })
      console.log(
        "[useCloudSettingsSync] Cloud settings overwritten with local settings successfully.",
      )
      setIsConflictOpen(false)
      setCloudSettings(null)
      showSyncNotification("synced")
    } catch (err: unknown) {
      console.error(
        "[useCloudSettingsSync] Failed to upload local settings to cloud:",
        err,
      )
      showSyncNotification("failed_upload")
    } finally {
      setIsProcessing(false)
    }
  }, [auth, settings, setIsConflictOpen, setCloudSettings])

  return {
    isConflictOpen,
    setIsConflictOpen,
    cloudSettings,
    isProcessing,
    handleKeepLocal,
    handleUseCloud,
    handleUploadLocal,
    triggerSyncCheck,
  }
}

export default useCloudSettingsSync
