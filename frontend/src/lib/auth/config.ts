import { AuthProviderProps } from "react-oidc-context"

export const oidcConfig: AuthProviderProps = {
  authority: "https://mobiflightids.ciamlogin.com/7af08b0b-58c9-4b14-a50b-a2fe8fa7bcb3/v2.0",
  client_id: "f63fa3f7-2c98-466e-b8e5-840ace8854a9",
  redirect_uri: `${window.location.origin}/auth/callback/login`,
  post_logout_redirect_uri: `${window.location.origin}/auth/callback/logout`,
  response_type: "code",
  scope: "openid profile email",
  automaticSilentRenew: true,
  /* removed prompt to allow for auto sign in of last user */
}