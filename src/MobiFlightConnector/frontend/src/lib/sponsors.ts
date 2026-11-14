import type { GoldSponsor } from "@/types/sponsors"

export const fetchRemoteGoldSponsors = async (
  url: string,
): Promise<GoldSponsor[]> => {
  const response = await fetch(url)
  if (!response.ok) {
    throw new Error(`Failed to fetch gold sponsors: ${response.status}`)
  }
  const sponsors = (await response.json()) as GoldSponsor[]

  return sponsors.map((sponsor) => ({
    ...sponsor,
    logo: new URL(sponsor.logo, response.url || url).toString(),
  }))
}
