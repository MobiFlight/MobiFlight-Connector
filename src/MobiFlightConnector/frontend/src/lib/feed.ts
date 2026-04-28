import { CommunityPost } from "@/types/feed"

interface FetchRemoteFeedOptions {
  baseUrl: string
  language: string
  timeoutMs?: number
}

interface RemoteFeedPayload {
  community: CommunityPost[]
}

const isStringArray = (value: unknown): value is string[] => {
  return Array.isArray(value) && value.every((item) => typeof item === "string")
}

const isCommunityPost = (value: unknown): value is CommunityPost => {
  if (!value || typeof value !== "object") {
    return false
  }
  const post = value as Record<string, unknown>
  return (
    typeof post.title === "string" &&
    typeof post.date === "string" &&
    isStringArray(post.content) &&
    isStringArray(post.tags)
  )
}

const isRemoteFeedPayload = (value: unknown): value is RemoteFeedPayload => {
  if (!value || typeof value !== "object") {
    return false
  }
  const payload = value as Record<string, unknown>
  if (!Array.isArray(payload.community)) {
    return false
  }
  return payload.community.every((post) => isCommunityPost(post))
}

const resolveLanguage = (language: string): string => {
  const [languageCode] = language.split("-")
  return languageCode || "en"
}

const buildRemoteFeedUrl = (baseUrl: string, language: string): string => {
  const normalizedBaseUrl = baseUrl.replace(/\/+$/, "")
  return `${normalizedBaseUrl}/${resolveLanguage(language)}/feed.json`
}

export const fetchRemoteCommunityFeed = async ({
  baseUrl,
  language,
  timeoutMs = 3000,
}: FetchRemoteFeedOptions): Promise<CommunityPost[]> => {
  const controller = new AbortController()
  const timeoutId = setTimeout(() => controller.abort(), timeoutMs)
  const url = buildRemoteFeedUrl(baseUrl, language)
  try {
    const response = await fetch(url, {
      signal: controller.signal,
      cache: "no-store",
    })
    if (!response.ok) {
      throw new Error(`Feed request failed with status ${response.status}`)
    }
    const payload = (await response.json()) as unknown
    if (!isRemoteFeedPayload(payload)) {
      throw new Error("Feed payload is invalid")
    }
    return payload.community
  } finally {
    clearTimeout(timeoutId)
  }
}
