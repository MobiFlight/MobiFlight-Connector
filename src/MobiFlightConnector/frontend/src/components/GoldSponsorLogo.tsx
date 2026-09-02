import useOpenUrl from "@/lib/hooks/useOpenUrl"
import { Trans, useTranslation } from "react-i18next"
import flitesim from "../assets/sponsors/flitesim-logo.png"
import moza from "../assets/sponsors/moza-logo.png"
import vkb from "../assets/sponsors/vkb-logo.png"
import wingflex from "../assets/sponsors/wingflex-logo.png"

const goldSponsors = [
  {
    name: "Flitesim",
    logo: flitesim,
    url: "https://flitesim.com/?ref=mobiflight",
    delay: "0ms",
  },
  {
    name: "Moza",
    logo: moza,
    url: "https://mozaracing.com/mobiflight",
    delay: "180ms",
  },
  {
    name: "VKB",
    logo: vkb,
    url: "https://vkb-sim.pro/?utm_source=mobiflight",
    delay: "360ms",
  },
  {
    name: "WingFlex",
    logo: wingflex,
    url: "https://www.wingflex.com?sca_ref=11453765.OPCgaGgkUj",
    delay: "540ms",
  },
]

const GoldSponsorLogo = () => {
  const { t } = useTranslation()
  const openUrl = useOpenUrl()

  return (
    <div className="flex w-full flex-col items-center gap-1 text-center">
      <div className="grid w-full grid-cols-4 items-center gap-12">
        {goldSponsors.map((sponsor) => (
          <button
            key={sponsor.name}
            type="button"
            aria-label={t("Startup.GoldSponsors.OpenSponsorLink", {
              sponsorName: sponsor.name,
            })}
            className="animate-sponsor-fade-in flex h-16 min-w-0 items-center justify-center"
            style={{ animationDelay: sponsor.delay }}
            onClick={() => openUrl(sponsor.url)}
          >
            <img
              src={sponsor.logo}
              alt={t("Startup.GoldSponsors.LogoAlt", {
                sponsorName: sponsor.name,
              })}
              className="max-h-16 w-full max-w-56 object-contain opacity-90 brightness-0 invert"
            />
          </button>
        ))}
      </div>
      <p className="text-xs leading-tight text-slate-300">
        <Trans
          i18nKey="Startup.GoldSponsors.Description"
          components={{
            gold: (
              <span className="bg-[linear-gradient(90deg,#fbbf24_0%,#fde68a_35%,#ffffff_50%,#fde68a_65%,#fbbf24_100%)] bg-size-[250%_100%] bg-clip-text bg-position-[-100%_0] text-sm font-semibold tracking-wide text-transparent uppercase drop-shadow-[0_1px_2px_rgba(0,0,0,0.85)] transition-[background-position] duration-1000 ease-out hover:bg-position-[200%_0]" />
            ),
          }}
        />
      </p>
    </div>
  )
}

export default GoldSponsorLogo
