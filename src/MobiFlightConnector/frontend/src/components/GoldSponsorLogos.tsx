import useOpenUrl from "@/lib/hooks/useOpenUrl"
import { useAppMessage } from "@/lib/hooks/appMessage"
import { AppMessage, GoldSponsorsUpdate, GoldSponsor } from "@/types/messages"
import { Trans, useTranslation } from "react-i18next"
import { Button } from "./ui/button"
import { CSSProperties, useCallback, useState } from "react"

const GoldSponsorLogos = () => {
  const { t } = useTranslation()
  const openUrl = useOpenUrl()

  const [goldSponsors, setGoldSponsors] = useState<GoldSponsor[]>([])

  const handleGoldSponsorsUpdate = useCallback((message: AppMessage) => {
    const payload = message.payload as GoldSponsorsUpdate
    setGoldSponsors(payload.Sponsors ?? [])
  }, [])

  useAppMessage("GoldSponsorsUpdate", handleGoldSponsorsUpdate)

  if (goldSponsors.length === 0) {
    return null
  }

  return (
    <div className="mx-auto flex w-full max-w-4xl flex-col items-center gap-1 text-center select-none">
      <div className="grid w-full grid-cols-4 items-center gap-8 md:gap-12">
        {goldSponsors.map((sponsor, index) => (
          <Button
            key={sponsor.Name}
            type="button"
            variant="ghost"
            disabled={!sponsor.Url}
            aria-label={t("Startup.GoldSponsors.OpenSponsorLink", {
              sponsorName: sponsor.Name,
            })}
            className="animate-sponsor-fade-in group/logo relative h-16 w-full min-w-0 border-0 bg-transparent! p-0 opacity-0 shadow-none hover:bg-transparent! hover:text-inherit! focus-visible:ring-amber-300 focus-visible:ring-offset-0 active:bg-transparent!"
            style={{ animationDelay: `${index * 180}ms` }}
            onClick={() => {
              if (sponsor.Url) {
                openUrl(sponsor.Url)
              }
            }}
          >
            <img
              src={sponsor.LogoDataUri}
              alt={t("Startup.GoldSponsors.LogoAlt", {
                sponsorName: sponsor.Name,
              })}
              className="max-h-16 w-full max-w-56 object-contain opacity-90 brightness-0 invert transition-opacity duration-700 ease-out group-hover/logo:opacity-0"
            />
            <span
              aria-hidden="true"
              className="gold-shimmer pointer-events-none absolute inset-0 m-auto h-16 w-full max-w-56 mask-(--sponsor-logo) mask-contain mask-center mask-no-repeat opacity-0 transition-[background-position,opacity,filter] duration-1000 ease-out [-webkit-mask-image:var(--sponsor-logo)] [-webkit-mask-position:center] [-webkit-mask-repeat:no-repeat] [-webkit-mask-size:contain] group-hover/logo:bg-position-[200%_0] group-hover/logo:opacity-100 group-hover/logo:drop-shadow-[0_0_10px_rgba(251,191,36,0.8)]"
              style={
                {
                  "--sponsor-logo": `url(${sponsor.LogoDataUri})`,
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
