import useOpenUrl from "@/lib/hooks/useOpenUrl"
import { fetchRemoteGoldSponsors } from "@/lib/sponsors"
import { Trans, useTranslation } from "react-i18next"
import { useQuery } from "@tanstack/react-query"
import { CSSProperties } from "react"
import { Button } from "@/components/ui/button"

const defaultGoldSponsorsUrl = "https://mobiflight.com/sponsors.json"

const GoldSponsorLogos = () => {
  const { t } = useTranslation()
  const openUrl = useOpenUrl()

  const golsSponsorsUrl = (
    import.meta.env.VITE_GOLD_SPONSORS_URL ?? defaultGoldSponsorsUrl
  ).trim()

  const goldSponsorsQuery = useQuery({
    queryKey: ["goldSponsors", golsSponsorsUrl],
    queryFn: () => fetchRemoteGoldSponsors({ url: golsSponsorsUrl }),
    retry: false,
    refetchOnWindowFocus: false,
  })

  const goldSponsors = goldSponsorsQuery.data ?? []

  if (goldSponsors.length === 0) {
    return null
  }

  return (
    <div className="mx-auto flex w-full max-w-4xl flex-col items-center gap-1 text-center select-none">
      <div className="flex w-full flex-row items-center justify-center gap-6 md:gap-8">
        {goldSponsors.map((sponsor, index) => (
          <Button
            key={sponsor.name}
            type="button"
            variant="ghost"
            disabled={!sponsor.href}
            aria-label={t("Startup.GoldSponsors.OpenSponsorLink", {
              sponsorName: sponsor.name,
            })}
            className="animate-sponsor-fade-in group/logo relative h-16 min-w-0 flex-1 border-0 bg-transparent! p-0 opacity-0 shadow-none hover:bg-transparent! hover:text-inherit! focus-visible:ring-amber-300 focus-visible:ring-offset-0 active:bg-transparent!"
            style={{ animationDelay: `${index * 180}ms` }}
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
              className="max-h-16 w-full max-w-56 object-contain opacity-90 brightness-0 invert transition-opacity duration-700 ease-out group-hover/logo:opacity-0"
            />
            <span
              aria-hidden="true"
              className="gold-shimmer pointer-events-none absolute inset-0 m-auto h-16 w-full max-w-56 mask-(--sponsor-logo) mask-contain mask-center mask-no-repeat opacity-0 transition-[background-position,opacity,filter] duration-1000 ease-out [-webkit-mask-image:var(--sponsor-logo)] [-webkit-mask-position:center] [-webkit-mask-repeat:no-repeat] [-webkit-mask-size:contain] group-hover/logo:bg-position-[200%_0] group-hover/logo:opacity-100 group-hover/logo:drop-shadow-[0_0_10px_rgba(251,191,36,0.8)]"
              style={
                {
                  "--sponsor-logo": `url(${sponsor.logo})`,
                } as CSSProperties
              }
            />
          </Button>
        ))}
      </div>
      <p className="animate-sponsor-tagline-fade-in text-xs leading-tight text-slate-300 opacity-0">
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
