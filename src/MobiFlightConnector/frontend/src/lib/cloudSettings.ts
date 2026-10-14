import Settings from "@/types/settings"
import { AuthContextProps } from "react-oidc-context"

export const DEFAULT_SCHEMA_VERSION = 1

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
  schemaVersion?: number
  content: Partial<Settings>
}

export interface CloudSettingsOptions {
  signal?: AbortSignal
  timeoutMs?: number
  schemaVersion?: number
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
 * Fetches user settings for a given schemaVersion from the cloud server.
 * Returns null if no settings exist (404).
 */
export const fetchCloudSettings = async (
  auth?: AuthContextProps,
  options: CloudSettingsOptions = {},
): Promise<CloudSettingsPayload | null> => {
  const timeoutMs = options.timeoutMs ?? 5000
  const schemaVersion = options.schemaVersion ?? DEFAULT_SCHEMA_VERSION
  const settingsUrl = buildSettingsUrl(schemaVersion.toString())
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
 * Uploads/upserts user settings for a schemaVersion to the cloud server.
 */
export const uploadCloudSettings = async (
  auth: AuthContextProps | undefined,
  payload: UploadCloudSettingsPayload,
  options: CloudSettingsOptions = {},
): Promise<CloudSettingsPayload> => {
  const timeoutMs = options.timeoutMs ?? 5000
  const schemaVersion = payload.schemaVersion ?? DEFAULT_SCHEMA_VERSION
  const settingsUrl = buildSettingsUrl(schemaVersion.toString())
  const authHeader = resolveAuthHeader(auth)

  const timeoutController = new AbortController()
  const timeoutId = setTimeout(() => timeoutController.abort("timeout"), timeoutMs)
  const signal = options.signal
    ? AbortSignal.any([options.signal, timeoutController.signal])
    : timeoutController.signal

  const requestBody = {
    schemaVersion,
    content: payload.content,
  }

  try {
    const response = await fetch(settingsUrl, {
      method: "PUT",
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
 * Deletes user settings for a given schemaVersion from the cloud server.
 */
export const deleteCloudSettings = async (
  auth?: AuthContextProps,
  schemaVersion: number = DEFAULT_SCHEMA_VERSION,
  options: CloudSettingsOptions = {},
): Promise<boolean> => {
  const timeoutMs = options.timeoutMs ?? 5000
  const settingsUrl = buildSettingsUrl(schemaVersion.toString())
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