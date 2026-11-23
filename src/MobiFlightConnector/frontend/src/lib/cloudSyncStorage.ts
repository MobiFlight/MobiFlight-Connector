import Settings from "@/types/settings"

const STORAGE_KEYS = {
  LAST_SYNCED_HASH: "cloud_last_synced_hash",
  LAST_SYNCED_SETTINGS: "cloud_last_synced_settings",
  LAST_SYNCED_USER_ID: "cloud_last_synced_user_id",
} as const

export const cloudSyncStorage = {
  getLastSyncedHash: (): string | null => {
    return localStorage.getItem(STORAGE_KEYS.LAST_SYNCED_HASH)
  },

  setLastSyncedHash: (hash: string): void => {
    localStorage.setItem(STORAGE_KEYS.LAST_SYNCED_HASH, hash)
  },

  getLastSyncedSettings: (): Settings | null => {
    const raw = localStorage.getItem(STORAGE_KEYS.LAST_SYNCED_SETTINGS)
    if (!raw) return null
    try {
      return JSON.parse(raw) as Settings
    } catch {
      return null
    }
  },

  setLastSyncedSettings: (settings: Settings): void => {
    localStorage.setItem(
      STORAGE_KEYS.LAST_SYNCED_SETTINGS,
      JSON.stringify(settings),
    )
  },

  getLastSyncedUserId: (): string | null => {
    return localStorage.getItem(STORAGE_KEYS.LAST_SYNCED_USER_ID)
  },

  setLastSyncedUserId: (userId: string): void => {
    localStorage.setItem(STORAGE_KEYS.LAST_SYNCED_USER_ID, userId)
  },

  clearSyncState: (): void => {
    localStorage.removeItem(STORAGE_KEYS.LAST_SYNCED_HASH)
    localStorage.removeItem(STORAGE_KEYS.LAST_SYNCED_SETTINGS)
    localStorage.removeItem(STORAGE_KEYS.LAST_SYNCED_USER_ID)
  },

  clearSyncDataForNewUser: (newUserId: string): void => {
    localStorage.removeItem(STORAGE_KEYS.LAST_SYNCED_HASH)
    localStorage.removeItem(STORAGE_KEYS.LAST_SYNCED_SETTINGS)
    localStorage.setItem(STORAGE_KEYS.LAST_SYNCED_USER_ID, newUserId)
  },
}

export default cloudSyncStorage
