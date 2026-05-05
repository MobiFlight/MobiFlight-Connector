import { test, expect } from "./fixtures"
test.describe("User without authentication", () => {
  test("Confirm community buttons in toolbar behave as expected", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")

    const CommunityButtons = [
      ["Support us", "help.donate"],
      ["Discord", "help.discord"],
      ["YouTube", "help.youtube"],
      ["HubHop", "help.hubhop"],
    ]

    for (const [buttonName, command] of CommunityButtons) {
      const button = page.getByRole("button", { name: buttonName })
      await expect(button).toBeVisible()
      await expect(button).toBeEnabled()

      // with tailwind v4, cursor pointer is not default anymore
      // this test ensures that our custom css for pointer cursor is applied
      await button.hover()
      await expect(button).toHaveCSS("cursor", "pointer")

      await button.click()
      const trackedCommands =
        await configListPage.mobiFlightPage.getTrackedCommands()

      if (trackedCommands == undefined || trackedCommands!.length === 0) {
        throw new Error(`No commands tracked after clicking ${buttonName}`)
      }

      const lastCommand = trackedCommands.pop()
      expect(lastCommand.key).toBe("CommandMainMenu")
      expect(lastCommand.payload.action).toBe(command)
    }
  })
})

test.describe("Confirm community toolbar works for basic user", () => {
  const basicEmail = process.env.TESTS_BASIC_EMAIL
  const basicPassword = process.env.TESTS_BASIC_PASSWORD
  const basicName = process.env.TESTS_BASIC_NAME

  test.skip(
    !basicEmail || !basicPassword || !basicName,
    "Skipping community toolbar tests for basic user: required secrets are missing. This typically happens on a PR from a fork where secrets are not available.",
  )

  test.use({ storageState: "./tests/.auth/basic.json" })
  test("Confirm community buttons in toolbar behave as expected", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")

    const button = page.getByRole("button", { name: "Support us" })
    await expect(button).toBeVisible()
    await expect(button).toBeEnabled()
  })
})

test.describe("Confirm community toolbar works for member user", () => {
  const memberEmail = process.env.TESTS_MEMBER_EMAIL
  const memberPassword = process.env.TESTS_MEMBER_PASSWORD
  const memberName = process.env.TESTS_MEMBER_NAME

  test.skip(
    !memberEmail || !memberPassword || !memberName,
    "Skipping community toolbar tests for member user: required secrets are missing. This typically happens on a PR from a fork where secrets are not available.",
  )

  test.use({ storageState: "./tests/.auth/member.json" })
  test("Confirm community buttons in toolbar behave as expected", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")

    const button = page.getByRole("button", { name: "Support us" })
    await expect(button).not.toBeVisible()
  })
})
