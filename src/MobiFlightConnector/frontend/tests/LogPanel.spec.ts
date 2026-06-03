import { test, expect } from "./fixtures"
import { AppMessage } from "../src/types/messages"
import Settings from "../src/types/settings"
import defaultSettings from "./data/settings.testdata.json" with { type: "json" }

// Sends a Settings message that masquerades as the real backend: a *complete*
// Settings object with only the fields under test overridden. Partial payloads
// must be avoided — setSettings() replaces the whole store object, so a missing
// required field (e.g. RecentFiles) crashes any component that reads it.
const sendSettings = async (
  page: import("@playwright/test").Page,
  overrides: Partial<Settings>,
) => {
  const payload: Settings = { ...(defaultSettings as Settings), ...overrides }
  await page.evaluate(
    (msg) => window.postMessage(msg, "*"),
    { key: "Settings", payload },
  )
}

// Opens the log panel via the View menu. Extracted because every test needs it.
const openLogPanel = async (page: import("@playwright/test").Page) => {
  await page
    .getByRole("menubar")
    .getByRole("menuitem", { name: "View" })
    .click()
  await page.getByRole("menuitem", { name: "Toggle Log Panel" }).click()
}

// Injects a fake backend log message into the app.
// The LogPanel listens for "LogEntry" messages on window via useAppMessage —
// the same channel all other backend messages use.
const sendLogEntry = async (
  page: import("@playwright/test").Page,
  severity: string,
  message: string,
) => {
  const msg: AppMessage = {
    key: "LogEntry",
    payload: {
      Message: message,
      Severity: severity,
      Timestamp: new Date().toISOString(),
    },
  }
  await page.evaluate((m) => window.postMessage(m, "*"), msg)
}

// Note: toggling the panel on/off via View > Toggle Log Panel is covered by
// MainMenu.spec.ts ("Confirm View > Toggle Log Panel shows and hides the log
// panel"). That's a menu-wiring concern; the tests below cover panel-owned
// behavior (the X button, re-opening, content rendering).
test("Log panel closes via X button", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await openLogPanel(page)

  const closeButton = page.getByRole("button", { name: "Close log panel" })
  await expect(closeButton).toBeVisible()

  await closeButton.click()
  await expect(closeButton).not.toBeVisible()
})

test("Log panel re-opens after being closed with X", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await openLogPanel(page)

  const closeButton = page.getByRole("button", { name: "Close log panel" })
  await closeButton.click()
  await expect(closeButton).not.toBeVisible()

  // View menu should be able to re-open it — verifies that onClose()
  // sets logVisible=false rather than unmounting permanently.
  await openLogPanel(page)
  await expect(closeButton).toBeVisible()
})

test("Log panel shows empty placeholder before any messages arrive", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await openLogPanel(page)

  // No LogEntry messages have been sent, so the panel should show the
  // "Waiting for log entries" placeholder from LogPanel.Empty translation key.
  await expect(page.getByText("Waiting for log entries")).toBeVisible()
})

test("Log entry messages appear in the panel", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await openLogPanel(page)

  // Crucially, the panel must be mounted *before* sending messages —
  // useAppMessage subscribes on mount, so messages sent before mount are lost.
  await sendLogEntry(page, "Info", "Hello from the test")

  await expect(page.getByText("Hello from the test")).toBeVisible()
})

test("Severity colours are applied to log entries", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await openLogPanel(page)

  // Set log level to trace so all severities pass the filter and render.
  // Without this, the default "info" level would hide debug entries before
  // they reach the DOM, making the colour assertion fail.
  await sendSettings(page, { LogLevel: "trace" })

  // Send one message per severity so all four colour classes get rendered.
  await sendLogEntry(page, "Error", "error message")
  await sendLogEntry(page, "Warn", "warn message")
  await sendLogEntry(page, "Info", "info message")
  await sendLogEntry(page, "Debug", "debug message")

  // The severity label span carries the colour class. Severity text is
  // lowercased in handleMessage() and displayed uppercase via Tailwind.
  // toHaveClass checks that the class is present among others on the element.
  const logContent = page.getByTestId("log-panel-content")

  await expect(
    logContent.locator("span.uppercase").filter({ hasText: "error" }),
  ).toHaveClass(/text-red-500/)

  await expect(
    logContent.locator("span.uppercase").filter({ hasText: "warn" }),
  ).toHaveClass(/text-yellow-500/)

  await expect(
    logContent.locator("span.uppercase").filter({ hasText: "info" }),
  ).toHaveClass(/text-blue-400/)

  await expect(
    logContent.locator("span.uppercase").filter({ hasText: "debug" }),
  ).toHaveClass(/text-gray-400/)
})

test("Default log level filters out debug messages", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await openLogPanel(page)

  // No Settings message sent → effectiveLevel defaults to "info" (see shouldShow()).
  // Debug (level 1) is below info (level 2), so it should be filtered out.
  await sendLogEntry(page, "Debug", "this should be hidden")
  await sendLogEntry(page, "Info", "this should be visible")

  await expect(page.getByText("this should be visible")).toBeVisible()
  await expect(page.getByText("this should be hidden")).not.toBeVisible()
})

test("Log panel height changes when title bar is dragged upward", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await openLogPanel(page)

  // The scrollable content div has an inline style={{ height }} driven by DEFAULT_HEIGHT.
  // We measure it before and after the drag (relative assertion, so the exact
  // default doesn't matter as long as there's headroom below MAX_HEIGHT).
  const logContent = page.getByTestId("log-panel-content")
  const before = await logContent.boundingBox()
  expect(before).not.toBeNull()

  // Drag the title bar (cursor-row-resize) upward.
  // The handler computes delta = startY - currentY, so moving the mouse
  // upward (decreasing Y) increases the panel height.
  // steps: 10 emits intermediate mousemove events, which is required since
  // the resize logic listens to document mousemove — not a Playwright drag target.
  const titleBar = page.getByTestId("log-panel-titlebar")
  const titleBox = await titleBar.boundingBox()
  expect(titleBox).not.toBeNull()

  const cx = titleBox!.x + titleBox!.width / 2
  const cy = titleBox!.y + titleBox!.height / 2

  await page.mouse.move(cx, cy)
  await page.mouse.down()
  await page.mouse.move(cx, cy - 100, { steps: 10 })
  await page.mouse.up()

  const after = await logContent.boundingBox()
  expect(after!.height).toBeGreaterThan(before!.height)
})

test("Log panel shows disabled message when LogEnabled is false", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await openLogPanel(page)

  // Disable logging via a Settings message. The Zustand store update causes
  // LogPanel to re-render in place, so it can be sent after the panel is open.
  await sendSettings(page, { LogEnabled: false })

  // LogPanel checks logEnabled === false (strict equality). When it's false,
  // it renders LogPanel.LoggingDisabled instead of entries or the empty placeholder.
  await expect(
    page.getByText("Logging is disabled. Enable logging in Settings to see logs here."),
  ).toBeVisible()

  // The empty-state placeholder should not appear — disabled takes precedence.
  await expect(page.getByText("Waiting for log entries")).not.toBeVisible()
})
