import { test as setup, expect } from "./fixtures"
import dotenv from "dotenv"
import fs from "fs"
dotenv.config()

const authFile = "./tests/.auth/user.json"

const email = process.env.TESTS_USER_EMAIL
const password = process.env.TESTS_USER_PASSWORD
const name = process.env.TESTS_USER_NAME

const skipSetup = !email || !password || !name
const createEmptyAuthFile = () => {
  const emptyState = {
    "cookies": [],
    "origins": []
  }

  fs.mkdirSync("./tests/.auth", { recursive: true })
  fs.writeFileSync(authFile, JSON.stringify(emptyState))
}

if (skipSetup) {  
  if (!fs.existsSync(authFile)) {
    createEmptyAuthFile()
  }
}

setup.skip(
  skipSetup,
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
