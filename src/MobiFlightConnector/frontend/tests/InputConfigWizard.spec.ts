import { Locator, Page } from "@playwright/test"
import { ScanForInputResult } from "../src/types/messages"
import { test, expect } from "./fixtures"
import { ConfigListPage } from "./fixtures/ConfigListPage"
import msfsPresetsResponse from "./data/inputaction/msfspresets.testdata.json" with { type: "json" }

// Helper: open the dialog for a given row and return the action-editor locator
// (onPress tab is active by default for button inputs)
const openActionEditor = async (
  configListPage: ConfigListPage,
  page: Page,
  row: number,
) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData("inputaction")
  await page.route(
    "*/**/presets/msfs2020_hubhop_presets.json",
    async (route) => {
      const json = msfsPresetsResponse
      await route.fulfill({ json })
    },
  )
  await configListPage.clickEditButtonForRow(row)
  return page.getByTestId("action-editor")
}

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

    await expect(
      triggerPanel
        .getByRole("combobox")
        .filter({ hasText: "Bravo Throttle Quadrant" }),
    ).toBeVisible()
    await expect(
      triggerPanel.getByRole("combobox").filter({ hasText: "Mode - ALT" }),
    ).toBeVisible()

    const clearSelectedInputButton = triggerPanel.getByRole("button", {
      name: "Clear input",
    })
    await expect(clearSelectedInputButton).toBeVisible()
    await clearSelectedInputButton.click()

    await expect(
      triggerPanel
        .getByRole("combobox")
        .filter({ hasText: "Bravo Throttle Quadrant" }),
    ).not.toBeVisible()
    await expect(
      triggerPanel.getByRole("combobox").filter({ hasText: "Mode - ALT" }),
    ).not.toBeVisible()

    await expect(
      triggerPanel
        .getByRole("combobox")
        .filter({ hasText: "Select controller..." }),
    ).toBeVisible()
    await expect(
      triggerPanel
        .getByRole("combobox")
        .filter({ hasText: "Select device..." }),
    ).toBeVisible()
    await expect(
      triggerPanel
        .getByRole("combobox")
        .filter({ hasText: "Select device..." }),
    ).toBeDisabled()
  })
})

test.describe("Input Config Wizard - Preconditions panel", () => {
  test("Preconditions panel shows correct data for existing preconditions", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")

    const preconditionsPanel = page.getByTestId("preconditions-panel")
    const preconditionEditButton = preconditionsPanel.getByRole("button", {
      name: "Preconditions",
    })

    await configListPage.clickEditButtonForRow(1)
    await expect(preconditionsPanel).toBeVisible()
    await expect(preconditionEditButton).toBeVisible()

    await expect(preconditionsPanel.getByText("MyVar=")).toBeVisible()
    await expect(
      preconditionsPanel.getByText(
        "Just an output config for references and preconditions=",
      ),
    ).toBeVisible()
  })

  test("Preconditions panel editing works correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")

    const preconditionsPanel = page.getByTestId("preconditions-panel")
    const preconditionEditButton = preconditionsPanel.getByRole("button", {
      name: "Preconditions",
    })

    await configListPage.clickEditButtonForRow(1)
    await expect(preconditionEditButton).toBeVisible()
    await preconditionEditButton.click()

    await configListPage.mobiFlightPage.publishMessage({
      key: "MobiFlightVariablesUpdate",
      payload: {
        Variables: [
          {
            Expression: "$",
            Name: "MyVar",
            Number: 0,
            TYPE: "number",
            Text: "",
          },
        ],
      },
    })

    const preconditionEditor = page.getByTestId("precondition-editor")
    await expect(preconditionEditor).toBeVisible()

    const preconditionItems = preconditionEditor.getByTestId(
      "precondition-item-row",
    )
    await expect(preconditionItems).toHaveCount(2)

    const comboBoxLocator = (locator: Locator, expectedText: string) => {
      return locator
        .getByRole("combobox")
        .filter({ hasText: new RegExp(`^${expectedText}$`) })
    }

    const expectedValues = [
      {
        type: "Variable",
        name: "MyVar",
        operand: "=",
        value: "1",
        logic: "and",
      },
      {
        type: "Config",
        name: "Just an output config for references and preconditions",
        operand: "=",
        value: null,
        logic: null,
      },
    ]

    let index = 0
    for (const expected of expectedValues) {
      const precondition = preconditionItems.nth(index)
      await expect(comboBoxLocator(precondition, expected.type)).toBeVisible()
      await expect(comboBoxLocator(precondition, expected.name)).toBeVisible()
      await expect(
        comboBoxLocator(precondition, expected.operand),
      ).toBeVisible()
      if (expected.value !== null) {
        await expect(
          precondition.getByRole("textbox", { name: "Value" }),
        ).toBeVisible()
        await expect(
          precondition.getByRole("textbox", { name: "Value" }),
        ).toHaveValue(expected.value)
      }
      if (expected.logic !== null) {
        await expect(
          comboBoxLocator(precondition, expected.logic),
        ).toBeVisible()
      }
      index++
    }
  })

  test("Preconditions can be added and deleted", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")

    const preconditionsPanel = page.getByTestId("preconditions-panel")
    const preconditionEditButton = preconditionsPanel.getByRole("button", {
      name: "Preconditions",
    })

    await configListPage.clickEditButtonForRow(1)
    await expect(preconditionEditButton).toBeVisible()
    await preconditionEditButton.click()

    const preconditionEditor = page.getByTestId("precondition-editor")
    await expect(preconditionEditor).toBeVisible()

    const addPreconditionButton = preconditionEditor.getByRole("button", {
      name: "Add Precondition",
    })
    await expect(addPreconditionButton).toBeVisible()
    await addPreconditionButton.click()

    let preconditionItems = preconditionEditor.getByTestId(
      "precondition-item-row",
    )
    await expect(preconditionItems).toHaveCount(3)

    const firstPreconditionDeleteButton = preconditionItems
      .nth(0)
      .getByRole("button", { name: "Delete precondition" })
    await expect(firstPreconditionDeleteButton).toBeVisible()
    await firstPreconditionDeleteButton.click()

    preconditionItems = preconditionEditor.getByTestId("precondition-item-row")
    await expect(preconditionItems).toHaveCount(2)
  })
})

test.describe("Input Config Wizard - Config References panel", () => {
  test("Config References panel shows correct data for existing references", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")

    const configReferencesPanel = page.getByTestId("config-references-panel")
    const editButton = configReferencesPanel.getByRole("button", {
      name: "Config References",
    })

    await configListPage.clickEditButtonForRow(1)
    await expect(configReferencesPanel).toBeVisible()
    await expect(editButton).toBeVisible()

    // Summary shows placeholder badges for each config reference
    await expect(configReferencesPanel.getByText("#")).toBeVisible()
    await expect(configReferencesPanel.getByText("!")).toBeVisible()
    await expect(configReferencesPanel.getByText("?")).toBeVisible()
  })

  test("Config References panel editing works correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")

    const configReferencesPanel = page.getByTestId("config-references-panel")
    const editButton = configReferencesPanel.getByRole("button", {
      name: "Config References",
    })

    await configListPage.clickEditButtonForRow(1)
    await editButton.click()

    const configReferenceEditor = page.getByTestId("config-reference-editor")
    await expect(configReferenceEditor).toBeVisible()

    const referenceItems = configReferenceEditor.getByTestId(
      "config-reference-item-row",
    )
    await expect(referenceItems).toHaveCount(3)

    const expectedReferences = [
      {
        configName: "Just an output config for references and preconditions",
        placeholder: "#",
        testValue: "1",
      },
      { configName: "config reference #2", placeholder: "!", testValue: "1" },
    ]

    for (const [index, expected] of expectedReferences.entries()) {
      const row = referenceItems.nth(index)
      await expect(row.getByText(expected.configName)).toBeVisible()
      await expect(row.getByRole("textbox").nth(0)).toHaveValue(
        expected.placeholder,
      )
      await expect(row.getByRole("textbox").nth(1)).toHaveValue(
        expected.testValue,
      )
    }
  })

  test("Config References can be added and deleted", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")

    const configReferencesPanel = page.getByTestId("config-references-panel")
    const editButton = configReferencesPanel.getByRole("button", {
      name: "Config References",
    })

    await configListPage.clickEditButtonForRow(1)
    await editButton.click()

    const configReferenceEditor = page.getByTestId("config-reference-editor")
    const referenceItems = configReferenceEditor.getByTestId(
      "config-reference-item-row",
    )
    await expect(referenceItems).toHaveCount(3)

    await configReferenceEditor
      .getByRole("button", { name: "Add Config Reference" })
      .click()
    await expect(referenceItems).toHaveCount(4)

    await referenceItems
      .nth(0)
      .getByRole("button", { name: "Delete config reference" })
      .click()
    await expect(referenceItems).toHaveCount(3)
  })
})

test.describe("Input Config Wizard - Action Editor", () => {})

test.describe("Input Config Wizard - MSFS Input Action Panel", () => {
  test("MSFS action type shows correct fields", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 1)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Microsoft Flight Simulator (all versions)" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("textbox", { name: "Code:" }),
    ).toHaveValue("(>K:AP_PANEL_HEADING_HOLD)")
  })

  test("MSFS preset panel works correctly", async ({
    configListPage,
    page,
  }) => {
    // Mock the preset API response to have consistent test data
    const actionEditor = await openActionEditor(configListPage, page, 1)
    const filterPresetsInput = actionEditor.getByPlaceholder("Filter presets")
    const presetCountLabel = actionEditor
      .getByRole("status")
      .filter({ hasText: /preset\(s\) found/ })
    await expect(filterPresetsInput).toBeVisible()
    await expect(presetCountLabel).toBeVisible()
    await expect(presetCountLabel).toHaveText("4 preset(s) found")

    await filterPresetsInput.fill("AP_PANEL_HEADING_HOLD")
    await expect(presetCountLabel).toHaveText("1 preset(s) found")

    await filterPresetsInput.fill("NonExistingPreset")
    await expect(presetCountLabel).toHaveText("0 preset(s) found")

    await filterPresetsInput.fill("")
    await expect(presetCountLabel).toHaveText("4 preset(s) found")
  })
})

test.describe("Input Config Wizard - X-Plane Input Action Panel", () => {
  test("X-Plane action type shows correct fields", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 2)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "X-Plane (all versions)" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Command" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("textbox", { name: "Code:" }),
    ).toHaveValue("laminar/B738/knob/land_alt_press_dn")
  })
})

test.describe("Input Config Wizard - X-Plane Input Action Panel", () => {
  test("Variable action type shows correct fields", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 3)
    await configListPage.mobiFlightPage.publishMessage({
      key: "MobiFlightVariablesUpdate",
      payload: {
        Variables: [
          {
            Expression: "$",
            Name: "MyVar",
            Number: 0,
            TYPE: "number",
            Text: "",
          },
        ],
      },
    })
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "MobiFlight - Variable" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: /^MyVar$/ }),
    ).toBeVisible()
  })
})

test.describe("Input Config Wizard - X-Plane Input Action Panel", () => {
  test("Retrigger action type shows correct fields", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 4)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "MobiFlight - Retrigger switches" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByText("re-trigger all button states"),
    ).toBeVisible()
  })
})

test.describe("Input Config Wizard - X-Plane Input Action Panel", () => {
  test("Keyboard action type shows correct fields", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 5)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "MobiFlight - Keyboard Input" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("button", { name: "Scan for keyboard" }),
    ).toBeVisible()
  })
})

test.describe("Input Config Wizard - X-Plane Input Action Panel", () => {
  test("vJoy button action type shows correct fields", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 6)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "MobiFlight - Virtual Joystick input (vJoy)" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("tab", { name: "button" }),
    ).toHaveAttribute("data-state", "active")
  })
})

test.describe("Input Config Wizard - X-Plane Input Action Panel", () => {
  test("vJoy axis action type shows correct fields", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 7)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "MobiFlight - Virtual Joystick input (vJoy)" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("tab", { name: "axis" }),
    ).toHaveAttribute("data-state", "active")
  })
})

test.describe("Input Config Wizard - X-Plane Input Action Panel", () => {
  test("FSUIPC Offset action type shows correct fields", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 8)
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "FSUIPC - Offset" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "4 Bytes" }),
    ).toBeVisible()
  })
})

test.describe("Input Config Wizard - FSUIPC - EventID Input Action Panel", () => {
  test("FSUIPC EventID action type shows correct fields", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 9)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "FSUIPC - EventID" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("textbox", { name: "Event ID" }),
    ).toBeVisible()
  })
})

test.describe("Input Config Wizard - FSUIPC - PMDG EventID Input Action Panel", () => {
  test("FSUIPC PMDG EventID action type shows correct fields", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 10)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "FSUIPC - PMDG - Event ID" }),
    ).toBeVisible()
    await expect(actionEditor.getByText("PMDG Aircraft")).toBeVisible()
  })
})

test.describe("Input Config Wizard - FSUIPC - Jeehell Input Action Panel", () => {
  test("FSUIPC Jeehell action type shows correct fields", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 11)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "FSUIPC - Jeehell - Events" }),
    ).toBeVisible()
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Select Jeehell function..." }),
    ).toBeVisible()
  })
})

test.describe("Input Config Wizard - FSUIPC - Lua Macro Input Action Panel", () => {
  test("FSUIPC Lua Macro action type shows correct fields", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 12)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "FSUIPC - Lua Macro" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("textbox", { name: "Macro Name:" }),
    ).toHaveValue("TestMacro")
    await expect(
      actionEditor.getByRole("textbox", { name: "Macro Value:" }),
    ).toHaveValue("TestValue")
  })
})

test.describe("Input Config Wizard - ProSim Input Action Panel", () => {
  test("ProSim action type shows correct fields", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 13)
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "ProSim" }),
    ).toBeVisible()
    await expect(actionEditor.getByText("ProSim Input Action")).toBeVisible()
  })
})

test.describe("Input Config Wizard - Action Binding Panels", () => {
  test("Button panel: each tab routes to the correct event slot", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
    await configListPage.clickEditButtonForRow(1) // MSFS onPress, onRelease=null

    const buttonPanel = page.getByTestId("button-action-panel")
    const actionEditor = page.getByTestId("action-editor")

    // onPress tab is active by default and shows the MSFS action
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Microsoft Flight Simulator (all versions)" }),
    ).toBeVisible()

    // Switching to onRelease shows an empty action editor (no type selected)
    await buttonPanel.getByRole("tab", { name: "onRelease" }).click()
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Select..." }),
    ).toBeVisible()

    // Switching to onHold shows an empty action editor
    await buttonPanel.getByRole("tab", { name: "onHold" }).click()
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Select..." }),
    ).toBeVisible()

    // Switching back to onPress still shows the MSFS action
    await buttonPanel.getByRole("tab", { name: "onPress" }).click()
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Microsoft Flight Simulator (all versions)" }),
    ).toBeVisible()
  })

  test("Encoder panel: each tab routes to the correct event slot", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
    await configListPage.clickEditButtonForRow(14) // MSFS Encoder Action

    const encoderPanel = page.getByTestId("encoder-action-panel")
    const actionEditor = page.getByTestId("action-editor")

    // onLeft tab is active by default
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Microsoft Flight Simulator (all versions)" }),
    ).toBeVisible()

    // onRight also has an MSFS action
    await encoderPanel
      .getByRole("tab", { name: "onRight", exact: true })
      .click()
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Microsoft Flight Simulator (all versions)" }),
    ).toBeVisible()

    // onLeftFast is empty
    await encoderPanel.getByRole("tab", { name: "onLeftFast" }).click()
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Select..." }),
    ).toBeVisible()
  })

  test("Analog panel: onChange routes to the correct event slot", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
    await configListPage.clickEditButtonForRow(15) // MSFS Analog Action

    const actionEditor = page.getByTestId("action-editor")

    // onChange is the only tab and shows the MSFS action
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Microsoft Flight Simulator (all versions)" }),
    ).toBeVisible()
  })
})