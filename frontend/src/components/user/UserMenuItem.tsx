import { Button } from "@/components/ui/button"
import useMessageExchange from "@/lib/hooks/useMessageExchange"
import { IconLoader2, IconUserCircle } from "@tabler/icons-react"
import { useAuth } from "react-oidc-context"

const UserMenuItem = () => {
  const auth = useAuth()
  const { publish } = useMessageExchange()

  const handleSignIn = () => {
    publish({
      key: "CommandUserAuthentication",
      payload: {
        action: "login",
        url: `${window.location.origin}/auth/login`,
      },
    })
  }

  const handleSignOut = () => {
    publish({
      key: "CommandUserAuthentication",
      payload: {
        action: "logout",
        url: `${window.location.origin}/auth/logout`,
      },
    })
  }

  if (auth.error) {
    return (
      <Button variant="ghost" disabled className="text-destructive mx-2">
        Error: {auth.error.message}
      </Button>
    )
  }

  if (auth.isLoading) {
    return (
      <Button
        variant={"ghost"}
        className="mx-2 h-8 rounded-full pr-1 [&_svg]:size-8"
        disabled
      >
        <IconLoader2 className="animate-spin" />
      </Button>
    )
  }

  console.log("Auth state:", {
    isAuthenticated: auth.isAuthenticated,
    user: auth.user,
  })

  return auth.isAuthenticated ? (
    <Button
      variant={"ghost"}
      className="mx-2 h-8 rounded-full pr-1 [&_svg]:size-8"
      onClick={handleSignOut}
    >
      <span className="text-md">Hi, {auth.user?.profile?.name}</span>
      <IconUserCircle />
    </Button>
  ) : (
    <Button
      variant={"ghost"}
      className="mx-2 h-8 rounded-full pr-1 [&_svg]:size-8"
      onClick={handleSignIn}
    >
      {auth.isAuthenticated ? (
        <span className="text-md">Hi, {auth.user?.profile?.name}</span>
      ) : (
        <span className="text-md">Sign In</span>
      )}
      <IconUserCircle />
    </Button>
  )
}
export default UserMenuItem
