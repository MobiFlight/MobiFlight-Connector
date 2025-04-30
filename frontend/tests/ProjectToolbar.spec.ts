import { test, expect } from "./fixtures"

test("Confirm file tabs count is correct", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()

  const tabs = page.getByRole('tablist').getByRole('tab', { name: 'Config'})
  await expect(tabs).toHaveCount(3) 
})

test("Confirm file tab actions work", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await configListPage.mobiFlightPage.trackCommand("CommandActiveConfigFile")

  // Select the second tab
  const secondTab = page.getByRole('tablist').getByRole('tab', { name: 'Config'}).nth(1)
  await secondTab.click()
  let postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  let lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandActiveConfigFile')
  expect (lastCommand.payload.index).toEqual(1)

  // Rename the second tab
  await configListPage.mobiFlightPage.trackCommand("CommandFileContextMenu")
  await page.getByRole('button', { name: 'Open menu' }).nth(1).click();
  await page.getByRole('menuitem', { name: 'Rename' }).click();
  await page.getByRole('button', { name: 'Config 2' }).getByRole('textbox').fill('Config 2 Renamed');
  await page.getByRole('button', { name: 'Config 2' }).getByRole('textbox').press('Enter');

  postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandFileContextMenu')
  expect (lastCommand.payload.action).toEqual('rename')
  expect (lastCommand.payload.index).toEqual(1)
  expect (lastCommand.payload.file.Label).toEqual('Config 2 Renamed')
  
  // Remove the second tab
  await configListPage.mobiFlightPage.trackCommand("CommandFileContextMenu")
  await page.getByRole('button', { name: 'Open menu' }).nth(1).click();
  await page.getByRole('menuitem', { name: 'Remove' }).click();
  postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandFileContextMenu')
  expect (lastCommand.payload.action).toEqual('remove')
  expect (lastCommand.payload.index).toEqual(1)
})