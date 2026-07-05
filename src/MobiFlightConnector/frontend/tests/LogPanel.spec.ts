import { test, expect } from "./fixtures"
import Settings from "../src/types/settings"

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
  const trackedCommands =
    await configListPage.mobiFlightPage.getTrackedCommands()
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
  await configListPage.mobiFlightPage.sendLogEntry(
    "info",
    "Hello from the test",
    "2026-07-05T12:34:56.789Z"
  )

  await configListPage.mobiFlightPage.openLogPanel()
  await expect(page.getByText("Hello from the test")).toBeVisible()
  await expect(page.getByText("[12:34:56]")).toBeVisible()

  await configListPage.mobiFlightPage.sendLogEntry(
    "info",
    "Single digit time components",
    "2026-07-05T01:02:03.300Z"
  )

  await expect(page.getByText("Single digit time components")).toBeVisible()
  await expect(page.getByText("[01:02:03]")).toBeVisible()
})

test("Severity colours are applied to log entries", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  // First, set the log level to Debug so all severities pass the filter and render.
  // Without this, the default "Info" level would hide Debug entries before
  // they reach the DOM, making the colour assertion fail.
  await configListPage.mobiFlightPage.sendSettings({
    LogEnabled: true,
    LogLevel: "debug",
  } as Partial<Settings>)

  // Send one message per severity so all four colour classes get rendered.
  await configListPage.mobiFlightPage.sendLogEntry("error", "error message")
  await configListPage.mobiFlightPage.sendLogEntry("warn", "warn message")
  await configListPage.mobiFlightPage.sendLogEntry("info", "info message")
  await configListPage.mobiFlightPage.sendLogEntry("debug", "debug message")

  // The severity label span carries the colour class. Severity text is
  // lowercased in handleMessage() and displayed uppercase via Tailwind.
  // toHaveClass checks that the class is present among others on the element.
  const logContent = page.getByTestId("log-panel-content")

  await expect(
    logContent
      .locator('[data-severity="error"]')
      .getByText("error", { exact: true }),
  ).toHaveClass(/text-red-500/)

  await expect(
    logContent
      .locator('[data-severity="warn"]')
      .getByText("warn", { exact: true }),
  ).toHaveClass(/text-yellow-500/)

  await expect(
    logContent
      .locator('[data-severity="info"]')
      .getByText("info", { exact: true }),
  ).toHaveClass(/text-blue-400/)

  await expect(
    logContent
      .locator('[data-severity="debug"]')
      .getByText("debug", { exact: true }),
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
  await configListPage.mobiFlightPage.sendLogEntry(
    "debug",
    "this should be hidden",
  )
  await configListPage.mobiFlightPage.sendLogEntry(
    "info",
    "this should be visible",
  )

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

test.describe("Log panel - Toolbar tests", () => {
  test("Copy to clipboard working correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.openLogPanel()
    const logPanel = page.getByTestId("log-panel")

    const copyToClipboardButton = logPanel.getByRole("button", {
      name: "Copy logs to clipboard",
    })
    await expect(copyToClipboardButton).toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")
    await copyToClipboardButton.click()
    const trackedCommands =
      await configListPage.mobiFlightPage.getTrackedCommands()
    expect(trackedCommands).toContainEqual({
      key: "CommandMainMenu",
      payload: {
        action: "extras.copylogs",
      },
    })
  })

  test("Pause and resume is working correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.openLogPanel()
    const logPanel = page.getByTestId("log-panel")
    const logContent = page.getByTestId("log-panel-content")

    const pauseButton = logPanel.getByRole("button", {
      name: "Pause scrolling",
    })
    const resumeButton = logPanel.getByRole("button", {
      name: "Resume scrolling",
    })
    await expect(pauseButton).toBeVisible()
    await expect(resumeButton).not.toBeVisible()

    // first fill the log panel with some entries so we can see the height change
    for (let i = 0; i < 10; i++) {
      await configListPage.mobiFlightPage.sendLogEntry(
        "info",
        "Test log entry " + i,
      )
    }

    const before = await logContent.getByText("Test log entry 9").boundingBox()

    await pauseButton.click()
    await expect(resumeButton).toBeVisible()

    // now add more messages... the offset should not change because the log is paused
    for (let i = 0; i < 10; i++) {
      await configListPage.mobiFlightPage.sendLogEntry(
        "info",
        "Test log entry " + (10 + i),
      )
    }

    const afterWithPause = await logContent
      .getByText("Test log entry 9")
      .boundingBox()
    expect(afterWithPause!.x).toBe(before!.x)
    expect(afterWithPause!.y).toBe(before!.y)

    await resumeButton.click()
    await expect(pauseButton).toBeVisible()

    const afterWithResume = await logContent
      .getByText("Test log entry 9")
      .boundingBox()
    expect(afterWithResume!.x).toBe(before!.x)
    expect(afterWithResume!.y).toBeLessThan(before!.y)
  })

  test("Filtering is working correctly", async ({ configListPage, page }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.openLogPanel()

    const logPanel = page.getByTestId("log-panel")
    const logContent = logPanel.getByTestId("log-panel-content")

    await configListPage.mobiFlightPage.sendLogEntry(
      "info",
      "Test log entry - filter me",
    )
    await configListPage.mobiFlightPage.sendLogEntry(
      "info",
      "Test log entry - hide me",
    )

    const visibleEntry = logContent.getByText("Test log entry - filter me")
    const hiddenEntry = logContent.getByText("Test log entry - hide me")

    await expect(visibleEntry).toBeVisible()
    await expect(hiddenEntry).toBeVisible()

    const filterInput = logPanel.getByPlaceholder("Filter log entries...")
    await filterInput.fill("filter me")

    await expect(visibleEntry).toBeVisible()
    await expect(hiddenEntry).not.toBeVisible()

    await filterInput.fill("no match")
    await expect(visibleEntry).not.toBeVisible()
    await expect(hiddenEntry).not.toBeVisible()
    await expect(
      logContent.getByText("No entries matching filter."),
    ).toBeVisible()

    const resetFilterButton = logPanel.getByRole("button", {
      name: "Clear filters",
    })
    await expect(resetFilterButton).toBeVisible()
    await resetFilterButton.click()
    await expect(visibleEntry).toBeVisible()
    await expect(hiddenEntry).toBeVisible()
  })
})
