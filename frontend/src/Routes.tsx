import App from "@/App"
import NewProjectModalRoute from "@/components/modals/ProjectFormModal"
import ConfigListPage from "@/pages/ConfigList"
import Dashboard from "@/pages/Dashboard"
import { Route, Routes, useLocation } from "react-router"

export function AppRoutes() {
  const location = useLocation()
  const state = location.state as { backgroundLocation?: Location }

  return (
    <>
      <Routes location={state?.backgroundLocation || location}>

        <Route path="/home" element={<App />}>
          <Route index element={<Dashboard />} />
        </Route>
        <Route path="/home/:content" element={<App />}>
          <Route index element={<Dashboard />} />
        </Route>
        <Route path="/config" element={<App />}>
          <Route index element={<ConfigListPage />} />
        </Route>
        <Route index path="/index.html" element={<App />} />
      </Routes>

      {/* Modal overlay - only when opened with background state */}
      {state?.backgroundLocation && (
        <Routes>
          <Route path="/project/new" element={<NewProjectModalRoute />} />
          <Route path="/project/edit" element={<NewProjectModalRoute />} />
        </Routes>
      )}

      {/* Support direct link to modal route (no background) */}
      {!state?.backgroundLocation && location.pathname === "/project/new" && (
        <NewProjectModalRoute />
      )}
    </>
  )
}
