import { test, expect } from "./fixtures"

test("Confirm Controller Binding Dialog opens via main menu and closes correctly", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  const menuItemExtras = page.getByRole("menubar").getByRole("menuitem", { name: "Extras" })
  const menuItemManageControllerBindings = page.getByRole("menuitem", { name: "Controller Bindings" })
  const dialog = page.getByRole("dialog", { name: "Controller Bindings" })
  const closeButton = dialog.getByRole("button", { name: "Close" }).first()
  
  await expect(dialog).not.toBeVisible()
  await expect(menuItemExtras).toBeVisible()
  
  await menuItemExtras.click()
  await expect(menuItemManageControllerBindings).toBeVisible()
  await menuItemManageControllerBindings.click()
  
  await expect(dialog).toBeVisible()
  await expect(closeButton).toBeVisible()
  await closeButton.click()

  await expect(dialog).not.toBeVisible()
})

test("Confirm Controller Binding Dialog opens via Project card and closes correctly", async ({
  dashboardPage,
  page,
}) => {
  await dashboardPage.gotoPage()
  await dashboardPage.mobiFlightPage.initWithTestData()

  const projectCard = page.getByTestId("project-card")
  const projectMenu = projectCard.getByRole("button", { name: "Open menu" })
  
  const dialog = page.getByRole("dialog", { name: "Controller Bindings" })
  const closeButton = dialog.getByRole("button", { name: "Close" }).first()
  
  await expect(dialog).not.toBeVisible()

  await projectMenu.click()
  const manageControllerBindingsItem = page.getByRole("menuitem", { name: "Controller Bindings" })
  await manageControllerBindingsItem.click()
    
  await expect(dialog).toBeVisible()
  await expect(closeButton).toBeVisible()
  await closeButton.click()
})

test("Confirm Controller Binding Dialog shows correct information", async ({
  configListPage,
  page,
}) => {
  const mobiFlightPage = configListPage.mobiFlightPage

  await configListPage.gotoPage()
  await mobiFlightPage.initWithTestData()
  await mobiFlightPage.openControllerBindingsDialog()
  const dialog = page.getByRole("dialog", { name: "Controller Bindings" })
  const controllerBindings = mobiFlightPage.getControllerBindings()
  const originalControllers = dialog.getByTestId("original-controller")
  const boundControllers = dialog.getByTestId("bound-controller")

  const filterAll = dialog.getByRole("button", { name: "All" })
  await filterAll.click()

  for(const controllerBinding of controllerBindings) {
    const [ name, serial ] = controllerBinding.OriginalController.split("/").map((s: string) => s.trim())
    await expect(originalControllers.getByText(name)).toBeVisible()
    await expect(originalControllers.getByText(serial)).toBeVisible()

    if (controllerBinding.BoundController === null) continue

    const [ boundName, boundSerial ] = controllerBinding.BoundController.split("/").map((s: string) => s.trim())
    await expect(boundControllers.getByText(boundName)).toBeVisible()
    await expect(boundControllers.getByText(boundSerial)).toBeVisible()
  }
})

test("Confirm Controller Binding Dialog filters correctly", async ({
  configListPage,
  page,
}) => {
  const mobiFlightPage = configListPage.mobiFlightPage

  await configListPage.gotoPage()
  await mobiFlightPage.initWithTestData()
  await mobiFlightPage.openControllerBindingsDialog()
  const dialog = page.getByRole("dialog", { name: "Controller Bindings" })

  const filterTest = [
    { filter: "Auto-bind", expectedCount: 1 },
    { filter: "Manual", expectedCount: 1 },
    { filter: "Match", expectedCount: 3 },
    { filter: "Missing", expectedCount: 2 },
    { filter: "All", expectedCount: 7 },
  ]

  for (const { filter, expectedCount } of filterTest) {
    const filterButton = dialog.getByRole("button", { name: filter })
    await filterButton.click()
    await expect(dialog.getByTestId("controller-binding-item").filter({ visible: true })).toHaveCount(expectedCount)
  }
})
