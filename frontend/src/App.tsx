import { Outlet, useOutlet } from "react-router"
import { useState } from "react"
import { useAppMessage } from "./lib/hooks/appMessage"
import { MainMenu } from "./components/MainMenu"
import { OverlayState } from "./types/messages"
import {
  useKeyAccelerators,
  GlobalKeyAccelerators,
} from "./lib/hooks/useKeyAccelerators"
import LoaderOverlay from "./components/tables/config-item-table/LoaderOverlay"
import { Toaster } from "./components/ui/sonner"
import { useTheme } from "@/lib/hooks/useTheme"
import { ToastNotificationHandler } from "./components/notifications/ToastNotificationHandler"

import DebugInfo from "@/components/DebugInfo"
import { useTranslation } from "react-i18next"

function App() {
  useKeyAccelerators(GlobalKeyAccelerators, true)
  const outlet = useOutlet()
  const [overlayVisible, setOverlayVisible] = useState(false)
  const { theme } = useTheme()
  
  useAppMessage("OverlayState", (message) => {
    const overlayState = message.payload as OverlayState
    console.log("OverlayState message received", overlayState)
    setOverlayVisible(overlayState.Visible)
  })

  const { t } = useTranslation()

  return (
    <>
      {overlayVisible && (
        <LoaderOverlay
          open={overlayVisible}
          onOpenChange={setOverlayVisible}
          message={t("General.Overlay.OpeningWizard")}
        />
      )}
      {outlet && (
        <div className="flex h-svh flex-row overflow-hidden p-0 select-none">
          {/* <Sidebar /> */}
          <div className="flex grow flex-col">
            <MainMenu />

            {/* Uncomment the Navbar if needed */}
            {/* <Navbar /> */}
            <div className="flex grow flex-col overflow-hidden">
              <Outlet />
            </div>
            <DebugInfo />
          </div>
        </div>
      ) }
      <ToastNotificationHandler />
      <Toaster
        expand
        visibleToasts={4}
        toastOptions={{ duration: 10000 }}
        position="bottom-right"
        theme={theme}
        className="flex w-full justify-center ![--width:540px]"
      />
    </>
  )
}

export default App
