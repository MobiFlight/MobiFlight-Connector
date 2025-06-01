import { test, expect } from "./fixtures"

test("Confirm project name can be renamed", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()

  const projectNameLabel = page.getByTestId("project-name-label") 
  await expect(projectNameLabel.getByText("Test Project")).toBeVisible()

  const projectContextMenu = projectNameLabel.getByRole('button')
  await projectContextMenu.click()
  await page.getByRole('menuitem', { name: 'Rename' }).click();
  await expect(projectNameLabel.getByRole('textbox')).toBeVisible()

  const textbox = projectNameLabel.getByRole('textbox')

  await configListPage.mobiFlightPage.trackCommand("CommandProjectToolbar")
  await textbox.fill('Test Project Renamed')
  await textbox.press('Enter');
  await expect(projectNameLabel.getByText("Test Project Renamed")).toBeVisible()
  const postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  const lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandProjectToolbar')
  expect (lastCommand.payload.action).toEqual("rename")
  expect (lastCommand.payload.value).toEqual("Test Project Renamed")

  await projectContextMenu.click()
  await page.getByRole('menuitem', { name: 'Rename' }).click();
  await expect(projectNameLabel.getByRole('textbox')).toBeVisible()
  await textbox.fill('Test Project Renamed Again')
  await textbox.press('Escape')
  await expect(projectNameLabel.getByText("Test Project Renamed")).toBeVisible()
})

test("Confirm execution toolbar works correctly", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()

  const toolbar = page.getByRole('toolbar').first()
  await expect(toolbar).toBeVisible() 

  await configListPage.mobiFlightPage.trackCommand("CommandProjectToolbar")


  const autoRunButton = toolbar.getByRole('button', { name: 'AutoRun' })
  await expect(autoRunButton).toBeVisible()
  await autoRunButton.click()  
  let postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  let lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandProjectToolbar')
  expect (lastCommand.payload.action).toEqual("toggleAutoRun")

  const runButton = toolbar.getByRole('button', { name: 'Run', exact: true })
  await expect(runButton).toBeVisible()
  await runButton.click()
  postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandProjectToolbar')
  expect (lastCommand.payload.action).toEqual("run")

  const testButton = toolbar.getByRole('button', { name: 'Test' })
  await expect(testButton).toBeVisible()
  await testButton.click()
  postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandProjectToolbar')
  expect (lastCommand.payload.action).toEqual("test")
  
  const stopButton = toolbar.getByRole('button', { name: 'Stop' })
  await expect(stopButton).toBeVisible()
  // stop button should be disable
  await expect(stopButton).toBeDisabled()
  await runButton.click()

  // Simulate that the config is running
  await configListPage.updateExecutionState(
    {
      IsRunning: true,
      IsTesting: false,
      RunAvailable: false,
      TestAvailable: false
    },
  )
  
  await expect(stopButton).toBeEnabled()
  await stopButton.click()
  postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandProjectToolbar')
  expect (lastCommand.payload.action).toEqual("stop")

})


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
  const secondTab = page.getByRole('tablist').getByRole('tab').nth(1)
  await secondTab.click()
  let postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  let lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandActiveConfigFile')
  expect (lastCommand.payload.index).toEqual(1)
  
  // Rename the second tab
  await configListPage.mobiFlightPage.trackCommand("CommandFileContextMenu")
  const secondTabContextMenu = page.getByRole('tablist').getByRole('tab').nth(1).getByRole('button').nth(1)
  await secondTabContextMenu.click();
  // the overlay is not nested in the dom
  // we have to use the page locator to find it
  await page.getByRole('menuitem', { name: 'Rename' }).click();
  await secondTab.getByRole('button', { name: 'Config 2' }).getByRole('textbox').fill('Config 2 Renamed');
  await secondTab.getByRole('button', { name: 'Config 2' }).getByRole('textbox').press('Enter');

  postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandFileContextMenu')
  expect (lastCommand.payload.action).toEqual('rename')
  expect (lastCommand.payload.index).toEqual(1)
  expect (lastCommand.payload.file.Label).toEqual('Config 2 Renamed')
  
  // Remove the second tab
  await configListPage.mobiFlightPage.trackCommand("CommandFileContextMenu")
  
  await secondTabContextMenu.click();
  await page.getByRole('menuitem', { name: 'Remove' }).click();
  postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandFileContextMenu')
  expect (lastCommand.payload.action).toEqual('remove')
  expect (lastCommand.payload.index).toEqual(1)
})