import { useCallback, useEffect, useRef, useState } from "react"
import { useAuth } from "react-oidc-context"
import { useSettingsStore } from "@/stores/settingsStore"
import Settings from "@/types/settings"
import {
  CloudSettingsPayload,
  fetchCloudSettings,
  uploadCloudSettings,
} from "@/lib/cloudSettings"
import messageExchange from "@/lib/messageExchange"
import { showSyncNotification } from "@/components/settings/SyncNotification"
import { cloudSyncStorage } from "@/lib/cloudSyncStorage"
import { isEqual } from "lodash-es"

export const useCloudSettingsSync = () => {
  const auth = useAuth()
  const {
    settings,
    setSettings,
    isConflictOpen,
    setIsConflictOpen,
    cloudSettings,
    setCloudSettings,
  } = useSettingsStore()

  const [isProcessing, setIsProcessing] = useState(false)

  const hasCheckedRef = useRef(false)
  const prevUserIdRef = useRef<string | undefined>(undefined)
  const cloudHashRef = useRef<string | undefined>(undefined)

  const userId =
    auth.user?.profile?.sub ||
    ((auth.user?.profile as Record<string, unknown> | undefined)?.oid as
      | string
      | undefined) ||
    auth.user?.id_token

  const isLoggedIn =
    !auth.isLoading && auth.isAuthenticated && Boolean(auth.user?.id_token)

  useEffect(() => {
    if (!auth.isLoading && !auth.isAuthenticated) {
      hasCheckedRef.current = false
      prevUserIdRef.current = undefined
      cloudHashRef.current = undefined
      setIsConflictOpen(false)
      setCloudSettings(null)
      cloudSyncStorage.clearSyncState()
    } else if (isLoggedIn && userId) {
      const storedUserId = cloudSyncStorage.getLastSyncedUserId()
      if (storedUserId && storedUserId !== userId) {
        hasCheckedRef.current = false
        cloudHashRef.current = undefined
        cloudSyncStorage.clearSyncDataForNewUser(userId)
      } else {
        cloudSyncStorage.setLastSyncedUserId(userId)
      }
      prevUserIdRef.current = userId
    }
  }, [
    auth.isLoading,
    auth.isAuthenticated,
    isLoggedIn,
    userId,
    setIsConflictOpen,
    setCloudSettings,
  ])

  const handleInitialUpload = useCallback(
    async (current: Settings) => {
      try {
        const uploadRes = await uploadCloudSettings(auth, {
          schemaVersion: 1,
          content: current,
        })
        if (uploadRes?.contentHash) {
          cloudSyncStorage.setLastSyncedHash(uploadRes.contentHash)
          cloudSyncStorage.setLastSyncedSettings(current)
        }
        showSyncNotification("saved")
      } catch {
        showSyncNotification("failed_upload")
      }
    },
    [auth],
  )

  const handleConflict = useCallback(
    (cloudContent: Partial<Settings>, cloudContentHash?: string) => {
      cloudHashRef.current = cloudContentHash
      setCloudSettings(cloudContent)
      setIsConflictOpen(true)
    },
    [setCloudSettings, setIsConflictOpen],
  )

  const handleAutoUpload = useCallback(
    async (
      current: Settings,
      baseHash: string,
      cloudContent?: Partial<Settings>,
      cloudContentHash?: string,
    ) => {
      try {
        const uploadRes = await uploadCloudSettings(auth, {
          schemaVersion: 1,
          baseHash,
          content: current,
        })
        if (uploadRes?.contentHash) {
          cloudSyncStorage.setLastSyncedHash(uploadRes.contentHash)
          cloudSyncStorage.setLastSyncedSettings(current)
        }
        showSyncNotification("saved")
      } catch (uploadErr: unknown) {
        const isConflict =
          (uploadErr as { status?: number })?.status === 409
        if (isConflict && cloudContent) {
          handleConflict(cloudContent, cloudContentHash)
        } else {
          showSyncNotification("failed_upload")
        }
      }
    },
    [auth, handleConflict],
  )

  const handleAutoDownload = useCallback(
    (current: Settings, cloudData: CloudSettingsPayload) => {
      const newSettings: Settings = { ...current, ...cloudData.content }
      setSettings(newSettings)
      messageExchange.publish({
        key: "CommandUpdateSettings",
        payload: {
          Settings: newSettings,
        },
      })
      cloudSyncStorage.setLastSyncedHash(cloudData.contentHash)
      cloudSyncStorage.setLastSyncedSettings(newSettings)
      showSyncNotification("restored")
    },
    [setSettings],
  )

  const triggerSyncCheck = useCallback(
    async (targetSettings?: Partial<Settings>) => {
      const current = (targetSettings || settings) as Settings | null
      if (!current || !isLoggedIn) return

      try {
        const cloudData = await fetchCloudSettings(auth)

        if (cloudData?.content == null) {
          await handleInitialUpload(current)
          return
        }

        const noCloudSettingsExistYet =
          Object.keys(cloudData.content).length === 0
        if (noCloudSettingsExistYet) {
          await handleInitialUpload(current)
          return
        }

        const localSettingsAndCloudAreSame = isEqual(
          current,
          cloudData.content,
        )
        if (localSettingsAndCloudAreSame) {
          if (cloudData.contentHash) {
            cloudSyncStorage.setLastSyncedHash(cloudData.contentHash)
            cloudSyncStorage.setLastSyncedSettings(current)
          }
          return
        }

        const lastSyncedHash = cloudSyncStorage.getLastSyncedHash()
        const lastSyncedSettings = cloudSyncStorage.getLastSyncedSettings()
        const localModified =
          !lastSyncedSettings || !isEqual(current, lastSyncedSettings)

        const cloudUnchangedSinceLastSync =
          lastSyncedHash !== null &&
          cloudData.contentHash === lastSyncedHash
        if (cloudUnchangedSinceLastSync && lastSyncedHash) {
          await handleAutoUpload(
            current,
            lastSyncedHash,
            cloudData.content,
            cloudData.contentHash,
          )
          return
        }

        const onlyCloudChangedRemotely =
          lastSyncedHash !== null &&
          !localModified &&
          cloudData.contentHash !== lastSyncedHash
        if (onlyCloudChangedRemotely) {
          handleAutoDownload(current, cloudData)
          return
        }

        handleConflict(cloudData.content, cloudData.contentHash)
      } catch {
        showSyncNotification("failed_fetch")
      }
    },
    [
      auth,
      isLoggedIn,
      settings,
      handleInitialUpload,
      handleAutoUpload,
      handleAutoDownload,
      handleConflict,
    ],
  )

  useEffect(() => {
    if (!isLoggedIn || hasCheckedRef.current || !settings) {
      return
    }

    if (!settings.CloudSettingsSyncEnabled) {
      return
    }

    hasCheckedRef.current = true
    triggerSyncCheck(settings)
  }, [isLoggedIn, settings, triggerSyncCheck])

  const handleKeepLocal = useCallback(() => {
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

  const handleUseCloud = useCallback(() => {
    if (!cloudSettings || !settings) return
    if (cloudHashRef.current) {
      cloudSyncStorage.setLastSyncedHash(cloudHashRef.current)
    }
    const newSettings: Settings = { ...settings, ...cloudSettings }
    cloudSyncStorage.setLastSyncedSettings(newSettings)
    setSettings(newSettings)
    messageExchange.publish({
      key: "CommandUpdateSettings",
      payload: {
        Settings: newSettings,
      },
    })
    setIsConflictOpen(false)
    setCloudSettings(null)
    showSyncNotification("restored")
  }, [
    cloudSettings,
    settings,
    setSettings,
    setIsConflictOpen,
    setCloudSettings,
  ])

  const handleUploadLocal = useCallback(async () => {
    if (!settings) return
    try {
      setIsProcessing(true)
      const lastSyncedHash = cloudSyncStorage.getLastSyncedHash()
      const uploadRes = await uploadCloudSettings(auth, {
        schemaVersion: 1,
        baseHash: lastSyncedHash || undefined,
        content: settings,
      })
      if (uploadRes?.contentHash) {
        cloudSyncStorage.setLastSyncedHash(uploadRes.contentHash)
        cloudSyncStorage.setLastSyncedSettings(settings)
      }
      setIsConflictOpen(false)
      setCloudSettings(null)
      showSyncNotification("synced")
    } catch {
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
