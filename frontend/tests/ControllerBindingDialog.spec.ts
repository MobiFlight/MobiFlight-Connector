import { test, expect } from "./fixtures"
import { ControllerBinding } from "../src/types/controller"
import { AppMessage, ControllerBindingsUpdate } from "../src/types/messages"

test("Confirm Controller Binding Dialog opens via main menu and closes correctly", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  const menuItemExtras = page
    .getByRole("menubar")
    .getByRole("menuitem", { name: "Extras" })
  const menuItemManageControllerBindings = page.getByRole("menuitem", {
    name: "Controller Bindings",
  })
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
  const manageControllerBindingsItem = page.getByRole("menuitem", {
    name: "Controller Bindings",
  })
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

  for (const controllerBinding of controllerBindings) {
    const name = controllerBinding.OriginalController?.Name
    const serial = controllerBinding.OriginalController?.Serial

    await expect(originalControllers.getByText(name)).toBeVisible()
    await expect(originalControllers.getByText(serial)).toBeVisible()

    if (controllerBinding.BoundController === null) continue

    const boundName = controllerBinding.BoundController.Name
    const boundSerial = controllerBinding.BoundController.Serial
    await expect(boundControllers.getByText(boundName)).toBeVisible()
    await expect(boundControllers.getByText(boundSerial)).toBeVisible()
  }
})

test("Confirm Controller Binding assignment works correctly", async ({
  configListPage,
  page,
}) => {
  const mobiFlightPage = configListPage.mobiFlightPage

  await configListPage.gotoPage()
  await mobiFlightPage.initWithTestData()
  await mobiFlightPage.openControllerBindingsDialog()

  // Start tracking commands after opening the dialog
  // this is necessary to use the correct page context
  mobiFlightPage.trackCommand("CommandControllerBindingsUpdate")

  const dialog = page.getByRole("dialog", { name: "Controller Bindings" })

  const manualFilterButton = dialog.getByRole("button", { name: "Manual" })
  await manualFilterButton.click()
  const controllerBindingOption = dialog
    .getByTestId("controller-binding-item")
    .filter({ visible: true })
    .first()
  const controllerDropDown = controllerBindingOption.getByRole("combobox")
  await controllerDropDown.click()
  const options = page.getByRole("listbox").getByRole("option")

  // click second option
  await options.nth(1).click()
  const saveButton = dialog.getByRole("button", { name: "Apply Changes" })
  await saveButton.click()
  await expect(dialog).not.toBeVisible()

  const commandsAfterClick = await mobiFlightPage.getTrackedCommands()
  expect(commandsAfterClick?.length).toBe(1)
  expect(commandsAfterClick![0].key).toBe("CommandControllerBindingsUpdate")
  const updatedBindings = commandsAfterClick![0].payload
    .bindings as ControllerBinding[]

  const updatedBinding = updatedBindings.find(
    (b) =>
      b.OriginalController?.Name == "Alpha Flight Controls" &&
      b.OriginalController?.Serial == "JS-b0875190-3b89-11ed-8007-444553540000",
  )
  expect(updatedBinding).toBeDefined()
  expect(updatedBinding!.BoundController?.Name).toBe(
    "Alpha Flight Controls Lite",
  )
  expect(updatedBinding!.BoundController?.Serial).toBe(
    "JS-c0875190-3b89-11ed-8007-444553540000",
  )
  expect(updatedBinding!.Status).toBe("Match")
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
    { filter: "Manual bind", expectedCount: 1 },
    { filter: "Match", expectedCount: 3 },
    { filter: "Missing", expectedCount: 2 },
    { filter: "All", expectedCount: 7 },
  ]

  for (const { filter, expectedCount } of filterTest) {
    const filterButton = dialog.getByRole("button", { name: filter })
    await filterButton.click()
    await expect(
      dialog.getByTestId("controller-binding-item").filter({ visible: true }),
    ).toHaveCount(expectedCount)
  }
})

test("Confirm Controller Binding Dialog filters correctly after updating status", async ({
  configListPage,
  page,
}) => {
  const mobiFlightPage = configListPage.mobiFlightPage

  await configListPage.gotoPage()
  await mobiFlightPage.initWithTestData()
  await mobiFlightPage.openControllerBindingsDialog()
  const dialog = page.getByRole("dialog", { name: "Controller Bindings" })

  // Update a binding and verify that the filtering still works correctly
  // The manual filter should now only show 1 item (the other one has been bound)
  // We will bind the controller that was previously unbound in the manual filter
  // And verify that you still can filter on the original status even though it has changed
  const manualFilterButton = dialog.getByRole("button", { name: "Manual bind" })
  await manualFilterButton.click()
  const controllerBindingOption = dialog
    .getByTestId("controller-binding-item")
    .filter({ visible: true })
    .first()
  const controllerDropDown = controllerBindingOption.getByRole("combobox")
  await controllerDropDown.click()

  const controllerDropDownList = page.getByRole("listbox")

  await expect(controllerDropDownList).toBeVisible()
  const options = controllerDropDownList.getByRole("option")
  await expect(options).toHaveCount(7)

  await options.nth(1).click()
  await expect(controllerDropDown).toHaveText(/Alpha Flight Controls Lite/)

  // Click on any other filter to update the controller bindings list
  const allFilterButton = dialog.getByRole("button", { name: "All" })
  await allFilterButton.click()
  await expect(
    dialog.getByTestId("controller-binding-item").filter({ visible: true }),
  ).toHaveCount(7)

  // then try to use the manual filter again
  // it will only be available if we are filtering results
  // based on original status, not current status (with current status the filter option would be disabled)
  await manualFilterButton.click()
  await expect(
    dialog.getByTestId("controller-binding-item").filter({ visible: true }),
  ).toHaveCount(1)
})

test.describe("Controller Bindings Update Message Tests", () => {
  test("Confirm controller bindings update in ControllerBindingDialog", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestDataAndSpecificProjectProps(
      {
        ControllerBindings: [],
      },
    )

    // Open the ControllerBindingDialog
    await configListPage.mobiFlightPage.openControllerBindingsDialog()

    // Get initial number of bindings displayed
    const controllerBindingItems = page.getByTestId("controller-binding-item")
    await expect(controllerBindingItems).toHaveCount(0)

    // Create updated bindings with different statuses
    const updatedBindings: ControllerBinding[] = [
      {
        BoundController: {
          Name: "Test Controller 1",
          Serial: "SN-TEST-001",
        },
        OriginalController: {
          Name: "Original Controller 1",
          Serial: "SN-ORIG-001",
        },
        Status: "Match",
      },
      {
        BoundController: null,
        OriginalController: {
          Name: "Missing Controller",
          Serial: "JS-MISSING-002",
        },
        Status: "Missing",
      },
      {
        BoundController: {
          Name: "Different Controller",
          Serial: "MI-DIFF-003",
        },
        OriginalController: {
          Name: "Original Controller 2",
          Serial: "MI-ORIG-003",
        },
        Status: "AutoBind",
      },
    ]

    const message: AppMessage = {
      key: "ControllerBindingsUpdate",
      payload: {
        Bindings: updatedBindings,
      } as ControllerBindingsUpdate,
    }

    // Send ControllerBindingsUpdate message
    await configListPage.mobiFlightPage.publishMessage(message)
    // Verify the number of rows matches
    await expect(controllerBindingItems).toHaveCount(3)

    const firstItem = controllerBindingItems.first()
    await expect(firstItem).toContainText("Missing Controller")

    const secondItem = controllerBindingItems.nth(1)
    await expect(secondItem).toContainText("Original Controller 2")

    const thirdItem = controllerBindingItems.nth(2)
    await expect(thirdItem).toContainText("Original Controller 1")
  })
})
