import { useTranslation } from "react-i18next"
import { IconExternalLink, IconHeart } from "@tabler/icons-react"
import useOpenUrl from "@/lib/hooks/useOpenUrl"
import { Card } from "@/components/ui/card"
import { ScrollArea } from "@/components/ui/scroll-area"
import licensesData from "@/components/about/data/licenses.json"

interface LibraryItem {
  name: string
  libraryLink: string
  licenseLink: string
}

export default function LicensesCard() {
  const { t } = useTranslation()
  const openUrl = useOpenUrl()
  const libraries: LibraryItem[] = licensesData

  return (
    <div className="p-4">
      {/* Single master card without any nested cards inside */}
      <Card className="w-full border border-border/70 bg-card/60 overflow-hidden shadow-xs">
        {/* Description Header */}
        <div className="bg-muted/30 px-4 py-2.5 border-b">
          <h3 className="text-xs font-semibold uppercase tracking-wider text-muted-foreground">
            {t("About.Licenses.Title", "Libraries & Dependencies")}
          </h3>
          <p className="text-xs text-muted-foreground mt-0.5 leading-relaxed">
            {t(
              "About.Licenses.Description",
              "Libraries used by MobiFlight Connector - Thanks to the authors for making them available!"
            )}
          </p>
        </div>

        {/* Scrollable Libraries List */}
        <ScrollArea className="h-64">
          <div className="divide-y divide-border/50">
            {libraries.map((lib) => (
              <div
                key={lib.name}
                className="flex items-center justify-between px-4 py-2.5 transition-colors hover:bg-muted/40"
              >
                <span className="text-sm font-medium text-foreground truncate">
                  {lib.name}
                </span>
                <div className="flex items-center gap-3 text-xs shrink-0">
                  <button
                    type="button"
                    className="text-primary hover:underline flex items-center gap-1 cursor-pointer font-medium"
                    onClick={() => openUrl(lib.libraryLink)}
                  >
                    <span>Project</span>
                    <IconExternalLink className="size-3 opacity-60" />
                  </button>
                  <span className="text-muted-foreground/40">•</span>
                  <button
                    type="button"
                    className="text-primary hover:underline flex items-center gap-1 cursor-pointer font-medium"
                    onClick={() => openUrl(lib.licenseLink)}
                  >
                    <span>License</span>
                    <IconExternalLink className="size-3 opacity-60" />
                  </button>
                </div>
              </div>
            ))}
          </div>
        </ScrollArea>

        {/* Special Thanks Footer Section with BoeingCDULarge font-family applied */}
        <div className="bg-muted/20 px-4 py-2.5 border-t flex items-center justify-between text-xs text-muted-foreground">
          <span
            className="font-medium text-foreground/90 tracking-wide"
            style={{
              fontFamily: "'BoeingCDULarge', 'Consolas', 'Courier New', monospace",
            }}
          >
            {t(
              "About.Credits.Font",
              '"BoeingCDULarge font" by Gijs de Rooij'
            )}
          </span>
          <div className="flex items-center gap-1.5 text-pink-500 font-medium shrink-0">
            <IconHeart className="size-3.5 fill-pink-500 shrink-0" />
            <span>{t("About.Credits.ThankYou", "Thank You!")}</span>
          </div>
        </div>
      </Card>
    </div>
  )
}
