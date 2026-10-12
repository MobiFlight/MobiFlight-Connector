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

const buildSettingsUrl = (path: string = ""): string => {
  const settingsEndpointBaseUrlDefault = "https://api.mobiflight.com/"
  const settingsEndpointBaseUrl =
    import.meta.env.VITE_SETTINGS_API_BASE_URL || settingsEndpointBaseUrlDefault
  const cleanBase = settingsEndpointBaseUrl.replace(/\/+$/, "")
  const cleanPath = path ? `/${path.replace(/^\/+/, "")}` : ""
  return `${cleanBase}/api/settings${cleanPath}`
}

/**
 * Resolves the Authorization header from localStorage.
 */
export const resolveAuthHeader = (auth?: AuthContextProps): string => {
  const localAuth =
    localStorage.getItem("authorization") ||
    (localStorage as unknown as Record<string, string>).authorization
  if (localAuth) {
    return localAuth.startsWith("Bearer ") ? localAuth : `Bearer ${localAuth}`
  }
  return auth?.user?.id_token ? `Bearer ${auth.user.id_token}` : ""
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
 * Returns null if no settings exist (404 or empty list).
 */
export const fetchCloudSettings = async (
  auth?: AuthContextProps,
  options: CloudSettingsOptions = {},
): Promise<CloudSettingsPayload | null> => {
  const timeoutMs = options.timeoutMs ?? 5000
  const settingsUrl = buildSettingsUrl()
  const authHeader = resolveAuthHeader(auth)

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
        Authorization: authHeader,
      },
    })

    if (response.status === 404) {
      return null
    }

    if (response.status === 401) {
      let serverError = ""
      try {
        const errJson = await response.json()
        serverError = JSON.stringify(errJson)
      } catch {
        // ignore
      }
      throw new Error(`Unauthorized (401): ${serverError || "Invalid or expired token"}`)
    }

    if (!response.ok) {
      throw new Error(`Fetch settings request failed with status ${response.status}`)
    }

    const data = await response.json()
    if (Array.isArray(data)) {
      if (data.length === 0) {
        return null
      }
      const sorted = [...data].sort((a, b) => (b.revision ?? 1) - (a.revision ?? 1))
      return sorted[0] as CloudSettingsPayload
    }

    return data as CloudSettingsPayload
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
 * Uploads user settings to the cloud server (POST to create, PUT to update if _id provided)
 */
export const uploadCloudSettings = async (
  auth: AuthContextProps | undefined,
  payload: UploadCloudSettingsPayload,
  options: CloudSettingsOptions = {},
): Promise<CloudSettingsPayload> => {
  const timeoutMs = options.timeoutMs ?? 5000
  const isUpdate = Boolean(payload._id)
  const settingsUrl = isUpdate
    ? buildSettingsUrl(encodeURIComponent(payload._id!))
    : buildSettingsUrl()

  const method = isUpdate ? "PUT" : "POST"
  const authHeader = resolveAuthHeader(auth)

  const timeoutController = new AbortController()
  const timeoutId = setTimeout(() => timeoutController.abort("timeout"), timeoutMs)
  const signal = options.signal
    ? AbortSignal.any([options.signal, timeoutController.signal])
    : timeoutController.signal

  const requestBody = {
    schemaVersion: payload.schemaVersion ?? 1,
    revision: payload.revision ?? 1,
    contentHash: payload.contentHash,
    content: payload.content,
  }

  try {
    const response = await fetch(settingsUrl, {
      method,
      signal,
      headers: {
        "Content-Type": "application/json",
        Authorization: authHeader,
      },
      body: JSON.stringify(requestBody),
    })

    if (response.status === 401) {
      let serverError = ""
      try {
        const errJson = await response.json()
        serverError = JSON.stringify(errJson)
      } catch {
        // ignore
      }
      throw new Error(`Unauthorized (401): ${serverError || "Invalid or expired token"}`)
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
  auth?: AuthContextProps,
  options: CloudSettingsOptions = {},
): Promise<boolean> => {
  const timeoutMs = options.timeoutMs ?? 5000
  const settingsUrl = buildSettingsUrl()
  const authHeader = resolveAuthHeader(auth)

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
        Authorization: authHeader,
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