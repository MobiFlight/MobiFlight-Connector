import { Locator } from "@playwright/test"
import { test, expect } from "./fixtures"
import { ConfigFile } from "../src/types"

test("Confirm project name can be renamed", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  const projectNameLabel = page.getByTestId("project-name-label")
  await expect(projectNameLabel.getByText("Test Project")).toBeVisible()

  const projectContextMenu = projectNameLabel.getByRole("button", {
    name: "Open menu",
  })
  await projectContextMenu.click()
  await page.getByRole("menuitem", { name: "Rename" }).click()
  await expect(projectNameLabel.getByRole("textbox")).toBeVisible()

  const textbox = projectNameLabel.getByRole("textbox")

  await configListPage.mobiFlightPage.trackCommand("CommandProjectToolbar")
  await textbox.fill("Test Project Renamed")
  await textbox.press("Enter")
  await expect(projectNameLabel.getByText("Test Project Renamed")).toBeVisible()
  const postedCommands =
    await configListPage.mobiFlightPage.getTrackedCommands()
  const lastCommand = postedCommands!.pop()
  expect(lastCommand.key).toEqual("CommandProjectToolbar")
  expect(lastCommand.payload.action).toEqual("rename")
  expect(lastCommand.payload.value).toEqual("Test Project Renamed")

  await projectContextMenu.click()
  await page.getByRole("menuitem", { name: "Rename" }).click()
  await expect(projectNameLabel.getByRole("textbox")).toBeVisible()
  await textbox.fill("Test Project Renamed Again")
  await textbox.press("Escape")
  await expect(projectNameLabel.getByText("Test Project Renamed")).toBeVisible()
})

test.describe("Test execution toolbar", () => {
  let toolbar: Locator

  test.beforeEach(async ({ configListPage, page }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    toolbar = page.getByRole("toolbar").first()
    await expect(toolbar).toBeVisible()
  })

  test("Confirm AutoRun works correctly", async ({ configListPage }) => {
    await configListPage.mobiFlightPage.trackCommand("CommandProjectToolbar")
    const autoRunButton = toolbar.getByRole("button", { name: "Auto" })
    await expect(autoRunButton).toBeVisible()
    await autoRunButton.click()
    const postedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    const lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandProjectToolbar")
    expect(lastCommand.payload.action).toEqual("toggleAutoRun")
  })

  test("Confirm Run works correctly", async ({ configListPage }) => {
    await configListPage.mobiFlightPage.trackCommand("CommandProjectToolbar")
    const runButton = toolbar.getByRole("button", { name: "Run", exact: true })
    await expect(runButton).toBeVisible()
    await runButton.click()

    const postedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    const lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandProjectToolbar")
    expect(lastCommand.payload.action).toEqual("run")

    // Simulate that the config is running
    await configListPage.updateExecutionState({
      IsRunning: true,
      IsTesting: false,
      RunAvailable: false,
      TestAvailable: false,
    })
    await expect(runButton).not.toBeVisible()

    const stopButton = toolbar.getByRole("button", { name: "Stop" })
    await expect(stopButton).toBeVisible()

    const testButton = toolbar.getByRole("button", { name: "Test" })
    await expect(testButton).toBeDisabled()
  })

  test("Confirm Test works correctly", async ({ configListPage }) => {
    await configListPage.mobiFlightPage.trackCommand("CommandProjectToolbar")
    const testButton = toolbar.getByRole("button", {
      name: "Test",
      exact: true,
    })
    await expect(testButton).toBeVisible()
    await testButton.click()
    const postedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    const lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandProjectToolbar")
    expect(lastCommand.payload.action).toEqual("test")

    // Simulate that the config is running
    await configListPage.updateExecutionState({
      IsRunning: false,
      IsTesting: true,
      RunAvailable: false,
      TestAvailable: false,
    })

    await expect(testButton).not.toBeVisible()
    const stopButton = toolbar.getByRole("button", { name: "Stop" })
    await expect(stopButton).toBeVisible()

    const runButton = toolbar.getByRole("button", { name: "Run" })
    await expect(runButton).toBeDisabled()
  })

  test("Confirm Stop works correctly", async ({ configListPage }) => {
    await configListPage.mobiFlightPage.trackCommand("CommandProjectToolbar")
    const stopButton = toolbar.getByRole("button", { name: "Stop" })
    await expect(stopButton).not.toBeVisible()

    // Simulate that the config is running
    await configListPage.updateExecutionState({
      IsRunning: true,
      IsTesting: false,
      RunAvailable: false,
      TestAvailable: false,
    })

    await expect(stopButton).toBeVisible()
    await stopButton.click()
    const postedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    const lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandProjectToolbar")
    expect(lastCommand.payload.action).toEqual("stop")
  })
})

test.describe("Test profile tabs basic features", () => {
  test("Confirm file tabs count is correct", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const tabs = page.getByRole("tablist").getByRole("tab", { name: "Config" })
    await expect(tabs).toHaveCount(3)
  })

  test("Confirm file tab actions work", async ({ configListPage, page }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandActiveConfigFile")

    // Select the second tab
    const secondTab = page.getByRole("tablist").getByRole("tab").nth(1)
    await secondTab.click()
    let postedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    let lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandActiveConfigFile")
    expect(lastCommand.payload.index).toEqual(1)

    // Rename the second tab
    await configListPage.mobiFlightPage.trackCommand("CommandFileContextMenu")
    const secondTabContextMenu = page
      .getByRole("tablist")
      .getByRole("tab")
      .nth(1)
      .getByRole("button", { name: "Open menu" })
    await secondTabContextMenu.click()
    // the overlay is not nested in the dom
    // we have to use the page locator to find it
    await page.getByRole("menuitem", { name: "Rename" }).click()
    await secondTab
      .getByRole("button", { name: "Config 2" })
      .getByRole("textbox")
      .fill("Config 2 Renamed")
    await secondTab
      .getByRole("button", { name: "Config 2" })
      .getByRole("textbox")
      .press("Enter")

    postedCommands = await configListPage.mobiFlightPage.getTrackedCommands()
    lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandFileContextMenu")
    expect(lastCommand.payload.action).toEqual("rename")
    expect(lastCommand.payload.index).toEqual(1)
    expect(lastCommand.payload.file.Label).toEqual("Config 2 Renamed")

    // Remove the second tab
    await configListPage.mobiFlightPage.trackCommand("CommandFileContextMenu")

    await secondTabContextMenu.click()
    await page.getByRole("menuitem", { name: "Remove" }).click()
    postedCommands = await configListPage.mobiFlightPage.getTrackedCommands()
    lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandFileContextMenu")
    expect(lastCommand.payload.action).toEqual("remove")
    expect(lastCommand.payload.index).toEqual(1)
    expect(lastCommand.payload.file as ConfigFile).not.toBeNull()
  })
})

test.describe("Test profile tabs overflow features", () => {
  test.use({ viewport: { width: 800, height: 600 } })

  test("Confirm tab overflow indicators hide correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestDataAndSpecificProfileCount(
      1,
    )
    const addTabMenu = page.getByTestId("add-profile-tab-menu-regular")
    const addTabMenuOverflow = page.getByTestId("add-profile-tab-menu-overflow")

    const tabOverflowIndicatorLeft = page.getByTestId(
      "tab-overflow-indicator-left",
    )
    const tabOverflowIndicatorRight = page.getByTestId(
      "tab-overflow-indicator-right",
    )
    const scrollLeftButton = page.getByTestId("tab-scroll-left")
    const scrollRightButton = page.getByTestId("tab-scroll-right")

    // Initially, no overflow indicators should be visible
    await expect(tabOverflowIndicatorLeft).toBeHidden()
    await expect(tabOverflowIndicatorRight).toBeHidden()

    // No scroll buttons should be visible
    await expect(scrollLeftButton).toBeHidden()
    await expect(scrollRightButton).toBeHidden()

    // Tab menu should show regular menu
    await expect(addTabMenuOverflow).toBeHidden()
    await expect(addTabMenu).toBeVisible()
  })

  test("Confirm tab overflow indicators show correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    const addTabMenu = page.getByTestId("add-profile-tab-menu-regular")
    const addTabMenuOverflow = page.getByTestId("add-profile-tab-menu-overflow")

    const tabOverflowIndicatorLeft = page.getByTestId(
      "tab-overflow-indicator-left",
    )
    const tabOverflowIndicatorRight = page.getByTestId(
      "tab-overflow-indicator-right",
    )
    const scrollLeftButton = page.getByTestId("tab-scroll-left")
    const scrollRightButton = page.getByTestId("tab-scroll-right")

    // Add multiple tabs to cause overflow
    await expect(tabOverflowIndicatorLeft).toBeHidden()
    await expect(tabOverflowIndicatorRight).toBeVisible()

    // Scroll buttons should be visible
    await expect(scrollLeftButton).toBeVisible()
    await expect(scrollRightButton).toBeVisible()

    // Tab menu should show overflow menu
    await expect(addTabMenuOverflow).toBeVisible()
    await expect(addTabMenu).toBeHidden()

    // Scroll to the right to see the left overflow indicator
    // use mouse wheel
    const tabList = page.getByRole("tablist")
    await tabList.hover()
    await page.mouse.wheel(10, 0)

    await expect(tabOverflowIndicatorLeft).toBeVisible()
    await expect(tabOverflowIndicatorRight).toBeVisible()

    await expect(addTabMenu).toBeHidden()
    await expect(addTabMenuOverflow).toBeVisible()

    await page.mouse.wheel(1000, 0)
    await expect(tabOverflowIndicatorLeft).toBeVisible()
    await expect(tabOverflowIndicatorRight).toBeHidden()

    // tab menu updated
    // because we scrolled to the end of the tab list
    await expect(addTabMenu).toBeVisible()
    await expect(addTabMenuOverflow).toBeHidden()
  })

  test("Confirm scroll buttons work correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    // Add multiple tabs to cause overflow

    await configListPage.mobiFlightPage.initWithTestData()

    const scrollLeftButton = page.getByTestId("tab-scroll-left")
    const scrollRightButton = page.getByTestId("tab-scroll-right")

    const firstTab = page.getByRole("tablist").getByRole("tab").nth(0)
    const thirdTab = page.getByRole("tablist").getByRole("tab").nth(2)

    await expect(firstTab).toBeInViewport()
    await expect(thirdTab).not.toBeInViewport()

    // Scroll to the right end
    await scrollRightButton.click()
    await page.waitForTimeout(300)
    await scrollRightButton.click()
    await page.waitForTimeout(300)

    await expect(firstTab).not.toBeInViewport()
    await expect(thirdTab).toBeInViewport()

    // Scroll back to the left
    await scrollLeftButton.click()
    await page.waitForTimeout(300)
    await scrollLeftButton.click()
    await page.waitForTimeout(300)

    await expect(firstTab).toBeInViewport()
    await expect(thirdTab).not.toBeInViewport()
  })
})
