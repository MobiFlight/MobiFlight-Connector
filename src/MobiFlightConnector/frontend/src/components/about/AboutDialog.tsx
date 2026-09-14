import { useState } from "react"
import { useTranslation } from "react-i18next"
import { IconCheck, IconCopy } from "@tabler/icons-react"
import logoBanner from "@/assets/mobiflight-banner.png"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import AboutCard from "./components/AboutCard"
import LicensesCard from "./components/LicensesCard"

export type AboutDialogProps = {
  open: boolean
  onOpenChange: (open: boolean) => void
  version?: string
  build?: string
}

export default function AboutDialog({
  open,
  onOpenChange,
  version = window.__MOBIFLIGHT__?.version ?? "10.4.0",
  build = window.__MOBIFLIGHT__?.build,
}: AboutDialogProps) {
  const { t } = useTranslation()
  const [copied, setCopied] = useState(false)

  const fullVersionText = `MobiFlight Connector Version ${version}${build ? ` (Build ${build})` : ""}`

  const handleCopyVersion = () => {
    navigator.clipboard.writeText(fullVersionText)
    setCopied(true)
    setTimeout(() => setCopied(false), 2000)
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-xl max-h-[90vh] flex flex-col gap-0 p-0 overflow-hidden">
        {/* Top Header Section */}
        <DialogHeader className="flex flex-col items-center justify-center p-6 pb-4 border-b bg-muted/10 gap-3">
          <img
            src={logoBanner}
            alt="MobiFlight Logo"
            className="h-10 sm:h-12 w-auto object-contain select-none"
          />
          <div className="flex flex-col items-center">
            <DialogTitle className="text-base font-semibold tracking-tight text-muted-foreground">
              Connector
            </DialogTitle>
            <DialogDescription className="sr-only">
              About MobiFlight Connector, community links, and third-party licenses
            </DialogDescription>
          </div>

          {/* Version Info & Ghost Copy Button */}
          <div className="flex items-center gap-1.5">
            <span className="text-xs font-mono font-medium px-2.5 py-1 rounded-md bg-muted border text-muted-foreground">
              Version {version}
              {build && <span className="opacity-70 ml-1">({build})</span>}
            </span>
            <Button
              variant="ghost"
              size="icon"
              className="size-6 text-muted-foreground hover:text-foreground"
              title={copied ? "Copied!" : "Copy version info"}
              onClick={handleCopyVersion}
            >
              {copied ? (
                <IconCheck className="size-3.5 text-emerald-500" />
              ) : (
                <IconCopy className="size-3.5" />
              )}
            </Button>
          </div>
        </DialogHeader>

        {/* Standard App Tabs */}
        <Tabs defaultValue="about" className="w-full flex-1 flex flex-col min-h-0">
          <div className="px-5 pt-3">
            <TabsList className="grid w-full grid-cols-2">
              <TabsTrigger value="about">
                {t("About.Tabs.About", "About")}
              </TabsTrigger>
              <TabsTrigger value="licenses">
                {t("About.Tabs.Licenses", "Licenses")}
              </TabsTrigger>
            </TabsList>
          </div>

          <div className="flex-1 overflow-y-auto">
            <TabsContent value="about" className="m-0 focus-visible:outline-none">
              <AboutCard />
            </TabsContent>
            <TabsContent
              value="licenses"
              className="m-0 focus-visible:outline-none"
            >
              <LicensesCard />
            </TabsContent>
          </div>
        </Tabs>

        {/* Footer */}
        <DialogFooter className="p-3 px-5 border-t bg-muted/10 flex flex-row items-center justify-between sm:justify-between">
          <span className="text-[11px] text-muted-foreground select-none">
            © 2014-2025 Sebastian Moebius
          </span>
          <Button
            variant="outline"
            size="sm"
            className="h-8 px-4 text-xs"
            onClick={() => onOpenChange(false)}
          >
            {t("General.Close", "Close")}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
