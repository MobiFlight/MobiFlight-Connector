import useOpenUrl from "@/lib/hooks/useOpenUrl"
import { Trans, useTranslation } from "react-i18next"
import flitesim from "../assets/sponsors/flitesim-logo.png"
import moza from "../assets/sponsors/moza-logo.png"
import vkb from "../assets/sponsors/vkb-logo.png"
import wingflex from "../assets/sponsors/wingflex-logo.png"
import { Button } from "./ui/button"
import { CSSProperties } from "react"

const goldSponsors = [
  {
    name: "Flitesim",
    logo: flitesim,
    url: "https://flitesim.com/?ref=mobiflight",
  },
  {
    name: "Moza",
    logo: moza,
    url: "https://mozaracing.com/mobiflight",
  },
  {
    name: "VKB",
    logo: vkb,
    url: "https://vkb-sim.pro/?utm_source=mobiflight",
  },
  {
    name: "WingFlex",
    logo: wingflex,
    url: "https://www.wingflex.com?sca_ref=11453765.OPCgaGgkUj",
  },
]

const GoldSponsorLogos = () => {
  const { t } = useTranslation()
  const openUrl = useOpenUrl()

  return (
    <div className="mx-auto flex w-full max-w-4xl flex-col items-center gap-1 text-center select-none">
      <div className="grid w-full grid-cols-4 items-center gap-8 md:gap-12">
        {goldSponsors.map((sponsor, index) => (
          <Button
            key={sponsor.name}
            type="button"
            variant="ghost"
            aria-label={t("Startup.GoldSponsors.OpenSponsorLink", {
              sponsorName: sponsor.name,
            })}
            className="animate-sponsor-fade-in group/logo relative h-16 w-full min-w-0 border-0 bg-transparent! p-0 opacity-0 shadow-none hover:bg-transparent! hover:text-inherit! focus-visible:ring-amber-300 focus-visible:ring-offset-0 active:bg-transparent!"
            style={{ animationDelay: `${index * 180}ms` }}
            onClick={() => openUrl(sponsor.url)}
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
