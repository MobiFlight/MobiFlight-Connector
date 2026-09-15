import { useTranslation } from "react-i18next"
import {
  IconBrandDiscordFilled,
  IconBrandGithubFilled,
  IconBrandYoutubeFilled,
  IconBuildingStore,
  IconExternalLink,
  IconMail,
  IconWorld,
} from "@tabler/icons-react"
import IconBrandHubHopLogo from "@/components/icons/IconBrandHubHopLogo"
import useOpenUrl from "@/lib/hooks/useOpenUrl"
import { Card } from "@/components/ui/card"

export default function AboutCard() {
  const { t } = useTranslation()
  const openUrl = useOpenUrl()

  const contactItems = [
    {
      title: t("About.Contact.Discord.Title", "Discord"),
      display: "https://mobiflight.com/discord",
      url: "https://mobiflight.com/discord",
      icon: <IconBrandDiscordFilled className="size-6 fill-[#5865F2]" />,
    },
    {
      title: t("About.Contact.Email.Title", "Email"),
      display: "info@mobiflight.com",
      url: "mailto:info@mobiflight.com",
      icon: <IconMail className="size-6 text-sky-500" />,
    },
  ]

  const ecosystemItems = [
    {
      title: "Website",
      display: "mobiflight.com",
      url: "https://www.mobiflight.com",
      icon: <IconWorld className="size-6 text-blue-500" />,
    },
    {
      title: "GitHub",
      display: "github.com/MobiFlight",
      url: "https://github.com/MobiFlight",
      icon: <IconBrandGithubFilled className="size-6" />,
    },
    {
      title: "Shop",
      display: "shop.mobiflight.com",
      url: "https://shop.mobiflight.com",
      icon: <IconBuildingStore className="size-6 text-emerald-500" />,
    },
    {
      title: "YouTube",
      display: "youtube.com/@MobiFlight",
      url: "https://www.youtube.com/c/MobiFlight",
      icon: <IconBrandYoutubeFilled className="size-6 fill-red-500" />,
    },
    {
      title: "HubHop",
      display: "hubhop.mobiflight.com",
      url: "https://hubhop.mobiflight.com",
      icon: <IconBrandHubHopLogo className="size-6 fill-orange-500" />,
    },
  ]

  return (
    <div className="p-4">
      {/* Single master card without any nested cards inside */}
      <Card className="w-full border border-border/70 bg-card/60 overflow-hidden shadow-xs">
        {/* Contact Section Header */}
        <div className="bg-muted/30 px-4 py-2 border-b">
          <h3 className="text-xs font-semibold uppercase tracking-wider text-muted-foreground">
            {t("About.Contact.Title", "How to contact us")}
          </h3>
        </div>

        {/* Contact Rows */}
        <div className="divide-y divide-border/50">
          {contactItems.map((item) => (
            <button
              type="button"
              key={item.title}
              className="group flex w-full items-center justify-between px-4 py-2.5 transition-colors hover:bg-muted/40 cursor-pointer text-left"
              onClick={() => openUrl(item.url)}
            >
              <div className="flex items-center gap-3">
                <div className="flex size-7 items-center justify-center">
                  {item.icon}
                </div>
                <span className="text-sm font-medium text-foreground">
                  {item.title}
                </span>
              </div>
              <div className="flex items-center gap-1.5 text-xs sm:text-sm text-primary group-hover:underline">
                <span>{item.display}</span>
                <IconExternalLink className="size-3.5 opacity-60 transition-opacity group-hover:opacity-100" />
              </div>
            </button>
          ))}
        </div>

        {/* Ecosystem Section Header */}
        <div className="bg-muted/30 px-4 py-2 border-t border-b">
          <h3 className="text-xs font-semibold uppercase tracking-wider text-muted-foreground">
            {t("About.Ecosystem.Title", "MobiFlight Ecosystem")}
          </h3>
        </div>

        {/* Ecosystem Rows */}
        <div className="divide-y divide-border/50">
          {ecosystemItems.map((item) => (
            <button
              type="button"
              key={item.title}
              className="group flex w-full items-center justify-between px-4 py-2.5 transition-colors hover:bg-muted/40 cursor-pointer text-left"
              onClick={() => openUrl(item.url)}
            >
              <div className="flex items-center gap-3">
                <div className="flex size-7 items-center justify-center">
                  {item.icon}
                </div>
                <span className="text-sm font-medium text-foreground">
                  {item.title}
                </span>
              </div>
              <div className="flex items-center gap-1.5 text-xs sm:text-sm text-primary group-hover:underline">
                <span>{item.display}</span>
                <IconExternalLink className="size-3.5 opacity-60 transition-opacity group-hover:opacity-100" />
              </div>
            </button>
          ))}
        </div>
      </Card>
    </div>
  )
}
