import Settings from "@/types/settings"
import { AuthContextProps } from "react-oidc-context"

export interface CloudSettingsPayload {
  _id?: string
  userId?: string
  schemaVersion: number
  revision: number
  contentHash: string
  content: Partial<Settings>
  lastAccess?: string | Date
}

export interface UploadCloudSettingsPayload {
  _id?: string
  schemaVersion?: number
  revision?: number
  contentHash: string
  content: Partial<Settings>
}

export interface CloudSettingsOptions {
  signal?: AbortSignal
  timeoutMs?: number
}

const buildSettingsUrl = (): string => {
  const settingsEndpointBaseUrlDefault = "https://api.mobiflight.com/"
  const settingsEndpointBaseUrl =
    import.meta.env.VITE_SETTINGS_API_BASE_URL || settingsEndpointBaseUrlDefault
  return `${settingsEndpointBaseUrl.replace(/\/+$/, "")}/api/settings`
}

/**
 * Computes a SHA-256 hash string for settings content to match contentHash.
 */
export const computeSettingsHash = async (
  content: Partial<Settings>,
): Promise<string> => {
  const sortedKeys = Object.keys(content).sort()
  const normalized: Record<string, unknown> = {}
  for (const key of sortedKeys) {
    normalized[key] = content[key as keyof Settings]
  }

  const jsonString = JSON.stringify(normalized)
  const encoder = new TextEncoder()
  const data = encoder.encode(jsonString)
  const hashBuffer = await crypto.subtle.digest("SHA-256", data)
  const hashArray = Array.from(new Uint8Array(hashBuffer))
  return hashArray.map((b) => b.toString(16).padStart(2, "0")).join("")
}

/**
 * Fetches user settings from the cloud server.
 * Returns null if no settings exist (404).
 */
export const fetchCloudSettings = async (
  auth: AuthContextProps,
  options: CloudSettingsOptions = {},
): Promise<CloudSettingsPayload | null> => {
  const timeoutMs = options.timeoutMs ?? 5000
  const settingsUrl = buildSettingsUrl()

  const timeoutController = new AbortController()
  const timeoutId = setTimeout(() => timeoutController.abort("timeout"), timeoutMs)
  const signal = options.signal
    ? AbortSignal.any([options.signal, timeoutController.signal])
    : timeoutController.signal

  try {
    const response = await fetch(settingsUrl, {
      method: "GET",
      signal,
      cache: "no-store",
      headers: {
        Authorization: `Bearer ${auth.user?.id_token ?? ""}`,
      },
    })

    if (response.status === 404) {
      return null
    }

    if (response.status === 401) {
      throw new Error("Unauthorized: Invalid or expired token")
    }

    if (!response.ok) {
      throw new Error(`Fetch settings request failed with status ${response.status}`)
    }

    return (await response.json()) as CloudSettingsPayload
  } catch (error) {
    if (timeoutController.signal.aborted && !options.signal?.aborted) {
      throw new Error("Fetch settings request timed out")
    }
    throw error
  } finally {
    clearTimeout(timeoutId)
  }
}

/**
 * Uploads user settings to the cloud server
 */
export const uploadCloudSettings = async (
  auth: AuthContextProps,
  payload: UploadCloudSettingsPayload,
  options: CloudSettingsOptions = {},
): Promise<CloudSettingsPayload> => {
  const timeoutMs = options.timeoutMs ?? 5000
  const settingsUrl = buildSettingsUrl()

  const timeoutController = new AbortController()
  const timeoutId = setTimeout(() => timeoutController.abort("timeout"), timeoutMs)
  const signal = options.signal
    ? AbortSignal.any([options.signal, timeoutController.signal])
    : timeoutController.signal

  const requestBody = {
    _id: payload._id,
    schemaVersion: payload.schemaVersion ?? 1,
    revision: payload.revision ?? 1,
    contentHash: payload.contentHash,
    content: payload.content,
  }

  try {
    const response = await fetch(settingsUrl, {
      method: "POST",
      signal,
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${auth.user?.id_token ?? ""}`,
      },
      body: JSON.stringify(requestBody),
    })

    if (response.status === 401) {
      throw new Error("Unauthorized: Invalid or expired token")
    }

    if (!response.ok) {
      throw new Error(`Upload settings request failed with status ${response.status}`)
    }

    return (await response.json()) as CloudSettingsPayload
  } catch (error) {
    if (timeoutController.signal.aborted && !options.signal?.aborted) {
      throw new Error("Upload settings request timed out")
    }
    throw error
  } finally {
    clearTimeout(timeoutId)
  }
}

/**
 * Deletes user settings from the cloud server.
 */
export const deleteCloudSettings = async (
  auth: AuthContextProps,
  options: CloudSettingsOptions = {},
): Promise<boolean> => {
  const timeoutMs = options.timeoutMs ?? 5000
  const settingsUrl = buildSettingsUrl()

  const timeoutController = new AbortController()
  const timeoutId = setTimeout(() => timeoutController.abort("timeout"), timeoutMs)
  const signal = options.signal
    ? AbortSignal.any([options.signal, timeoutController.signal])
    : timeoutController.signal

  try {
    const response = await fetch(settingsUrl, {
      method: "DELETE",
      signal,
      headers: {
        Authorization: `Bearer ${auth.user?.id_token ?? ""}`,
      },
    })

    if (response.status === 401) {
      throw new Error("Unauthorized: Invalid or expired token")
    }

    if (response.status === 404 || response.ok) {
      return true
    }

    throw new Error(`Delete settings request failed with status ${response.status}`)
  } catch (error) {
    if (timeoutController.signal.aborted && !options.signal?.aborted) {
      throw new Error("Delete settings request timed out")
    }
    throw error
  } finally {
    clearTimeout(timeoutId)
  }
}