import { ScanForInputResult } from "../src/types/messages"
import { test, expect } from "./fixtures"

test.describe("General Input Config Wizard Tests", () => {
  test("Dialog open for input config items", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")

    await expect(page.getByText("Edit Input Configuration")).not.toBeVisible()
    await configListPage.clickEditButtonForRow(1)
    await expect(page.getByText("Edit Input Configuration")).toBeVisible()

    await expect(
      page.getByText(
        "Edit all settings for this input configuration in this dialog.",
      ),
    ).toBeVisible()

    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
    const firstRow = await configListPage.getConfigItemRow(1)
    await expect(page.getByText("Edit Input Configuration")).not.toBeVisible()
    await firstRow.dblclick()
    await expect(page.getByText("Edit Input Configuration")).toBeVisible()

    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")

    const contextMenuButton = firstRow
      .getByRole("button", { name: "Open menu" })
      .first()
    await contextMenuButton.click()
    const contextMenu = page.getByTestId("config-item-context-menu")
    await expect(contextMenu).toBeVisible()
    const menuItem = contextMenu.getByRole("menuitem", { name: "Edit" })
    await expect(menuItem).toBeVisible()
    await menuItem.click()
    await expect(page.getByText("Edit Input Configuration")).toBeVisible()
  })

  test("Dialog closes with save button", async ({ configListPage, page }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")

    await configListPage.clickEditButtonForRow(1)
    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    await expect(page.getByText("Edit Input Configuration")).not.toBeVisible()
  })

  test("Dialog closes with cancel button", async ({ configListPage, page }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")

    await configListPage.clickEditButtonForRow(1)
    const cancelButton = page.getByRole("button", { name: "Cancel" })
    await expect(cancelButton).toBeVisible()
    await cancelButton.click()

    await expect(page.getByText("Edit Input Configuration")).not.toBeVisible()
  })
})

test.describe("Input Config Wizard - Trigger Panel", () => {
  test("Trigger panel interactions work correctly - Scan for input", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
    await configListPage.mobiFlightPage.trackCommand("CommandScanForInput")

    await configListPage.clickEditButtonForRow(1)
    const triggerPanel = page.getByTestId("trigger-panel")
    await expect(triggerPanel).toBeVisible()

    const scanForInputButton = triggerPanel.getByRole("button", {
      name: "Scan for Input",
    })
    await expect(scanForInputButton).toBeVisible()
    await scanForInputButton.click()

    let commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toContainEqual({
      key: "CommandScanForInput",
      payload: {
        isScanning: true,
      },
    })

    await configListPage.mobiFlightPage.clearTrackedCommands()

    const useAnyInputText = triggerPanel.getByText("Use any input")
    await expect(useAnyInputText).toBeVisible()
    await useAnyInputText.click()

    commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toContainEqual({
      key: "CommandScanForInput",
      payload: {
        isScanning: false,
      },
    })

    await expect(useAnyInputText).not.toBeVisible()
    await expect(scanForInputButton).toBeVisible()

    await configListPage.mobiFlightPage.publishMessage({
      key: "ScanForInputResult",
      payload: {
        Controller: {
          Devices: [],
          Name: "Bravo Throttle Quadrant",
          Serial: "JS-87654321",
        },
        Device: {
          Name: "Button 21",
          Label: "Mode - ALT",
          Type: "Button",
        },
      } as ScanForInputResult,
    })

    await expect(triggerPanel.getByRole("combobox").filter({hasText: "Bravo Throttle Quadrant"})).toBeVisible()
    await expect(triggerPanel.getByRole("combobox").filter({hasText: "Mode - ALT"})).toBeVisible()

    const clearSelectedInputButton = triggerPanel.getByRole("button", {
      name: "Clear input",
    })
    await expect(clearSelectedInputButton).toBeVisible()
    await clearSelectedInputButton.click()

    await expect(triggerPanel.getByRole("combobox").filter({hasText: "Bravo Throttle Quadrant"})).not.toBeVisible()
    await expect(triggerPanel.getByRole("combobox").filter({hasText: "Mode - ALT"})).not.toBeVisible()
    
    await expect(triggerPanel.getByRole("combobox").filter({hasText: "Select controller..."})).toBeVisible()
    await expect(triggerPanel.getByRole("combobox").filter({hasText: "Select device..."})).toBeVisible()
    await expect(triggerPanel.getByRole("combobox").filter({hasText: "Select device..."})).toBeDisabled()
  })

  // test("Trigger panel shows correct data for button triggers", async ({
  //   configListPage,
  //   page,
  // }) => {
  //   await configListPage.gotoPage()
  //   await configListPage.mobiFlightPage.initWithTestData("inputaction")

  //   await configListPage.clickEditButtonForRow(1)
  //   const triggerPanel = page.getByTestId("trigger-panel")
  //   await expect(triggerPanel).toBeVisible()

  //   const scanForInputButton = triggerPanel.getByRole("button", { name: "Scan for Input" })
  //   await expect(scanForInputButton).toBeVisible()
  //   await scanForInputButton.click()

  //   const useAnyInputText = triggerPanel.getByText("Use any input")
  //   await expect(useAnyInputText).toBeVisible()
  //   await useAnyInputText.click()

  //   scanForInputButton.click()
  //   await expect(useAnyInputText).not.toBeVisible()
  //   await expect(scanForInputButton).toBeVisible()
  // })

  // test("Trigger panel shows correct data for encoder triggers", async ({
  //   configListPage,
  //   page,
  // }) => {})

  // test("Trigger panel shows correct data for analog input triggers", async ({
  //   configListPage,
  //   page,
  // }) => {})

  // test("Trigger panel allows editing button triggers", async ({
  //   configListPage,
  //   page,
  // }) => {})

  // test("Trigger panel allows editing encoder triggers", async ({
  //   configListPage,
  //   page,
  // }) => {})
})

// test.describe("Input Config Wizard - Precondition Panel", () => {
//   test("Precondition panel opens when clicking on precondition section", async ({
//     configListPage,
//     page,
//   }) => {})

//   test("Precondition panel shows correct data for existing preconditions", async ({
//     configListPage,
//     page,
//   }) => {})

//   test("Precondition panel allows editing preconditions", async ({
//     configListPage,
//     page,
//   }) => {})
// })
