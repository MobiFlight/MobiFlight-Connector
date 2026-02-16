import { useEffect } from "react"
import { useAuth } from "react-oidc-context"
import { IconLoader2 } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export default function AuthLogout() {
  const auth = useAuth()
  const { t } = useTranslation()
  

  useEffect(() => {
    if (!auth.isLoading) {
      // Trigger the signout redirect
      auth.signoutRedirect()
    }
  }, [auth])

  return (
    <div className="flex items-center justify-center h-screen">
      <div className="flex flex-col gap-4 items-center justify-center bg-background h-128 w-lg shadow-xl">
        <IconLoader2 className="h-12 w-12 animate-spin mx-auto text-primary" />
        <p className="text-lg">{t("Auth.Redirect.SignOut")}</p>
      </div>
    </div>
  )
}