import type { GoldSponsor } from "@/types/sponsors"

interface FetchRemoteGoldSponsorsOptions {
  url: string
  timeoutMs?: number
}

interface RemoteGoldSponsorsPayload {
  sponsors: GoldSponsor[]
}

const isGoldSponsor = (value: unknown): value is GoldSponsor => {
  if (typeof value !== "object" || !value) {
    return false
  }
  const sponsor = value as Record<string, unknown>
  return (
    typeof sponsor.name === "string" &&
    typeof sponsor.logo === "string" &&
    (typeof sponsor.href === "string" || sponsor.href === null)
  )
}

const isRemoteGoldSponsorsPayload = (
  value: unknown,
): value is RemoteGoldSponsorsPayload => {
  if (typeof value !== "object" || !value) {
    return false
  }
  const payload = value as Record<string, unknown>
  if (!Array.isArray(payload.sponsors)) {
    return false
  }
  return payload.sponsors.every((sponsor) => isGoldSponsor(sponsor))
}

const resolveLogoUrl = (logo: string, responseUrl: string): string => {
  const url = new URL(logo, responseUrl)
  if (url.protocol !== "http:" && url.protocol !== "https:") {
    throw new Error(`Invalid logo URL protocol: ${url.protocol}`)
  }
  return url.toString()
}

const normalizeSponsorHref = (href: string | null): string | null => {
  if (!href) {
    return null
  }
  try {
    const url = new URL(href)
    if (url.protocol !== "http:" && url.protocol !== "https:") {
      return null
    }
    return url.toString()
  } catch {
    return null
  }
}

export const fetchRemoteGoldSponsors = async ({
  url,
  timeoutMs = 3000,
}: FetchRemoteGoldSponsorsOptions): Promise<GoldSponsor[]> => {
  const controller = new AbortController()
  const timeoutId = setTimeout(() => controller.abort(), timeoutMs)

  try {
    const response = await fetch(url, {
      signal: controller.signal,
      cache: "no-store",
    })
    if (!response.ok) {
      throw new Error(
        `Failed to fetch gold sponsors: ${response.status} ${response.statusText}`,
      )
    }

    const payload = (await response.json()) as unknown

    if (!isRemoteGoldSponsorsPayload(payload)) {
      throw new Error("Invalid gold sponsors payload")
    }

    return payload.sponsors.map((sponsor) => ({
      name: sponsor.name.trim(),
      logo: resolveLogoUrl(sponsor.logo, response.url || url),
      href: normalizeSponsorHref(sponsor.href),
    }))
  } finally {
    clearTimeout(timeoutId)
  }
}
