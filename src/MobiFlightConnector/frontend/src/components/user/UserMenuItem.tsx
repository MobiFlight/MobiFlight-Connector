import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
  DropdownMenuLabel,
} from "@/components/ui/dropdown-menu"
import { MenubarSeparator } from "@/components/ui/menubar"
import toast from "@/components/ui/ToastWrapper"
import useMessageExchange from "@/lib/hooks/useMessageExchange"
import { cn } from "@/lib/utils"
import { useUserProfileStore } from "@/stores/userProfileStore"
import { CommandOpenLinkInBrowser } from "@/types/commands"
import {
  IconClipboard,
  IconClipboardCheck,
  IconLoader2,
  IconLogout,
  IconRosetteDiscountCheckFilled,
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
  const { userProfile } = useUserProfileStore()

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

  const handleProfileClick = () => {
    publish({
      key: "CommandOpenLinkInBrowser",
      payload: {
        url: `https://club.mobiflight.com`,
      },
    } as CommandOpenLinkInBrowser)
  }

  const handleUpgradeClick = () => {
    publish({
      key: "CommandOpenLinkInBrowser",
      payload: {
        url: `https://club.mobiflight.com/subscribe`,
      },
    } as CommandOpenLinkInBrowser)
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

  const memberStatus = userProfile?.membership

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
          <div className="relative">
            <IconUserCircle />
            {memberStatus === "member" && (
              <div className="absolute -right-1 -bottom-0.5 h-5 w-5 rounded-full bg-white stroke-0">
                <IconRosetteDiscountCheckFilled
                  className={cn("size-5! fill-pink-600")}
                />
              </div>
            )}
          </div>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="min-w-70 [&_svg]:size-5">
        <div className="px-2 font-medium">{auth.user?.profile?.name}</div>
        <div className="text-muted-foreground px-2 text-sm">
          {auth.user?.profile?.email}
        </div>
        <MenubarSeparator />
        <div className="-mx-2 bg-linear-to-br from-sky-500 to-emerald-500 px-2">
          <DropdownMenuLabel className="text-md text-background pb-0">
            {t("Membership.Club")}
          </DropdownMenuLabel>
          <DropdownMenuItem className="flex cursor-default flex-row justify-center px-2 pt-0 focus:bg-transparent">
            {memberStatus === "member" ? (
              <Badge
                className="flex flex-row items-center gap-2 rounded-full bg-pink-600 hover:bg-pink-600 px-1 pr-2 my-2"
              >
                <IconRosetteDiscountCheckFilled />
                <span className="text-sm">{t("Membership.Status.Member")}</span>
              </Badge>
            ) : (
              <div className="flex flex-row items-center">
                <Badge
                  variant="outline"
                  className="border-background rounded-full px-2"
                >
                  <span className="text-background text-sm">
                    {t("Membership.Status.Basic")}
                  </span>
                </Badge>
                <Button
                  variant={"link"}
                  onClick={handleUpgradeClick}
                  className="text-background font-normal"
                >
                  {t("Membership.Action.Upgrade")}
                </Button>
              </div>
            )}
          </DropdownMenuItem>
        </div>
        <MenubarSeparator />
        <DropdownMenuItem onClick={handleProfileClick} className="">
          <IconUser />
          <span>{t("Auth.User.Profile")}</span>
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
