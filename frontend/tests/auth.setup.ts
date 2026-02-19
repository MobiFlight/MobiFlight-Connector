import { test as setup, expect } from "./fixtures"
import dotenv from "dotenv"
dotenv.config()

const authFile = "./tests/.auth/user.json"

setup("authenticate", async ({ mobiFlightPage }) => {
  const user = {
    email: process.env.TESTS_USER_EMAIL!,
    password: process.env.TESTS_USER_PASSWORD!,
    name: process.env.TESTS_USER_NAME!,
  }

  expect(user.email).toBeDefined()
  expect(user.password).toBeDefined()
  expect(user.name).toBeDefined()

  // Perform authentication steps. Replace these actions with your own.
  await mobiFlightPage.setupSignInUser(user)
  await mobiFlightPage.page.context().storageState({ path: authFile })
})
