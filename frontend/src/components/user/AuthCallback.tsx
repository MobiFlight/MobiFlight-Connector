import useMessageExchange from "@/lib/hooks/useMessageExchange"
import { useEffect } from "react"
import { useAuth } from "react-oidc-context"
import { useNavigate } from "react-router-dom"

export default function AuthCallback() {
  const auth = useAuth()
  const navigate = useNavigate()
  const { publish } = useMessageExchange()

  useEffect(() => {
    if (!auth.isLoading && auth.isAuthenticated) {
      // In separate auth window: notify C# and wait to be closed
      publish({
        key: "CommandUserAuthentication",
        payload: { action: "successful" , url: `${window.location.origin}/auth/silent-renew`},
      })
    } else if (!auth.isLoading && auth.error) {
      console.error("Auth error:", auth.error)
      publish({
        key: "CommandUserAuthentication",
        payload: { action: "error", url: `${window.location.origin}/auth/silent-renew` },
      })
    }
  }, [
    auth.isLoading,
    auth.isAuthenticated,
    auth.error,
    navigate,
    publish,
  ])

  return (
    <div className="flex h-screen items-center justify-center">
      <div className="text-center">
        <p className="text-lg">Completing sign in...</p>
        {auth.error && (
          <p className="text-destructive mt-2">Error: {auth.error.message}</p>
        )}
      </div>
    </div>
  )
}
