import { test, expect } from "./fixtures"
import { CommandMainMenuPayload, CommandMessage } from "../src/types/commands"
import {
  ConvertKeyAcceleratorToString,
  GlobalKeyAccelerators,
} from "../src/lib/hooks/useKeyAccelerators"

test("Confirm `File` menu items are displayed and trigger correct command", async ({
  configListPage,
  page,
}) => {
  const FileMenuItems = [
    // New is covered in its own test since it opens a modal,
    // so we just check for the presence of the menu item here
    { name: "New Ctrl+N", action: null },
    { name: "Open... Ctrl+O", action: "file.open" },
    // Save is covered in its own test,
    // so we just check for the presence of the menu item here
    { name: "Save Ctrl+S", action: null },
    { name: "Save As... Ctrl+Shift+S", action: "file.saveas" },
    { name: "Recent Projects", action: null },
    { name: "Exit Ctrl+Q", action: "file.exit" },
  ]

  for (const menuItem of FileMenuItems) {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")
    await configListPage.mobiFlightPage.clearTrackedCommands()

    const FileMenu = page
      .getByRole("menubar")
      .getByRole("menuitem", { name: "File" })
    await expect(FileMenu).toBeVisible()

    await FileMenu.click()

    const item = page.getByRole("menuitem", {
      name: menuItem.name,
      exact: true,
    })
    await expect(item).toBeVisible()

    if (menuItem.action == null) continue
    await item.click()

    const trackedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()

    if (trackedCommands == undefined) {
      throw new Error(`No commands tracked after clicking ${menuItem.name}`)
    }
    expect(trackedCommands.length).toBeGreaterThan(0)

    const lastCommand = trackedCommands.pop() as CommandMessage
    expect((lastCommand.payload as CommandMainMenuPayload).action).toEqual(
      menuItem.action,
    )
  }
})

test("Confirm `Extras` menu items are displayed and trigger correct command", async ({
  configListPage,
  page,
}) => {
  const mainMenuItems = [
    {
      name: "HubHop",
      children: [
        { name: "Download latest presets", action: "extras.hubhop.download" },
      ],
    },
    {
      name: "Microsoft Flight Simulator",
      children: [
        { name: "Re-install WASM Module", action: "extras.msfs.reinstall" },
      ],
    },
    { name: "Copy logs to clipboard", action: "extras.copylogs" },
    // covered by special test, so we just check for their presence and skip the command check
    { name: "Controller Bindings", action: null },
    { name: "Manage controllers", action: "extras.serials" },
    { name: "Settings", action: null },
  ]

  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")

  for (const menuItem of mainMenuItems) {
    const hasChildren = !!menuItem.children?.length
    const subItems = hasChildren
      ? menuItem.children!
      : [{ name: menuItem.name, action: menuItem.action! }]

    for (const subItem of subItems) {
      await configListPage.mobiFlightPage.clearTrackedCommands()

      // Open Extras menu
      const ExtrasMenu = page
        .getByRole("menubar")
        .getByRole("menuitem", { name: "Extras" })
      await expect(ExtrasMenu).toBeVisible()
      await ExtrasMenu.click()

      // Verify main menu item (e.g. "HubHop") is visible
      const item = page.getByRole("menuitem", {
        name: menuItem.name,
        exact: true,
      })
      await expect(item).toBeVisible()

      // If subItems are actually children
      if (hasChildren) {
        await item.click()
        const sub = page.getByRole("menuitem", {
          name: subItem.name,
          exact: true,
        })
        await expect(sub).toBeVisible()

        if (subItem.action == null) {
          // close the menu for the next loop
          await ExtrasMenu.click()
          continue
        }

        await sub.click()
      } else {
        if (menuItem.action == null) {
          // close the menu for the next loop
          await ExtrasMenu.click()
          continue
        }
        await item.click()
      }

      const trackedCommands =
        await configListPage.mobiFlightPage.getTrackedCommands()
      if (trackedCommands == undefined) {
        throw new Error(`No commands tracked after clicking ${subItem.name}`)
      }
      expect(trackedCommands.length).toBeGreaterThan(0)
      const lastCommand = trackedCommands.pop() as CommandMessage
      expect((lastCommand.payload as CommandMainMenuPayload).action).toEqual(
        subItem.action,
      )
    }
  }
})
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
    const recentFilesInput = dialog.locator("#recent-files")
    await recentFilesInput.fill("10")

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
    expect(lastCommand?.payload.RecentFilesMaxCount).toBe(10)
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
    const recentFilesInput = dialog.locator("#recent-files")
    await recentFilesInput.fill("15")

    // Click Cancel button
    const cancelButton = dialog.getByRole("button", { name: "Cancel" })
    await cancelButton.click()

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
    await expect(dialog.getByText("Recent files")).toBeVisible()

    // Switch to Simulator tab
    const simulatorTab = dialog.getByRole("tab", { name: "Simulator" })
    await simulatorTab.click()
    await expect(dialog.locator("#prosim-host")).toBeVisible()
    await expect(dialog.locator("#prosim-port")).toBeVisible()

    // Switch back to General tab
    const generalTab = dialog.getByRole("tab", { name: "General" })
    await generalTab.click()
    await expect(dialog.getByText("Recent files")).toBeVisible()
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
    const recentFilesInput = dialog.locator("#recent-files")
    await recentFilesInput.fill("15")

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


test("Confirm `Help` menu items are displayed and trigger correct command", async ({
  configListPage,
  page,
}) => {
  const helpMenuItems = [
    { name: "Documentation F1", action: "help.docs" },
    { name: "Check for update", action: "help.checkforupdate" },
    { name: "Visit Discord server", action: "help.discord" },
    { name: "Visit HubHop website", action: "help.hubhop" },
    { name: "Visit YouTube channel", action: "help.youtube" },
    { name: "About", action: "help.about" },
    { name: "Release notes", action: "help.releasenotes" },
  ]

  for (const menuItem of helpMenuItems) {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")
    await configListPage.mobiFlightPage.clearTrackedCommands()

    const menu = page
      .getByRole("menubar")
      .getByRole("menuitem", { name: "Help" })
    await expect(menu).toBeVisible()

    await menu.click()

    const item = page.getByRole("menuitem", {
      name: menuItem.name,
      exact: true,
    })
    await expect(item).toBeVisible()

    if (menuItem.action == null) continue
    await item.click()

    const trackedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()

    if (trackedCommands == undefined) {
      throw new Error(`No commands tracked after clicking ${menuItem.name}`)
    }
    expect(trackedCommands.length).toBeGreaterThan(0)

    const lastCommand = trackedCommands.pop() as CommandMessage
    expect((lastCommand.payload as CommandMainMenuPayload).action).toEqual(
      menuItem.action,
    )
  }
})

test("Confirm save menu item behaves as expected", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  const FileMenu = page
    .getByRole("menubar")
    .getByRole("menuitem", { name: "File" })
  await expect(FileMenu).toBeVisible()

  await FileMenu.click()
  const FileMenuSaveItem = page.getByRole("menuitem", { name: "Save Ctrl+S" })
  await expect(FileMenuSaveItem).toBeVisible()
  await expect(FileMenuSaveItem).toBeDisabled()
  await configListPage.mobiFlightPage.updateProjectState({
    HasChanged: true,
  })
  await expect(FileMenuSaveItem).toBeEnabled()
  await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")
  await FileMenuSaveItem.click()

  const command = await configListPage.mobiFlightPage.getTrackedCommands()
  expect(command).toHaveLength(1)
  expect(command![0].key).toBe("CommandMainMenu")
  expect(command![0].payload.action).toBe("file.save")
})

test("Confirm accelerator keys are working correctly", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()
  await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")

  for (const accelerator of GlobalKeyAccelerators) {
    const key = ConvertKeyAcceleratorToString(accelerator)
    await page.keyboard.press(key)

    const trackedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()

    if (trackedCommands == undefined) {
      throw new Error(`No commands tracked after pressing ${key}`)
    }
    expect(trackedCommands.length).toBeGreaterThan(0)

    const lastCommand = trackedCommands.pop() as CommandMessage
    expect(lastCommand).toEqual(accelerator.message)
  }
})

test("Confirm zoom menu items are present in View menu", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  const ViewMenu = page
    .getByRole("menubar")
    .getByRole("menuitem", { name: "View" })
  await expect(ViewMenu).toBeVisible()

  await ViewMenu.click()

  // Check for direct zoom menu items (not a submenu)
  const ResetZoomItem = page.getByRole("menuitem", {
    name: "Reset Zoom Ctrl+0",
  })
  await expect(ResetZoomItem).toBeVisible()

  const ZoomInItem = page.getByRole("menuitem", { name: "Zoom In Ctrl++" })
  await expect(ZoomInItem).toBeVisible()

  const ZoomOutItem = page.getByRole("menuitem", { name: "Zoom Out Ctrl+-" })
  await expect(ZoomOutItem).toBeVisible()
})

test("Confirm View menu contains opens and closes Log Panel item", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.openLogPanel()

  const logPanel = page.getByTestId("log-panel")
  await expect(logPanel).toBeVisible()

  await configListPage.mobiFlightPage.closeLogPanel()
  await expect(logPanel).not.toBeVisible()
})

test("Confirm zoom menu items send correct commands", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()
  await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")

  const ViewMenu = page
    .getByRole("menubar")
    .getByRole("menuitem", { name: "View" })
  await ViewMenu.click()

  // Test Reset Zoom
  const ResetZoomItem = page.getByRole("menuitem", {
    name: "Reset Zoom Ctrl+0",
  })
  await ResetZoomItem.click()

  let commands = await configListPage.mobiFlightPage.getTrackedCommands()
  expect(commands).toHaveLength(1)
  expect(commands![0].key).toBe("CommandMainMenu")
  expect(commands![0].payload.action).toBe("view.zoom.reset")

  // Clear commands array for next test
  await configListPage.mobiFlightPage.clearTrackedCommands()

  // Test Zoom In
  await ViewMenu.click()
  const ZoomInItem = page.getByRole("menuitem", { name: "Zoom In Ctrl++" })
  await ZoomInItem.click()

  commands = await configListPage.mobiFlightPage.getTrackedCommands()
  expect(commands).toHaveLength(1)
  expect(commands![0].key).toBe("CommandMainMenu")
  expect(commands![0].payload.action).toBe("view.zoom.in")

  // Clear commands array for next test
  await configListPage.mobiFlightPage.clearTrackedCommands()

  // Test Zoom Out
  await ViewMenu.click()
  const ZoomOutItem = page.getByRole("menuitem", { name: "Zoom Out Ctrl+-" })
  await ZoomOutItem.click()

  commands = await configListPage.mobiFlightPage.getTrackedCommands()
  expect(commands).toHaveLength(1)
  expect(commands![0].key).toBe("CommandMainMenu")
  expect(commands![0].payload.action).toBe("view.zoom.out")
})
