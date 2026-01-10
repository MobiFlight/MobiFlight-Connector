import { test, expect } from "./fixtures"
import { IConfigItem } from "../src/types"

test("Confirm empty list view", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithEmptyData()
  const noConfigsMessage = page.getByText(
    "This is a new configuration. Please add some items.",
  )
  await expect(noConfigsMessage).toBeVisible()

  const addOutputConfigButton = page.getByRole("button", {
    name: "Add Output Config",
  })
  const addInputConfigButton = page.getByRole("button", {
    name: "Add Input Config",
  })

  await expect(addOutputConfigButton).toBeVisible()
  await expect(addInputConfigButton).toBeVisible()

  // the filter toolbar is disabled because we don't have any items.
  await expect(
    page.getByRole("textbox", { name: "Filter items..." }),
  ).toBeDisabled()
})

test("Confirm populated list view", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()
  await expect(
    page.getByRole("cell", { name: "No results." }),
  ).not.toBeVisible()
})

test("Confirm active toggle is working", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()
  await configListPage.setupConfigItemEditConfirmationResponse()
  const firstRow = page.locator("tbody tr").nth(1)
  const toggleSwitch = firstRow.getByRole("switch")
  await toggleSwitch.click()
  await expect(toggleSwitch).not.toBeChecked()
  await toggleSwitch.click()
  await expect(toggleSwitch).toBeChecked()
})

test("Confirm edit function for name is working", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()
  await configListPage.setupConfigItemEditConfirmationResponse()
  await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

  const nameCell = page.getByRole("row", { name: "LED 1" }).first()

  // Click on the text span to enter edit mode
  await nameCell.getByText("LED 1").dblclick()

  // Now find the textbox that appears after clicking
  const inlineEdit = nameCell.getByRole("textbox")
  await inlineEdit.fill("LED 1245")

  // We confirm the change by pressing Enter
  await page.keyboard.press("Enter")

  // The value should now be updated
  await expect(page.getByRole("cell", { name: "LED 1245" })).toBeVisible()

  // verify that the correct command was sent to the backend
  let postedCommands = await configListPage.mobiFlightPage.getTrackedCommands()
  const lastCommand = postedCommands!.pop()
  expect(lastCommand.key).toEqual("CommandUpdateConfigItem")
  expect((lastCommand.payload.item as IConfigItem).Name).toEqual("LED 1245")

  // Click on the text span to enter edit mode
  await nameCell.getByText("LED 1245").dblclick()
  await inlineEdit.fill("LED 9999")

  // We cancel the change by pressing Escape
  await page.keyboard.press("Escape")
  await expect(page.getByRole("cell", { name: "LED 1245" })).toBeVisible()

  postedCommands = await configListPage.mobiFlightPage.getTrackedCommands()
  expect(postedCommands?.length).toEqual(1)
})

test("Confirm status icons working", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  const PreconditionIcon = configListPage.getStatusIconInRow("Precondition", 1)
  const TestIcon = configListPage.getStatusIconInRow("Test", 1)
  const ConfigRefIcon = configListPage.getStatusIconInRow("ConfigRef", 1)

  const SourceIcon = configListPage.getStatusIconInRow("Source", 1)
  const DeviceIcon = configListPage.getStatusIconInRow("Device", 1)
  const ModifierIcon = configListPage.getStatusIconInRow("Modifier", 1)

  const statusTests = [
    {
      status: { Precondition: "not satisfied" },
      icon: PreconditionIcon,
      toolTipText: "Precondition is not satisfied.",
      alwaysVisible: true,
    },
    {
      status: { Source: "SIMCONNECT_NOT_AVAILABLE" },
      icon: SourceIcon,
      toolTipText: "This config uses SimConnect,",
      alwaysVisible: false,
    },
    {
      status: { Source: "FSUIPC_NOT_AVAILABLE" },
      icon: SourceIcon,
      toolTipText: "This config uses FSUIPC,",
      alwaysVisible: false,
    },
    {
      status: { Source: "XPLANE_NOT_AVAILABLE" },
      icon: SourceIcon,
      toolTipText: "This config uses X-Plane,",
      alwaysVisible: false,
    },
    {
      status: { Source: "PROSIM_NOT_AVAILABLE" },
      icon: SourceIcon,
      toolTipText: "This config uses ProSim,",
      alwaysVisible: false,
    },
    {
      status: { Device: "NotConnected" },
      icon: DeviceIcon,
      toolTipText: "The device used in this config is not connected.",
      alwaysVisible: false,
    },
    {
      status: { Modifier: "Error" },
      icon: ModifierIcon,
      toolTipText: "A modifier is applied which has an error.",
      alwaysVisible: false,
    },
    {
      status: { Test: "Executing" },
      icon: TestIcon,
      toolTipText: "This config is currently being tested.",
      alwaysVisible: true,
    },
    {
      status: { ConfigRef: "Missing" },
      icon: ConfigRefIcon,
      toolTipText: "One or more referenced configs are missing.",
      alwaysVisible: true,
    },
  ]

  for (const test of statusTests) {
    if (test.alwaysVisible) {
      await expect(test.icon).toBeVisible()
      await expect(test.icon).toHaveAttribute("aria-disabled", "true")
    } else {
      await expect(test.icon).not.toBeVisible()
    }

    await configListPage.updateConfigItemStatus(0, test.status)
    await expect(test.icon).toBeVisible()
    await expect(test.icon).toHaveAttribute("aria-disabled", "false")
    await test.icon.hover()
    await expect(
      configListPage.mobiFlightPage.getTooltipByText(test.toolTipText),
    ).toBeVisible()
    await page.click("body")
    await page.waitForTimeout(500)

    const statusKey = Object.keys(test.status)[0]
    await configListPage.removeConfigItemStatus(0, statusKey)
  }
})

test("Confirm status icons are initialized correctly when status is not present", async ({
  configListPage,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  const PreconditionIcon = configListPage.getStatusIconInRow("Precondition", 1)
  const TestIcon = configListPage.getStatusIconInRow("Test", 1)
  const ConfigRefIcon = configListPage.getStatusIconInRow("ConfigRef", 1)

  const statusTests = [
    {
      status: { Precondition: "not satisfied" },
      icon: PreconditionIcon,
      toolTipText: "Precondition is not satisfied.",
      alwaysVisible: true,
    },
    {
      status: { Test: "Executing" },
      icon: TestIcon,
      toolTipText: "This config is currently being tested.",
      alwaysVisible: true,
    },
    {
      status: { ConfigRef: "Missing" },
      icon: ConfigRefIcon,
      toolTipText: "One or more referenced configs are missing.",
      alwaysVisible: true,
    },
  ]

  // enable icons one by one and verify
  for (const test of statusTests) {
    await configListPage.updateConfigItemStatus(0, test.status)
    await expect(test.icon).toBeVisible()
    await expect(test.icon).toHaveAttribute("aria-disabled", "false")
  }

  const configItem = configListPage.getConfigItemByIndex(0)
  delete configItem.Status
  await configListPage.configValueFullUpdate([configItem], 0)

  // all icons should be disabled now
  await expect(PreconditionIcon).toHaveAttribute("aria-disabled", "true")
  await expect(TestIcon).toHaveAttribute("aria-disabled", "true")
  await expect(ConfigRefIcon).toHaveAttribute("aria-disabled", "true")
})

test.describe("Drag and drop tests", () => {
  test("Confirm single drag n drop is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandResortConfigItem")
    await page.getByRole("row").nth(1).getByRole("button").first().hover()
    await page.mouse.down()
    await page.mouse.move(10, 10)
    await page
      .getByRole("row", { name: "ShiftRegister" })
      .getByRole("button")
      .first()
      .hover()
    await page.mouse.up()
    await expect(page.getByRole("row").nth(6)).toContainText("7-Segment")

    const postedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    const lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandResortConfigItem")
    expect(lastCommand.payload.items.length).toEqual(1)
    expect(lastCommand.payload.items[0].Name).toEqual("7-Segment")
    expect(lastCommand.payload.newIndex).toEqual(5)
  })

  test("Confirm multi drag n drop is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandResortConfigItem")

    const firstRow = page.getByRole("row").nth(1)
    const thirdRow = page.getByRole("row").nth(3)
    const fifthRow = page.getByRole("row").nth(5)

    // select the first row
    await firstRow.click()
    await page.keyboard.down("Control")
    // add the third row to the selection
    await thirdRow.click()
    // let go of Control key otherwise drag n drop does not activate
    await page.keyboard.up("Control")

    // activate drag and drop after fifth
    const dragHandle = thirdRow.getByRole("button").first()
    await dragHandle.hover()
    await page.mouse.down()
    await page.mouse.move(10, 10)
    await expect(
      page.getByTestId("config-item-drag-overlay-inside-table"),
    ).toBeVisible()

    await fifthRow.getByRole("button").first().hover()
    await page.mouse.up()

    await expect(page.getByRole("row").nth(5)).toContainText("7-Segment")
    await expect(page.getByRole("row").nth(6)).toContainText("Servo")

    const postedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    const lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandResortConfigItem")
    expect(lastCommand.payload.items.length).toEqual(2)
    expect(lastCommand.payload.items[0].Name).toEqual("7-Segment")
    expect(lastCommand.payload.items[1].Name).toEqual("Servo")
    expect(lastCommand.payload.newIndex).toEqual(4)
  })

  test("Confirm multi drag n drop across tabs is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandResortConfigItem")

    const firstRow = page.getByRole("row").nth(1)
    const thirdRow = page.getByRole("row").nth(3)
    const secondTab = page.getByRole("tab").nth(1)

    // select the first row
    await firstRow.click()
    await page.keyboard.down("Control")
    // add the third row to the selection
    await thirdRow.click()
    await page.keyboard.up("Control")

    // activate drag by hovering over drag handle
    const dragHandle = thirdRow.getByRole("button").first()
    await dragHandle.hover()
    await page.mouse.down()
    await page.mouse.move(10, 10)
    await expect(
      page.getByTestId("config-item-drag-overlay-inside-table"),
    ).toBeVisible()

    // without the second mouse move
    // the hover won't trigger on the tab
    await page.mouse.move(10, 20)

    // drag over to the second tab to trigger cross-config move
    await secondTab.hover()

    // wait for the overlay to appear
    await expect(
      page.getByTestId("config-item-drag-overlay-outside-table"),
    ).toBeVisible()

    // ensure the tab switch has completed
    await expect(secondTab).toHaveAttribute("aria-selected", "true")

    // find a target row in the second tab to drop on
    const targetRowInSecondTab = page.getByRole("row").nth(1)
    await targetRowInSecondTab.hover()
    await page.mouse.up()

    // verify the items moved to the second tab
    // Note: You'll need to verify the actual items based on your test data structure
    // This assumes the second tab now shows the moved items
    await expect(page.getByRole("row").nth(1)).toContainText("7-Segment")
    await expect(page.getByRole("row").nth(2)).toContainText("Servo")

    const postedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    const lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandResortConfigItem")
    expect(lastCommand.payload.items.length).toEqual(2)
    expect(lastCommand.payload.items[0].Name).toEqual("7-Segment")
    expect(lastCommand.payload.items[1].Name).toEqual("Servo")
    expect(lastCommand.payload.sourceFileIndex).toEqual(0) // Original tab
    expect(lastCommand.payload.targetFileIndex).toEqual(1) // Second tab
    expect(lastCommand.payload.newIndex).toEqual(0) // Dropped at top of second tab
  })

  test("Confirm drag cancel is working on same tab", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandResortConfigItem")

    const firstRow = page.getByRole("row").nth(1)
    const thirdRow = page.getByRole("row").nth(3)
    const fifthRow = page.getByRole("row").nth(5)

    // select the first row
    await firstRow.click()
    await page.keyboard.down("Control")
    // add the third row to the selection
    await thirdRow.click()

    // activate drag
    const dragHandle = thirdRow.getByRole("button").first()
    await dragHandle.hover()
    await page.mouse.down()

    // and move after fifth
    await fifthRow.getByRole("button").first().hover()

    // but cancel by pressing Escape
    await page.keyboard.press("Escape")

    await expect(firstRow).toContainText("7-Segment")
    await expect(thirdRow).toContainText("Servo")

    const postedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    expect(postedCommands?.length ?? 0).toEqual(0)
  })

  test("Confirm drag cancel is working on different tab", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandResortConfigItem")

    const firstRow = page.getByRole("row").nth(1)
    const thirdRow = page.getByRole("row").nth(3)

    const secondTab = page.getByRole("tab").nth(1)

    // select the first row
    await firstRow.click()
    await page.keyboard.down("Control")
    // add the third row to the selection
    await thirdRow.click()

    // activate drag and drop after fifth
    const dragHandle = thirdRow.getByRole("button").first()
    await dragHandle.hover()
    await page.mouse.down()
    await secondTab.hover()
    await page.waitForTimeout(500)

    // but cancel by pressing Escape
    await page.keyboard.press("Escape")
    await expect(page.getByRole("row").nth(1)).toContainText("7-Segment")
    await expect(page.getByRole("row").nth(3)).toContainText("Servo")

    const postedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    expect(postedCommands?.length ?? 0).toEqual(0)
  })

  test("Confirm drag n drop is working with filter active", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandResortConfigItem")

    const firstRow = page.getByRole("row").nth(1)
    const secondTab = page.getByRole("tab").nth(1)

    await configListPage.filterByText("7-Segment")
    // only one row should be visible now
    await expect(page.getByRole("row")).toHaveCount(2)

    // select the first row
    await firstRow.click()

    // activate drag and drop
    const dragHandle = firstRow.getByRole("button").first()
    const placeholder = page.getByText("Drop here to add new items")

    await dragHandle.hover()
    await page.mouse.down()
    // move the mouse to start the drag
    await page.mouse.move(10, 10)

    // with active drag, the UI updates
    await expect(firstRow).not.toBeVisible()
    await expect(placeholder).toBeVisible()

    // drag over to the second tab to trigger cross-config move
    await secondTab.hover()

    // wait for the overlay to appear
    await expect(
      page.getByTestId("config-item-drag-overlay-outside-table"),
    ).toBeVisible()

    await secondTab.hover({ position: { x: 10, y: 10 } })

    // ensure the tab switch has completed
    await expect(secondTab).toHaveAttribute("aria-selected", "true")

    await expect(placeholder).toBeVisible()
    await placeholder.hover()
    await page.mouse.up()

    // verify the items moved to the second tab
    await expect(page.getByRole("row").nth(1)).toContainText("7-Segment")
  })
})

test("Confirm dark mode is working", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()
  await expect(page.locator("html")).toHaveAttribute("class", "light")
  await page.getByRole("button", { name: "Toggle dark mode" }).click()
  await expect(page.locator("html")).toHaveAttribute("class", "dark")
  await page.getByRole("button", { name: "Toggle light mode" }).click()
  await expect(page.locator("html")).toHaveAttribute("class", "light")
})

test("Confirm add output config is working", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.trackCommand("CommandAddConfigItem")

  const addOutputConfigButton = page.getByRole("button", {
    name: "Add Output Config",
  })
  await addOutputConfigButton.click()

  const postedCommands =
    await configListPage.mobiFlightPage.getTrackedCommands()
  const lastCommand = postedCommands!.pop()
  expect(lastCommand.key).toEqual("CommandAddConfigItem")
  expect(lastCommand.payload.type).toEqual("OutputConfig")
})

test("Confirm add input config is working", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.trackCommand("CommandAddConfigItem")

  const addOutputConfigButton = page.getByRole("button", {
    name: "Add Input Config",
  })
  await addOutputConfigButton.click()

  const postedCommands =
    await configListPage.mobiFlightPage.getTrackedCommands()
  const lastCommand = postedCommands!.pop()
  expect(lastCommand.key).toEqual("CommandAddConfigItem")
  expect(lastCommand.payload.type).toEqual("InputConfig")
})

test.describe("Filter toolbar tests", () => {
  test("Confirm `Search text` filter toolbar is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const searchTextBox = page.getByRole("textbox", { name: "Filter items" })
    const rows = page.locator("tbody tr")
    await expect(rows).toHaveCount(14)

    await searchTextBox.fill("A")
    await expect(rows).toHaveCount(2)

    await searchTextBox.fill("Ana")
    await expect(rows).toHaveCount(1)

    await searchTextBox.fill("Anaz")
    await expect(rows).toHaveCount(0)

    const clearButton = page.getByRole("button", { name: "Reset filters" })
    await expect(clearButton).not.toHaveCount(0)

    await clearButton.first().click()
    await expect(rows).toHaveCount(14)

    await configListPage.mobiFlightPage.trackCommand("CommandConfigBulkAction")
    await rows.first().click()
    await searchTextBox.press("Backspace")
    const postedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    await expect(postedCommands?.length).toBeUndefined()
  })

  test("Confirm `Config Type` filter toolbar is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const configTypeFilterButton = page.getByRole("button", {
      name: "Config Type",
    })
    const outputOption = page
      .getByRole("option", { name: "Output" })
      .locator("div")
    const inputOption = page
      .getByRole("option", { name: "Input" })
      .locator("div")
    const visibleRows = page.locator("tbody tr")
    const clearFiltersOption = page.getByRole("option", {
      name: "Clear filters",
    })

    await configTypeFilterButton.click()
    await outputOption.click()
    await expect(visibleRows).toHaveCount(7)

    await inputOption.click()
    await outputOption.click()
    await expect(visibleRows).toHaveCount(7)

    await clearFiltersOption.click()
    await expect(visibleRows).toHaveCount(14)

    const inputField = page.getByPlaceholder("Config Type")
    await inputField.click()
    await inputField.fill("In")
    await inputField.press("Enter")
    await expect(visibleRows).toHaveCount(7)
    await page.locator("#root").click()
    await page.waitForTimeout(500)

    await configTypeFilterButton.click()
    await clearFiltersOption.click()
    await inputField.click()
    await inputField.fill("Out")
    await inputField.press("Enter")
    await expect(visibleRows).toHaveCount(7)
  })

  test("Confirm `Controller` filter toolbar is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    const rows = page.locator("tbody tr")
    const clearFilterOption = page.getByRole("option", {
      name: "Clear filters",
    })

    await page.getByRole("button", { name: "Controller" }).click()
    await page
      .getByRole("option", { name: "WINWING Orion Joystick Base 2" })
      .locator("div")
      .click()
    await expect(rows).toHaveCount(1)
    await clearFilterOption.click()
    await expect(rows).toHaveCount(14)

    await page
      .getByRole("option", { name: "ProtoBoard" })
      .locator("div")
      .click()
    await expect(rows).toHaveCount(8)
    await clearFilterOption.click()

    await page
      .getByRole("option", { name: "not set" })
      .locator("div")
      .first()
      .click()
    await expect(rows).toHaveCount(1)
    await clearFilterOption.click()

    await page.getByPlaceholder("Controller").click()
    await page.getByPlaceholder("Controller").fill("Proto")
    await page.getByPlaceholder("Controller").press("Enter")
    await expect(rows).toHaveCount(8)
  })

  test("Confirm `Devices` filter toolbar is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    const rows = page.locator("tbody tr")
    const clearFilterOption = page.getByRole("option", {
      name: "Clear filters",
    })

    await page.getByRole("button", { name: "Devices" }).click()

    // Inline function to test device type filters
    const testDeviceTypeFilter = async (
      deviceType: string,
      expectedCount: number,
    ) => {
      await page.getByRole("option", { name: deviceType }).first().click()
      await expect(rows).toHaveCount(expectedCount)
      await clearFilterOption.click()
      await expect(rows).toHaveCount(14)
    }

    const deviceTypes = [
      ["Analog Input", 2],
      ["Encoder", 2],
      ["Button", 3],
      ["Output", 1],
      ["Output Shift Register", 1],
      ["LCD Display", 1],
      ["Servo", 1],
      ["Stepper", 1],
      ["7-Segment", 1],
      ["not set", 1],
    ] as Array<[string, number]>

    for (const [deviceTypeName, expectedCount] of deviceTypes) {
      await testDeviceTypeFilter(deviceTypeName, expectedCount)
    }
  })

  test("Confirm `Names` filter toolbar is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    const rows = page.locator("tbody tr")
    const clearFilterOption = page.getByRole("option", {
      name: "Clear filters",
    })

    await page.getByRole("button", { name: "Names" }).click()

    // Inline function to test device type filters
    const testDeviceNameFilter = async (
      deviceType: string,
      expectedCount: number,
    ) => {
      await page.getByRole("option", { name: deviceType }).first().click()
      await expect(rows).toHaveCount(expectedCount)
      await clearFilterOption.click()
      await expect(rows).toHaveCount(14)
    }

    const deviceNames = [
      "POT 1",
      "Button 4",
      "ShiftRegister 1",
      "LED 2",
      "LCD 1",
      "Servo 1",
      "Stepper 1",
      "7-Segment",
      "not set",
    ]

    for (const deviceName of deviceNames) {
      await testDeviceNameFilter(deviceName, 1)
    }
  })

  test("Confirm filter toolbar reset is working on Project change", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const searchTextBox = page.getByRole("textbox", { name: "Filter items" })
    await searchTextBox.fill("Test")
    await expect(searchTextBox).toHaveValue("Test")

    await configListPage.mobiFlightPage.initWithTestDataAndSpecificProjectProps(
      { Name: "Specific Project" },
    )

    await expect(searchTextBox).toHaveValue("")
  })

  test("Confirm notification is displayed when new items are added while filter is active", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    const searchTextBox = page.getByRole("textbox", { name: "Filter items" })
    await searchTextBox.fill("foobar")
    const clearButton = page
      .getByTestId("filter-toolbar")
      .getByRole("button", { name: "Reset filters" })
    await expect(clearButton).toHaveCount(1)

    const addOutputConfigButton = page.getByRole("button", {
      name: "Add Output Config",
    })
    await addOutputConfigButton.click()

    // simulate a newly created item
    await configListPage.addNewConfigItem("OutputConfigItem")

    // the config wizard opens and triggers this state
    await configListPage.setOverlayState({ Visible: true })
    const loaderOverlay = page.getByTestId("loader-overlay")
    await expect(loaderOverlay).toBeVisible()
    await expect(page.getByText("Opening wizard...")).toBeVisible()

    // the config wizard closes and triggers this state
    await configListPage.setOverlayState({ Visible: false })
    await expect(loaderOverlay).not.toBeVisible()

    const notification = page.getByRole("alert")
    await expect(
      notification.getByText("New config created but not visible."),
    ).toBeVisible()

    await notification.getByRole("button").click()
    await expect(searchTextBox).toHaveValue("")
    await expect(clearButton).toHaveCount(0)
  })
})

test.describe("Controller device labels are displayed correctly", () => {
  test("Confirm Joystick device labels are displayed correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initControllerDefinitions()
    await configListPage.mobiFlightPage.initWithTestData()
    await expect(
      page.getByRole("cell", { name: "Line-Select-Key 1L" }),
    ).toBeVisible()
  })

  test("Confirm Midi controller device labels are displayed correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initControllerDefinitions()
    await configListPage.mobiFlightPage.initWithTestData()

    const midiButtonRow = page
      .getByRole("row")
      .filter({ hasText: "Intech Studio: AC" })
      .first()
    await expect(
      midiButtonRow.getByRole("cell", { name: "Button 1" }),
    ).toBeVisible()

    const midiKnobRow = page
      .getByRole("row")
      .filter({ hasText: "Intech Studio: AC" })
      .nth(1)
    await expect(
      midiKnobRow.getByRole("cell", { name: "Knob 1" }),
    ).toBeVisible()

    const midiSliderRow = page
      .getByRole("row")
      .filter({ hasText: "Intech Studio: AC" })
      .nth(2)
    await expect(
      midiSliderRow.getByRole("cell", { name: "Slider 1" }),
    ).toBeVisible()
  })
})

test("Confirm `Controller Settings` link is working", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()
  await configListPage.mobiFlightPage.trackCommand("CommandConfigContextMenu")

  const firstRow = page.getByRole("row").nth(1)
  const controllerSettingsButton = firstRow.getByRole("link").first()

  await expect(controllerSettingsButton).toHaveCSS("opacity", "0")
  await firstRow.hover()
  
  await expect(controllerSettingsButton).toHaveCSS("opacity", "0.25")

  await controllerSettingsButton.hover()
  await expect(controllerSettingsButton).toHaveCSS("opacity", "1")
  await controllerSettingsButton.click()
  
  const postedCommands =
    await configListPage.mobiFlightPage.getTrackedCommands()
  expect(postedCommands!.pop().key).toEqual("CommandConfigContextMenu")
})

test("Confirm Raw and Final Value update correctly", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  const rawValue = page.getByRole("row").nth(1).getByTestId("raw-value")
  const finalValue = page.getByRole("row").nth(1).getByTestId("final-value")

  await configListPage.updateConfigItemRawAndFinalValue(0, "1234", "5678")
  await expect(rawValue).toHaveText("1234")
  await expect(finalValue).toHaveText("5678")

  await configListPage.updateConfigItemRawAndFinalValue(0, "4321", "8765")
  await expect(rawValue).toHaveText("4321")
  await expect(finalValue).toHaveText("8765")
})

test.describe("Responsiveness: Full Screen", () => {
  test.use({ viewport: { width: 1920, height: 1080 } })
  test("Confirm `Columns are visible` in full screen", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const columnHeaders = [
      { name: "Active", visible: true },
      { name: "Name", visible: true },
      { name: "Controller", visible: true },
      { name: "Device", visible: true },
      { name: "Status", visible: true },
      { name: "Raw Value", visible: true },
      { name: "Final Value", visible: true },
      { name: "Actions", visible: true },
    ]

    for (const header of columnHeaders) {
      const column = page
        .getByRole("columnheader", { name: header.name })
        .first()

      if (!header.visible) {
        await expect(column).not.toBeVisible()
        continue
      }
      await expect(column).toBeVisible()
    }
  })
})

test.describe("Responsiveness: Medium Screen", () => {
  test.use({ viewport: { width: 800, height: 600 } })
  test("Confirm `Columns are visible` in full screen", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const columnHeaders = [
      { name: "Active", visible: true },
      { name: "Name", visible: true },
      { name: "Controller", visible: false },
      { name: "Device", visible: true },
      { name: "Status", visible: true },
      { name: "Raw Value", visible: true },
      { name: "Final Value", visible: true },
      { name: "Actions", visible: true },
    ]

    for (const header of columnHeaders) {
      const column = page
        .getByRole("columnheader", { name: header.name })
        .first()

      if (!header.visible) {
        await expect(column).not.toBeVisible()
        continue
      }
      await expect(column).toBeVisible()
    }
  })
})

test.describe("Responsiveness: Small Screen", () => {
  test.use({ viewport: { width: 784, height: 600 } })
  test("Confirm `Columns are visible` in full screen", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const columnHeaders = [
      { name: "Active", visible: true },
      { name: "Name", visible: true },
      { name: "Controller", visible: false },
      { name: "Device", visible: true },
      { name: "Status", visible: true },
      { name: "Raw Value", visible: true },
      { name: "Final Value", visible: true },
      { name: "Actions", visible: true },
    ]

    for (const header of columnHeaders) {
      const column = page
        .getByRole("columnheader", { name: header.name })
        .first()

      if (!header.visible) {
        await expect(column).not.toBeVisible()
        continue
      }
      await expect(column).toBeVisible()
    }
  })
})

test.describe("Selection: Select / Deselect actions", () => {
  test.use({ viewport: { width: 1024, height: 800 } })
  test("Confirm `selection` of single item is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandConfigBulkAction")

    const firstRow = page.getByRole("row").nth(1)
    const selectionButton = page.getByRole("button", { name: "rows selected" })

    await firstRow.click()
    await expect(selectionButton).toHaveText("1 rows selected")
  })

  test("Confirm `selection` of multiple items is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandConfigBulkAction")

    const firstRow = page.getByRole("row").nth(1)
    const fourthRow = page.getByRole("row").nth(4)
    const selectionButton = page.getByRole("button", { name: "rows selected" })

    await firstRow.click()
    await expect(selectionButton).toHaveText("1 rows selected")
    await page.keyboard.down("Shift")
    await fourthRow.click()
    await page.keyboard.up("Shift")
    await expect(selectionButton).toHaveText("4 rows selected")
  })

  test("Confirm `selection` context menu is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandConfigBulkAction")

    const firstRow = page.getByRole("row").nth(1)
    const fourthRow = page.getByRole("row").nth(4)
    const selectionButton = page.getByRole("button", { name: "rows selected" })
    const selectionDeleteButton = page.getByRole("option", {
      name: "Delete selected",
    })
    const selectionToggleButton = page.getByRole("option", {
      name: "Toggle selected",
    })
    const selectionClearButton = page.getByRole("option", {
      name: "Clear selection",
    })

    await firstRow.click()
    await page.keyboard.down("Shift")
    await fourthRow.click()
    await page.keyboard.up("Shift")
    await selectionButton.click()

    await expect(selectionDeleteButton).toBeVisible()
    await expect(selectionToggleButton).toBeVisible()
    await expect(selectionClearButton).toBeVisible()
  })

  test("Confirm `delete` of multiple items is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandConfigBulkAction")

    const firstRow = page.getByRole("row").nth(1)
    const fourthRow = page.getByRole("row").nth(4)
    const selectionButton = page.getByRole("button", { name: "rows selected" })
    const selectionDeleteButton = page.getByRole("option", {
      name: "Delete selected",
    })

    await firstRow.click()
    await page.keyboard.down("Shift")
    await fourthRow.click()
    await page.keyboard.up("Shift")
    await selectionButton.click()

    await selectionDeleteButton.click()
    const postedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    const lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandConfigBulkAction")
    expect(lastCommand.payload.action).toEqual("delete")

    await expect(selectionButton).not.toBeVisible()
  })

  test("Confirm `toggle` of multiple items is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandConfigBulkAction")

    const firstRow = page.getByRole("row").nth(1)
    const fourthRow = page.getByRole("row").nth(4)
    const selectionButton = page.getByRole("button", { name: "rows selected" })
    const selectionToggleButton = page.getByRole("option", {
      name: "Toggle selected",
    })

    await firstRow.click()
    await page.keyboard.down("Shift")
    await fourthRow.click()
    await page.keyboard.up("Shift")
    await selectionButton.click()

    await selectionToggleButton.click()
    const postedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    const lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandConfigBulkAction")
    expect(lastCommand.payload.action).toEqual("toggle")
  })
})
