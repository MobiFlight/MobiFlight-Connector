import { test, expect } from "./fixtures"
import { CommandMessage } from "../src/types/commands"
import { ConvertKeyAcceleratorToString, GlobalKeyAccelerators } from "../src/lib/hooks/useKeyAccelerators"

test("Confirm save menu item behaves as expected", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()

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
  await configListPage.initWithTestData()
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