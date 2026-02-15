import { useEffect } from "react"
import { useAuth } from "react-oidc-context"
import { useNavigate } from "react-router-dom"

export default function AuthCallback() {
  const auth = useAuth()
  const navigate = useNavigate()

  useEffect(() => {
    if (!auth.isLoading) {
      if (auth.isAuthenticated) {
        // Successfully authenticated, redirect to return URL
        const returnUrl = sessionStorage.getItem('returnUrl') || '/home'
        sessionStorage.removeItem('returnUrl')
        navigate(returnUrl, { replace: true })
      } else if (auth.error) {
        // Auth failed, show error or redirect home
        console.error('Auth error:', auth.error)
        navigate('/home', { replace: true })
      }
    }
  }, [auth.isLoading, auth.isAuthenticated, auth.error, navigate])

  return (
    <div className="flex items-center justify-center h-screen">
      <div className="text-center">
        <p className="text-lg">Completing sign in...</p>
        {auth.error && (
          <p className="text-destructive mt-2">
            Error: {auth.error.message}
          </p>
        )}
      </div>
    </div>
  )
}