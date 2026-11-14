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
  const cloudHashRef = useRef<string | undefined>(undefined)

  const userId =
    auth.user?.profile?.sub ||
    (auth.user?.profile as Record<string, unknown> | undefined)?.oid as string | undefined ||
    auth.user?.id_token

  const isLoggedIn =
    !auth.isLoading && auth.isAuthenticated && Boolean(auth.user?.id_token)

  useEffect(() => {
    if (!isLoggedIn) {
      hasCheckedRef.current = false
      prevUserIdRef.current = undefined
      cloudHashRef.current = undefined
      setIsConflictOpen(false)
      setCloudSettings(null)
      localStorage.removeItem("cloud_last_synced_hash")
      localStorage.removeItem("cloud_last_synced_settings")
      localStorage.removeItem("cloud_last_synced_user_id")
    } else if (userId) {
      const storedUserId = localStorage.getItem("cloud_last_synced_user_id")
      if (storedUserId && storedUserId !== userId) {
        hasCheckedRef.current = false
        cloudHashRef.current = undefined
        localStorage.removeItem("cloud_last_synced_hash")
        localStorage.removeItem("cloud_last_synced_settings")
      }
      localStorage.setItem("cloud_last_synced_user_id", userId)
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
          try {
            const uploadRes = await uploadCloudSettings(auth, {
              schemaVersion: 1,
              content: current,
            })
            if (uploadRes?.contentHash) {
              localStorage.setItem("cloud_last_synced_hash", uploadRes.contentHash)
              localStorage.setItem(
                "cloud_last_synced_settings",
                JSON.stringify(current),
              )
            }
            console.log(
              "[useCloudSettingsSync] Initial settings uploaded to cloud successfully.",
            )
            showSyncNotification("saved")
          } catch (uploadErr) {
            console.error(
              "[useCloudSettingsSync] Failed to upload initial settings to cloud:",
              uploadErr,
            )
            showSyncNotification("failed_upload")
          }

          return
        }

        const lastSyncedHash = localStorage.getItem("cloud_last_synced_hash")
        const lastSyncedSettingsRaw = localStorage.getItem(
          "cloud_last_synced_settings",
        )
        const isMatch = isEqual(current, cloudData.content)

        // Case 2: Local settings are already identical to cloud settings
        if (isMatch) {
          if (cloudData.contentHash) {
            localStorage.setItem("cloud_last_synced_hash", cloudData.contentHash)
            localStorage.setItem(
              "cloud_last_synced_settings",
              JSON.stringify(current),
            )
          }
          console.log(
            "[useCloudSettingsSync] Local settings are already up to date with cloud.",
          )
          return
        }

        // Check if local settings were modified since last sync
        let localModified = false
        if (lastSyncedSettingsRaw) {
          try {
            const lastSyncedSettings = JSON.parse(lastSyncedSettingsRaw)
            localModified = !isEqual(current, lastSyncedSettings)
          } catch {
            localModified = true
          }
        }

        // Case 3: Cloud unchanged since last sync, only local changed (Auto-upload)
        if (lastSyncedHash && cloudData.contentHash === lastSyncedHash) {
          console.log(
            "[useCloudSettingsSync] Cloud unchanged since last sync. Uploading local changes...",
          )
          try {
            const uploadRes = await uploadCloudSettings(auth, {
              schemaVersion: 1,
              baseHash: lastSyncedHash,
              content: current,
            })
            if (uploadRes?.contentHash) {
              localStorage.setItem("cloud_last_synced_hash", uploadRes.contentHash)
              localStorage.setItem(
                "cloud_last_synced_settings",
                JSON.stringify(current),
              )
            }
            showSyncNotification("saved")
          } catch (uploadErr: unknown) {
            const isConflict =
              (uploadErr as { status?: number })?.status === 409
            if (isConflict) {
              console.log(
                "[useCloudSettingsSync] Server reported concurrent conflict. Prompting user...",
              )
              cloudHashRef.current = cloudData.contentHash
              setCloudSettings(cloudData.content)
              setIsConflictOpen(true)
            } else {
              console.error(
                "[useCloudSettingsSync] Failed to upload local changes:",
                uploadErr,
              )
              showSyncNotification("failed_upload")
            }
          }
          return
        }

        // Case 4: Only Cloud changed from another device, local is untouched (Auto-download)
        if (lastSyncedHash && !localModified && cloudData.contentHash !== lastSyncedHash) {
          console.log(
            "[useCloudSettingsSync] Cloud updated remotely and local is untouched. Auto-downloading...",
          )
          const newSettings: Settings = { ...current, ...cloudData.content }
          setSettings(newSettings)
          messageExchange.publish({
            key: "CommandUpdateSettings",
            payload: {
              Settings: newSettings,
            },
          })
          localStorage.setItem("cloud_last_synced_hash", cloudData.contentHash)
          localStorage.setItem(
            "cloud_last_synced_settings",
            JSON.stringify(newSettings),
          )
          showSyncNotification("restored")
          return
        }

        // Case 5: Both Local and Cloud changed (or no baseline hash exists) -> Conflict
        console.log(
          "[useCloudSettingsSync] Settings conflict detected. Prompting user...",
        )
        cloudHashRef.current = cloudData.contentHash
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
    if (cloudHashRef.current) {
      localStorage.setItem("cloud_last_synced_hash", cloudHashRef.current)
    }
    const newSettings: Settings = { ...settings, ...cloudSettings }
    localStorage.setItem(
      "cloud_last_synced_settings",
      JSON.stringify(newSettings),
    )
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
      const lastSyncedHash = localStorage.getItem("cloud_last_synced_hash")
      const uploadRes = await uploadCloudSettings(auth, {
        schemaVersion: 1,
        baseHash: lastSyncedHash || undefined,
        content: settings,
      })
      if (uploadRes?.contentHash) {
        localStorage.setItem("cloud_last_synced_hash", uploadRes.contentHash)
        localStorage.setItem(
          "cloud_last_synced_settings",
          JSON.stringify(settings),
        )
      }
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
