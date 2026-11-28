import { Project } from "@/types"

/**
 * The main views of the app. The last one the user worked in
 * is shown again on the next start.
 */
export type MainView = "dashboard" | "config"

const STORAGE_KEY = "last_main_view"

/**
 * Returns the main view of a route, or null for all other routes,
 * e.g. the startup screen or the auth routes.
 */
export const getMainViewFromPath = (pathname: string): MainView | null => {
  if (pathname === "/home" || pathname.startsWith("/home/")) return "dashboard"
  if (pathname === "/config" || pathname.startsWith("/config/")) return "config"
  return null
}

/**
 * Returns the route to show once the startup has finished.
 * The config list is only shown again if a project file got loaded,
 * otherwise there is nothing to continue with and the dashboard is shown.
 */
export const getStartupRoute = (
  lastView: MainView | null,
  project: Project | null,
): string => {
  if (lastView === "config" && project?.FilePath) return "/config"
  return "/home"
}

export const lastViewStorage = {
  get: (): MainView | null => {
    const view = localStorage.getItem(STORAGE_KEY)
    return view === "dashboard" || view === "config" ? view : null
  },

  set: (view: MainView): void => {
    localStorage.setItem(STORAGE_KEY, view)
  },
}

export default lastViewStorage
