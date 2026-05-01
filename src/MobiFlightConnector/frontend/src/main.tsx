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
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { UserProfileLoader } from "@/components/UserProfileLoader.tsx"

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: false,
      staleTime: 5 * 60 * 1000,
    },
  },
})

if (process.env.NODE_ENV !== "development") {
  console.log = () => {}
  console.warn = () => {}
  console.error = () => {}
  console.info = () => {}
  console.debug = () => {}
}

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <ThemeProvider defaultTheme="light" storageKey="ui-mode">
        <AuthProvider {...oidcConfig}>
          <TooltipProvider skipDelayDuration={0}>
            <BrowserRouter>
              <UserProfileLoader>
                <BackendStateMessageHandler>
                  <AppRoutes />
                </BackendStateMessageHandler>
              </UserProfileLoader>
            </BrowserRouter>
          </TooltipProvider>
        </AuthProvider>
      </ThemeProvider>
    </QueryClientProvider>
  </StrictMode>,
)
