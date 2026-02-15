import useMessageExchange from "@/lib/hooks/useMessageExchange"
import { useEffect } from "react"
import { useAuth } from "react-oidc-context"
import { useNavigate } from "react-router-dom"

export type AuthCallbackProps = {
  variant: "login" | "logout"
}

export default function AuthCallback({ variant }: AuthCallbackProps) {
  const auth = useAuth()
  const navigate = useNavigate()
  const { publish } = useMessageExchange()

  useEffect(() => {
    // Wait for auth state to stabilize (e.g., after signinSilent triggered by AuthModal)
    // only then we can evaluate the result and notify the backend
    if (auth.isLoading) return

    // Any error during the auth flow should be treated as auth failure
    if (auth.error) {
      console.error("Auth error:", auth.error)
      publish({
        key: "CommandUserAuthentication",
        payload: {
          flow: variant,
          state: "error"
        },
      })

      return
    }

    const isLoginAndAuthenticated = variant === "login" && auth.isAuthenticated
    if (isLoginAndAuthenticated) {
      publish({
        key: "CommandUserAuthentication",
        payload: {
          flow: variant,
          state: "success"
        },
      })

      return
    }

    const isLogoutAndNotAuthenticated = variant === "logout" && !auth.isAuthenticated
    if (isLogoutAndNotAuthenticated) {
      publish({
        key: "CommandUserAuthentication",
        payload: {
          flow: variant,
          state: "success"
        },
      })

      return
    }
  }, [
    auth.isLoading,
    auth.isAuthenticated,
    auth.error,
    navigate,
    publish,
    variant,
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
