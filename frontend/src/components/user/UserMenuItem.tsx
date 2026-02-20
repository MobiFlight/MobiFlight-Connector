import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { MenubarSeparator } from "@/components/ui/menubar"
import toast from "@/components/ui/ToastWrapper"
import useMessageExchange from "@/lib/hooks/useMessageExchange"
import { cn } from "@/lib/utils"
import {
  IconClipboard,
  IconClipboardCheck,
  IconLoader2,
  IconLogout,
  IconUser,
  IconUserCircle,
} from "@tabler/icons-react"
import { useEffect, useState } from "react"
import { useTranslation } from "react-i18next"
import { useAuth } from "react-oidc-context"

const CopyToClipboardIcon = ({
  label,
  clipboardContent,
}: {
  label: string
  clipboardContent: string
}) => {
  const [copied, setCopied] = useState(false)
  return (
    <div
      className="flex cursor-pointer flex-row items-center gap-1"
      onClick={() => {
        navigator.clipboard.writeText(clipboardContent)
        setCopied(true)
        setTimeout(() => setCopied(false), 2000) // Optional: reset after 2s
      }}
    >
      <div>{label}</div>
      {copied ? <IconClipboardCheck /> : <IconClipboard />}
    </div>
  )
}

const UserMenuItem = () => {
  const auth = useAuth()
  const { publish } = useMessageExchange()
  const { t } = useTranslation()
  const [open, setOpen] = useState(false)

  /* Leaving this here for testing purposes */
  // const myauth = {
  //   error: {
  //     name: "Test Error",
  //     message: "This is a test error message for demonstration purposes.",
  //     stack: "Error stack trace would go here.",
  //   } as unknown as ErrorContext,
  // }

  const error = auth.error /* || myauth.error */

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

  useEffect(() => {
    if (error) {
      const clipboardContent = `Name: ${error.name}\nMessage: ${error.message}\nStack: ${error.stack}`
      toast({
        title: t("Auth.Notification.Error.Title"),
        description: (
          <div className="flex flex-col gap-1">
            <div title={error.message}>
              {t("Auth.Notification.Error.Description")}
            </div>
          </div>
        ),
        button: {
          label: (
            <CopyToClipboardIcon
              label={t("Auth.Notification.Error.ActionCopyToClipboard")}
              clipboardContent={clipboardContent}
            />
          ),
          onClick: (e) => {
            e.stopPropagation() // Prevent toast from closing when button is clicked
          },
        },
        id: "auth-error",
      })
    }
  }, [error, t])

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
          <span className={cn(open && "opacity-0", "text-md")}>
            Hi, {auth.user?.profile?.name}
          </span>
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
