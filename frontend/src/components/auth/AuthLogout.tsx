import { useEffect } from "react"
import { useAuth } from "react-oidc-context"
import { IconLoader2 } from "@tabler/icons-react"

export default function AuthLogout() {
  const auth = useAuth()

  useEffect(() => {
    if (!auth.isLoading) {
      // Trigger the signout redirect
      auth.signoutRedirect()
    }
  }, [auth])

  return (
    <div className="flex items-center justify-center h-screen">
      <div className="text-center space-y-4">
        <IconLoader2 className="h-12 w-12 animate-spin mx-auto text-primary" />
        <p className="text-lg">Signing out...</p>
      </div>
    </div>
  )
}