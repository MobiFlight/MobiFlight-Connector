import useMessageExchange from "@/lib/hooks/useMessageExchange"
import {
  IconCircleCheck,
  IconExclamationCircle,
  IconLoader2,
} from "@tabler/icons-react"
import { useEffect } from "react"
import { useTranslation } from "react-i18next"
import { useAuth } from "react-oidc-context"
import { useNavigate } from "react-router-dom"

export type AuthCallbackProps = {
  variant: "login" | "logout"
}

export default function AuthCallback({ variant }: AuthCallbackProps) {
  const auth = useAuth()
  const navigate = useNavigate()
  const { publish } = useMessageExchange()
  const { t } = useTranslation()

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
          state: "error",
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
          state: "success",
        },
      })
      return
    }

    const isLogoutAndNotAuthenticated =
      variant === "logout" && !auth.isAuthenticated
    if (isLogoutAndNotAuthenticated) {
      publish({
        key: "CommandUserAuthentication",
        payload: {
          flow: variant,
          state: "success",
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
      <div className="bg-background flex h-128 w-lg flex-col items-center justify-center gap-4 shadow-xl">
        {(auth.isLoading || (!auth.isAuthenticated && !auth.error)) && (
          <>
            <IconLoader2 className="text-primary mx-auto h-12 w-12 animate-spin" />
            <p className="text-lg">{t("Auth.Redirect.CompletingFlow")}</p>
          </>
        )}
        {auth.isAuthenticated && (
          <>
            <IconCircleCheck className="mx-auto h-12 w-12 text-green-500" />
            <p className="text-lg">{t("Auth.Redirect.FlowSuccessful")}</p>
          </>
        )}
        {auth.error && (
          <>
            <IconExclamationCircle className="mx-auto h-12 w-12 text-red-500" />
            <p className="text-destructive mt-2">Error: {auth.error.message}</p>
          </>
        )}
      </div>
    </div>
  )
}
