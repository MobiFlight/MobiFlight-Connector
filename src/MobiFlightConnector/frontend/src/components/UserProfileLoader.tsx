import { fetchProfile } from "@/lib/profile"
import { useUserProfileStore } from "@/stores/userProfileStore"
import { useQuery } from "@tanstack/react-query"
import { ReactNode, useEffect } from "react"
import { useAuth } from "react-oidc-context"

type UserProfileLoaderProps = {
  children: ReactNode
}

export function UserProfileLoader({ children }: UserProfileLoaderProps) {
  const auth = useAuth()
  const { setUserProfile } = useUserProfileStore()

  const queryEnabled = !auth.isLoading && auth.isAuthenticated && Boolean(auth.user?.id_token)

  const userProfileQuery = useQuery({
    queryKey: ["user-profile", auth.user?.id_token],
    queryFn: ({ signal }) => fetchProfile(auth, { signal }),
    enabled: queryEnabled,
  })

  // store the user profile after successful fetch
  useEffect(() => {
    if (userProfileQuery.data) {
      setUserProfile(userProfileQuery.data)
    }
  }, [userProfileQuery.data, setUserProfile])

  // clear the user profile on logout
  useEffect(() => {
    if (!auth.isAuthenticated) {
      setUserProfile(null)
    }
  }, [auth.isAuthenticated, setUserProfile])

  return <>{children}</>
}
