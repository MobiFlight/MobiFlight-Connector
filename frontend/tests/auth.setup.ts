import { test as setup, expect } from "./fixtures"
import dotenv from "dotenv"
dotenv.config()

const authFile = "./tests/.auth/user.json"

const email = process.env.TESTS_USER_EMAIL
const password = process.env.TESTS_USER_PASSWORD
const name = process.env.TESTS_USER_NAME

setup.skip(
  !email || !password || !name,
  "Skipping user menu item tests: required secrets are missing",
)

setup("authenticate", async ({ mobiFlightPage }) => {
  const user = {
    email: email!,
    password: password!,
    name: name!,
  }

  expect(user.email).toBeDefined()
  expect(user.password).toBeDefined()
  expect(user.name).toBeDefined()

  // Perform authentication steps. Replace these actions with your own.
  await mobiFlightPage.setupSignInUser(user)
  await mobiFlightPage.page.context().storageState({ path: authFile })
})
