import useOpenUrl from "@/lib/hooks/useOpenUrl"
import { Trans, useTranslation } from "react-i18next"
import flitesim from "../assets/sponsors/flitesim-logo.png"
import moza from "../assets/sponsors/moza-logo.png"
import vkb from "../assets/sponsors/vkb-logo.png"
import wingflex from "../assets/sponsors/wingflex-logo.png"
import { Button } from "./ui/button"

const goldSponsors = [
  {
    name: "Flitesim",
    logo: flitesim,
    url: "https://flitesim.com/?ref=mobiflight",
    mask: "sponsor-logo-mask-flitesim",
  },
  {
    name: "Moza",
    logo: moza,
    url: "https://mozaracing.com/mobiflight",
    mask: "sponsor-logo-mask-moza",
  },
  {
    name: "VKB",
    logo: vkb,
    url: "https://vkb-sim.pro/?utm_source=mobiflight",
    mask: "sponsor-logo-mask-vkb",
  },
  {
    name: "WingFlex",
    logo: wingflex,
    url: "https://www.wingflex.com?sca_ref=11453765.OPCgaGgkUj",
    mask: "sponsor-logo-mask-wingflex",
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
              className={`gold-shimmer sponsor-logo-shimmer ${sponsor.mask}`}
            />
          </Button>
        ))}
      </div>
      <p className="animate-sponsor-tagline-fade-in text-xs leading-tight text-slate-300 opacity-0">
        <Trans
          i18nKey="Startup.GoldSponsors.Description"
          components={{
            gold: <span className="gold-shimmer text-shimmer" />,
          }}
        />
      </p>
    </div>
  )
}

export default GoldSponsorLogos
