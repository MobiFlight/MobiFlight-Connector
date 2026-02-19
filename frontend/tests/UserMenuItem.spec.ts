import { test, expect } from "./fixtures"
import dotenv from 'dotenv';
dotenv.config();

// Reset storage state for this file to avoid being authenticated
test.use({ storageState: { cookies: [], origins: [] } });

const email = process.env.TESTS_USER_EMAIL
const password = process.env.TESTS_USER_PASSWORD
const name = process.env.TESTS_USER_NAME

test.skip(!email || !password || !name, 'Skipping user menu item tests: required secrets are missing')

test("Confirm SignIn and SignOut Commands to Backend are correct", async ({
  dashboardPage,
  page,
}) => {
  expect(email).toBeDefined()
  expect(password).toBeDefined()
  expect(name).toBeDefined()
  
  await dashboardPage.gotoPage()
  await dashboardPage.mobiFlightPage.trackCommand("CommandUserAuthentication")

  const signInButton = page.getByRole("button", { name: "Sign in" })
  await expect(signInButton).toBeVisible()
  await expect(signInButton).toBeEnabled()

  await signInButton.click()

  const signInCommands = await dashboardPage.mobiFlightPage.getTrackedCommands()
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
  await emailInput.fill(email!)
  await nextButton.click()

  const passwordInput = page.getByPlaceholder("Password")
  await expect(emailInput).toBeVisible()
  await passwordInput.fill(password!)
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
  const loggedInUserName = page.getByText("Hi, Test 01")
  await expect(loggedInUserName).toBeVisible()
})
