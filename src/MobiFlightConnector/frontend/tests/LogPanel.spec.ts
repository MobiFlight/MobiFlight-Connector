import { test, expect } from "./fixtures"

// Note: toggling the panel on/off via View > Toggle Log Panel is covered by
// MainMenu.spec.ts ("Confirm View > Toggle Log Panel shows and hides the log
// panel"). That's a menu-wiring concern; the tests below cover panel-owned
// behavior (the X button, re-opening, content rendering).
test("Log panel closes via X button", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.openLogPanel()

  const closeButton = page.getByRole("button", { name: "Close log panel" })
  await expect(closeButton).toBeVisible()

  await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")

  await closeButton.click()

  // The command should have been sent to the backend, and the panel should
  const trackedCommands = await configListPage.mobiFlightPage.getTrackedCommands()
  expect(trackedCommands).toContainEqual({
    key: "CommandMainMenu",
    payload: {
      action: "view.log.toggle",
    },
  })
})

test("Log panel shows empty placeholder before any messages arrive", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.openLogPanel()

  // No LogEntry messages have been sent, so the panel should show the
  // "Waiting for log entries" placeholder from LogPanel.Empty translation key.
  await expect(page.getByText("Waiting for log entries")).toBeVisible()
})

test("Log entry messages appear in the panel", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.openLogPanel()

  // Crucially, the panel must be mounted *before* sending messages —
  // useAppMessage subscribes on mount, so messages sent before mount are lost.
  await configListPage.mobiFlightPage.sendLogEntry("Info", "Hello from the test")

  await expect(page.getByText("Hello from the test")).toBeVisible()
})

test("Severity colours are applied to log entries", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.openLogPanel()

  // Set log level to trace so all severities pass the filter and render.
  // Without this, the default "info" level would hide debug entries before
  // they reach the DOM, making the colour assertion fail.
  await configListPage.mobiFlightPage.sendSettings({ LogLevel: "trace" })

  // Send one message per severity so all four colour classes get rendered.
  await configListPage.mobiFlightPage.sendLogEntry("Error", "error message")
  await configListPage.mobiFlightPage.sendLogEntry("Warn", "warn message")
  await configListPage.mobiFlightPage.sendLogEntry("Info", "info message")
  await configListPage.mobiFlightPage.sendLogEntry("Debug", "debug message")

  // The severity label span carries the colour class. Severity text is
  // lowercased in handleMessage() and displayed uppercase via Tailwind.
  // toHaveClass checks that the class is present among others on the element.
  const logContent = page.getByTestId("log-panel-content")

  await expect(
    logContent.locator('[data-severity="error"]').getByText("error", { exact: true }),
  ).toHaveClass(/text-red-500/)

  await expect(
    logContent.locator('[data-severity="warn"]').getByText("warn", { exact: true }),
  ).toHaveClass(/text-yellow-500/)

  await expect(
    logContent.locator('[data-severity="info"]').getByText("info", { exact: true }),
  ).toHaveClass(/text-blue-400/)

  await expect(
    logContent.locator('[data-severity="debug"]').getByText("debug", { exact: true }),
  ).toHaveClass(/text-gray-400/)
})

test("Default log level filters out debug messages", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.openLogPanel()

  // No Settings message sent → effectiveLevel defaults to "info" (see shouldShow()).
  // Debug (level 1) is below info (level 2), so it should be filtered out.
  await configListPage.mobiFlightPage.sendLogEntry("Debug", "this should be hidden")
  await configListPage.mobiFlightPage.sendLogEntry("Info", "this should be visible")

  await expect(page.getByText("this should be visible")).toBeVisible()
  await expect(page.getByText("this should be hidden")).not.toBeVisible()
})

test("Log panel height changes when title bar is dragged upward", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.openLogPanel()
  const logContent = page.getByTestId("log-panel")
  const before = await logContent.boundingBox()

  const separator = page.getByRole("separator")
  await expect(separator).toBeVisible()
  
  await separator.hover()
  await page.mouse.down()
  await page.mouse.move(0, -100)
  await page.mouse.up()

  const after = await logContent.boundingBox()
  expect(after!.height).toBeGreaterThan(before!.height)
})