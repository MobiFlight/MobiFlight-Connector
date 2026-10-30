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

  const renameInputConfigDraft = async (page: Page, newName: string) => {
    const nameContainer = page.getByTestId("dialog-config-name")

    await nameContainer.getByRole("button").dblclick()

    const nameInput = nameContainer.getByRole("textbox")

    await nameInput.fill(newName)
    await nameInput.press("Enter")

    await expect(
      nameContainer.getByText(newName, { exact: true }),
    ).toBeVisible()
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

  test("Applying a local edit preserves previous Global Undo history", async ({
    configListPage,
    page,
  }) => {
    const backend = configListPage.mobiFlightPage

    const originalName = "MSFS Input Action"
    const editedName = "Edited Input Config"

    // Initialize project and backend mocks.
    await configListPage.gotoPage()
    await backend.initWithTestData("inputaction")
    await configListPage.setupConfigItemEditConfirmationResponse()

    await backend.trackCommand("CommandConfigContextMenu")
    await backend.trackCommand("CommandUpdateConfigItem")
    await backend.trackCommand("CommandUndoConfigList")

    const controls = page.getByTestId("config-list-history-controls")

    const globalUndoButton = controls.getByTestId("config-list-undo-button")

    const firstRow = await configListPage.getConfigItemRow(1)
    const toggleSwitch = firstRow.getByRole("switch")

    await expect(toggleSwitch).toBeChecked()
    await expect(globalUndoButton).toBeDisabled()

    // Toggle the ConfigItem.
    await toggleSwitch.click()

    await expect(toggleSwitch).not.toBeChecked()

    await expect
      .poll(async () => {
        const commands = await backend.getTrackedCommands()

        return (
          commands?.filter(
            (command) =>
              command.key === "CommandConfigContextMenu" &&
              command.payload.action === "toggle",
          ).length ?? 0
        )
      })
      .toBe(1)

    // Simulate the backend recording the Toggle.
    await backend.publishMessage({
      key: "ConfigListUndoState",
      payload: {
        CanUndo: true,
        History: ["Toggle"],
        HistoryStart: 0,
      },
    })

    await expect(globalUndoButton).toBeEnabled()

    // Open the local editor and change the name.
    await configListPage.clickEditButtonForRow(1)

    await renameInputConfigDraft(page, editedName)

    const localUndoButton = page.getByTestId("config-undo-button")

    await expect(localUndoButton).toBeEnabled()

    // Local editing must not commit anything yet.
    const commandsBeforeApply = await backend.getTrackedCommands()

    expect(
      commandsBeforeApply?.filter(
        (command) => command.key === "CommandUpdateConfigItem",
      ),
    ).toHaveLength(0)

    // The previously recorded Global Undo remains available.
    await expect(globalUndoButton).toBeEnabled()

    // Apply the local changes.
    await page.getByRole("button", { name: "Apply changes" }).click()

    await expect(page.getByTestId("inputconfigwizard-dialog")).not.toBeVisible()

    // Verify that Apply sends one committed ConfigItem.
    await expect
      .poll(async () => {
        const commands = await backend.getTrackedCommands()

        return (
          commands?.filter(
            (command) => command.key === "CommandUpdateConfigItem",
          ).length ?? 0
        )
      })
      .toBe(1)

    const commandsAfterApply = await backend.getTrackedCommands()

    const updateCommand = commandsAfterApply?.find(
      (command) => command.key === "CommandUpdateConfigItem",
    )

    expect(updateCommand?.payload.item.Name).toBe(editedName)

    // Existing Global History must not disappear on Apply.
    await expect(globalUndoButton).toBeEnabled()

    // Simulate the backend recording the committed edit
    // as one ItemEdited action.
    await backend.publishMessage({
      key: "ConfigListUndoState",
      payload: {
        CanUndo: true,
        History: ["Toggle", "ItemEdited"],
        HistoryStart: 0,
      },
    })

    // Verify Global History contains both actions.
    const historyPanel = await openGlobalHistory(page, controls)

    await expect(
      historyPanel.getByText("Toggled", { exact: true }),
    ).toBeVisible()

    await expect(
      historyPanel.getByText("Config edited", {
        exact: true,
      }),
    ).toBeVisible()

    await expect(
      historyPanel.getByTestId("config-list-history-entry"),
    ).toHaveCount(2)

    // Close the dropdown before clicking Undo.
    await page.keyboard.press("Escape")

    await expect(historyPanel).not.toBeVisible()

    // Undo the committed ItemEdited operation.
    // Click the left side to avoid the History arrow.
    await globalUndoButton.click({
      position: { x: 8, y: 12 },
    })

    await expect
      .poll(async () => {
        const commands = await backend.getTrackedCommands()

        return (
          commands?.filter((command) => command.key === "CommandUndoConfigList")
            .length ?? 0
        )
      })
      .toBe(1)

    const undoCommands = (await backend.getTrackedCommands())?.filter(
      (command) => command.key === "CommandUndoConfigList",
    )

    expect(undoCommands?.[0]).toMatchObject({
      key: "CommandUndoConfigList",
      payload: {
        steps: 1,
      },
    })

    // Simulate the backend restoring the original item.
    const testProject = await backend.getTestProjectData("inputaction")

    const restoredItems = testProject.ConfigFiles[0].ConfigItems.map(
      (item, index) =>
        index === 0
          ? {
              ...item,
              Active: false,
            }
          : item,
    )

    await configListPage.configValueFullUpdate(
      restoredItems as import("../src/types").IConfigItem[],
    )

    await backend.publishMessage({
      key: "ConfigListUndoState",
      payload: {
        CanUndo: true,
        History: ["Toggle"],
        HistoryStart: 0,
      },
    })

    // The name is restored, but the earlier Toggle remains.
    await expect(
      page.getByRole("cell", {
        name: originalName,
        exact: true,
      }),
    ).toBeVisible()

    await expect(
      (await configListPage.getConfigItemRow(1)).getByRole("switch"),
    ).not.toBeChecked()

    await expect(globalUndoButton).toBeEnabled()

    const remainingHistory = await openGlobalHistory(page, controls)

    await expect(
      remainingHistory.getByText("Toggled", {
        exact: true,
      }),
    ).toBeVisible()

    await expect(
      remainingHistory.getByText("Config edited", {
        exact: true,
      }),
    ).toHaveCount(0)
  })

  test("Canceling local edits keeps the existing Global Undo history", async ({
    configListPage,
    page,
  }) => {
    const backend = configListPage.mobiFlightPage

    const originalName = "MSFS Input Action"
    const editedName = "Unsaved Input Config"

    // Initialize project and backend mocks.
    await configListPage.gotoPage()
    await backend.initWithTestData("inputaction")
    await configListPage.setupConfigItemEditConfirmationResponse()

    await backend.trackCommand("CommandConfigContextMenu")
    await backend.trackCommand("CommandUpdateConfigItem")
    await backend.trackCommand("CommandUndoConfigList")

    const controls = page.getByTestId("config-list-history-controls")

    const globalUndoButton = controls.getByTestId("config-list-undo-button")

    const firstRow = await configListPage.getConfigItemRow(1)
    const toggleSwitch = firstRow.getByRole("switch")

    // Toggle an existing ConfigItem.
    await expect(toggleSwitch).toBeChecked()

    await toggleSwitch.click()

    await expect(toggleSwitch).not.toBeChecked()

    await backend.publishMessage({
      key: "ConfigListUndoState",
      payload: {
        CanUndo: true,
        History: ["Toggle"],
        HistoryStart: 0,
      },
    })

    await expect(globalUndoButton).toBeEnabled()

    // Open editor and create a local change.
    await configListPage.clickEditButtonForRow(1)

    await renameInputConfigDraft(page, editedName)

    const localUndoButton = page.getByTestId("config-undo-button")

    await expect(localUndoButton).toBeEnabled()

    // Cancel instead of Apply.
    await page
      .getByTestId("inputconfigwizard-dialog-footer")
      .getByRole("button", { name: "Cancel", exact: true })
      .click()

    await expect(page.getByTestId("inputconfigwizard-dialog")).not.toBeVisible()

    // No committed edit should have been sent to the backend.
    const commands = await backend.getTrackedCommands()

    expect(
      commands?.filter((command) => command.key === "CommandUpdateConfigItem"),
    ).toHaveLength(0)

    // The original name must remain unchanged.
    await expect(
      page.getByRole("cell", {
        name: originalName,
        exact: true,
      }),
    ).toBeVisible()

    await expect(
      page.getByRole("cell", {
        name: editedName,
        exact: true,
      }),
    ).toHaveCount(0)

    // The Toggle state must remain unchanged.
    await expect(
      (await configListPage.getConfigItemRow(1)).getByRole("switch"),
    ).not.toBeChecked()

    // Verify existing Global History is preserved.
    await expect(globalUndoButton).toBeEnabled()

    const historyPanel = await openGlobalHistory(page, controls)

    await expect(
      historyPanel.getByText("Toggled", {
        exact: true,
      }),
    ).toBeVisible()

    await expect(
      historyPanel.getByTestId("config-list-history-entry"),
    ).toHaveCount(1)

    await expect(
      historyPanel.getByText("Config edited", {
        exact: true,
      }),
    ).toHaveCount(0)

    // Global Undo must still be available.
    await page.keyboard.press("Escape")

    await expect(historyPanel).not.toBeVisible()

    await globalUndoButton.click({
      position: { x: 8, y: 12 },
    })

    await expect
      .poll(async () => {
        const commands = await backend.getTrackedCommands()

        return (
          commands?.filter((command) => command.key === "CommandUndoConfigList")
            .length ?? 0
        )
      })
      .toBe(1)

    const undoCommands = (await backend.getTrackedCommands())?.filter(
      (command) => command.key === "CommandUndoConfigList",
    )

    expect(undoCommands?.[0]).toMatchObject({
      key: "CommandUndoConfigList",
      payload: {
        steps: 1,
      },
    })
  })
})
