import { Outlet, useNavigate, useOutlet, useSearchParams } from "react-router"
import StartupProgress from "./components/StartupProgress"
import { useEffect, useState } from "react"
import { useAppMessage } from "./lib/hooks/appMessage"
import { StatusBarUpdate } from "./types"
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
import { useBackendStateAppMessages } from "@/lib/hooks/useBackendStateAppMessages"

function App() {
  // Initialize global app message handlers
  useBackendStateAppMessages() 
  const [queryParameters] = useSearchParams()
  const navigate = useNavigate()

  useKeyAccelerators(GlobalKeyAccelerators, true)
  const outlet = useOutlet()
  const [overlayVisible, setOverlayVisible] = useState(false)
  const { theme } = useTheme()

  // State for startup progress from app messages
  const [appStartupProgress, setAppStartupProgress] = useState<StatusBarUpdate>(
    {
      Value: 0,
      Text: "Starting...",
    },
  )

  useAppMessage("StatusBarUpdate", (message) => {
    setAppStartupProgress(message.payload as StatusBarUpdate)
  })

  useAppMessage("OverlayState", (message) => {
    const overlayState = message.payload as OverlayState
    console.log("OverlayState message received", overlayState)
    setOverlayVisible(overlayState.Visible)
  })

  const queryProgressValue = Number.parseInt(
    queryParameters.get("progress")?.toString() ?? "0",
  )

  const startupProgress =
    queryProgressValue > 0
      ? {
          Value: queryProgressValue,
          Text:
            queryProgressValue === 100 ? "Loading complete..." : "Loading...",
        }
      : appStartupProgress

  useEffect(() => {
    if (startupProgress.Value == 100 && location.pathname == "/index.html") {
      console.log("Finished loading, navigating to home")
      navigate("/home")
    }
  }, [startupProgress.Value, navigate])

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
      {outlet ? (
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
      ) : (
        <StartupProgress
          value={startupProgress.Value}
          text={startupProgress.Text}
        />
      )}
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
