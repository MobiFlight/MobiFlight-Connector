import { Locator, Page } from "@playwright/test"
import {
  ConfigValueFullUpdate,
  ScanForInputResult,
} from "../src/types/messages"
import { test, expect } from "./fixtures"
import { ConfigListPage } from "./fixtures/ConfigListPage"
import msfsPresetsResponse from "./data/inputaction/msfspresets.testdata.json" with { type: "json" }
import xplanePresetsResponse from "./data/inputaction/xplanepresets.testdata.json" with { type: "json" }
import { ActionTypeOptions } from "../src/lib/configWizard"
import { Project } from "../src/types"

const jeehellPresetsContent = `FCU_KNOBS:GROUP
FCU_HDGKNOB_PRESS:6:FCU Heading Knob Press
FCU_HDGKNOB_LONGPRESS:7:FCU Heading Knob Long Press
AP_ENGAGE:8:Autopilot Engage`

// Helper: open the dialog for a given row and return the action-editor locator
// (onPress tab is active by default for button inputs)
const openActionEditor = async (
  configListPage: ConfigListPage,
  page: Page,
  row: number,
  callback?: (configListPage: ConfigListPage) => Promise<void>,
  projectOptions?: Partial<Project>,
) => {
  await configListPage.gotoPage()
  if (projectOptions) {
    await configListPage.mobiFlightPage.initWithTestDataAndSpecificProjectProps(projectOptions, "inputaction")
  } else {
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
  }
  await page.route(
    "*/**/presets/msfs2020_hubhop_presets.json",
    async (route) => {
      await route.fulfill({ json: msfsPresetsResponse })
    },
  )
  await page.route("*/**/presets/xplane_hubhop_presets.json", async (route) => {
    await route.fulfill({ json: xplanePresetsResponse })
  })
  await page.route("*/**/presets/presets_jeehell.cip", async (route) => {
    await route.fulfill({
      body: jeehellPresetsContent,
      contentType: "text/plain",
    })
  })

  // Invoke
  await callback?.(configListPage)

  await configListPage.clickEditButtonForRow(row)
  const actionEditor = page.getByTestId("action-editor")

  // expect it to become visible
  // this ensures that react render has completed and, e.g., useEffects have run
  await expect(actionEditor).toBeVisible()

  return actionEditor
}

test.describe("General Input Config Wizard Tests", () => {
  test("Dialog open for input config items - via Edit button", async ({
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
  })

  test("Dialog open for input config items - via double click", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
    const firstRow = await configListPage.getConfigItemRow(1)

    await expect(page.getByText("Edit Input Configuration")).not.toBeVisible()
    await firstRow.dblclick()
    await expect(page.getByText("Edit Input Configuration")).toBeVisible()

    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
  })

  test("Dialog open for input config items - via context menu", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
    const firstRow = await configListPage.getConfigItemRow(1)

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

  test("Dialog open for input config items - via 'Add Input Config' button", async ({
    configListPage,
    page,
  }) => {
    // Opens after clicking "Add Input Config" button and goes through the creation flow
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
    const addInputConfigButton = page.getByRole("button", {
      name: "Add Input Config",
    })
    await addInputConfigButton.click()
    await configListPage.addNewConfigItem("InputConfigItem", 0, "inputaction")
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
  test("Preconditions panel shows correct summary for existing preconditions", async ({
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

  test("Preconditions panel shows correct form data for existing preconditions", async ({
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

test.describe("Input Config Wizard - Action Type Panel", () => {
  test("Action types honor project settings and features", async ({
    configListPage,
    page,
  }) => {
    const projectSettingsToTest: Partial<Project>[] = [
      {
        Sim: "msfs",
        Features: { ProSim: false, FSUIPC: false },
      },
      {
        Sim: "msfs",
        Features: { ProSim: true, FSUIPC: false },
      },
      {
        Sim: "msfs",
        Features: { ProSim: false, FSUIPC: true },
      },
      {
        Sim: "xplane",
        Features: { ProSim: false, FSUIPC: false },
      },
      {
        Sim: "fsx",
        Features: { ProSim: false, FSUIPC: true },
      },
    ]

    const actionTypeOptionLabels = {
      MSFS2020CustomInputAction: "Microsoft Flight Simulator (all versions)",
      XplaneInputAction: "X-Plane (all versions)",
      ProSimInputAction: "ProSim",
      VariableInputAction: "MobiFlight - Variable",
      RetriggerInputAction: "MobiFlight - Retrigger switches",
      KeyInputAction: "MobiFlight - Keyboard Input",
      VJoyInputAction: "MobiFlight - Virtual Joystick input (vJoy)",
      FsuipcOffsetInputAction: "FSUIPC - Offset",
      PmdgEventIdInputAction: "FSUIPC - PMDG - Event ID",
      LuaMacroInputAction: "FSUIPC - Lua Macro",
      JeehellInputAction: "FSUIPC - Jeehell - Events",
      EventIdInputAction: "FSUIPC - EventID",
    } as Record<string, string>

    const inputActionOption = ActionTypeOptions

    for (const projectSettings of projectSettingsToTest) {
      await configListPage.gotoPage()
      await configListPage.mobiFlightPage.initWithTestDataAndSpecificProjectProps(
        projectSettings,
        "inputaction",
      )

      await configListPage.clickEditButtonForRow(1)
      const actionTypeComboBox = page.getByTestId("action-type-combobox")
      await expect(actionTypeComboBox).toBeVisible()
      await actionTypeComboBox.click()

      const expectedOptionVisiblity = inputActionOption.map((option) => ({
        value: option.value,
        label: actionTypeOptionLabels[option.value],
        isVisible: option.isAvailable(projectSettings),
      }))

      for (const expected of expectedOptionVisiblity) {
        const option = page.getByRole("listbox").getByRole("option", {
          name: expected.label,
          exact: true,
        })
        if (expected.isVisible) {
          await expect(option).toBeVisible()
        } else {
          await expect(option).not.toBeVisible()
        }
      }
    }
  })

  test("Action type panel copy and paste is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")

    const actionEditor = await openActionEditor(configListPage, page, 1)
    const copyButton = actionEditor.getByRole("button", { name: "Copy" })
    const pasteButton = actionEditor.getByRole("button", { name: "Paste" })

    await expect(copyButton).toBeVisible()
    await expect(pasteButton).toBeVisible()
    await expect(pasteButton).toBeDisabled()

    await copyButton.click()
    await expect(pasteButton).toBeEnabled()

    const onReleaseTab = page.getByRole("tab", { name: "On Release" })
    await onReleaseTab.click()
    await expect(actionEditor.getByRole("combobox").filter({ hasText: "Select..." })).toBeVisible()

    await pasteButton.click()
    await expect(actionEditor.getByRole("combobox").filter({ hasText: "Microsoft Flight Simulator (all versions)" })).toBeVisible()
  })
})

test.describe("Input Config Wizard - MSFS Input Action Panel", () => {
  test("Loaded config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 1)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Microsoft Flight Simulator (all versions)" }),
    ).toBeVisible()
    // Pre-selected preset label is shown
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "AP_PANEL_HEADING_HOLD" }),
    ).toBeVisible()
    // The preset has no description in the mock data
    await expect(
      actionEditor.getByText("No description available"),
    ).toBeVisible()
    // Code field reflects the preset command
    await expect(
      actionEditor.getByRole("textbox", { name: "Code:" }),
    ).toHaveValue("(>K:AP_PANEL_HEADING_HOLD)")
  })

  test("Preset filter narrows the list and the count updates", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 1)
    const filterInput = actionEditor.getByPlaceholder("Filter presets")
    const countLabel = actionEditor.getByRole("status")

    await expect(countLabel).toHaveText("4 preset(s) found")

    await filterInput.fill("AP_PANEL_HEADING_HOLD")
    await expect(countLabel).toHaveText("1 preset(s) found")

    await filterInput.fill("NonExistingPreset")
    await expect(countLabel).toHaveText("0 preset(s) found")

    await filterInput.fill("")
    await expect(countLabel).toHaveText("4 preset(s) found")
  })

  test("Selecting a preset updates the code field and description", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 1)
    // Open the preset ComboBox (currently shows the selected preset)
    await actionEditor
      .getByRole("combobox")
      .filter({ hasText: "AP_PANEL_HEADING_HOLD" })
      .click()
    await page.getByRole("option", { name: "AS1000_PFD_VOL_1_DEC" }).click()
    // Code field updates to the new preset's command
    await expect(
      actionEditor.getByRole("textbox", { name: "Code:" }),
    ).toHaveValue("(>H:AS1000_PFD_VOL_1_DEC)")
    // Description updates
    await expect(actionEditor.getByText("Garmin G1000")).toBeVisible()
  })
})

test.describe("Input Config Wizard - X-Plane Input Action Panel", () => {
  test("Loaded config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 2, undefined, { Sim: "xplane" })
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "X-Plane (all versions)" }),
    ).toBeVisible()
    // Input type from test data
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Command" }),
    ).toBeVisible()
    // Pre-selected preset label is shown (code matches the preset in mock data)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "land_alt_press_dn" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByText("Landing Altitude Pressure Down"),
    ).toBeVisible()
    // Code field reflects the path
    await expect(
      actionEditor.getByRole("textbox", { name: "Code:" }),
    ).toHaveValue("laminar/B738/knob/land_alt_press_dn")
  })

  test("Preset filter narrows the list and the count updates", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 2)
    const filterInput = actionEditor.getByPlaceholder("Filter presets")
    const countLabel = actionEditor.getByRole("status")

    await expect(countLabel).toHaveText("3 preset(s) found")

    await filterInput.fill("land_alt_press_dn")
    await expect(countLabel).toHaveText("1 preset(s) found")

    await filterInput.fill("NoMatch")
    await expect(countLabel).toHaveText("0 preset(s) found")

    await filterInput.fill("")
    await expect(countLabel).toHaveText("3 preset(s) found")
  })

  test("Selecting a preset updates the code field and input type", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 2)
    // Select a DataRef preset (different code type)
    await actionEditor
      .getByRole("combobox")
      .filter({ hasText: "land_alt_press_dn" })
      .click()
    await page.getByRole("option", { name: "test_dataref" }).click()
    // Code field updates
    await expect(
      actionEditor.getByRole("textbox", { name: "Code:" }),
    ).toHaveValue("laminar/B738/test/dataref")
    // Input type updates from Command to DataRef
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: /^DataRef$/ }),
    ).toBeVisible()
    // Description updates
    await expect(actionEditor.getByText("Test DataRef Preset")).toBeVisible()
  })
})

test.describe("Input Config Wizard - Variable Input Action Panel", () => {
  test("Loaded config data is displayed correctly", async ({
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
    // Variable presets
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: /^MyVar \(number\)$/ }),
    ).toBeVisible()
    // Variable type
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: /^Number$/ }),
    ).toBeVisible()
    // Variable name
    await expect(
      actionEditor.getByPlaceholder("Enter variable name..."),
    ).toHaveValue("MyVar")
    // Expression field
    await expect(
      actionEditor.getByPlaceholder("Enter expression..."),
    ).toHaveValue("$")
  })
})

test.describe("Input Config Wizard - Retrigger Input Action Panel", () => {
  test("Panel description is shown", async ({ configListPage, page }) => {
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

test.describe("Input Config Wizard - Keyboard Input Action Panel", () => {
  test("New config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithEmptyData()

    const addInputConfigButton = page.getByRole("button", {
      name: "Add Input Config",
    })
    await addInputConfigButton.click()
    await configListPage.mobiFlightPage.publishMessage({
      key: "ConfigValueFullUpdate",
      payload: {
        ConfigIndex: 0,
        ConfigItems: [
          {
            Type: "InputConfigItem",
            Name: "Keyboard Input Example",
            Active: true,
            GUID: "87654321-4321-4321-4321-BA0987654321",
            Preconditions: [],
            ConfigRefs: [],
            Status: {},
          },
        ],
      } as ConfigValueFullUpdate,
    })
    await expect(page.getByText("Edit Input Configuration")).toBeVisible()

    const scanForInputButton = page.getByRole("button", {
      name: "Scan for input",
    })
    await expect(scanForInputButton).toBeVisible()
    await scanForInputButton.click()

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

    const actionEditor = page.getByTestId("action-editor")
    await expect(actionEditor).toBeVisible()

    const actionComboBox = actionEditor.getByRole("combobox")
    await expect(actionComboBox).toBeVisible()
    await actionComboBox.click()

    const keyboardInputOption = page.getByRole("option", {
      name: "MobiFlight - Keyboard Input",
    })
    await expect(keyboardInputOption).toBeVisible()
    await keyboardInputOption.click()
    await expect(keyboardInputOption).not.toBeVisible()

    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "MobiFlight - Keyboard Input" }),
    ).toBeVisible()
    await expect(actionEditor.getByText("Key combo:None")).toBeVisible()
  })

  test("Loaded config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 5)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "MobiFlight - Keyboard Input" }),
    ).toBeVisible()
    // Key combo from test data: Control=true, Alt=true, Shift=true, Key=68 ('D')
    await expect(actionEditor).toContainText("Ctrl + Alt + Shift + D")
  })

  test("Scan toggle switches button label and back", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 5)
    const scanButton = actionEditor.getByRole("button", {
      name: "Scan for keyboard",
    })
    await expect(scanButton).toBeVisible()

    await scanButton.click()
    await expect(
      actionEditor.getByRole("button", { name: "Stop scanning" }),
    ).toBeVisible()
    await expect(scanButton).not.toBeVisible()

    await actionEditor.getByRole("button", { name: "Stop scanning" }).click()
    await expect(scanButton).toBeVisible()
  })

  test("Clear button resets the key combo to None", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 5)
    await expect(actionEditor).toContainText("Ctrl + Alt + Shift + D")

    await actionEditor.getByRole("button", { name: "Clear input" }).click()
    await expect(actionEditor).toContainText("None")
    await expect(actionEditor).not.toContainText("Ctrl +")
  })
})

test.describe("Input Config Wizard - vJoy Input Action Panel", () => {
  const vJoyDefinitions = {
    key: "VJoyDefinitionsUpdate",
    payload: {
      Definitions: [
        {
          Id: 1,
          Buttons: 16,
          Axis: { X: true, Y: true, Z: true, RX: false, RY: false, RZ: false },
        },
      ],
    },
  }

  test("Button config: device, button number and state are displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(
      configListPage,
      page,
      6,
      async (configListPage) => {
        await configListPage.mobiFlightPage.trackCommand(
          "CommandRefreshPresets",
        )
      },
    )
    // The panel will as for the current vJoy definitions to get the device and button labels
    const command = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(command).toContainEqual({
      key: "CommandRefreshPresets",
      payload: { type: "vjoy" },
    })

    // Publish the vJoy definitions so the panel can render the correct labels
    await configListPage.mobiFlightPage.publishMessage(vJoyDefinitions)

    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "MobiFlight - Virtual Joystick input (vJoy)" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "vJoy Device 1" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("tab", { name: "button" }),
    ).toHaveAttribute("data-state", "active")
    // buttonNr=4 from test data
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Button 4" }),
    ).toBeVisible()
    // buttonComand=true → "Pressed"
    await expect(actionEditor.getByText("Pressed")).toBeVisible()
  })

  test("Axis config: device, axis and send value are displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(
      configListPage,
      page,
      7,
      async (configListPage) => {
        await configListPage.mobiFlightPage.trackCommand(
          "CommandRefreshPresets",
        )
      },
    )
    // The panel will as for the current vJoy definitions to get the device and button labels
    const command = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(command).toContainEqual({
      key: "CommandRefreshPresets",
      payload: { type: "vjoy" },
    })

    await configListPage.mobiFlightPage.publishMessage(vJoyDefinitions)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "MobiFlight - Virtual Joystick input (vJoy)" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "vJoy Device 1" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("tab", { name: "axis" }),
    ).toHaveAttribute("data-state", "active")
    // axisString="Z" from test data
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Z" }),
    ).toBeVisible()
    // sendValue="1024"
    await expect(actionEditor.getByLabel("Axis value")).toHaveValue("1024")
  })
})

test.describe("Input Config Wizard - FSUIPC Offset Input Action Panel", () => {
  test("Loaded config data is displayed correctly with hex formatting", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 8)
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "FSUIPC - Offset" }),
    ).toBeVisible()
    // Size=4 → "4 Bytes"
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "4 Bytes" }),
    ).toBeVisible()
    // Offset=26316 decimal → 0x66CC, padded to 4 chars
    await expect(
      actionEditor.getByRole("textbox", { name: "Offset" }),
    ).toHaveValue("66CC")
    // Mask=733295205870 decimal → 0xAABBCCDDEE, sliced to last 8 chars for size=4
    await expect(
      actionEditor.getByRole("textbox", { name: "Mask" }),
    ).toHaveValue("BBCCDDEE")
    // BcdMode=true
    await expect(actionEditor.getByLabel("BCD Mode")).toHaveAttribute(
      "aria-checked",
      "true",
    )
  })
})

test.describe("Input Config Wizard - FSUIPC EventID Input Action Panel", () => {
  test("Loaded config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 9)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "FSUIPC - EventID" }),
    ).toBeVisible()
    // EventId=68036 from test data
    await expect(
      actionEditor.getByRole("textbox", { name: "Event ID" }),
    ).toHaveValue("68036")
    // Param="0" — shown in Custom Param
    await expect(
      actionEditor.getByRole("textbox", { name: "Custom Param" }),
    ).toHaveValue("0")
    // Preset selector visible
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Select preset..." }),
    ).toBeVisible()
  })
})

test.describe("Input Config Wizard - FSUIPC PMDG EventID Input Action Panel", () => {
  test("Loaded config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(
      configListPage,
      page,
      10,
      async (configListPage) => {
        await configListPage.mobiFlightPage.page.route(
          "*/**/presets/presets_eventids_pmdg_747.cip",
          async (route) => {
            await route.fulfill({
              body: "EVT_OH_ELEC_APU_GEN1_SWITCH:69648",
              contentType: "text/plain",
            })
          },
        )
      },
    )

    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "FSUIPC - PMDG - Event ID" }),
    ).toBeVisible()
    // Aircraft type B737 selected (first radio button)
    await expect(actionEditor.getByRole("radio").nth(1)).toHaveAttribute(
      "aria-checked",
      "true",
    )
    // EventId=69648 from test data
    await expect(
      actionEditor.getByRole("textbox", { name: "Event ID" }),
    ).toHaveValue("69648")
    // Param="536870912" matches MOUSE_FLAG_LEFTSINGLE
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "MOUSE_FLAG_LEFTSINGLE" }),
    ).toBeVisible()
  })
})

test.describe("Input Config Wizard - FSUIPC Jeehell Input Action Panel", () => {
  test("Loaded config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 11)
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "FSUIPC - Jeehell - Events" }),
    ).toBeVisible()
    // EventId=6 maps to FCU_HDGKNOB_PRESS in the mocked .cip file
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "FCU_HDGKNOB_PRESS" }),
    ).toBeVisible()
    await expect(actionEditor.getByText("FCU Heading Knob Press")).toBeVisible()
    // Param="1" in the Value field
    await expect(
      actionEditor.getByRole("textbox", { name: "Value" }),
    ).toHaveValue("1")
  })

  test("Selecting a preset updates the function and description", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 11)
    await actionEditor
      .getByRole("combobox")
      .filter({ hasText: "FCU_HDGKNOB_PRESS" })
      .click()
    await page.getByRole("option", { name: "FCU_HDGKNOB_LONGPRESS" }).click()
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "FCU_HDGKNOB_LONGPRESS" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByText("FCU Heading Knob Long Press"),
    ).toBeVisible()
  })
})

test.describe("Input Config Wizard - FSUIPC Lua Macro Input Action Panel", () => {
  test("Loaded config data is displayed correctly", async ({
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

  test("Editing macro name and value updates the fields", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 12)
    const macroNameInput = actionEditor.getByRole("textbox", {
      name: "Macro Name:",
    })
    const macroValueInput = actionEditor.getByRole("textbox", {
      name: "Macro Value:",
    })

    await macroNameInput.fill("UpdatedMacro")
    await macroValueInput.fill("42")

    await expect(macroNameInput).toHaveValue("UpdatedMacro")
    await expect(macroValueInput).toHaveValue("42")
  })
})

test.describe("Input Config Wizard - ProSim Input Action Panel", () => {
  test("Without presets shows Refresh Presets button and sends refresh command", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 13)
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "ProSim" }),
    ).toBeVisible()
    await expect(actionEditor.getByText("No presets available")).toBeVisible()
    // Track the refresh command
    await configListPage.mobiFlightPage.trackCommand("CommandRefreshPresets")
    await actionEditor.getByRole("button", { name: "Refresh Presets" }).click()
    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toContainEqual({
      key: "CommandRefreshPresets",
      payload: { type: "prosim" },
    })
  })

  test("With presets loaded: filter and select updates the path", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await openActionEditor(configListPage, page, 13)
    await configListPage.mobiFlightPage.publishMessage({
      key: "ProSimDataRefDefinitionUpdate",
      payload: {
        DataRefs: {
          "aircraft.heading": {
            Name: "aircraft.heading",
            Description: "Aircraft Heading",
            CanRead: true,
            CanWrite: true,
            DataType: "float",
            DataUnit: "degrees",
          },
          "autopilot.altitude": {
            Name: "autopilot.altitude",
            Description: "Autopilot Altitude",
            CanRead: true,
            CanWrite: true,
            DataType: "float",
            DataUnit: "feet",
          },
        },
      },
    })
    // Filter input appears once presets are available
    await expect(actionEditor.getByPlaceholder("Filter presets")).toBeVisible()
    // Filter narrows the list
    await actionEditor.getByPlaceholder("Filter presets").fill("Heading")
    await expect(actionEditor.getByText("Aircraft Heading")).toBeVisible()
    await expect(actionEditor.getByText("Autopilot Altitude")).not.toBeVisible()
    // Clicking a preset updates the path
    await actionEditor.getByText("Aircraft Heading").click()
    await expect(actionEditor.locator("#path")).toContainText(
      "aircraft.heading",
    )
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
    await buttonPanel.getByRole("tab", { name: "On Release" }).click()
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Select..." }),
    ).toBeVisible()

    // Switching to onHold shows an empty action editor
    await buttonPanel.getByRole("tab", { name: "On Hold" }).click()
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Select..." }),
    ).toBeVisible()

    // Switching to onLongRelease shows an empty action editor
    await buttonPanel.getByRole("tab", { name: "On Long Release" }).click()
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Select..." }),
    ).toBeVisible()

    // Switching back to onPress still shows the MSFS action
    await buttonPanel.getByRole("tab", { name: "On Press" }).click()
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
      .getByRole("tab", { name: "On Right", exact: true })
      .click()
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Microsoft Flight Simulator (all versions)" }),
    ).toBeVisible()

    // onLeftFast is empty
    await encoderPanel.getByRole("tab", { name: "On Left Fast" }).click()
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Select..." }),
    ).toBeVisible()


    // onRightFast is empty
    await encoderPanel.getByRole("tab", { name: "On Right Fast" }).click()
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Select..." }),
    ).toBeVisible()

    // switching back to onLeft shows the MSFS action
    await encoderPanel
      .getByRole("tab", { name: "On Left", exact: true })
      .click()
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Microsoft Flight Simulator (all versions)" }),
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
