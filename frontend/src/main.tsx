import { StrictMode } from "react"
import { createRoot } from "react-dom/client"
import "./index.css"
import "./i18n.ts"
import { ThemeProvider } from "./components/theme-provider.tsx"
import { TooltipProvider } from "./components/ui/tooltip.tsx"
import { AppRoutes } from "@/Routes.tsx"
import { BrowserRouter } from "react-router"
import { AuthProvider } from "react-oidc-context"
import { oidcConfig } from "@/lib/auth/config"
import { BackendStateMessageHandler } from "@/components/BackendStateMessageHandler.tsx"

if (process.env.NODE_ENV !== "development") {
  console.log = () => {}
  console.warn = () => {}
  console.error = () => {}
  console.info = () => {}
  console.debug = () => {}
}

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider defaultTheme="light" storageKey="ui-mode">
      <AuthProvider {...oidcConfig}>
        <TooltipProvider skipDelayDuration={0}>
          <BrowserRouter>
            <BackendStateMessageHandler>
              <AppRoutes />
            </BackendStateMessageHandler>
          </BrowserRouter>
        </TooltipProvider>
      </AuthProvider>
    </ThemeProvider>
  </StrictMode>,
)
