import flitesim from "../assets/sponsors/flitesim-logo.png"
import moza from "../assets/sponsors/moza-logo.png"
import vkb from "../assets/sponsors/vkb-logo.png"
import wingflex from "../assets/sponsors/wingflex-logo.png"

const goldSponsors = [
  {
    name: "Flitesim",
    logo: flitesim,
  },
  {
    name: "Moza",
    logo: moza,
  },
  {
    name: "VKB",
    logo: vkb,
  },
  {
    name: "WingFlex",
    logo: wingflex,
  },
]

const GoldSponsorLogos = () => {
  return (
    <div className="mx-auto flex w-full max-w-6xl flex-col items-center gap-4 text-center">
      <div className="space-y-1">
        <p className="text-xs font-semibold text-slate-300 uppercase">
          Supported by our Gold Sponsors
        </p>
        <p className="text-xs text-slate-300">
          Thank you for helping keep Mobiflight alive!
        </p>
      </div>

      <div className="flex w-full flex-nowrap items-center justify-center gap-12">
        {goldSponsors.map((sponsor) => (
          <img
            key={sponsor.name}
            src={sponsor.logo}
            alt={`${sponsor.name} logo`}
            className="max-h-16 max-w-56 object-contain opacity-90"
          />
        ))}
      </div>
    </div>
  )
}

export default GoldSponsorLogos
