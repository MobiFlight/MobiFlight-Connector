import { StrictMode } from "react"
import { createRoot } from "react-dom/client"
import "./index.css"
import "./i18n.ts"
import App from "./App.tsx"
import { BrowserRouter, Route, Routes } from "react-router"
import { ThemeProvider } from "./components/theme-provider.tsx"
import ConfigListPage from "./pages/ConfigList.tsx"
import { TooltipProvider } from "./components/ui/tooltip.tsx"

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
    <TooltipProvider skipDelayDuration={0}>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<App />} />
          <Route path="/config" element={<App />}>
            <Route index element={<ConfigListPage />} />
          </Route>
          <Route index path="/index.html" element={<App />} />
        </Routes>
      </BrowserRouter>
    </TooltipProvider>
    </ThemeProvider>
  </StrictMode>,
)
