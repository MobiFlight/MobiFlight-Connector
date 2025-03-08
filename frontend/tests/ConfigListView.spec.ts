import { test, expect } from "./fixtures"

test("Confirm empty list view", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithEmptyData()
  await expect(
    page.getByRole("cell", {
      name: "This is a new configuration. Please add some items.",
    }),
  ).toBeVisible()
  await expect(
    page.getByRole("button", { name: "Add Output Config" }),
  ).toBeVisible()
  await expect(
    page.getByRole("button", { name: "Add Input Config" }),
  ).toBeVisible()
  await expect(
    page.getByRole("textbox", { name: "Filter items..." }),
  ).toBeVisible()
  await page.getByRole("button", { name: "Devices" }).click()
  await expect(page.getByText("No results found.")).toBeVisible()
  await page.getByRole("button", { name: "Type" }).click()
  await page.waitForTimeout(500)
  await expect(
    page.getByRole("listbox").getByText("No results found.", { exact: true }),
  ).toBeVisible()
  await page.getByRole("button", { name: "Name" }).click()
  await page.waitForTimeout(500)
  await expect(page.getByText("No results found.")).toBeVisible()
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
  const SourceIcon = configListPage.getStatusIconInRow("Source", 1)
  const DeviceIcon = configListPage.getStatusIconInRow("Device", 1)
  const ModifierIcon = configListPage.getStatusIconInRow("Modifier", 1)
  const TestIcon = configListPage.getStatusIconInRow("Test", 1)
  const ConfigRefIcon = configListPage.getStatusIconInRow("ConfigRef", 1)

  const statusTests = [
    {
      status: { Precondition: "not satisfied" },
      icon: PreconditionIcon,
      toolTipText: "Precondition is not satisfied.",
    },
    {
      status: { Source: "SIMCONNECT_NOT_AVAILABLE" },
      icon: SourceIcon,
      toolTipText: "This config uses SimConnect,",
    },
    {
      status: { Source: "FSUIPC_NOT_AVAILABLE" },
      icon: SourceIcon,
      toolTipText: "This config uses FSUIPC,",
    },
    {
      status: { Source: "XPLANE_NOT_AVAILABLE" },
      icon: SourceIcon,
      toolTipText: "This config uses X-Plane,",
    },
    {
      status: { Device: "NotConnected" },
      icon: DeviceIcon,
      toolTipText: "The device used in this config is not connected.",
    },
    {
      status: { Modifier: "Error" },
      icon: ModifierIcon,
      toolTipText: "A modifier is applied which has an error.",
    },
    {
      status: { Test: "Executing" },
      icon: TestIcon,
      toolTipText: "This config is currently being tested.",
    },
    {
      status: { ConfigRef: "Missing" },
      icon: ConfigRefIcon,
      toolTipText: "One or more referenced configs are missing.",
    },
  ]

  statusTests.forEach(async (item) => {
    expect(item.icon).toHaveAttribute("aria-disabled", "true")
  })

  for (const test of statusTests) {
    await configListPage.updateConfigItemStatus(0, test.status)
    await expect(test.icon).toHaveAttribute("aria-disabled", "false")
    await test.icon.hover()
    await expect(
      configListPage.mobiFlightPage.getTooltipByText(test.toolTipText),
    ).toBeVisible()
    await page.click("body")
    await page.waitForTimeout(500)
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
  await page.getByRole("button", { name: "Controller" }).click()
  await page
    .getByRole("option", { name: "WINWING Orion Joystick Base 2" })
    .locator("div")
    .click()
  await expect(page.locator("tbody tr")).toHaveCount(1)
  await page.getByRole("option", { name: "Clear filters" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(10)

  await page.getByRole("option", { name: "ProtoBoard" }).locator("div").click()
  await expect(page.locator("tbody tr")).toHaveCount(8)
  await page.getByRole("option", { name: "Clear filters" }).click()

  await page
    .getByRole("option", { name: "not set" })
    .locator("div")
    .first()
    .click()
  await expect(page.locator("tbody tr")).toHaveCount(1)
  await page.getByRole("option", { name: "Clear filters" }).click()

  await page.getByPlaceholder("Controller").click()
  await page.getByPlaceholder("Controller").fill("Proto")
  await page.getByPlaceholder("Controller").press("Enter")
  await expect(page.locator("tbody tr")).toHaveCount(8)
})

test("Confirm `Devices` filter toolbar is working", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await page.getByRole("button", { name: "Devices" }).click()
  await page.getByRole("option", { name: "Output Shift Register" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(1)
  await page.getByRole("option", { name: "Clear filters" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(10)

  await page.getByRole("option", { name: "Output" }).first().click()
  await expect(page.locator("tbody tr")).toHaveCount(1)
  await page.getByRole("option", { name: "Clear filters" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(10)

  await page.getByRole("option", { name: "LCD Display" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(1)
  await page.getByRole("option", { name: "Clear filters" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(10)

  await page.getByRole("option", { name: "Servo" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(1)
  await page.getByRole("option", { name: "Clear filters" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(10)

  await page.getByRole("option", { name: "Stepper" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(1)
  await page.getByRole("option", { name: "Clear filters" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(10)

  await page.getByRole("option", { name: "7-Segment" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(1)
  await page.getByRole("option", { name: "Clear filters" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(10)

  await page.getByRole("option", { name: "not set" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(1)
  await page.getByRole("option", { name: "Clear filters" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(10)
})

test("Confirm `Names` filter toolbar is working", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await page.getByRole("button", { name: "Names" }).click()

  await page
    .getByRole("option", { name: "ShiftRegister" })
    .locator("div")
    .click()
  await expect(page.locator("tbody tr")).toHaveCount(1)
  await page.getByRole("option", { name: "Clear filters" }).click()
  await expect(page.locator("tbody tr")).toHaveCount(10)

  await page
    .getByRole("option", { name: "not set" })
    .locator("div")
    .first()
    .click()
  await expect(page.locator("tbody tr")).toHaveCount(1)
  await page.getByRole("option", { name: "Clear filters" }).click()

  await expect(page.locator("tbody tr")).toHaveCount(10)
})
