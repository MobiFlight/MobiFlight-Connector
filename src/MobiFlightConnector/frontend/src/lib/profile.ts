import { Profile } from "@/types/profile"
import { AuthContextProps } from "react-oidc-context"

interface FetchProfileOptions {
  signal?: AbortSignal
  timeoutMs?: number
}

const buildProfileUrl = (): string => {
  const profileEndpointBaseUrlDefault = "https://api.mobiflight.com/"
  const profileEndpointBaseUrl = import.meta.env.VITE_PROFILE_API_BASE_URL || profileEndpointBaseUrlDefault
  return `${profileEndpointBaseUrl.replace(/\/+$/, "")}/api/profile`
}

export const fetchProfile = async (
  auth: AuthContextProps,
  options: FetchProfileOptions = {},
): Promise<Profile> => {
  const timeoutMs = options.timeoutMs ?? 5000
  const profileUrl = buildProfileUrl()

  const timeoutController = new AbortController()
  const timeoutId = setTimeout(() => timeoutController.abort("timeout"), timeoutMs)
  const signal = options.signal
    ? AbortSignal.any([options.signal, timeoutController.signal])
    : timeoutController.signal

  try {
    const response = await fetch(profileUrl, {
      signal,
      cache: "no-store",
      headers: {
        "Authorization": `Bearer ${auth.user?.id_token ?? ""}`,
      },
    })

    if (response.status === 401) {
      throw new Error("Unauthorized: Invalid or expired token")
    }

    if (!response.ok) {
      throw new Error(`Profile request failed with status ${response.status}`)
    }

    return await response.json() as Profile

  } catch (error) {
    if (timeoutController.signal.aborted && !options.signal?.aborted) {
      throw new Error("Profile request timed out")
    }
    throw error
  } finally {
    clearTimeout(timeoutId)
  }
}