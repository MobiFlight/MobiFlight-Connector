import { test, expect } from "./fixtures"

test("Confirm empty list view", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithEmptyData()
  const noConfigsMessage = page.getByText("This is a new configuration. Please add some items.")
  await expect(noConfigsMessage).toBeVisible()

  const addOutputConfigButton = page.getByRole("button", { name: "Add Output Config" })
  const addInputConfigButton = page.getByRole("button", { name: "Add Input Config" })

  await expect(addOutputConfigButton).toBeVisible()
  await expect(addInputConfigButton).toBeVisible()

  // the filter toolbar is not yet visible because we don't have any items
  await expect(
    page.getByRole("textbox", { name: "Filter items..." }),
  ).not.toBeVisible()
})

test("Confirm populated list view", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await expect(
    page.getByRole("cell", { name: "No results." }),
  ).not.toBeVisible()
})

test("Confirm active toggle is working", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await configListPage.setupConfigItemEditConfirmationResponse()
  const rowSelector = { name: "LED 1 Edit ProtoBoard-v2" }
  const toggleSwitch = page.getByRole("row", rowSelector).getByRole("switch")
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
  await configListPage.initWithTestData()
  await configListPage.setupConfigItemEditConfirmationResponse()
  await page
    .getByRole("cell", { name: "LED 1" })
    .getByRole("button", { name: "Edit" })
    .click()
  await page.locator('input[type="text"]').click()
  await page.locator('input[type="text"]').fill("LED 1245")
  await page
    .getByRole("cell", { name: "LED 1245" })
    .getByRole("button", { name: "Save" })
    .click()
  await expect(page.getByRole("cell", { name: "LED 1245" })).toBeVisible()
  await page
    .getByRole("cell", { name: "LED 1245" })
    .getByRole("button", { name: "Edit" })
    .click()
  await page.locator('input[type="text"]').click()
  await page.locator('input[type="text"]').fill("LED 9999")
  await page
    .getByRole("cell", { name: "LED 9999" })
    .getByRole("button", { name: "Discard" })
    .click()
  await expect(page.getByRole("cell", { name: "LED 1245" })).toBeVisible()
})

test("Confirm status icons working", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()

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

    const statusKey = Object.keys(test.status)[0];
    await configListPage.removeConfigItemStatus(0, statusKey);
  }
})

test("Confirm drag n drop is working", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await page.getByRole("row").nth(1).getByRole("button").first().hover()
  await page.mouse.down()
  await page
    .getByRole("row", { name: "ShiftRegister" })
    .getByRole("button")
    .first()
    .hover()
  await page.mouse.up()
  await expect(page.getByRole("row").nth(6)).toContainText("7-Segment")
})

test("Confirm dark mode is working", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await expect(page.locator("html")).toHaveAttribute("class", "light")
  await page.getByRole("button", { name: "Toggle dark mode" }).click()
  await expect(page.locator("html")).toHaveAttribute("class", "dark")
  await page.getByRole("button", { name: "Toggle light mode" }).click()
  await expect(page.locator("html")).toHaveAttribute("class", "light")
})

test("Confirm `Search text` filter toolbar is working", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()

  const searchTextBox = page.getByRole("textbox", { name: "Filter items" })
  const rows = page.locator("tbody tr")
  await expect(rows).toHaveCount(10)

  await searchTextBox.fill("A")
  await expect(rows).toHaveCount(2)

  await searchTextBox.fill("Ana")
  await expect(rows).toHaveCount(1)

  await searchTextBox.fill("Anaz")
  await expect(rows).toHaveCount(0)

  const clearButton = page.getByRole("button", { name: "Reset filters" })
  await expect(clearButton).not.toHaveCount(0)

  await clearButton.first().click()
  await expect(rows).toHaveCount(10)
})

test("Confirm `Config Type` filter toolbar is working", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()

  const configTypeFilterButton = page.getByRole("button", {
    name: "Config Type",
  })
  const outputOption = page
    .getByRole("option", { name: "Output" })
    .locator("div")
  const inputOption = page.getByRole("option", { name: "Input" }).locator("div")
  const visibleRows = page.locator("tbody tr")
  const clearFiltersOption = page.getByRole("option", { name: "Clear filters" })

  await configTypeFilterButton.click()
  await outputOption.click()
  await expect(visibleRows).toHaveCount(7)

  await inputOption.click()
  await outputOption.click()
  await expect(visibleRows).toHaveCount(3)

  await clearFiltersOption.click()
  await expect(visibleRows).toHaveCount(10)

  const inputField = page.getByPlaceholder("Config Type")
  await inputField.click()
  await inputField.fill("In")
  await inputField.press("Enter")
  await expect(visibleRows).toHaveCount(3)
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
  await configListPage.initWithTestData()
  const rows = page.locator("tbody tr")
  const clearFilterOption = page.getByRole("option", { name: "Clear filters" })

  await page.getByRole("button", { name: "Controller" }).click()
  await page
    .getByRole("option", { name: "WINWING Orion Joystick Base 2" })
    .locator("div")
    .click()
  await expect(rows).toHaveCount(1)
  await clearFilterOption.click()
  await expect(rows).toHaveCount(10)

  await page.getByRole("option", { name: "ProtoBoard" }).locator("div").click()
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
  await configListPage.initWithTestData()
  const rows = page.locator("tbody tr")
  const clearFilterOption = page.getByRole("option", { name: "Clear filters" })

  await page.getByRole("button", { name: "Devices" }).click()
  
  // Inline function to test device type filters
  const testDeviceTypeFilter = async (deviceType: string, expectedCount: number) => {
    await page.getByRole("option", { name: deviceType }).first().click()
    await expect(rows).toHaveCount(expectedCount)
    await clearFilterOption.click()
    await expect(rows).toHaveCount(10)
  }

  const deviceTypes = [
    "Analog Input",
    "Encoder",
    "Button",
    "Output",
    "Output Shift Register",
    "LCD Display",
    "Servo",
    "Stepper",
    "7-Segment",
    "not set",
  ]

  for (const deviceType of deviceTypes) {
    await testDeviceTypeFilter(deviceType, 1)
  }

})

test("Confirm `Names` filter toolbar is working", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  const rows = page.locator("tbody tr")
  const clearFilterOption = page.getByRole("option", { name: "Clear filters" })

  await page.getByRole("button", { name: "Names" }).click()

  // Inline function to test device type filters
  const testDeviceNameFilter = async (deviceType: string, expectedCount: number) => {
    await page.getByRole("option", { name: deviceType }).first().click()
    await expect(rows).toHaveCount(expectedCount)
    await clearFilterOption.click()
    await expect(rows).toHaveCount(10)
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

test("Confirm `Controller Settings` link is working", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await configListPage.mobiFlightPage.trackCommand("CommandConfigContextMenu")

  const controllerSettingsLabel = page.getByRole('row').nth(1).getByText('ProtoBoard')
  const controllerSettingsButton = page.getByRole("link").first()
  await expect (controllerSettingsButton).toHaveCSS('opacity', '0')
  await controllerSettingsLabel.hover()
  await expect (controllerSettingsButton).toHaveCSS('opacity', '1')
  await controllerSettingsButton.click()
  const postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  expect (postedCommands!.pop().key).toEqual('CommandConfigContextMenu')
})

test.describe('Responsiveness: Full Screen', () => {
  test.use({ viewport: { width: 1920, height: 1080 } });
  test("Confirm `Columns are visible` in full screen", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    const activeColumn = page.getByRole("cell", { name: "Active" }).first() 
    const nameColumn = page.getByRole("cell", { name: "Name" }).first() 
    const controllerColumn = page.getByRole("cell", { name: "Controller" }).first() 
    const deviceColumn = page.getByRole("cell", { name: "Device" }).first() 
    const statusColumn = page.getByRole("cell", { name: "Status" }).first() 
    const rawValueColumn = page.getByRole("cell", { name: "Raw Value" }).first() 
    const finalValueColumn = page.getByRole("cell", { name: "Final Value" }).first() 
    const actionsColumn = page.getByRole("cell", { name: "Actions" }).first() 

    await expect(activeColumn).toBeVisible()
    await expect(nameColumn).toBeVisible()
    await expect(controllerColumn).toBeVisible()
    await expect(deviceColumn).toBeVisible()
    await expect(statusColumn).toBeVisible()
    await expect(rawValueColumn).toBeVisible()
    await expect(finalValueColumn).toBeVisible()
    await expect(actionsColumn).toBeVisible()
  })
})

test.describe('Responsiveness: Medium Screen', () => {
  test.use({ viewport: { width: 800, height: 600 } });
  test("Confirm `Columns are visible` in full screen", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    const activeColumn = page.getByRole("cell", { name: "Active" }).first() 
    const nameColumn = page.getByRole("cell", { name: "Name" }).first() 
    const controllerColumn = page.getByRole("cell", { name: "Controller" }).first() 
    const deviceColumn = page.getByRole("cell", { name: "Device" }).first() 
    const statusColumn = page.getByRole("cell", { name: "Status" }).first() 
    const rawValueColumn = page.getByRole("cell", { name: "Raw Value" }).first() 
    const finalValueColumn = page.getByRole("cell", { name: "Final Value" }).first() 
    const actionsColumn = page.getByRole("cell", { name: "Actions" }).first() 

    await expect(activeColumn).toBeVisible()
    await expect(nameColumn).toBeVisible()
    await expect(controllerColumn).not.toBeVisible()
    await expect(deviceColumn).toBeVisible()
    await expect(statusColumn).toBeVisible()
    await expect(rawValueColumn).toBeVisible()
    await expect(finalValueColumn).toBeVisible()
    await expect(actionsColumn).toBeVisible()
  })
})

test.describe('Responsiveness: Small Screen', () => {
  test.use({ viewport: { width: 784, height: 600 } });
  test("Confirm `Columns are visible` in full screen", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    const activeColumn = page.getByRole("cell", { name: "Active" }).first() 
    const nameColumn = page.getByRole("cell", { name: "Name" }).first() 
    const controllerColumn = page.getByRole("cell", { name: "Controller" }).first() 
    const deviceColumn = page.getByRole("cell", { name: "Device" }).first() 
    const statusColumn = page.getByRole("cell", { name: "Status" }).first() 
    const rawValueColumn = page.getByRole("cell", { name: "Raw Value" }).first() 
    const finalValueColumn = page.getByRole("cell", { name: "Final Value" }).first() 
    const actionsColumn = page.getByRole("cell", { name: "Actions" }).first() 

    await expect(activeColumn).toBeVisible()
    await expect(nameColumn).toBeVisible()
    await expect(controllerColumn).not.toBeVisible()
    await expect(deviceColumn).toBeVisible()
    await expect(statusColumn).toBeVisible()
    await expect(rawValueColumn).toBeVisible()
    await expect(finalValueColumn).toBeVisible()
    await expect(actionsColumn).toBeVisible()
  })
})
