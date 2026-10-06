import useOpenUrl from "@/lib/hooks/useOpenUrl"
import { fetchRemoteGoldSponsors } from "@/lib/sponsors"
import { Trans, useTranslation } from "react-i18next"
import { useQuery } from "@tanstack/react-query"
import { CSSProperties, useState } from "react"
import { Button } from "@/components/ui/button"
import type { GoldSponsor } from "@/types/sponsors"

const defaultGoldSponsorsUrl = "https://mobiflight.com/sponsors.json"

const MAX_SPONSORS_PER_ROW = 5

const createSponsorRows = (sponsors: GoldSponsor[]): GoldSponsor[][] => {
  if (sponsors.length === 0) {
    return []
  }

  const rowCount = Math.ceil(sponsors.length / MAX_SPONSORS_PER_ROW)

  const baseRowSize = Math.floor(sponsors.length / rowCount)

  const rowWithExtraSponsor = sponsors.length % rowCount

  const rows: GoldSponsor[][] = []

  let sponsorIndex = 0

  for (let rowIndex = 0; rowIndex < rowCount; rowIndex++) {
    const rowSize = baseRowSize + (rowIndex < rowWithExtraSponsor ? 1 : 0)

    rows.push(sponsors.slice(sponsorIndex, sponsorIndex + rowSize))
    sponsorIndex += rowSize
  }
  return rows
}

const GoldSponsorLogos = () => {
  const { t } = useTranslation()
  const openUrl = useOpenUrl()

  const [cycleIndex, setCycleIndex] = useState(0)

  const goldSponsorsUrl = (
    import.meta.env.VITE_GOLD_SPONSORS_URL ?? defaultGoldSponsorsUrl
  ).trim()

  const goldSponsorsQuery = useQuery({
    queryKey: ["goldSponsors", goldSponsorsUrl],
    queryFn: () => fetchRemoteGoldSponsors({ url: goldSponsorsUrl }),
    retry: false,
    refetchOnWindowFocus: false,
  })

  const goldSponsors = goldSponsorsQuery.data ?? []

  if (goldSponsors.length === 0) {
    return null
  }

  const sponsorRows = createSponsorRows(goldSponsors)

  const currentRow = sponsorRows[cycleIndex % sponsorRows.length]

  const showNextRow = () => {
    setCycleIndex((currentIndex) => currentIndex + 1)
  }

  return (
    <div className="mx-auto flex w-full max-w-4xl flex-col items-center gap-1 text-center select-none">
      <div className="flex h-16 w-full items-center justify-center">
        <div
          key={cycleIndex}
          className="animate-sponsor-row-cycle flex items-center justify-center gap-6 md:gap-8"
          onAnimationEnd={showNextRow}
        >
          {currentRow.map((sponsor) => (
            <Button
              key={sponsor.name}
              type="button"
              variant="ghost"
              disabled={!sponsor.href}
              aria-label={t("Startup.GoldSponsors.OpenSponsorLink", {
                sponsorName: sponsor.name,
              })}
              className="group/logo relative h-16 flex-1 p-0 hover:bg-transparent hover:text-inherit focus-visible:ring-amber-300 focus-visible:ring-offset-0"
              onClick={() => {
                if (sponsor.href) {
                  openUrl(sponsor.href)
                }
              }}
            >
              <img
                src={sponsor.logo}
                alt={t("Startup.GoldSponsors.LogoAlt", {
                  sponsorName: sponsor.name,
                })}
                className="max-h-16 w-full max-w-40 object-contain opacity-60 brightness-0 invert transition-opacity duration-700 ease-out group-hover/logo:opacity-0 lg:max-w-56"
              />
              <span
                aria-hidden="true"
                className="gold-shimmer pointer-events-none absolute inset-0 mask-(--sponsor-logo) mask-contain mask-center mask-no-repeat opacity-0 [-webkit-mask-image:var(--sponsor-logo)] [-webkit-mask-position:center] [-webkit-mask-repeat:no-repeat] [-webkit-mask-size:contain] group-hover/logo:bg-position-[200%_0] group-hover/logo:opacity-60 group-hover/logo:drop-shadow-[0_0_10px_rgba(251,191,36,0.8)]"
                style={
                  {
                    "--sponsor-logo": `url(${sponsor.logo})`,
                  } as CSSProperties
                }
              />
            </Button>
          ))}
        </div>
      </div>
      <p className="animate-sponsor-tagline-fade-in text-xs leading-tight text-slate-300">
        <Trans
          t={t}
          i18nKey="Startup.GoldSponsors.Description"
          components={{
            gold: (
              <span className="gold-shimmer bg-clip-text text-sm font-semibold tracking-wide text-transparent uppercase drop-shadow-[0_1px_2px_rgba(0,0,0,0.85)] hover:bg-position-[200%_0]" />
            ),
          }}
        />
      </p>
    </div>
  )
}

export default GoldSponsorLogos
