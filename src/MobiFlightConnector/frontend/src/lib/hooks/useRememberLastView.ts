import { useEffect } from "react"
import { useLocation } from "react-router"
import { getMainViewFromPath, lastViewStorage } from "@/lib/lastView"

/**
 * Remembers the main view (dashboard or config list) that is currently shown,
 * so that it can be shown again on the next start.
 */
export const useRememberLastView = () => {
  const { pathname } = useLocation()

  useEffect(() => {
    const view = getMainViewFromPath(pathname)
    if (view === null) return
    lastViewStorage.set(view)
  }, [pathname])
}
