import { useEffect } from "react"
import { useAuth } from "react-oidc-context"
import { IconLoader2 } from "@tabler/icons-react"

export default function AuthLogin() {
  const auth = useAuth()

  useEffect(() => {
    if (!auth.isLoading && !auth.isAuthenticated) {
      // Trigger the signin redirect
      auth.signinRedirect()
    }
  }, [auth])

  return (
    <div className="flex items-center justify-center h-screen">
      <div className="text-center space-y-4">
        <IconLoader2 className="h-12 w-12 animate-spin mx-auto text-primary" />
        <p className="text-lg">Redirecting to sign in...</p>
      </div>
    </div>
  )
}