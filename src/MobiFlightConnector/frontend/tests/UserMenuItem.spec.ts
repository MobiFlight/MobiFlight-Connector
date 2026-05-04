import { test, expect } from "./fixtures"
import dotenv from "dotenv"
dotenv.config()

// Reset storage state for this file to avoid being authenticated
test.use({ storageState: { cookies: [], origins: [] } })

const basicEmail = process.env.TESTS_BASIC_EMAIL
const basicPassword = process.env.TESTS_BASIC_PASSWORD
const basicName = process.env.TESTS_BASIC_NAME

const memberEmail = process.env.TESTS_MEMBER_EMAIL
const memberName = process.env.TESTS_MEMBER_NAME

test.describe("User Login flow", () => {
  test.skip(
    !basicEmail || !basicPassword || !basicName,
    "Skipping user menu item tests: required secrets are missing",
  )
  test("Confirm SignIn Commands to Backend are correct", async ({
    dashboardPage,
    page,
  }) => {
    expect(basicEmail).toBeDefined()
    expect(basicPassword).toBeDefined()
    expect(basicName).toBeDefined()

    await dashboardPage.gotoPage()
    await dashboardPage.mobiFlightPage.trackCommand("CommandUserAuthentication")

    const signInButton = page.getByRole("button", { name: "Sign in" })
    await expect(signInButton).toBeVisible()
    await expect(signInButton).toBeEnabled()

    await signInButton.click()

    const signInCommands =
      await dashboardPage.mobiFlightPage.getTrackedCommands()
    expect(signInCommands).toBeDefined()
    expect(signInCommands!.length).toBe(1)

    const signInCommand = signInCommands!.pop()
    expect(signInCommand).toBeDefined()
    expect(signInCommand?.key).toBe("CommandUserAuthentication")
    expect(signInCommand?.payload).toEqual({
      flow: "login",
      state: "started",
      url: `http://localhost:5173/auth/login`,
    })

    await dashboardPage.mobiFlightPage.clearTrackedCommands()

    // Initiate the sign in flow
    // This is done by the second WebView once it receives
    // CommandUserAuthentication with flow: login and state: started
    await dashboardPage.mobiFlightPage.page.goto(
      `http://localhost:5173/auth/login`,
    )

    const emailInput = page.getByPlaceholder("Email address")
    const nextButton = page.getByRole("button", { name: "Next" })
    await expect(emailInput).toBeVisible()
    await emailInput.fill(basicEmail!)
    await nextButton.click()

    const passwordInput = page.getByPlaceholder("Password")
    await expect(passwordInput).toBeVisible()
    await passwordInput.fill(basicPassword!)
    await signInButton.click()

    await expect(page.getByText("Stay signed in")).toBeVisible()
    const noButton = page.getByRole("button", { name: "No" })
    await expect(noButton).toBeVisible()
    await noButton.click()

    await expect(page).toHaveURL(
      /http:\/\/localhost:5173\/auth\/callback\/login\?code=.+&state=.+/,
    )
    // Go back to the dashboard (required for the hook that listens to authentication changes)
    // simulate the response by the second WebView
    // after successful sign in
    // this triggers the reload of auth object
    await dashboardPage.gotoPage()
    await dashboardPage.mobiFlightPage.publishMessage({
      key: "AuthenticationStatus",
      payload: {
        Authenticated: true,
      },
    })
    const loggedInUserName = page.getByText(`Hi, ${basicName}`)
    await expect(loggedInUserName).toBeVisible()
  })
})

test.describe("Verify user menu item works for basic user", () => {
  test.skip(
    !basicEmail || !basicName,
    "Skipping user menu item tests: required secrets are missing",
  )
  test.use({ storageState: "./tests/.auth/basic.json" })
  test("Confirm account information and interactions work correctly", async ({
    dashboardPage,
    page,
  }) => {
    await dashboardPage.gotoPage()

    // click on avatar icon to open user menu
    const UserButton = page.getByRole("button", { name: `Hi, ${basicName}` })
    await expect(UserButton).toBeVisible()
    await expect(UserButton).toBeEnabled()

    await UserButton.click()

    const usernameLabel = page.getByText(basicName!, { exact: true })
    await expect(usernameLabel).toBeVisible()
    const emailLabel = page.getByText(basicEmail!, { exact: true })
    await expect(emailLabel).toBeVisible()

    const memberStatusLabel = page.getByText("Basic Account")
    await expect(memberStatusLabel).toBeVisible()

    const upgradeLink = page.getByRole("button", { name: "Upgrade" })
    await expect(upgradeLink).toBeVisible()
  })

  test("Confirm profil menu item behaves as expected", async ({
    dashboardPage,
    page,
  }) => {
    await dashboardPage.gotoPage()

    // click on avatar icon to open user menu
    const UserButton = page.getByRole("button", { name: `Hi, ${basicName}` })
    await UserButton.click()

    // Verify that the "Profile" button opens the correct link
    const profileButton = page.getByRole("menuitem", {
      name: "Member Profile",
    })
    await expect(profileButton).toBeVisible()

    // Set up command tracking before click
    await dashboardPage.mobiFlightPage.trackCommand("CommandOpenLinkInBrowser")
    
    // click the profile button
    await profileButton.click()
    
    // verify that the correct command was sent to the backend with the correct URL
    const trackedCommands =
      await dashboardPage.mobiFlightPage.getTrackedCommands()
    expect(trackedCommands).toBeDefined()
    expect(trackedCommands!.length).toBe(1)
    const lastCommand = trackedCommands!.pop()
    expect(lastCommand?.key).toBe("CommandOpenLinkInBrowser")
    expect(lastCommand?.payload.url).toBe("https://club.mobiflight.com")
  })

  test("Confirm Sign Out menu item behaves as expected", async ({
    dashboardPage,
    page,
  }) => {
    await dashboardPage.gotoPage()
    await dashboardPage.mobiFlightPage.trackCommand("CommandUserAuthentication")

    // click on avatar icon to open user menu
    const UserButton = page.getByRole("button", { name: `Hi, ${basicName}` })
    await UserButton.click()

    const signOutButton = page.getByRole("menuitem", { name: "Sign out" })
    await expect(signOutButton).toBeVisible()
    await signOutButton.click()
    const signOutCommands =
      await dashboardPage.mobiFlightPage.getTrackedCommands()
    expect(signOutCommands).toBeDefined()
    expect(signOutCommands!.length).toBe(1)
    const signOutCommand = signOutCommands!.pop()
    expect(signOutCommand?.key).toBe("CommandUserAuthentication")
    expect(signOutCommand?.payload).toEqual({
      flow: "logout",
      state: "started",
      url: `http://localhost:5173/auth/logout`,
    })
  })
})

test.describe("Verify user menu item works for member user", () => {
  test.skip(
    !memberEmail || !memberName,
    "Skipping user menu item tests: required secrets are missing",
  )
  test.use({ storageState: "./tests/.auth/member.json" })
  test("Confirm member information and interactions work correctly", async ({
    dashboardPage,
    page,
  }) => {
    await dashboardPage.gotoPage()

    const UserButton = page.getByRole("button", { name: `Hi, ${memberName}` })
    await expect(UserButton).toBeVisible()
    await expect(UserButton).toBeEnabled()

    await UserButton.click()

    const usernameLabel = page.getByText(memberName!, { exact: true })
    await expect(usernameLabel).toBeVisible()
    const emailLabel = page.getByText(memberEmail!, { exact: true })
    await expect(emailLabel).toBeVisible()

    const memberStatusLabel = page.getByText("Club Member", { exact: true })
    await expect(memberStatusLabel).toBeVisible()
  })

  test("Confirm profil menu item behaves as expected", async ({
    dashboardPage,
    page,
  }) => {
    await dashboardPage.gotoPage()

    // click on avatar icon to open user menu
    const UserButton = page.getByRole("button", { name: `Hi, ${memberName}` })
    await UserButton.click()

    // Verify that the "Profile" button opens the correct link
    const profileButton = page.getByRole("menuitem", {
      name: "Member Profile",
    })
    await expect(profileButton).toBeVisible()

    // Set up command tracking before click
    await dashboardPage.mobiFlightPage.trackCommand("CommandOpenLinkInBrowser")
    
    // click the profile button
    await profileButton.click()
    
    // verify that the correct command was sent to the backend with the correct URL
    const trackedCommands =
      await dashboardPage.mobiFlightPage.getTrackedCommands()
    expect(trackedCommands).toBeDefined()
    expect(trackedCommands!.length).toBe(1)
    const lastCommand = trackedCommands!.pop()
    expect(lastCommand?.key).toBe("CommandOpenLinkInBrowser")
    expect(lastCommand?.payload.url).toBe("https://club.mobiflight.com")
  })

  test("Confirm Sign Out menu item behaves as expected", async ({
    dashboardPage,
    page,
  }) => {
    await dashboardPage.gotoPage()
    
    // click on avatar icon to open user menu
    const UserButton = page.getByRole("button", { name: `Hi, ${memberName}` })
    await UserButton.click()
    
    const signOutButton = page.getByRole("menuitem", { name: "Sign out" })
    await expect(signOutButton).toBeVisible()
    
    // Set up command tracking before click
    await dashboardPage.mobiFlightPage.trackCommand("CommandUserAuthentication")

    // click the sign out button
    await signOutButton.click()
    
    // verify that the correct command was sent to the backend with the correct payload
    const signOutCommands =
      await dashboardPage.mobiFlightPage.getTrackedCommands()
    expect(signOutCommands).toBeDefined()
    expect(signOutCommands!.length).toBe(1)
    const signOutCommand = signOutCommands!.pop()
    expect(signOutCommand?.key).toBe("CommandUserAuthentication")
    expect(signOutCommand?.payload).toEqual({
      flow: "logout",
      state: "started",
      url: `http://localhost:5173/auth/logout`,
    })
  })
})
