import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { MenubarSeparator } from "@/components/ui/menubar"
import useMessageExchange from "@/lib/hooks/useMessageExchange"
import {
  IconLoader2,
  IconLogout,
  IconUser,
  IconUserCircle,
} from "@tabler/icons-react"
import { useState } from "react"
import { useTranslation } from "react-i18next"
import { useAuth } from "react-oidc-context"

const UserMenuItem = () => {
  const auth = useAuth()
  const { publish } = useMessageExchange()
  const { t } = useTranslation()
  const [open, setOpen] = useState(false)

  const handleSignIn = () => {
    publish({
      key: "CommandUserAuthentication",
      payload: {
        flow: "login",
        state: "started",
        url: `${window.location.origin}/auth/login`,
      },
    })
  }

  const handleSignOut = () => {
    publish({
      key: "CommandUserAuthentication",
      payload: {
        flow: "logout",
        state: "started",
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
    <DropdownMenu onOpenChange={setOpen}>
      <DropdownMenuTrigger asChild>
        <Button
          variant={"ghost"}
          className="mx-2 h-8 rounded-full pr-1 [&_svg]:size-8"
        >
          {!open && (
            <span className="text-md">Hi, {auth.user?.profile?.name}</span>
          )}
          <IconUserCircle />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="min-w-40 [&_svg]:size-5">
        <div className="text-md px-2 py-1 font-medium">
          {auth.user?.profile?.name}
        </div>
        <div className="text-muted-foreground px-2 py-0 text-sm">
          {auth.user?.profile?.email}
        </div>
        <MenubarSeparator />
        <DropdownMenuItem className="">
          <IconUser />
          <span>{t("Auth.User.Profile")}</span>
          <Badge variant="outline" className="ml-auto">
            {t("Auth.User.ProfileFeatureComingSoon")}
          </Badge>
        </DropdownMenuItem>
        <MenubarSeparator />
        <DropdownMenuItem onClick={handleSignOut} className="text-md">
          <IconLogout />
          <span>{t("Auth.User.SignOut")}</span>
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  ) : (
    <Button
      variant={"ghost"}
      className="mx-2 h-8 rounded-full pr-1 [&_svg]:size-8"
      onClick={handleSignIn}
    >
      <span className="text-md">{t("Auth.User.SignIn")}</span>
      <IconUserCircle />
    </Button>
  )
}
export default UserMenuItem
