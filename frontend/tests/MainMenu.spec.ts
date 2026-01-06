import { test, expect } from "./fixtures"
import { CommandMessage } from "../src/types/commands"
import { ConvertKeyAcceleratorToString, GlobalKeyAccelerators } from "../src/lib/hooks/useKeyAccelerators"

test("Confirm save menu item behaves as expected", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  const FileMenu = page.getByRole("menubar").getByRole("menuitem", { name: "File" })
  await expect(FileMenu).toBeVisible()

  await FileMenu.click()
  const FileMenuSaveItem = page.getByRole("menuitem", { name: "Save Ctrl+S" })
  await expect(FileMenuSaveItem).toBeVisible()
  await expect(FileMenuSaveItem).toBeDisabled()
  await configListPage.updateProjectState({
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

  for(const accelerator of GlobalKeyAccelerators) {
    const key = ConvertKeyAcceleratorToString(accelerator)
    await page.keyboard.press(key)

    const trackedCommands = await configListPage.mobiFlightPage.getTrackedCommands()

    if (trackedCommands == undefined) {
      throw new Error(`No commands tracked after pressing ${key}`)
    }
    expect(trackedCommands.length).toBeGreaterThan(0)

    const lastCommand = trackedCommands.pop() as CommandMessage
    expect(lastCommand).toEqual(accelerator.message)
  }
})

test("Confirm zoom menu items are present in Extras menu", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  const ExtrasMenu = page.getByRole("menubar").getByRole("menuitem", { name: "Extras" })
  await expect(ExtrasMenu).toBeVisible()

  await ExtrasMenu.click()
  
  // Check for Zoom submenu
  const ZoomSubmenu = page.getByRole("menuitem", { name: "Zoom" })
  await expect(ZoomSubmenu).toBeVisible()
  
  await ZoomSubmenu.hover()
  
  // Check for zoom menu items with shortcuts
  const ResetZoomItem = page.getByRole("menuitem", { name: "Reset Zoom Ctrl+0" })
  await expect(ResetZoomItem).toBeVisible()
  
  const ZoomInItem = page.getByRole("menuitem", { name: "Zoom In Ctrl++" })
  await expect(ZoomInItem).toBeVisible()
  
  const ZoomOutItem = page.getByRole("menuitem", { name: "Zoom Out Ctrl+-" })
  await expect(ZoomOutItem).toBeVisible()
})

test("Confirm zoom menu items send correct commands", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()
  await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")

  const ExtrasMenu = page.getByRole("menubar").getByRole("menuitem", { name: "Extras" })
  await ExtrasMenu.click()
  
  const ZoomSubmenu = page.getByRole("menuitem", { name: "Zoom" })
  await ZoomSubmenu.hover()
  
  // Test Reset Zoom
  const ResetZoomItem = page.getByRole("menuitem", { name: "Reset Zoom Ctrl+0" })
  await ResetZoomItem.click()
  
  let commands = await configListPage.mobiFlightPage.getTrackedCommands()
  expect(commands).toHaveLength(1)
  expect(commands![0].key).toBe("CommandMainMenu")
  expect(commands![0].payload.action).toBe("extras.zoom.reset")
  
  // Reset tracking for next test
  await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")
  
  // Test Zoom In
  await ExtrasMenu.click()
  await ZoomSubmenu.hover()
  const ZoomInItem = page.getByRole("menuitem", { name: "Zoom In Ctrl++" })
  await ZoomInItem.click()
  
  commands = await configListPage.mobiFlightPage.getTrackedCommands()
  expect(commands).toHaveLength(1)
  expect(commands![0].key).toBe("CommandMainMenu")
  expect(commands![0].payload.action).toBe("extras.zoom.in")
  
  // Reset tracking for next test
  await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")
  
  // Test Zoom Out
  await ExtrasMenu.click()
  await ZoomSubmenu.hover()
  const ZoomOutItem = page.getByRole("menuitem", { name: "Zoom Out Ctrl+-" })
  await ZoomOutItem.click()
  
  commands = await configListPage.mobiFlightPage.getTrackedCommands()
  expect(commands).toHaveLength(1)
  expect(commands![0].key).toBe("CommandMainMenu")
  expect(commands![0].payload.action).toBe("extras.zoom.out")
})