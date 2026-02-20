import App from "@/App"
import AuthLogin from "@/components/auth/AuthLogin"
import AuthLogout from "@/components/auth/AuthLogout"
import ControllerBindingsModal from "@/components/modals/ControllerBindingsModal"
import ProjectFormModal from "@/components/modals/ProjectFormModal"
import SplashLogo from "@/components/SplashLogo"
import AuthCallback from "@/components/auth/AuthCallback"
import ConfigListPage from "@/pages/ConfigList"
import Dashboard from "@/pages/Dashboard"
import Plain from "@/pages/Plain"
import { Route, Routes, useLocation } from "react-router"

export function AppRoutes() {
  const location = useLocation()
  const state = location.state as { backgroundLocation?: Location }

  return (
    <>
      <Routes location={state?.backgroundLocation || location}>
        <Route path="/" element={<Plain />}>
          <Route index element={<SplashLogo />} />
        </Route>
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

        <Route path="/auth" element={<Plain />}>
          <Route path="login" element={<AuthLogin />} />
          <Route path="logout" element={<AuthLogout />} />
          <Route
            path="callback/login"
            element={<AuthCallback variant="login" />}
          />
          <Route
            path="callback/logout"
            element={<AuthCallback variant="logout" />}
          />
        </Route>
      </Routes>

      {/* Modal overlay - only when opened with background state */}
      {state?.backgroundLocation && (
        <Routes>
          <Route path="/project/new" element={<ProjectFormModal />} />
          <Route path="/project/edit" element={<ProjectFormModal />} />
          <Route path="/bindings" element={<ControllerBindingsModal />} />
        </Routes>
      )}

      {/* Support direct link to modal route (no background) */}
      {!state?.backgroundLocation && location.pathname === "/project/new" && (
        <ProjectFormModal />
      )}
      {!state?.backgroundLocation && location.pathname === "/bindings" && (
        <ControllerBindingsModal />
      )}
    </>
  )
}
