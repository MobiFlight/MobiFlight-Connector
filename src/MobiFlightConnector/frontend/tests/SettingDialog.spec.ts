import { test, expect } from "./fixtures"

test.describe("Settings menu item tests", () => {
  test("Confirm `Extras > Settings` menu item opens the correct modal", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()

    const menuItemExtras = page
      .getByRole("menubar")
      .getByRole("menuitem", { name: "Extras" })
    const menuItemSettings = page.getByRole("menuitem", { name: "Settings" })
    const dialog = page.getByRole("dialog", { name: "Settings" })

    // Verify modal is not visible initially
    await expect(dialog).not.toBeVisible()

    // Open Extras menu and click Settings
    await menuItemExtras.click()
    await menuItemSettings.click()

    // Verify modal is visible
    await expect(dialog).toBeVisible()
  })

  test("Confirm `Extras > Settings` modal can be closed", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    const menuItemExtras = page
      .getByRole("menubar")
      .getByRole("menuitem", { name: "Extras" })
    const menuItemSettings = page.getByRole("menuitem", { name: "Settings" })
    const dialog = page.getByRole("dialog", { name: "Settings" })

    // Open Settings dialog
    await menuItemExtras.click()
    await menuItemSettings.click()
    await expect(dialog).toBeVisible()

    // Click Cancel button and verify dialog closes
    const cancelButton = dialog.getByRole("button", { name: "Cancel" })
    await cancelButton.click()
    await expect(dialog).not.toBeVisible()
  })

  test("Confirm `Extras > Settings` saves changes and publishes CommandUpdateSettings", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandUpdateSettings")

    const menuItemExtras = page
      .getByRole("menubar")
      .getByRole("menuitem", { name: "Extras" })
    const menuItemSettings = page.getByRole("menuitem", { name: "Settings" })
    const dialog = page.getByRole("dialog", { name: "Settings" })

    // Open Settings dialog
    await menuItemExtras.click()
    await menuItemSettings.click()
    await expect(dialog).toBeVisible()

    // Modify a setting value
    const betaUpdatesSwitch = dialog.locator("#beta-updates")
    await betaUpdatesSwitch.click()

    // Click Save button
    const saveButton = dialog.getByRole("button", { name: "Save" })
    await saveButton.click()

    // Verify dialog is closed
    await expect(dialog).not.toBeVisible()

    // Verify CommandUpdateSettings was published with updated values
    const trackedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    if (trackedCommands == undefined) {
      throw new Error(`No commands tracked after saving settings`)
    }
    expect(trackedCommands).toBeDefined()
    expect(trackedCommands.length).toBeGreaterThan(0)
    const lastCommand = trackedCommands.pop()
    expect(lastCommand?.key).toBe("CommandUpdateSettings")
    expect(lastCommand?.payload.Settings.BetaUpdates).toBe(true)
  })

  test("Confirm `Extras > Settings` cancels changes without publishing command", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandUpdateSettings")

    const menuItemExtras = page
      .getByRole("menubar")
      .getByRole("menuitem", { name: "Extras" })
    const menuItemSettings = page.getByRole("menuitem", { name: "Settings" })
    const dialog = page.getByRole("dialog", { name: "Settings" })

    // Open Settings dialog
    await menuItemExtras.click()
    await menuItemSettings.click()
    await expect(dialog).toBeVisible()

    // Modify a setting value
    const betaUpdatesSwitch = dialog.locator("#beta-updates")
    await betaUpdatesSwitch.click()

    // Click Cancel button
    const cancelButton = dialog.getByRole("button", { name: "Cancel" })
    await cancelButton.click()

    // Discard confirmation appears
    const discardDialog = page.getByRole("dialog", { name: "Discard changes?" })
    await expect(discardDialog).toBeVisible()
    const discardChangesButton = discardDialog.getByRole("button", {
      name: "Discard changes",
    })
    await discardChangesButton.click()

    // Verify dialog is closed
    await expect(dialog).not.toBeVisible()

    // Verify no update command was published
    const trackedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    if (trackedCommands == undefined) {
      throw new Error(`No commands tracked after cancelling settings`)
    }
    expect(trackedCommands.length).toBe(0)
  })

  test("Confirm `Extras > Settings` tab switching works between General and Simulator", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const menuItemExtras = page
      .getByRole("menubar")
      .getByRole("menuitem", { name: "Extras" })
    const menuItemSettings = page.getByRole("menuitem", { name: "Settings" })
    const dialog = page.getByRole("dialog", { name: "Settings" })

    // Open Settings dialog
    await menuItemExtras.click()
    await menuItemSettings.click()
    await expect(dialog).toBeVisible()

    // Verify General tab content is visible by default
    await expect(dialog.locator("#beta-updates")).toBeVisible()

    // Switch to Simulator tab
    const simulatorTab = dialog.getByRole("tab", { name: "Simulator" })
    await simulatorTab.click()
    await expect(dialog.locator("#prosim-host")).toBeVisible()
    await expect(dialog.locator("#prosim-port")).toBeVisible()

    // Switch back to General tab
    const generalTab = dialog.getByRole("tab", { name: "General" })
    await generalTab.click()
    await expect(dialog.locator("#beta-updates")).toBeVisible()
  })

  test("Confirm discard changes confirmation dialog appears when closing with unsaved changes", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const menuItemExtras = page
      .getByRole("menubar")
      .getByRole("menuitem", { name: "Extras" })
    const menuItemSettings = page.getByRole("menuitem", { name: "Settings" })
    const dialog = page.getByRole("dialog", { name: "Settings" })

    // Open Settings dialog
    await menuItemExtras.click()
    await menuItemSettings.click()
    await expect(dialog).toBeVisible()

    // Modify a setting value
    const betaUpdatesSwitch = dialog.locator("#beta-updates")
    await betaUpdatesSwitch.click()

    // Click Cancel button
    const cancelButton = dialog.getByRole("button", { name: "Cancel" })
    await cancelButton.click()

    // Verify discard confirmation dialog appears
    const discardDialog = page.getByRole("dialog", { name: "Discard changes?" })
    await expect(discardDialog).toBeVisible()
    await expect(
      discardDialog.getByText(
        "You have unsaved changes. Are you sure you want to discard them?",
      ),
    ).toBeVisible()

    // Click Keep editing button
    const keepEditingButton = discardDialog.getByRole("button", {
      name: "Keep editing",
    })
    await keepEditingButton.click()

    // Verify discard dialog is closed but Settings dialog remains open
    await expect(discardDialog).not.toBeVisible()
    await expect(dialog).toBeVisible()

    // Click Cancel button again
    await cancelButton.click()
    await expect(discardDialog).toBeVisible()

    // Click Discard changes button
    const discardChangesButton = discardDialog.getByRole("button", {
      name: "Discard changes",
    })
    await discardChangesButton.click()

    // Verify all dialogs are closed
    await expect(discardDialog).not.toBeVisible()
    await expect(dialog).not.toBeVisible()
  })
})
