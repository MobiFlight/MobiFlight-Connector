import type { GoldSponsor } from "@/types/sponsors"

type RemoteGoldSponsorData = {
  sponsors: GoldSponsor[]
}

export const fetchRemoteGoldSponsors = async (
  url: string,
): Promise<GoldSponsor[]> => {
  const response = await fetch(url)
  if (!response.ok) {
    throw new Error(`Failed to fetch gold sponsors: ${response.status}`)
  }
  const data = (await response.json()) as RemoteGoldSponsorData

  return data.sponsors.map((sponsor) => ({
    ...sponsor,
    logo: new URL(sponsor.logo, response.url || url).toString(),
  }))
}
