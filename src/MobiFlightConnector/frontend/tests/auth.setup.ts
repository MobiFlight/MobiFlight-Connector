import { test as setup, expect } from "./fixtures"
import dotenv from "dotenv"
import fs from "fs"
dotenv.config()

const authFileAnonymous = "./tests/.auth/anonymous.json"
const authFileBasic = "./tests/.auth/basic.json"
const authFileMember = "./tests/.auth/member.json"

const memberEmail = process.env.TESTS_MEMBER_EMAIL
const memberPassword = process.env.TESTS_MEMBER_PASSWORD
const memberName = process.env.TESTS_MEMBER_NAME

const basicEmail = process.env.TESTS_BASIC_EMAIL
const basicPassword = process.env.TESTS_BASIC_PASSWORD
const basicName = process.env.TESTS_BASIC_NAME

const skipMemberSetup = !memberEmail || !memberPassword || !memberName
const skipBasicSetup = !basicEmail || !basicPassword || !basicName

const createEmptyAuthFile = (filePath: string) => {
  const emptyState = {
    "cookies": [],
    "origins": []
  }

  fs.mkdirSync("./tests/.auth", { recursive: true })
  fs.writeFileSync(filePath, JSON.stringify(emptyState))
}

createEmptyAuthFile(authFileAnonymous)

if (skipMemberSetup) {
  if (!fs.existsSync(authFileMember)) {
    createEmptyAuthFile(authFileMember)
  }
}

if (skipBasicSetup) {
  if (!fs.existsSync(authFileBasic)) {
    createEmptyAuthFile(authFileBasic)
  }
}

setup.skip(
  skipMemberSetup,
  "Skipping member user tests: required secrets are missing",
)

setup.skip(
  skipBasicSetup,
  "Skipping basic user tests: required secrets are missing",
)

setup("authenticate member", async ({ mobiFlightPage }) => {
  const user = {
    email: memberEmail!,
    password: memberPassword!,
    name: memberName!,
  }

  expect(user.email).toBeDefined()
  expect(user.password).toBeDefined()
  expect(user.name).toBeDefined()

  // Perform authentication steps. Replace these actions with your own.
  await mobiFlightPage.setupSignInUser(user)
  await mobiFlightPage.page.context().storageState({ path: authFileMember })
})

setup("authenticate basic", async ({ mobiFlightPage }) => {
  const user = {
    email: basicEmail!,
    password: basicPassword!,
    name: basicName!,
  }

  expect(user.email).toBeDefined()
  expect(user.password).toBeDefined()
  expect(user.name).toBeDefined()

  // Perform authentication steps. Replace these actions with your own.
  await mobiFlightPage.setupSignInUser(user)
  await mobiFlightPage.page.context().storageState({ path: authFileBasic })
})
