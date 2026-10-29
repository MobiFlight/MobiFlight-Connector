import { test, expect } from "./fixtures"
import { type Locator, type Page } from "@playwright/test"

test.describe("Config List Global Undo", () => {
  const openGlobalHistory = async (page: Page, controls: Locator) => {
    const historyButton = controls.getByTestId("config-list-history-button")

    // Hover over the left side of the container
    // to avoid the overlapping history trigger.
    await controls.hover({
      position: { x: 8, y: 12 },
    })

    await expect(historyButton).toHaveCSS("opacity", "1")

    await historyButton.click()

    const historyPanel = page.getByTestId("config-list-history-panel")

    await expect(historyPanel).toBeVisible()

    return historyPanel
  }

  test("History state enables Undo and supports multi-step navigation", async ({
    configListPage,
    page,
  }) => {
    const backend = configListPage.mobiFlightPage

    await configListPage.gotoPage()
    await backend.initWithTestData()
    await backend.trackCommand("CommandUndoConfigList")

    const controls = page.getByTestId("config-list-history-controls")

    const undoButton = controls.getByTestId("config-list-undo-button")

    // Initially, no history is available.
    await expect(undoButton).toBeDisabled()

    // Simulate the backend recording three actions.
    await backend.publishMessage({
      key: "ConfigListUndoState",
      payload: {
        CanUndo: true,
        History: ["Toggle", "ItemRename", "ItemEdited"],
        HistoryStart: 0,
      },
    })

    await expect(undoButton).toBeEnabled()

    const historyPanel = await openGlobalHistory(page, controls)

    // Verify translated history labels.
    await expect(
      historyPanel.getByText("Toggled", { exact: true }),
    ).toBeVisible()

    await expect(
      historyPanel.getByText("Item renamed", {
        exact: true,
      }),
    ).toBeVisible()

    await expect(
      historyPanel.getByText("Config edited", {
        exact: true,
      }),
    ).toBeVisible()

    // Select the first action.
    // Two later actions must be undone.
    await historyPanel.getByText("Toggled", { exact: true }).click()

    await expect
      .poll(async () => {
        const commands = await backend.getTrackedCommands()
        return commands?.length ?? 0
      })
      .toBe(1)

    const commands = await backend.getTrackedCommands()

    expect(commands?.[0]).toMatchObject({
      key: "CommandUndoConfigList",
      payload: {
        steps: 2,
      },
    })

    // Simulate the backend confirming the Undo.
    await backend.publishMessage({
      key: "ConfigListUndoState",
      payload: {
        CanUndo: true,
        History: ["Toggle"],
        HistoryStart: 0,
      },
    })

    // The user can still undo the remaining action.
    await expect(undoButton).toBeEnabled()

    // Simulate undoing the last remaining action.
    await backend.publishMessage({
      key: "ConfigListUndoState",
      payload: {
        CanUndo: false,
        History: [],
        HistoryStart: 0,
      },
    })

    // All actions have been undone.
    await expect(undoButton).toBeDisabled()

    await expect(
      controls.getByTestId("config-list-history-button"),
    ).toHaveCount(0)
  })

  test("Ctrl+Z does not trigger Global Undo while editing text", async ({
    configListPage,
    page,
  }) => {
    const backend = configListPage.mobiFlightPage

    await configListPage.gotoPage()
    await backend.initWithTestData()
    await backend.trackCommand("CommandUndoConfigList")

    await backend.publishMessage({
      key: "ConfigListUndoState",
      payload: {
        CanUndo: true,
        History: ["Toggle"],
        HistoryStart: 0,
      },
    })

    const undoButton = page.getByTestId("config-list-undo-button")

    await expect(undoButton).toBeEnabled()

    // Focus a text input.
    const filterInput = page.getByRole("textbox", {
      name: "Filter items...",
    })

    await filterInput.focus()

    // Native text Undo should take priority.
    await page.keyboard.press("Control+z")

    let commands = await backend.getTrackedCommands()

    expect(commands).toHaveLength(0)

    // Move focus outside the text input.
    await undoButton.focus()

    // Now Ctrl+Z should trigger Global Undo.
    await page.keyboard.press("Control+z")

    await expect
      .poll(async () => {
        const tracked = await backend.getTrackedCommands()
        return tracked?.length ?? 0
      })
      .toBe(1)

    commands = await backend.getTrackedCommands()

    expect(commands?.[0]).toMatchObject({
      key: "CommandUndoConfigList",
    })
  })

  test("Undo failure notification is displayed without clearing history", async ({
    configListPage,
    page,
  }) => {
    const backend = configListPage.mobiFlightPage

    await configListPage.gotoPage()
    await backend.initWithTestData()

    // Simulate an existing global history.
    await backend.publishMessage({
      key: "ConfigListUndoState",
      payload: {
        CanUndo: true,
        History: ["Toggle", "ProfileDeleted"],
        HistoryStart: 0,
      },
    })

    const undoButton = page.getByTestId("config-list-undo-button")

    await expect(undoButton).toBeEnabled()

    // Simulate a backend Undo validation failure.
    await backend.publishMessage({
      key: "Notification",
      payload: {
        Event: "ConfigListUndoFailed",
        Context: {
          Action: "ProfileDeleted",
          Reason: "Profile no longer exists",
        },
      },
    })

    // Verify that the notification is displayed.
    await expect(page.getByText(/Could not undo Profile deleted/)).toBeVisible()

    // The history should remain available.
    await expect(undoButton).toBeEnabled()

    const controls = page.getByTestId("config-list-history-controls")

    const historyPanel = await openGlobalHistory(page, controls)

    await expect(
      historyPanel.getByText("Toggled", {
        exact: true,
      }),
    ).toBeVisible()

    await expect(
      historyPanel.getByText("Profile deleted", {
        exact: true,
      }),
    ).toBeVisible()
  })
})
