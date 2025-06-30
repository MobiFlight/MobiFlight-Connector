import { test, expect } from "./fixtures"

test("Confirm community buttons in toolbar behave as expected", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")

  const CommunityButtons = [
    [ "Support us", "help.donate" ],
    [ "Discord", "help.discord" ],
    [ "YouTube", "help.youtube" ],
    [ "HubHop", "help.hubhop" ],
  ]

  for (const [buttonName, command] of CommunityButtons) {
    const button = page.getByRole("button", { name: buttonName })
    await expect(button).toBeVisible()
    await expect(button).toBeEnabled()
    await button.click()
    const trackedCommands = await configListPage.mobiFlightPage.getTrackedCommands()

    if (trackedCommands == undefined || trackedCommands!.length === 0) {
      throw new Error(`No commands tracked after clicking ${buttonName}`)
    }

    const lastCommand = trackedCommands.pop()
    expect(lastCommand.key).toBe("CommandMainMenu")
    expect(lastCommand.payload.action).toBe(command)
  }
})