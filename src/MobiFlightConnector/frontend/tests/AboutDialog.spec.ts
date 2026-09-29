import { test, expect } from "./fixtures"

test.describe("Confirm `About` Dialog functionality", () => {
  test("Confirm `About` modal opens via main menu and closes correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()

    const menuItemHelp = page
      .getByRole("menubar")
      .getByRole("menuitem", { name: "Help" })
    const menuItemAbout = page.getByRole("menuitem", { name: "About" })
    const dialog = page.getByRole("dialog")
    await expect(dialog).not.toBeVisible()

    // Open About dialog from MainMenu > Help > About
    await menuItemHelp.click()
    await menuItemAbout.click()
    await expect(dialog).toBeVisible()

    // Close the dialog using the footer Close button
    const closeButton = dialog.getByRole("button", { name: "Close" }).first()
    await closeButton.click()
    await expect(dialog).not.toBeVisible()
  })

  test("Confirm `Copy Version` button works correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()

    const menuItemHelp = page
      .getByRole("menubar")
      .getByRole("menuitem", { name: "Help" })
    const menuItemAbout = page.getByRole("menuitem", { name: "About" })
    const dialog = page.getByRole("dialog")
    await expect(dialog).not.toBeVisible()

    // Open About dialog
    await menuItemHelp.click()
    await menuItemAbout.click()
    await expect(dialog).toBeVisible()

    // Click copy button and verify feedback tooltip/title changes to 'Copied!'
    const copyButton = dialog.getByTitle("Copy version info")
    await expect(copyButton).toBeVisible()
    await copyButton.click()
    await expect(dialog.getByTitle("Copied!")).toBeVisible()
  })

  test("Confirm tab switching works between About and Licenses", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()

    const menuItemHelp = page
      .getByRole("menubar")
      .getByRole("menuitem", { name: "Help" })
    const menuItemAbout = page.getByRole("menuitem", { name: "About" })
    const dialog = page.getByRole("dialog")
    await expect(dialog).not.toBeVisible()

    // Open About dialog
    await menuItemHelp.click()
    await menuItemAbout.click()
    await expect(dialog).toBeVisible()

    // Verify About tab content is visible by default
    await expect(dialog.getByText("How to contact us")).toBeVisible()
    await expect(dialog.getByText("MobiFlight Ecosystem")).toBeVisible()

    // Switch to Licenses tab and verify library content
    const licensesTab = dialog.getByRole("tab", { name: "Licenses" })
    await licensesTab.click()
    await expect(dialog.getByText("Libraries & Dependencies")).toBeVisible()
    await expect(dialog.getByText("CmdMessenger")).toBeVisible()
    await expect(
      dialog.getByText(/"BoeingCDULarge font" by Gijs de Rooij/)
    ).toBeVisible()

    // Switch back to About tab
    const aboutTab = dialog.getByRole("tab", { name: "About" })
    await aboutTab.click()
    await expect(dialog.getByText("How to contact us")).toBeVisible()
  })

  test("Confirm clicking contact link triggers CommandOpenLinkInBrowser", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.trackCommand("CommandOpenLinkInBrowser")
    await configListPage.mobiFlightPage.clearTrackedCommands()

    const menuItemHelp = page
      .getByRole("menubar")
      .getByRole("menuitem", { name: "Help" })
    const menuItemAbout = page.getByRole("menuitem", { name: "About" })
    const dialog = page.getByRole("dialog")

    // Open About dialog
    await menuItemHelp.click()
    await menuItemAbout.click()
    await expect(dialog).toBeVisible()

    // Click Email link row
    const emailRow = dialog.getByText("info@mobiflight.com")
    await emailRow.click()

    const trackedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    expect(trackedCommands?.length).toBeGreaterThan(0)
    const lastCommand = trackedCommands?.pop()
    expect(lastCommand?.payload.url).toBe("mailto:info@mobiflight.com")
  })
})
