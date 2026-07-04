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
import {
  EventIdInputAction,
  FsuipcOffsetInputAction,
  JeehellInputAction,
  KeyInputAction,
  LuaMacroInputAction,
  MsfsInputAction,
  PmdgEventIdInputAction,
  ProSimInputAction,
  VJoyInputAction,
  XplaneInputAction,
} from "../src/types/config"
import {
  Blink,
  Comparison,
  ComparisonOperators,
  Interpolation,
  MODIFIER_TYPES,
  Padding,
  Substring,
  Transformation,
} from "../src/types/modifier"

const jeehellPresetsContent = `FCU_KNOBS:GROUP
FCU_HDGKNOB_PRESS:6:FCU Heading Knob Press
FCU_HDGKNOB_LONGPRESS:7:FCU Heading Knob Long Press
AP_ENGAGE:8:Autopilot Engage`

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

// Helper: open the dialog for a given row and return the action-panel locator
// (onPress tab is active by default for button inputs)
const openWizardAndReturnActionPanel = async (
  configListPage: ConfigListPage,
  page: Page,
  row: number,
  callback?: (configListPage: ConfigListPage) => Promise<void>,
  projectOptions?: Partial<Project>,
) => {
  await configListPage.gotoPage()
  if (projectOptions) {
    await configListPage.mobiFlightPage.initWithTestDataAndSpecificProjectProps(
      projectOptions,
      "inputaction",
    )
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
  const actionPanel = page.getByTestId("action-panel")

  // expect it to become visible
  // this ensures that react render has completed and, e.g., useEffects have run
  await expect(actionPanel).toBeVisible()

  return actionPanel
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

test.describe("Input Config Wizard - Edit name", () => {
  test("Editing name works correctly", async ({ configListPage, page }) => {
    // Add new config
    const addInputConfigButton = page.getByRole("button", {
      name: "Add Input Config",
    })
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
    await addInputConfigButton.click()
    await configListPage.addNewConfigItem("InputConfigItem", 0, "inputaction")
    await expect(page.getByText("Edit Input Configuration")).toBeVisible()

    // click on the Name label to enter edit mode
    const nameLabel = page.getByTestId("dialog-config-name").getByRole("button")
    const nameInput = page
      .getByTestId("dialog-config-name")
      .getByRole("textbox")
    const testLabel = "My new input config"

    await expect(nameInput).not.toBeVisible()
    await expect(nameLabel).toBeVisible()
    await nameLabel.dblclick()

    await expect(nameInput).toBeVisible()
    await nameInput.fill(testLabel)

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.Name).toEqual(testLabel)
  })

  test("Config name is automatically in edit mode for new configs", async ({
    configListPage,
    page,
  }) => {
    // Add new config with default name
    const addInputConfigButton = page.getByRole("button", {
      name: "Add Input Config",
    })
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
    await addInputConfigButton.click()
    await configListPage.addNewConfigItem("InputConfigItem", 0, "inputaction", {
      Name: "New Input Config",
      Controller: null, // ensure it's a new config
    })
    await expect(page.getByText("Edit Input Configuration")).toBeVisible()
    await expect(
      page.getByTestId("dialog-config-name").getByRole("textbox"),
    ).toBeVisible()
    await expect(
      page.getByTestId("dialog-config-name").getByRole("textbox"),
    ).toBeFocused()
  })

  test("Config name is not in edit mode for configs with non-default name", async ({
    configListPage,
    page,
  }) => {
    // Add new config with default name
    const addInputConfigButton = page.getByRole("button", {
      name: "Add Input Config",
    })
    // Open a config with non-default name (we create a new one for the test and simplicity)
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
    await addInputConfigButton.click()
    await configListPage.addNewConfigItem("InputConfigItem", 0, "inputaction", {
      Name: "New Input Config With Non-Default Name",
      Controller: null, // ensure it looks like a new config
    })

    await expect(page.getByText("Edit Input Configuration")).toBeVisible()
    // Label should be visible
    await expect(
      page
        .getByTestId("dialog-config-name")
        .getByText("New Input Config With Non-Default Name", { exact: true }),
    ).toBeVisible()

    // No input should be visible 
    await expect(
      page.getByTestId("dialog-config-name").getByRole("textbox"),
    ).not.toBeVisible()

    //no button should be focused
    await expect(
      page.getByTestId("dialog-config-name").getByRole("button"),
    ).not.toBeFocused()
  })

  test("Config name is not in edit mode for configs with default name but other settings made by user", async ({
    configListPage,
    page,
  }) => {
    // Add new config with default name
    const addInputConfigButton = page.getByRole("button", {
      name: "Add Input Config",
    })
    // Open a config with non-default name (we create a new one for the test and simplicity)
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
    await addInputConfigButton.click()

    // We are setting the `controller` property which indicates that
    // this is not a new default config anymore despite having the default name
    await configListPage.addNewConfigItem("InputConfigItem", 0, "inputaction", {
      Name: "New Input Config",
      Controller: {
        Name: "Bravo Throttle Quadrant",
        Serial: "JS-87654321",
      }
    })

    await expect(page.getByText("Edit Input Configuration")).toBeVisible()
    // Label should be visible
    await expect(
      page
        .getByTestId("dialog-config-name")
        .getByText("New Input Config", { exact: true }),
    ).toBeVisible()

    // No input should be visible 
    await expect(
      page.getByTestId("dialog-config-name").getByRole("textbox"),
    ).not.toBeVisible()

    //no button should be focused
    await expect(
      page.getByTestId("dialog-config-name").getByRole("button"),
    ).not.toBeFocused()
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

  test("Updating controller doesn't send Devices back to backend", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    await configListPage.clickEditButtonForRow(1)
    const triggerPanel = page.getByTestId("trigger-panel")
    await expect(triggerPanel).toBeVisible()

    // Clear existing selection first
    const clearSelectedInputButton = triggerPanel.getByRole("button", {
      name: "Clear input",
    })
    await expect(clearSelectedInputButton).toBeVisible()
    await clearSelectedInputButton.click()

    const controllerDropDown = triggerPanel
      .getByRole("combobox")
      .filter({ hasText: "Select controller..." })

    const optionsPopup = page.getByRole("listbox")
    await controllerDropDown.click()
    await expect(optionsPopup).toBeVisible()
    const options = optionsPopup.getByRole("option")

    // click throttle option
    await expect(
      options.filter({ hasText: "Bravo Throttle Quadrant" }),
    ).toBeVisible()
    await options.filter({ hasText: "Bravo Throttle Quadrant" }).click()
    await expect(
      options.filter({ hasText: "Bravo Throttle Quadrant" }),
    ).not.toBeVisible()
    const saveButton = page.getByRole("button", {
      name: "Save",
    })
    await saveButton.click()

    const commandsAfterClick =
      await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commandsAfterClick?.length).toBe(1)
    expect(commandsAfterClick![0].key).toBe("CommandUpdateConfigItem")
    const updatedController = commandsAfterClick![0].payload.item.Controller

    expect(updatedController?.Devices).toBeUndefined()
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

test.describe("Input Config Wizard - Modifier Panel", () => {
  test("Summary is displayed correctly", async ({ configListPage, page }) => {
    await CreateNewInputConfigItemAndWaitForDialog(configListPage, page)

    const modifiersPanel = page.getByTestId("modifiers-panel")
    await expect(modifiersPanel).toBeVisible()

    const addModifierButton = modifiersPanel.getByRole("button", {
      name: "Add modifier",
    })
    await expect(addModifierButton).toBeVisible()

    await addModifierButton.click()

    const modifierEditor = page.getByTestId("modifier-editor")
    await expect(modifierEditor).toBeVisible()

    const addModifierButtonInEditor = modifierEditor.getByRole("button", {
      name: "Add modifier",
    })
    await expect(addModifierButtonInEditor).toBeVisible()

    await addModifierButtonInEditor.click()

    const modifierItems = page.getByRole("menuitem")
    await expect(modifierItems).toHaveCount(6)

    for (const modifier of await modifierItems.all()) {
      await expect(modifier).toBeVisible()
      await modifier.click()
      await expect(modifier).not.toBeVisible()
      // open the popup with the options
      await addModifierButtonInEditor.click()
    }

    // close the popup with the options
    await page.keyboard.press("Escape")

    const goBackButton = page.getByRole("button", { name: "Go back" })
    await expect(goBackButton).toBeVisible()
    await goBackButton.click()

    await expect(modifierEditor).not.toBeVisible()

    const labels = [
      "Transformation",
      "Substring",
      "Padding",
      "Interpolation",
      "+ 2 more",
    ]

    for (const label of labels) {
      await expect(modifiersPanel.getByText(label)).toBeVisible()
    }
  })

  test("All modifiers can be added and removed", async ({
    configListPage,
    page,
  }) => {
    await CreateNewInputConfigItemAndWaitForDialog(configListPage, page)

    const modifiersPanel = page.getByTestId("modifiers-panel")
    await expect(modifiersPanel).toBeVisible()
    const addModifierButton = modifiersPanel.getByRole("button", {
      name: "Add modifier",
    })
    await expect(addModifierButton).toBeVisible()
    await addModifierButton.click()

    const modifiers = MODIFIER_TYPES

    for (const modifier of modifiers) {
      const modifierEditor = page.getByTestId("modifier-editor")
      await expect(modifierEditor).toBeVisible()

      const addModifierButtonInEditor = modifierEditor.getByRole("button", {
        name: "Add modifier",
      })
      await expect(addModifierButtonInEditor).toBeVisible()
      await addModifierButtonInEditor.click()

      const modifierLabel = modifier

      const transformationOption = page.getByRole("menuitem", {
        name: modifierLabel,
      })
      await expect(transformationOption).toBeVisible()
      await transformationOption.click()

      const modifierHeader = modifierEditor.getByRole("button", {
        name: modifierLabel,
      })
      await expect(modifierHeader).toBeVisible()

      const removeButton = modifierEditor.getByRole("button", {
        name: "Remove Modifier",
      })
      await expect(removeButton).toBeVisible()
      await removeButton.click()

      await expect(removeButton).not.toBeVisible()
      await expect(modifierHeader).not.toBeVisible()
    }
  })

  test("Modifiers can be moved up and down", async ({
    configListPage,
    page,
  }) => {
    await CreateNewInputConfigItemAndWaitForDialog(configListPage, page)

    const modifiersPanel = page.getByTestId("modifiers-panel")
    await expect(modifiersPanel).toBeVisible()
    const addModifierButton = modifiersPanel.getByRole("button", {
      name: "Add modifier",
    })
    await expect(addModifierButton).toBeVisible()
    await addModifierButton.click()

    // only add first 3 modifiers for this test
    const modifiers = MODIFIER_TYPES.slice(0, 3)

    for (const modifier of modifiers) {
      const modifierEditor = page.getByTestId("modifier-editor")
      await expect(modifierEditor).toBeVisible()

      const addModifierButtonInEditor = modifierEditor.getByRole("button", {
        name: "Add modifier",
      })
      await expect(addModifierButtonInEditor).toBeVisible()
      await addModifierButtonInEditor.click()

      const modifierLabel = modifier

      const transformationOption = page.getByRole("menuitem", {
        name: modifierLabel,
      })
      await expect(transformationOption).toBeVisible()
      await transformationOption.click()
    }

    const firstModifierItem = page.getByTestId("modifier-item").nth(0)
    const secondModifierItem = page.getByTestId("modifier-item").nth(1)

    await expect(firstModifierItem).toHaveText(/Transformation/)
    await expect(secondModifierItem).toHaveText(/Substring/)

    const firstMoveUpButton = firstModifierItem.getByRole("button", {
      name: "Move modifier up",
    })
    // first item cannot be moved up, so the button should be disabled
    await expect(firstMoveUpButton).toBeVisible()
    await expect(firstMoveUpButton).toBeDisabled()

    const firstMoveDownButton = firstModifierItem.getByRole("button", {
      name: "Move modifier down",
    })
    await expect(firstMoveDownButton).toBeVisible()
    await expect(firstMoveDownButton).toBeEnabled()
    // move down
    await firstMoveDownButton.click()

    // Verify that the first and second items have swapped positions
    await expect(firstModifierItem).toHaveText(/Substring/)
    await expect(secondModifierItem).toHaveText(/Transformation/)

    const secondMoveUpButton = secondModifierItem.getByRole("button", {
      name: "Move modifier up",
    })
    await expect(secondMoveUpButton).toBeVisible()
    // move up
    await secondMoveUpButton.click()

    // Verify that the first and second items have swapped positions back
    await expect(firstModifierItem).toHaveText(/Transformation/)
    await expect(secondModifierItem).toHaveText(/Substring/)

    // Verify last move down button is disabled
    const lastModifierItem = page.getByTestId("modifier-item").last()
    const lastMoveDownButton = lastModifierItem.getByRole("button", {
      name: "Move modifier down",
    })
    await expect(lastMoveDownButton).toBeVisible()
    await expect(lastMoveDownButton).toBeDisabled()
  })

  test("Transformation modifier works correctly", async ({
    configListPage,
    page,
  }) => {
    await CreateNewInputConfigItemAndWaitForDialog(configListPage, page)
    const modifierLabel = "Transformation"
    const modifierEditor = await addModifierItemAndReturnEditor(
      modifierLabel,
      page,
    )

    // switch is visible and clickable
    const switchToggle = modifierEditor.getByRole("switch")
    await expect(switchToggle).toBeVisible()
    await switchToggle.click()

    const modifierHeader = modifierEditor.getByRole("button", {
      name: modifierLabel,
    })
    await expect(modifierHeader).toBeVisible()
    await modifierHeader.click()

    // The modifier is now expanded and the input field is visible
    const inputField = modifierEditor.getByRole("textbox", {
      name: "Expression",
    })
    await expect(inputField).toBeVisible()
    await inputField.fill("$*2")

    await modifierHeader.click()
    await expect(inputField).not.toBeVisible()

    await expect(modifierHeader.getByText("$*2")).toBeVisible()

    const goBackButton = page.getByRole("button", { name: "Go back" })
    await expect(goBackButton).toBeVisible()
    await goBackButton.click()

    configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")
    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.Modifiers.Items[0]).toEqual({
      Type: "Transformation",
      Active: false,
      Expression: "$*2",
    } as Transformation)
  })

  test("Substring modifier works correctly", async ({
    configListPage,
    page,
  }) => {
    await CreateNewInputConfigItemAndWaitForDialog(configListPage, page)
    const modifierLabel = "Substring"
    const modifierEditor = await addModifierItemAndReturnEditor(
      modifierLabel,
      page,
    )

    // switch is visible and clickable
    const switchToggle = modifierEditor.getByRole("switch")
    await expect(switchToggle).toBeVisible()
    await switchToggle.click()

    const modifierHeader = modifierEditor.getByRole("button", {
      name: modifierLabel,
    })
    await expect(modifierHeader).toBeVisible()
    await modifierHeader.click()

    // The modifier is now expanded
    // Start input field is visible
    const startInputField = modifierEditor.getByRole("textbox", {
      name: "Start position",
    })
    await expect(startInputField).toBeVisible()
    await startInputField.fill("3")

    // End input field is visible
    const endInputField = modifierEditor.getByRole("textbox", {
      name: "End position",
    })
    await expect(endInputField).toBeVisible()
    await endInputField.fill("6")

    // The modifier is now collapsed1
    await modifierHeader.click()
    await expect(startInputField).not.toBeVisible()
    await expect(endInputField).not.toBeVisible()

    // Summary has updated
    await expect(modifierHeader.getByText("from 3 to 6")).toBeVisible()

    // Close the drawer
    const goBackButton = page.getByRole("button", { name: "Go back" })
    await expect(goBackButton).toBeVisible()
    await goBackButton.click()

    // Save the config
    configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")
    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    // Verify that the command sent to the backend has the correct modifier data
    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.Modifiers.Items[0]).toEqual({
      Type: "Substring",
      Active: false,
      Start: 3,
      End: 6,
    } as Substring)
  })

  test("Padding modifier works correctly", async ({ configListPage, page }) => {
    await CreateNewInputConfigItemAndWaitForDialog(configListPage, page)
    const modifierLabel = "Padding"
    const modifierEditor = await addModifierItemAndReturnEditor(
      modifierLabel,
      page,
    )

    // switch is visible and clickable
    const switchToggle = modifierEditor.getByRole("switch")
    await expect(switchToggle).toBeVisible()
    await switchToggle.click()

    const modifierHeader = modifierEditor.getByRole("button", {
      name: modifierLabel,
    })
    await expect(modifierHeader).toBeVisible()
    await modifierHeader.click()

    // The modifier is now expanded
    // Length input field is visible
    const lengthInputField = modifierEditor.getByRole("textbox", {
      name: "Length",
    })
    await expect(lengthInputField).toBeVisible()
    await lengthInputField.fill("3")

    // Value input field is visible
    const valueInputField = modifierEditor.getByRole("textbox", {
      name: "Character",
    })
    await expect(valueInputField).toBeVisible()
    // Summary updates correctly
    await valueInputField.fill(" ")
    await valueInputField.blur()

    await expect(
      modifierEditor.getByRole("button", {
        name: "Length: 3 Character: Space Direction: Left",
      }),
    ).toBeVisible()
    await valueInputField.fill("0")
    await valueInputField.blur()
    await expect(
      modifierEditor.getByRole("button", {
        name: "Length: 3 Character: 0 Direction: Left",
      }),
    ).toBeVisible()

    // Direction combobox
    const directionComboBox = modifierEditor.getByRole("combobox", {
      name: "Direction",
    })
    await expect(directionComboBox).toBeVisible()
    await directionComboBox.click()

    const directionOptions = page.getByRole("listbox").getByRole("option")
    // we expect two options (left/right)
    await expect(directionOptions).toHaveCount(2)
    await expect(directionOptions.filter({ hasText: "Left" })).toBeVisible()
    await expect(directionOptions.filter({ hasText: "Right" })).toBeVisible()

    // click on Right
    await directionOptions.filter({ hasText: "Right" }).click()

    // The modifier is now collapsed
    await modifierHeader.click()
    await expect(lengthInputField).not.toBeVisible()
    await expect(valueInputField).not.toBeVisible()
    await expect(directionComboBox).not.toBeVisible()

    await expect(
      modifierEditor.getByRole("button", {
        name: "Length: 3 Character: 0 Direction: Right",
      }),
    ).toBeVisible()

    // Close the drawer
    const goBackButton = page.getByRole("button", { name: "Go back" })
    await expect(goBackButton).toBeVisible()
    await goBackButton.click()

    // Save the config
    configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")
    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    // Verify that the command sent to the backend has the correct modifier data
    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.Modifiers.Items[0]).toEqual({
      Type: "Padding",
      Active: false,
      Length: 3,
      Character: "0",
      Direction: "Right",
    } as Padding)
  })

  test("Interpolation modifier works correctly", async ({
    configListPage,
    page,
  }) => {
    await CreateNewInputConfigItemAndWaitForDialog(configListPage, page)
    const modifierLabel = "Interpolation"
    const modifierEditor = await addModifierItemAndReturnEditor(
      modifierLabel,
      page,
    )

    // switch is visible and clickable
    const switchToggle = modifierEditor.getByRole("switch")
    await expect(switchToggle).toBeVisible()
    await switchToggle.click()

    const modifierHeader = modifierEditor.getByRole("button", {
      name: modifierLabel,
    })
    await expect(modifierHeader).toBeVisible()
    await modifierHeader.click()

    // The modifier is now expanded
    // Length input field is visible
    const mappingRows = modifierEditor.getByRole("row")

    // intially there are 3 rows for the header
    // and the two default mappings
    await expect(mappingRows).toHaveCount(3)

    const firstFromInput = mappingRows.nth(1).getByRole("textbox").first()

    await expect(firstFromInput).toBeVisible()
    await expect(firstFromInput).toHaveValue("0")
    const firstToInput = mappingRows.nth(1).getByRole("textbox").last()
    await expect(firstToInput).toBeVisible()
    await expect(firstToInput).toHaveValue("0")

    const secondFromInput = mappingRows.nth(2).getByRole("textbox").first()
    await expect(secondFromInput).toBeVisible()
    await expect(secondFromInput).toHaveValue("10")
    const secondToInput = mappingRows.nth(2).getByRole("textbox").last()
    await expect(secondToInput).toBeVisible()
    await expect(secondToInput).toHaveValue("1000")

    await firstFromInput.fill("5")
    await firstToInput.fill("50")
    await secondFromInput.fill("15")
    await secondToInput.fill("1500")

    // Add another mapping row
    const addMappingButton = modifierEditor.getByRole("button", {
      name: "Add mapping",
    })
    await expect(addMappingButton).toBeVisible()

    // bring focus out of input fields
    // this will normally automatically happen
    // when a user clicks the element manually
    await addMappingButton.focus()
    await addMappingButton.click()
    await expect(mappingRows).toHaveCount(4)

    const thirdFromInput = mappingRows.nth(3).getByRole("textbox").first()
    await expect(thirdFromInput).toBeVisible()
    await expect(thirdFromInput).toHaveValue("30")
    const thirdToInput = mappingRows.nth(3).getByRole("textbox").last()
    await expect(thirdToInput).toBeVisible()
    await expect(thirdToInput).toHaveValue("3000")

    // add fourth mapping row
    await addMappingButton.click()
    await expect(mappingRows).toHaveCount(5)
    const fourthRow = mappingRows.nth(4)
    await expect(fourthRow).toBeVisible()

    // and remove it
    await fourthRow.getByRole("button", { name: "Remove mapping" }).click()
    await expect(fourthRow).not.toBeVisible()
    await expect(mappingRows).toHaveCount(4)

    // Summary updates correctly
    await expect(
      modifierEditor.getByRole("button", {
        name: "3 values, range from 5 to 3000",
      }),
    ).toBeVisible()

    // Close the drawer
    const goBackButton = page.getByRole("button", { name: "Go back" })
    await expect(goBackButton).toBeVisible()
    await goBackButton.click()

    // Save the config
    configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")
    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    // Verify that the command sent to the backend has the correct modifier data
    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.Modifiers.Items[0]).toEqual({
      Type: "Interpolation",
      Active: false,
      Values: {
        5: 50,
        15: 1500,
        30: 3000,
      } as Record<number, number>,
    } as Interpolation)
  })

  test("Interpolation modifier items maintain position correctly", async ({
    configListPage,
    page,
  }) => {
    await CreateNewInputConfigItemAndWaitForDialog(configListPage, page)
    const modifierLabel = "Interpolation"
    const modifierEditor = await addModifierItemAndReturnEditor(
      modifierLabel,
      page,
    )

    const modifierHeader = modifierEditor.getByRole("button", {
      name: modifierLabel,
    })
    await expect(modifierHeader).toBeVisible()
    await modifierHeader.click()

    // The modifier is now expanded
    // Length input field is visible
    const mappingRows = modifierEditor.getByRole("row")

    // intially there are 3 rows for the header
    // and the two default mappings
    await expect(mappingRows).toHaveCount(3)

    const firstFromInput = mappingRows.nth(1).getByRole("textbox").first()
    const secondFromInput = mappingRows.nth(2).getByRole("textbox").first()

    await expect(firstFromInput).toBeVisible()
    await expect(firstFromInput).toHaveValue("0")

    await expect(secondFromInput).toBeVisible()
    await expect(secondFromInput).toHaveValue("10")

    // use a value that is higher than the next row
    // verify that the row still stays in the same place
    await firstFromInput.fill("20")
    await firstFromInput.blur()

    await expect(firstFromInput).toBeVisible()
    await expect(firstFromInput).toHaveValue("20")

    // Add another mapping row
    const addMappingButton = modifierEditor.getByRole("button", {
      name: "Add mapping",
    })
    await expect(addMappingButton).toBeVisible()

    // bring focus out of input fields
    // this will normally automatically happen
    // when a user clicks the element manually
    await addMappingButton.focus()
    await addMappingButton.click()
    await expect(mappingRows).toHaveCount(4)

    const thirdFromInput = mappingRows.nth(3).getByRole("textbox").first()
    await expect(thirdFromInput).toBeVisible()
    await expect(thirdFromInput).toHaveValue("20")

    // use a value that is smaller than the prior rows
    // verify that the row still stays in the same place
    await thirdFromInput.fill("5")
    await thirdFromInput.blur()

    await expect(thirdFromInput).toBeVisible()
    await expect(thirdFromInput).toHaveValue("5")

    // Close the drawer
    const goBackButton = page.getByRole("button", { name: "Go back" })
    await expect(goBackButton).toBeVisible()
    await goBackButton.click()

    // Save the config
    configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")
    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    // Verify that the command sent to the backend has the correct modifier data
    // and that the order of the items is now sorted ASC for "from" value
    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.Modifiers.Items[0]).toEqual({
      Type: "Interpolation",
      Active: true,
      Values: {
        5: 2000,
        10: 1000,
        20: 0,
      } as Record<number, number>,
    } as Interpolation)
  })


  test("Interpolation modifier remove buttons work correctly", async ({
    configListPage,
    page,
  }) => {
    await CreateNewInputConfigItemAndWaitForDialog(configListPage, page)
    const modifierLabel = "Interpolation"
    const modifierEditor = await addModifierItemAndReturnEditor(
      modifierLabel,
      page,
    )

    const modifierHeader = modifierEditor.getByRole("button", {
      name: modifierLabel,
    })
    await expect(modifierHeader).toBeVisible()
    await modifierHeader.click()

    // The modifier is now expanded
    // Length input field is visible
    const mappingRows = modifierEditor.getByRole("row")

    // intially there are 3 rows for the header
    // and the two default mappings
    await expect(mappingRows).toHaveCount(3)

    // Remove buttons are disabled
    const firstRemoveButton = mappingRows.nth(1).getByRole("button", {
      name: "Remove mapping",
    })
    const secondRemoveButton = mappingRows.nth(2).getByRole("button", {
      name: "Remove mapping",
    })

    await expect(firstRemoveButton).toBeVisible()
    await expect(firstRemoveButton).toBeDisabled()

    await expect(secondRemoveButton).toBeVisible()
    await expect(secondRemoveButton).toBeDisabled()

    // Add another mapping row
    const addMappingButton = modifierEditor.getByRole("button", {
      name: "Add mapping",
    })
    await expect(addMappingButton).toBeVisible()
    await addMappingButton.click()

    // Remove buttons are now enabled
    await expect(firstRemoveButton).toBeEnabled()
    await expect(secondRemoveButton).toBeEnabled()

    // Remove the first mapping row
    await firstRemoveButton.click()

    // Remove buttons are now disabled again
    await expect(firstRemoveButton).toBeDisabled()
    await expect(secondRemoveButton).toBeDisabled()
  })

  test("Comparison modifier works correctly", async ({
    configListPage,
    page,
  }) => {
    await CreateNewInputConfigItemAndWaitForDialog(configListPage, page)
    const modifierLabel = "Comparison"
    const modifierEditor = await addModifierItemAndReturnEditor(
      modifierLabel,
      page,
    )

    // switch is visible and clickable
    const switchToggle = modifierEditor.getByRole("switch")
    await expect(switchToggle).toBeVisible()
    await switchToggle.click()

    const modifierHeader = modifierEditor.getByRole("button", {
      name: modifierLabel,
    })
    await expect(modifierHeader).toBeVisible()
    await modifierHeader.click()

    // The modifier is now expanded
    // Operator combobox
    const operatorComboBox = modifierEditor.getByRole("combobox", {
      name: "Operator",
    })
    await expect(operatorComboBox).toBeVisible()
    await operatorComboBox.click()

    const operatorOptions = page.getByRole("listbox").getByRole("option")
    await expect(operatorOptions).toHaveCount(ComparisonOperators.length)

    // all options are available
    for (const operator of ComparisonOperators) {
      await expect(
        operatorOptions.getByText(operator, { exact: true }),
      ).toBeVisible()
    }

    // select the "!=" operator
    operatorOptions.getByText("!=", { exact: true }).click()

    // Value input field is visible
    const valueInputField = modifierEditor.getByRole("textbox", {
      name: "Value",
    })
    await expect(valueInputField).toBeVisible()
    await valueInputField.fill("3")

    // Then input field is visible
    const thenInputField = modifierEditor.getByRole("textbox", {
      name: "Then",
    })
    await expect(thenInputField).toBeVisible()
    await thenInputField.fill("1")

    // Else input field is visible
    const elseInputField = modifierEditor.getByRole("textbox", {
      name: "Else",
    })
    await expect(elseInputField).toBeVisible()
    await elseInputField.fill("0")

    // Summary updates correctly
    await expect(
      modifierEditor.getByRole("button", { name: "if $ != 3 then 1 else 0" }),
    ).toBeVisible()

    // The modifier is now collapsed
    await modifierHeader.click()
    await expect(operatorComboBox).not.toBeVisible()
    await expect(valueInputField).not.toBeVisible()
    await expect(thenInputField).not.toBeVisible()
    await expect(elseInputField).not.toBeVisible()

    // Close the drawer
    const goBackButton = page.getByRole("button", { name: "Go back" })
    await expect(goBackButton).toBeVisible()
    await goBackButton.click()

    // Save the config
    configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")
    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    // Verify that the command sent to the backend has the correct modifier data
    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.Modifiers.Items[0]).toEqual({
      Type: "Comparison",
      Active: false,
      Value: "3",
      IfValue: "1",
      ElseValue: "0",
      Operand: "!=",
    } as Comparison)
  })

  test("Blink modifier works correctly", async ({ configListPage, page }) => {
    await CreateNewInputConfigItemAndWaitForDialog(configListPage, page)
    const modifierLabel = "Blink"
    const modifierEditor = await addModifierItemAndReturnEditor(
      modifierLabel,
      page,
    )

    // switch is visible and clickable
    const switchToggle = modifierEditor.getByRole("switch")
    await expect(switchToggle).toBeVisible()
    await switchToggle.click()

    const modifierHeader = modifierEditor.getByRole("button", {
      name: modifierLabel,
    })
    await expect(modifierHeader).toBeVisible()
    await modifierHeader.click()

    // The modifier is now expanded
    // Alternate value input field is visible
    const alternateValueInputField = modifierEditor.getByRole("textbox", {
      name: "Alternate value (Off)",
    })
    await expect(alternateValueInputField).toBeVisible()
    await expect(alternateValueInputField).toHaveValue("0")

    // Fill in the alternate value
    await alternateValueInputField.fill("1")

    const sequenceRows = modifierEditor.getByRole("row")

    // intially there are 2 rows
    // one for the header
    // and one for default sequence
    await expect(sequenceRows).toHaveCount(2)

    const firstOnInput = sequenceRows.nth(1).getByRole("textbox").first()

    await expect(firstOnInput).toBeVisible()
    await expect(firstOnInput).toHaveValue("500")
    const firstOffInput = sequenceRows.nth(1).getByRole("textbox").last()
    await expect(firstOffInput).toBeVisible()
    await expect(firstOffInput).toHaveValue("500")

    await firstOnInput.fill("350")
    await firstOffInput.fill("650")
    // blur to trigger the change event
    await firstOffInput.blur()

    // Add another mapping row
    const addIntervalButton = modifierEditor.getByRole("button", {
      name: "Add blink interval",
    })
    await expect(addIntervalButton).toBeVisible()
    await addIntervalButton.click()

    const secondOnInput = sequenceRows.nth(2).getByRole("textbox").first()
    await expect(secondOnInput).toBeVisible()
    await expect(secondOnInput).toHaveValue("350")
    const secondOffInput = sequenceRows.nth(2).getByRole("textbox").last()
    await expect(secondOffInput).toBeVisible()
    await expect(secondOffInput).toHaveValue("650")
    await secondOffInput.blur()

    // add third mapping row
    await addIntervalButton.click()
    await expect(sequenceRows).toHaveCount(4)
    const thirdRow = sequenceRows.nth(3)
    await expect(thirdRow).toBeVisible()

    // and remove it
    await thirdRow
      .getByRole("button", { name: "Remove blink interval" })
      .click()
    await expect(thirdRow).not.toBeVisible()
    await expect(sequenceRows).toHaveCount(3)

    // Summary updates correctly
    await expect(
      modifierEditor.getByRole("button", {
        name: "Value 1 Sequence 350 / 650",
      }),
    ).toBeVisible()

    // Close the drawer
    const goBackButton = page.getByRole("button", { name: "Go back" })
    await expect(goBackButton).toBeVisible()
    await goBackButton.click()

    // Save the config
    configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")
    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    // Verify that the command sent to the backend has the correct modifier data
    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.Modifiers.Items[0]).toEqual({
      Type: "Blink",
      Active: false,
      BlinkValue: "1",
      OnOffSequence: [350, 650, 350, 650],
    } as Blink)
  })

  test("Blink modifier remove buttons work correctly", async ({
    configListPage,
    page,
  }) => {
    await CreateNewInputConfigItemAndWaitForDialog(configListPage, page)
    const modifierLabel = "Blink"
    const modifierEditor = await addModifierItemAndReturnEditor(
      modifierLabel,
      page,
    )

    const modifierHeader = modifierEditor.getByRole("button", {
      name: modifierLabel,
    })
    await expect(modifierHeader).toBeVisible()
    await modifierHeader.click()

    // The modifier is now expanded
    // Alternate value input field is visible
    const sequenceRows = modifierEditor.getByRole("row")
    await expect(sequenceRows).toHaveCount(2)

    const firstRemoveButton = sequenceRows.nth(1).getByRole("button")
    const secondRemoveButton = sequenceRows.nth(2).getByRole("button")

    await expect(firstRemoveButton).toBeVisible()
    await expect(firstRemoveButton).toBeDisabled()

    // Add another mapping row
    const addIntervalButton = modifierEditor.getByRole("button", {
      name: "Add blink interval",
    })
    await expect(addIntervalButton).toBeVisible()
    await addIntervalButton.click()

    await expect(firstRemoveButton).toBeEnabled()
    await expect(secondRemoveButton).toBeEnabled()

    // Remove the first mapping row
    await firstRemoveButton.click()
    await expect(firstRemoveButton).toBeDisabled()
  })
})

test.describe("Input Config Wizard - Action Type Panel", () => {
  test("Action types honor project settings and features", async ({
    configListPage,
    page,
  }) => {
    test.slow()
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
    const inputActionOption = ActionTypeOptions

    for (const projectSettings of projectSettingsToTest) {
      await configListPage.gotoPage()
      await configListPage.mobiFlightPage.initWithTestDataAndSpecificProjectProps(
        projectSettings,
        "inputaction",
      )

      await configListPage.clickEditButtonForRow(1)

      const actionEditButton = page.getByRole("button", {
        name: "Edit On Press Action",
      })
      await expect(actionEditButton).toBeVisible()
      await actionEditButton.click()

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

    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      1,
    )
    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")
    const copyButton = actionEditor.getByRole("button", { name: "Copy" })
    const pasteButton = actionEditor.getByRole("button", { name: "Paste" })

    await expect(copyButton).toBeVisible()
    await expect(pasteButton).toBeVisible()
    await expect(pasteButton).toBeDisabled()

    await copyButton.click()
    await expect(pasteButton).toBeEnabled()

    // Navigate away and close the action editor
    const goBackButton = page.getByRole("button", { name: "Go back" })
    await expect(goBackButton).toBeVisible()
    await goBackButton.click()

    const onReleaseTab = page.getByRole("button", { name: "On Release" })
    await onReleaseTab.click()
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Select..." }),
    ).toBeVisible()

    await pasteButton.click()
    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Microsoft Flight Simulator (all versions)" }),
    ).toBeVisible()
  })
})

test.describe("Input Config Wizard - MSFS Input Action Panel", () => {
  test("Loaded config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      1,
    )
    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Microsoft Flight Simulator (all versions)" }),
    ).toBeVisible()
    // Pre-selected preset label is shown
    await expect(
      actionEditor
        .getByRole("combobox")
        .getByText("AP_PANEL_HEADING_HOLD_TEST", { exact: true }),
    ).toBeVisible()
    // The preset has no description in the mock data
    await expect(
      actionEditor.getByText("No description available"),
    ).toBeVisible()
    // Code field reflects the preset command
    await expect(
      actionEditor.getByRole("textbox", { name: "Enter RPN code" }),
    ).toHaveValue("(>K:AP_PANEL_HEADING_HOLD)")
  })

  test("Summary information is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionPanel = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      1,
    )
    // Action type is shown in the summary
    await expect(actionPanel.getByText("MSFS Preset")).toBeVisible()

    // The preset label is shown in the summary
    await expect(
      actionPanel.getByText("AP_PANEL_HEADING_HOLD_TEST", { exact: true }),
    ).toBeVisible()

    // The code is shown in the summary
    await expect(
      actionPanel.getByText("(>K:AP_PANEL_HEADING_HOLD)", { exact: true }),
    ).toBeVisible()
  })

  test("Preset filter narrows the list and the count updates", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      1,
    )
    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")
    const filterInput = actionEditor.getByPlaceholder("Filter presets")
    const countLabel = actionEditor.getByRole("status")
    const resetFiltersButton = actionEditor.getByRole("button", {
      name: "Reset filters",
    })

    // we will have only 1 preset available because of the test data filtering
    await expect(countLabel).toHaveText("1 preset(s) found")
    await resetFiltersButton.click()

    // now all options are available
    await expect(countLabel).toHaveText("4 preset(s) found")

    // Filter by an exact preset name -> 1 preset found
    await filterInput.fill("AP_PANEL_HEADING_HOLD")
    await expect(countLabel).toHaveText("1 preset(s) found")

    // filter by a non-existing preset name -> 0 presets found
    await filterInput.fill("NonExistingPreset")
    await expect(countLabel).toHaveText("0 preset(s) found")

    // Reset the filter -> all presets are available again
    await filterInput.fill("")
    await expect(countLabel).toHaveText("4 preset(s) found")
  })

  test("Selecting a preset updates the code field and description", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      1,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

    // reset filters to really have all options available
    const resetFiltersButton = actionEditor.getByRole("button", {
      name: "Reset filters",
    })
    await resetFiltersButton.click()

    // Open the preset ComboBox (currently shows the selected preset)
    await actionEditor
      .getByRole("combobox")
      .getByText("AP_PANEL_HEADING_HOLD_TEST", { exact: true })
      .click()
    await page.getByRole("option", { name: "AS1000_PFD_VOL_1_DEC" }).click()
    // Code field updates to the new preset's command
    await expect(
      actionEditor.getByRole("textbox", { name: "Enter RPN code" }),
    ).toHaveValue("(>H:AS1000_PFD_VOL_1_DEC)")
    // Description updates
    await expect(actionEditor.getByText("Garmin G1000")).toBeVisible()
  })

  test("Preset filter combo boxes work correctly", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      1,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

    const countLabel = actionEditor.getByRole("status")
    const resetFiltersButton = actionEditor.getByRole("button", {
      name: "Reset filters",
    })

    await expect(countLabel).toHaveText("1 preset(s) found")
    await resetFiltersButton.click()
    await expect(countLabel).toHaveText("4 preset(s) found")

    const optionsList = page.getByRole("listbox")

    // Select a vendor filter
    await actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Filter by vendor" })
      .click()
    await expect(optionsList).toBeVisible()
    const vendorOption = optionsList.getByRole("option", { name: "Microsoft" })
    await expect(vendorOption).toBeVisible()
    await vendorOption.click()
    await expect(vendorOption).not.toBeVisible()

    await expect(countLabel).toHaveText("3 preset(s) found")

    // Select an aircraft filter
    await actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Filter by aircraft" })
      .click()
    await expect(optionsList).toBeVisible()
    const aircraftOption = optionsList.getByRole("option", { name: "Generic" })
    await expect(aircraftOption).toBeVisible()
    await aircraftOption.click()
    await expect(aircraftOption).not.toBeVisible() // Should be removed from options since it's already selected as a filter

    await expect(countLabel).toHaveText("2 preset(s) found")

    // Select a system filter
    await actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Filter by system" })
      .click()
    await expect(optionsList).toBeVisible()
    const systemOption = optionsList.getByRole("option", { name: "Avionics" })
    await expect(systemOption).toBeVisible()
    await systemOption.click()
    await expect(systemOption).not.toBeVisible()

    await expect(countLabel).toHaveText("1 preset(s) found")

    await expect(resetFiltersButton).toBeVisible()
    await resetFiltersButton.click()

    await expect(countLabel).toHaveText("4 preset(s) found")
  })

  test("Preset list honors aircraft settings", async ({
    configListPage,
    page,
  }) => {
    const testSettings = [
      { Aircraft: [], ExpectedPresetCount: 4, ExpectedVendorCount: 2 },
      {
        Aircraft: [{ Vendor: "Microsoft", Name: "Generic" }],
        ExpectedPresetCount: 2,
        ExpectedVendorCount: 1,
      },
    ]

    for (const settings of testSettings) {
      const actionDialog = await openWizardAndReturnActionPanel(
        configListPage,
        page,
        1,
        undefined,
        {
          Aircraft: settings.Aircraft,
        } as Partial<Project>,
      )

      const actionEditButton = actionDialog.getByRole("button", {
        name: "Edit On Press Action",
      })
      await expect(actionEditButton).toBeVisible()
      await actionEditButton.click()

      const actionEditor = page.getByTestId("action-editor")

      const countLabel = actionEditor.getByRole("status")
      const resetFiltersButton = actionEditor.getByRole("button", {
        name: "Reset filters",
      })

      await resetFiltersButton.click()
      await expect(countLabel).toHaveText(
        `${settings.ExpectedPresetCount} preset(s) found`,
      )

      const optionsList = page.getByRole("listbox")
      const vendorOptions = optionsList.getByRole("option")

      // Select the vendor filter
      await actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Filter by vendor" })
        .click()
      await expect(optionsList).toBeVisible()
      await expect(vendorOptions).toHaveCount(settings.ExpectedVendorCount)
    }
  })

  test("Newly created MSFS config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const actionInputOption = page.getByRole("option", {
      name: "Microsoft Flight Simulator (all versions)",
    })
    await expect(actionInputOption).toBeVisible()
    await actionInputOption.click()

    const codeInput = actionEditor.getByPlaceholder("Enter RPN code")

    await expect(codeInput).toBeVisible()
    await codeInput.fill("Test Code Input")

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "MSFS2020CustomInputAction",
      Command: "Test Code Input",
      PresetId: "",
    } as MsfsInputAction)
  })
})

test.describe("Input Config Wizard - X-Plane Input Action Panel", () => {
  test("Loaded config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionPanel = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      2,
      undefined,
      { Sim: "xplane" },
    )

    const actionEditButton = actionPanel.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

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
      actionEditor.getByPlaceholder(
        "Enter path for DataRef or Command, or select a preset above",
      ),
    ).toHaveValue("laminar/B738/knob/land_alt_press_dn")
  })

  test("Summary information is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionPanel = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      2,
      undefined,
      { Sim: "xplane" },
    )

    // Action type is shown in the summary
    await expect(actionPanel.getByText("X-Plane preset")).toBeVisible()

    // The preset label is shown in the summary
    await expect(
      actionPanel.getByText("land_alt_press_dn", { exact: true }),
    ).toBeVisible()

    // The code is shown in the summary
    await expect(
      actionPanel.getByText("laminar/B738/knob/land_alt_press_dn", {
        exact: true,
      }),
    ).toBeVisible()
  })

  test("Preset filter narrows the list and the count updates", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      2,
      undefined,
      { Sim: "xplane" },
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

    const filterInput = actionEditor.getByPlaceholder("Filter presets")
    const countLabel = actionEditor.getByRole("status")

    // we will have only 1 preset available because of the test data filtering
    await expect(countLabel).toHaveText("1 preset(s) found")

    // reset filters to have all options available
    const resetFiltersButton = actionEditor.getByRole("button", {
      name: "Reset filters",
    })
    await resetFiltersButton.click()
    await expect(countLabel).toHaveText("4 preset(s) found")

    // Filter by exact preset label -> 1 preset should be found
    await filterInput.fill("land_alt_press_dn")
    await expect(countLabel).toHaveText("1 preset(s) found")

    // Filter by a non-existing preset label -> 0 presets should be found
    await filterInput.fill("NoMatch")
    await expect(countLabel).toHaveText("0 preset(s) found")

    // Clear the filter -> all presets should be found
    await filterInput.fill("")
    await expect(countLabel).toHaveText("4 preset(s) found")
  })

  test("Selecting a preset updates the code field and input type", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      2,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")
    const resetFiltersButton = actionEditor.getByRole("button", {
      name: "Reset filters",
    })
    await resetFiltersButton.click()

    // Select a DataRef preset (different code type)
    await actionEditor
      .getByRole("combobox")
      .filter({ hasText: "land_alt_press_dn" })
      .click()
    await page.getByRole("option", { name: "test_dataref" }).click()
    // Code field updates
    await expect(
      actionEditor.getByPlaceholder(
        "Enter path for DataRef or Command, or select a preset above",
      ),
    ).toHaveValue("laminar/B739/test/dataref")
    // Input type updates from Command to DataRef
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: /^DataRef$/ }),
    ).toBeVisible()
    // Description updates
    await expect(actionEditor.getByText("Test DataRef Preset")).toBeVisible()
  })

  test("Preset filter combo boxes work correctly", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      2,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

    const countLabel = actionEditor.getByRole("status")
    const resetFiltersButton = actionEditor.getByRole("button", {
      name: "Reset filters",
    })

    // initially only 1 preset is available
    // because of current test data filtering
    await expect(countLabel).toHaveText("1 preset(s) found")

    // now reset all filters to show all options
    await resetFiltersButton.click()
    await expect(countLabel).toHaveText("4 preset(s) found")

    const optionsList = page.getByRole("listbox")

    // Select a vendor filter
    await actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Filter by vendor" })
      .click()
    await expect(optionsList).toBeVisible()
    const vendorOption = optionsList.getByRole("option", {
      name: "Laminar Research",
    })
    await expect(vendorOption).toBeVisible()
    await vendorOption.click()
    await expect(vendorOption).not.toBeVisible()

    await expect(countLabel).toHaveText("3 preset(s) found")

    // Select an aircraft filter
    await actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Filter by aircraft" })
      .click()
    await expect(optionsList).toBeVisible()
    const aircraftOption = optionsList.getByRole("option", {
      name: "Boeing 737-800",
    })
    await expect(aircraftOption).toBeVisible()
    await aircraftOption.click()
    await expect(aircraftOption).not.toBeVisible() // Should be removed from options since it's already selected as a filter

    await expect(countLabel).toHaveText("2 preset(s) found")

    // Select a system filter
    await actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Filter by system" })
      .click()
    await expect(optionsList).toBeVisible()
    const systemOption = optionsList.getByRole("option", { name: "Autopilot" })
    await expect(systemOption).toBeVisible()
    await systemOption.click()
    await expect(systemOption).not.toBeVisible()

    await expect(countLabel).toHaveText("1 preset(s) found")

    await expect(resetFiltersButton).toBeVisible()
    await resetFiltersButton.click()

    await expect(countLabel).toHaveText("4 preset(s) found")
  })

  test("Preset list honors aircraft settings", async ({
    configListPage,
    page,
  }) => {
    const testSettings = [
      { Aircraft: [], ExpectedPresetCount: 4, ExpectedVendorCount: 2 },
      {
        Aircraft: [{ Vendor: "Laminar Research", Name: "Boeing 737-800" }],
        ExpectedPresetCount: 2,
        ExpectedVendorCount: 1,
      },
    ]

    for (const settings of testSettings) {
      const actionDialog = await openWizardAndReturnActionPanel(
        configListPage,
        page,
        2,
        undefined,
        {
          Aircraft: settings.Aircraft,
          Sim: "xplane",
        } as Partial<Project>,
      )

      const actionEditButton = actionDialog.getByRole("button", {
        name: "Edit On Press Action",
      })
      await expect(actionEditButton).toBeVisible()
      await actionEditButton.click()

      const actionEditor = page.getByTestId("action-editor")

      const countLabel = actionEditor.getByRole("status")
      const resetFiltersButton = actionEditor.getByRole("button", {
        name: "Reset filters",
      })

      // make sure we have all options available by resetting filters
      await resetFiltersButton.click()

      await expect(countLabel).toHaveText(
        `${settings.ExpectedPresetCount} preset(s) found`,
      )

      const optionsList = page.getByRole("listbox")
      const vendorOptions = optionsList.getByRole("option")

      // Select the vendor filter
      await actionEditor
        .getByRole("combobox")
        .filter({ hasText: "Filter by vendor" })
        .click()
      await expect(optionsList).toBeVisible()
      await expect(vendorOptions).toHaveCount(settings.ExpectedVendorCount)
    }
  })

  test("Switching between DataRef and Command updates the value field visibility correctly", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      2,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

    const inputTypeComboBox = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Command" })

    await expect(inputTypeComboBox).toBeVisible()
    const valueInput = actionEditor.getByPlaceholder("Enter value")

    await expect(valueInput).not.toBeVisible()
    // Switch to DataRef preset
    await inputTypeComboBox.click()
    await page.getByRole("option", { name: "DataRef" }).click()

    await expect(valueInput).toBeVisible()
  })

  test("Newly created XPlane Input Action (command) config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
      "Button",
      "On Press",
      { Sim: "xplane" },
    )

    // Open the action type combo box to get access to the options
    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    // Select X-Plane action type
    const actionInputOption = page.getByRole("option", {
      name: "X-Plane (all versions)",
    })
    await expect(actionInputOption).toBeVisible()
    await actionInputOption.click()

    // Open the input type combo box to get access to the options
    const inputTypeComboBox = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Select input type" })
    await expect(inputTypeComboBox).toBeVisible()
    await inputTypeComboBox.click()

    // Select DataRef input type
    await page.getByRole("option", { name: "Command" }).click()

    // Fill out form fields
    const codeInput = actionEditor.getByPlaceholder("Enter path")
    await expect(codeInput).toBeVisible()
    await codeInput.fill("Test Code Input")

    // Close the drawer
    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    // Set up command tracking
    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    // Save the config
    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "XplaneInputAction",
      InputType: "Command",
      Path: "Test Code Input",
    } as XplaneInputAction)
  })

  test("Newly created XPlane Input Action (DataRef) config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
      "Button",
      "On Press",
      { Sim: "xplane" },
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const actionInputOption = page.getByRole("option", {
      name: "X-Plane (all versions)",
    })
    await expect(actionInputOption).toBeVisible()
    await actionInputOption.click()

    const inputTypeComboBox = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Select input type" })

    await expect(inputTypeComboBox).toBeVisible()
    await inputTypeComboBox.click()
    await page.getByRole("option", { name: "DataRef" }).click()

    const codeInput = actionEditor.getByPlaceholder("Enter path")

    await expect(codeInput).toBeVisible()
    await codeInput.fill("Test Code Input")

    const valueInput = actionEditor.getByPlaceholder("Enter value")
    await expect(valueInput).toBeVisible()
    await valueInput.fill("Test Value")

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "XplaneInputAction",
      InputType: "DataRef",
      Path: "Test Code Input",
      Expression: "Test Value",
    } as XplaneInputAction)
  })
})

test.describe("Input Config Wizard - Variable Input Action Panel", () => {
  test("Loaded config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      3,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

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

  test("Summary information is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionPanel = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      3,
    )

    // Action type is shown in the summary
    await expect(actionPanel.getByText("Variable")).toBeVisible()

    // The variable name is shown in the summary
    await expect(
      actionPanel.getByText("MyVarnumber", { exact: true }),
    ).toBeVisible()

    // The code is shown in the summary
    await expect(
      actionPanel.getByText("$", {
        exact: true,
      }),
    ).toBeVisible()
  })

  test("Newly created (number) Variable config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const type = "Button"
    const eventType = "On Press"

    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
      type,
      eventType,
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const variableInputOption = page.getByRole("option", {
      name: "MobiFlight - Variable",
    })
    await expect(variableInputOption).toBeVisible()
    await variableInputOption.click()

    const variableNameInput = actionEditor.getByPlaceholder(
      "Enter variable name...",
    )
    const variableValueInput = actionEditor.getByPlaceholder(
      "Enter expression...",
    )

    await expect(variableNameInput).toBeVisible()
    await expect(variableValueInput).toBeVisible()

    await variableNameInput.fill("TestVariable")
    await variableValueInput.fill("123")

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "VariableInputAction",
      Variable: {
        TYPE: "number",
        Name: "TestVariable",
        Text: "",
        Expression: "123",
      },
    })
  })

  test("Newly created (string) Variable config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const type = "Button"
    const eventType = "On Press"

    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
      type,
      eventType,
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const variableInputOption = page.getByRole("option", {
      name: "MobiFlight - Variable",
    })
    await expect(variableInputOption).toBeVisible()
    await variableInputOption.click()

    const variableNameInput = actionEditor.getByPlaceholder(
      "Enter variable name...",
    )
    const variableValueInput = actionEditor.getByPlaceholder(
      "Enter expression...",
    )

    const variableTypeComboBox = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Number" })

    await expect(variableNameInput).toBeVisible()
    await expect(variableValueInput).toBeVisible()
    await expect(variableTypeComboBox).toBeVisible()

    await variableTypeComboBox.click()
    await page.getByRole("option", { name: "String" }).click()

    await variableNameInput.fill("TestVariable")
    await variableValueInput.fill("123")

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "VariableInputAction",
      Variable: {
        TYPE: "string",
        Name: "TestVariable",
        Text: "",
        Expression: "123",
      },
    })
  })
})

test.describe("Input Config Wizard - Retrigger Input Action Panel", () => {
  test("Panel description is shown", async ({ configListPage, page }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      4,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "MobiFlight - Retrigger switches" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByText("re-trigger all button states"),
    ).toBeVisible()
  })

  test("Summary information is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionPanel = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      4,
    )

    // Action type is shown in the summary
    await expect(actionPanel.getByText("Retrigger")).toBeVisible()

    // The note is shown in the summary
    await expect(
      actionPanel.getByText("Note:Sync input devices with sim.", {
        exact: true,
      }),
    ).toBeVisible()
  })

  test("Newly created retrigger config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const actionTypeOption = page.getByRole("option", {
      name: "MobiFlight - Retrigger switches",
    })
    await expect(actionTypeOption).toBeVisible()
    await actionTypeOption.click()

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "RetriggerInputAction",
    })
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

    const actionPanel = page.getByTestId("action-panel")
    // expect it to become visible
    // this ensures that react render has completed and, e.g., useEffects have run
    await expect(actionPanel).toBeVisible()
    const addOnLeftButton = actionPanel.getByRole("button", {
      name: "On Press",
    })
    await expect(addOnLeftButton).toBeVisible()
    await addOnLeftButton.click()

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
    await expect(actionEditor.getByText("Key comboNone")).toBeVisible()
  })

  test("Summary information is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionPanel = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      5,
    )

    // Action type is shown in the summary
    await expect(actionPanel.getByText("Keyboard")).toBeVisible()

    // The variable name is shown in the summary
    await expect(
      actionPanel.getByText("Ctrl + Alt + Shift + D", { exact: true }),
    ).toBeVisible()
  })

  test("Loaded config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      5,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

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
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      5,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

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
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      5,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

    await expect(actionEditor).toContainText("Ctrl + Alt + Shift + D")

    await actionEditor.getByRole("button", { name: "Clear input" }).click()
    await expect(actionEditor).toContainText("None")
    await expect(actionEditor).not.toContainText("Ctrl +")
  })

  test("Newly created Keyboard Input Action config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const actionTypeOption = page.getByRole("option", {
      name: "MobiFlight - Keyboard Input",
    })
    await expect(actionTypeOption).toBeVisible()
    await actionTypeOption.click()

    // Provide specific user input
    const scanForKeyboardButton = actionEditor.getByRole("button", {
      name: "Scan for keyboard",
    })
    await expect(scanForKeyboardButton).toBeVisible()
    await scanForKeyboardButton.click()

    await page.keyboard.down("Control")
    await page.keyboard.down("Alt")
    await page.keyboard.down("Shift")
    await page.keyboard.down("F8")

    // the up event will end scanning
    await page.keyboard.up("F8")
    // End: provide specific user input

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "KeyInputAction",
      Code: "F8",
      Control: true,
      Alt: true,
      Shift: true,
    } as KeyInputAction)
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
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      6,
      async (configListPage) => {
        await configListPage.mobiFlightPage.trackCommand(
          "CommandRefreshPresets",
        )
      },
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

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
      actionEditor.getByRole("combobox").filter({ hasText: "vJoy Joystick 1" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("tab", { name: "button" }),
    ).toHaveAttribute("data-state", "active")
    // buttonNr=4 from test data
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Button 4" }),
    ).toBeVisible()
    // buttonComand=true → "Pressed"
    await expect(
      actionEditor.getByTestId("vjoy-button-command-state"),
    ).toHaveText("Pressed")
  })

  test("Summary information is displayed correctly (Button)", async ({
    configListPage,
    page,
  }) => {
    const actionPanel = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      6,
    )

    // Publish the vJoy definitions so the panel can render the correct labels
    await configListPage.mobiFlightPage.publishMessage(vJoyDefinitions)

    // Action type is shown in the summary
    await expect(actionPanel.getByText("vJoy", { exact: true })).toBeVisible()

    // The controller name is shown in the summary
    await expect(
      actionPanel.getByText("vJoy Joystick 1", { exact: true }),
    ).toBeVisible()

    // The device name is shown in the summary
    await expect(
      actionPanel.getByText("Button 4", { exact: true }),
    ).toBeVisible()

    // The button state is shown in the summary
    await expect(
      actionPanel.getByText("Pressed", {
        exact: true,
      }),
    ).toBeVisible()
  })

  test("Summary information is displayed correctly (Axis)", async ({
    configListPage,
    page,
  }) => {
    const actionPanel = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      7,
    )

    // Publish the vJoy definitions so the panel can render the correct labels
    await configListPage.mobiFlightPage.publishMessage(vJoyDefinitions)

    // Action type is shown in the summary
    await expect(actionPanel.getByText("vJoy", { exact: true })).toBeVisible()

    // The controller name is shown in the summary
    await expect(
      actionPanel.getByText("vJoy Joystick 1", { exact: true }),
    ).toBeVisible()

    // The device name is shown in the summary
    await expect(actionPanel.getByText("Axis Z", { exact: true })).toBeVisible()

    // The axis value is shown in the summary
    await expect(actionPanel.getByText("1024", { exact: true })).toBeVisible()
  })

  test("Axis config: device, axis and send value are displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      7,
      async (configListPage) => {
        await configListPage.mobiFlightPage.trackCommand(
          "CommandRefreshPresets",
        )
      },
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

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
      actionEditor.getByRole("combobox").filter({ hasText: "vJoy Joystick 1" }),
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

  test("Newly created vJoy Input Action (Button) config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const actionTypeOption = page.getByRole("option", {
      name: "MobiFlight - Virtual Joystick input (vJoy)",
    })
    await expect(actionTypeOption).toBeVisible()
    await actionTypeOption.click()
    await expect(actionTypeOption).not.toBeVisible()

    // Publish the vJoy definitions so the panel can render the correct labels
    await configListPage.mobiFlightPage.publishMessage(vJoyDefinitions)

    // Provide specific user input
    const vJoyDeviceComboBox = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Select vJoy device" })
    await expect(vJoyDeviceComboBox).toBeVisible()
    await vJoyDeviceComboBox.click()
    await page.getByRole("option", { name: "vJoy Joystick 1" }).click()
    await expect(
      page.getByRole("option", { name: "vJoy Joystick 1" }),
    ).not.toBeVisible()

    const typeTab = actionEditor.getByRole("tab", { name: "button" })
    await expect(typeTab).toBeVisible()
    await typeTab.click()

    const buttonComboBox = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Select button..." })
    await expect(buttonComboBox).toBeVisible()
    await buttonComboBox.click()
    await page.getByRole("option", { name: "Button 4" }).click()
    await expect(
      page.getByRole("option", { name: "Button 4" }),
    ).not.toBeVisible()

    const stateSwitch = actionEditor.getByRole("switch")
    await expect(stateSwitch).toBeVisible()
    await stateSwitch.click() // from true to false (not pressed)
    // End: provide specific user input

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "VJoyInputAction",
      vJoyID: 1,
      buttonNr: 4,
      buttonComand: true,
    } as VJoyInputAction)
  })

  test("Newly created vJoy Input Action (Axis) config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const actionTypeOption = page.getByRole("option", {
      name: "MobiFlight - Virtual Joystick input (vJoy)",
    })
    await expect(actionTypeOption).toBeVisible()
    await actionTypeOption.click()
    await expect(actionTypeOption).not.toBeVisible()

    // Publish the vJoy definitions so the panel can render the correct labels
    await configListPage.mobiFlightPage.publishMessage(vJoyDefinitions)

    // Provide specific user input
    const vJoyDeviceComboBox = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Select vJoy device" })
    await expect(vJoyDeviceComboBox).toBeVisible()
    await vJoyDeviceComboBox.click()
    await page.getByRole("option", { name: "vJoy Joystick 1" }).click()
    await expect(
      page.getByRole("option", { name: "vJoy Joystick 1" }),
    ).not.toBeVisible()

    const typeTab = actionEditor.getByRole("tab", { name: "axis" })
    await expect(typeTab).toBeVisible()
    await typeTab.click()

    const buttonComboBox = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Select axis..." })
    await expect(buttonComboBox).toBeVisible()
    await buttonComboBox.click()
    await page.getByRole("option", { name: "Y" }).click()
    await expect(page.getByRole("option", { name: "Y" })).not.toBeVisible()

    const valueInput = actionEditor.getByLabel("Axis value")
    await expect(valueInput).toBeVisible()
    await valueInput.fill("16384")
    // End: provide specific user input

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "VJoyInputAction",
      vJoyID: 1,
      axisString: "Y",
      sendValue: "16384",
      buttonNr: -1,
    } as VJoyInputAction)
  })
})

test.describe("Input Config Wizard - FSUIPC Offset Input Action Panel", () => {
  test("Loaded config data is displayed correctly with hex formatting", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      8,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "FSUIPC - Offset" }),
    ).toBeVisible()
    // Type=Integer
    await expect(
      actionEditor.getByRole("combobox").filter({ hasText: "Integer" }),
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
    const bcdModeSwitch = actionEditor.getByRole("switch").filter()
    await expect(bcdModeSwitch).toHaveAttribute("aria-checked", "true")
  })

  test("Summary information is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionPanel = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      8,
    )

    // Action type is shown in the summary
    await expect(actionPanel.getByText("FSUIPC Offset")).toBeVisible()

    // The Size is shown in the summary
    await expect(actionPanel.getByText("4", { exact: true })).toBeVisible()

    // The Offset is shown in the summary
    await expect(actionPanel.getByText("66CC", { exact: true })).toBeVisible()

    // The mask is shown in the summary
    await expect(
      actionPanel.getByText("AABBCCDDEE", { exact: true }),
    ).toBeVisible()

    // The BCD mode is shown in the summary
    await expect(actionPanel.getByText("True", { exact: true })).toBeVisible()

    // The value is shown in the summary
    await expect(actionPanel.getByText("$+123", { exact: true })).toBeVisible()
  })

  test("BCDMode and Mask Visibility are displayed correctly based on type", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const actionTypeOption = page.getByRole("option", {
      name: "FSUIPC - Offset",
    })
    await expect(actionTypeOption).toBeVisible()
    await actionTypeOption.click()
    await expect(actionTypeOption).not.toBeVisible()

    const bcdModeSwitch = actionEditor.getByRole("switch")
    await expect(bcdModeSwitch).toBeVisible()

    const maskInput = actionEditor.getByRole("textbox", { name: "Mask" })
    await expect(maskInput).toBeVisible()

    // Provide specific user input
    let currentType = "Integer"
    const typeComboBoxInteger = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Integer" })
    const typeComboBoxFloat = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Float" })
    const typeComboBoxString = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "String" })

    await expect(typeComboBoxInteger).toBeVisible()
    await typeComboBoxInteger.click()

    // Select float
    currentType = "Float"
    await page.getByRole("option", { name: currentType }).click()
    await expect(
      page.getByRole("option", { name: currentType }),
    ).not.toBeVisible()

    await expect(maskInput).not.toBeVisible()
    await expect(bcdModeSwitch).not.toBeVisible()

    // change type via combo box
    currentType = "String"
    await expect(typeComboBoxFloat).toBeVisible()
    await typeComboBoxFloat.click()
    await page.getByRole("option", { name: currentType }).click()
    await expect(
      page.getByRole("option", { name: currentType }),
    ).not.toBeVisible()

    await expect(maskInput).not.toBeVisible()
    await expect(bcdModeSwitch).not.toBeVisible()

    // change type via combo box
    currentType = "Integer"
    await expect(typeComboBoxString).toBeVisible()
    await typeComboBoxString.click()
    await page.getByRole("option", { name: currentType }).click()
    await expect(
      page.getByRole("option", { name: currentType }),
    ).not.toBeVisible()

    // mask and bcdmode switch are visible again
    await expect(maskInput).toBeVisible()
    await expect(bcdModeSwitch).toBeVisible()

    // End: provide specific user input
  })

  test("Newly created FSUIPC Input Action (Integer) config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const actionTypeOption = page.getByRole("option", {
      name: "FSUIPC - Offset",
    })
    await expect(actionTypeOption).toBeVisible()
    await actionTypeOption.click()
    await expect(actionTypeOption).not.toBeVisible()

    // Provide specific user input
    const fsuipcSizeComboBox = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "1 Byte" })
    await expect(fsuipcSizeComboBox).toBeVisible()
    await fsuipcSizeComboBox.click()

    await page.getByRole("option", { name: "4 Bytes" }).click()
    await expect(
      page.getByRole("option", { name: "4 Bytes" }),
    ).not.toBeVisible()

    const offsetInput = actionEditor.getByRole("textbox", { name: "Offset" })
    await expect(offsetInput).toBeVisible()
    await offsetInput.fill("66CC")

    const maskInput = actionEditor.getByRole("textbox", { name: "Mask" })
    await expect(maskInput).toBeVisible()
    await maskInput.fill("AABBCCDD")

    const bcdModeSwitch = actionEditor.getByRole("switch")
    await expect(bcdModeSwitch).toBeVisible()
    // from false to true
    await bcdModeSwitch.click()

    const valueInput = actionEditor.getByRole("textbox", { name: "Value" })
    await expect(valueInput).toBeVisible()
    await valueInput.fill("1234")

    // End: provide specific user input

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "FsuipcOffsetInputAction",
      FSUIPC: {
        OffsetType: 0,
        Size: 4,
        Offset: 0x66cc,
        Mask: 0xaabbccdd,
        BcdMode: true,
      },
      Value: "1234",
      Modifiers: [],
    } as FsuipcOffsetInputAction)
  })

  test("Newly created FSUIPC Input Action (Float) config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const actionTypeOption = page.getByRole("option", {
      name: "FSUIPC - Offset",
    })
    await expect(actionTypeOption).toBeVisible()
    await actionTypeOption.click()
    await expect(actionTypeOption).not.toBeVisible()

    // Provide specific user input
    const typeComboBoxFloat = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Integer" })
    await expect(typeComboBoxFloat).toBeVisible()
    await typeComboBoxFloat.click()
    await page.getByRole("option", { name: "Float" }).click()
    await expect(page.getByRole("option", { name: "Float" })).not.toBeVisible()

    const fsuipcSizeComboBox = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "1 Byte" })
    await expect(fsuipcSizeComboBox).toBeVisible()
    await fsuipcSizeComboBox.click()

    await page.getByRole("option", { name: "4 Bytes" }).click()
    await expect(
      page.getByRole("option", { name: "4 Bytes" }),
    ).not.toBeVisible()

    const offsetInput = actionEditor.getByRole("textbox", { name: "Offset" })
    await expect(offsetInput).toBeVisible()
    await offsetInput.fill("66CC")

    const valueInput = actionEditor.getByRole("textbox", { name: "Value" })
    await expect(valueInput).toBeVisible()
    await valueInput.fill("1.234")
    // End: provide specific user input

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "FsuipcOffsetInputAction",
      FSUIPC: {
        OffsetType: 1,
        Size: 4,
        Offset: 0x66cc,
        Mask: 0xff,
        BcdMode: false,
      },
      Value: "1.234",
      Modifiers: [],
    } as FsuipcOffsetInputAction)
  })

  test("Newly created FSUIPC Input Action (String) config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const actionTypeOption = page.getByRole("option", {
      name: "FSUIPC - Offset",
    })
    await expect(actionTypeOption).toBeVisible()
    await actionTypeOption.click()
    await expect(actionTypeOption).not.toBeVisible()

    // Provide specific user input
    const typeComboBoxString = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Integer" })
    await expect(typeComboBoxString).toBeVisible()
    await typeComboBoxString.click()
    await page.getByRole("option", { name: "String" }).click()
    await expect(page.getByRole("option", { name: "String" })).not.toBeVisible()

    const offsetInput = actionEditor.getByRole("textbox", { name: "Offset" })
    await expect(offsetInput).toBeVisible()
    await offsetInput.fill("66CC")

    const valueInput = actionEditor.getByRole("textbox", { name: "Value" })
    await expect(valueInput).toBeVisible()
    await valueInput.fill("Test String")
    // End: provide specific user input

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "FsuipcOffsetInputAction",
      FSUIPC: {
        OffsetType: 2,
        Size: 255,
        Offset: 0x66cc,
        Mask: 0xff,
        BcdMode: false,
      },
      Value: "Test String",
      Modifiers: [],
    } as FsuipcOffsetInputAction)
  })
})

test.describe("Input Config Wizard - FSUIPC EventID Input Action Panel", () => {
  test("Loaded config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      9,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

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

  test("Summary information is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionPanel = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      9,
    )

    // Action type is shown in the summary
    await expect(actionPanel.getByText("EventID")).toBeVisible()

    // The event ID is shown in the summary
    await expect(actionPanel.getByText("68036", { exact: true })).toBeVisible()

    // The custom param is shown in the summary
    await expect(actionPanel.getByText("0", { exact: true })).toBeVisible()
  })

  test("Newly created FSUIPC EventID Input Action config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    await configListPage.mobiFlightPage.page.route(
      "*/**/presets/presets_eventids.cip",
      async (route) => {
        await route.fulfill({
          body: "COM1_TRANSMIT_SELECT:66463",
          contentType: "text/plain",
        })
      },
    )

    const actionTypeOption = page.getByRole("option", {
      name: "FSUIPC - EventID",
    })
    await expect(actionTypeOption).toBeVisible()
    await actionTypeOption.click()
    await expect(actionTypeOption).not.toBeVisible()

    // Provide specific user input
    const presetComboBox = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Select preset..." })
    await expect(presetComboBox).toBeVisible()
    await presetComboBox.click()
    await page.getByRole("option", { name: "COM1_TRANSMIT_SELECT" }).click()
    await expect(
      page.getByRole("option", { name: "COM1_TRANSMIT_SELECT" }),
    ).not.toBeVisible()

    const eventIdInput = actionEditor.getByLabel("Event ID")
    await expect(eventIdInput).toBeVisible()
    // after preset selection, the EventIdInput shall be preset value
    await expect(eventIdInput).toHaveValue("66463")
    await eventIdInput.fill("12345")

    const customParamInput = actionEditor.getByLabel("Custom Param")
    await expect(customParamInput).toBeVisible()
    // after preset selection, the param shall be 0
    await expect(customParamInput).toHaveValue("0")
    await customParamInput.fill("54321")
    // End: provide specific user input

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "EventIdInputAction",
      EventId: "12345",
      Param: "54321",
    } as EventIdInputAction)
  })
})

test.describe("Input Config Wizard - FSUIPC PMDG EventID Input Action Panel", () => {
  test("Loaded config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
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

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

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

  test("Summary information is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionPanel = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      10,
    )

    // Action type is shown in the summary
    await expect(actionPanel.getByText("PMDG Event ID")).toBeVisible()

    // The event ID is shown in the summary
    await expect(actionPanel.getByText("69648", { exact: true })).toBeVisible()

    // The mouse parameter is shown in the summary
    await expect(
      actionPanel.getByText("MOUSE_FLAG_LEFTSINGLE", { exact: true }),
    ).toBeVisible()
  })

  test("Newly created FSUIPC PMDG EventID Input Action config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
    )

    await configListPage.mobiFlightPage.page.route(
      "*/**/presets/presets_eventids_pmdg_747.cip",
      async (route) => {
        await route.fulfill({
          body: "EVT_OH_ELEC_APU_GEN1_SWITCH:69648",
          contentType: "text/plain",
        })
      },
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const actionTypeOption = page.getByRole("option", {
      name: "FSUIPC - PMDG - Event ID",
    })
    await expect(actionTypeOption).toBeVisible()
    await actionTypeOption.click()
    await expect(actionTypeOption).not.toBeVisible()

    // Provide specific user input\
    const aircraftTypeRadioButton = actionEditor.getByRole("radio").nth(1)
    await expect(aircraftTypeRadioButton).toBeVisible()
    await aircraftTypeRadioButton.click()

    const presetComboBox = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Select preset..." })
    await expect(presetComboBox).toBeVisible()
    await presetComboBox.click()
    await page
      .getByRole("option", { name: "EVT_OH_ELEC_APU_GEN1_SWITCH" })
      .click()
    await expect(
      page.getByRole("option", { name: "EVT_OH_ELEC_APU_GEN1_SWITCH" }),
    ).not.toBeVisible()

    const eventIdInput = actionEditor.getByLabel("Event ID")
    await expect(eventIdInput).toBeVisible()
    await eventIdInput.fill("12345")

    const mouseParamComboBox = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Select mouse param" })
    await expect(mouseParamComboBox).toBeVisible()
    await mouseParamComboBox.click()
    await page.getByRole("option", { name: "MOUSE_FLAG_LEFTSINGLE" }).click()
    await expect(
      page.getByRole("option", { name: "MOUSE_FLAG_LEFTSINGLE" }),
    ).not.toBeVisible()

    const customParamInput = actionEditor.getByLabel("Custom Param")
    await expect(customParamInput).not.toBeVisible()
    // End: provide specific user input

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "PmdgEventIdInputAction",
      AircraftType: "B747",
      EventId: "12345",
      Param: "536870912",
    } as PmdgEventIdInputAction)
  })
})

test.describe("Input Config Wizard - FSUIPC Jeehell Input Action Panel", () => {
  test("Loaded config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      11,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

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

  test("Summary information is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionPanel = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      11,
    )

    // Action type is shown in the summary
    await expect(actionPanel.getByText("Jeehell Events")).toBeVisible()

    // The function name is shown in the summary
    await expect(
      actionPanel.getByText("FCU_HDGKNOB_PRESS", { exact: true }),
    ).toBeVisible()

    // The value is shown in the summary
    await expect(actionPanel.getByText("1", { exact: true })).toBeVisible()
  })

  test("Selecting a preset updates the function and description", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      11,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

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

  test("Newly created FSUIPC Jeehell Input Action config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
    )

    await page.route("*/**/presets/presets_jeehell.cip", async (route) => {
      await route.fulfill({
        body: jeehellPresetsContent,
        contentType: "text/plain",
      })
    })

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const actionTypeOption = page.getByRole("option", {
      name: "FSUIPC - Jeehell - Events",
    })
    await expect(actionTypeOption).toBeVisible()
    await actionTypeOption.click()
    await expect(actionTypeOption).not.toBeVisible()

    // Provide specific user input
    const presetComboBox = actionEditor
      .getByRole("combobox")
      .filter({ hasText: "Select Jeehell function..." })
    await expect(presetComboBox).toBeVisible()
    await presetComboBox.click()
    await page.getByRole("option", { name: "AP_ENGAGE" }).click()
    await expect(
      page.getByRole("option", { name: "AP_ENGAGE" }),
    ).not.toBeVisible()

    const valueInput = actionEditor.getByLabel("Value")
    await expect(valueInput).toBeVisible()
    // after preset selection, the EventIdInput shall be preset value
    await expect(valueInput).toHaveValue("")
    await valueInput.fill("12345")
    // End: provide specific user input

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "JeehellInputAction",
      EventId: "8",
      Param: "12345",
    } as JeehellInputAction)
  })
})

test.describe("Input Config Wizard - FSUIPC Lua Macro Input Action Panel", () => {
  test("Loaded config data is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      12,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

    await expect(
      actionEditor
        .getByRole("combobox")
        .filter({ hasText: "FSUIPC - Lua Macro" }),
    ).toBeVisible()
    await expect(
      actionEditor.getByRole("textbox", { name: "Macro Name" }),
    ).toHaveValue("TestMacro")
    await expect(
      actionEditor.getByRole("textbox", { name: "Macro Value" }),
    ).toHaveValue("TestValue")
  })

  test("Summary information is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionPanel = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      12,
    )

    // Action type is shown in the summary
    await expect(actionPanel.getByText("Lua Macro")).toBeVisible()

    // The macro name is shown in the summary
    await expect(
      actionPanel.getByText("TestMacro", { exact: true }),
    ).toBeVisible()

    // The macro value is shown in the summary
    await expect(
      actionPanel.getByText("TestValue", { exact: true }),
    ).toBeVisible()
  })

  test("Editing macro name and value updates the fields", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      12,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

    const macroNameInput = actionEditor.getByRole("textbox", {
      name: "Macro Name",
    })
    const macroValueInput = actionEditor.getByRole("textbox", {
      name: "Macro Value",
    })

    await macroNameInput.fill("UpdatedMacro")
    await macroValueInput.fill("42")

    await expect(macroNameInput).toHaveValue("UpdatedMacro")
    await expect(macroValueInput).toHaveValue("42")
  })

  test("Newly created FSUIPC Lua Macro Input Action config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()
    await actionTypeComboBox.click()

    const actionTypeOption = page.getByRole("option", {
      name: "FSUIPC - Lua Macro",
    })
    await expect(actionTypeOption).toBeVisible()
    await actionTypeOption.click()
    await expect(actionTypeOption).not.toBeVisible()

    // Provide specific user input
    const macroNameInput = actionEditor.getByLabel("Macro Name")
    await expect(macroNameInput).toBeVisible()
    await macroNameInput.fill("MACRO NAME")

    const macroValueInput = actionEditor.getByLabel("Macro Value")
    await expect(macroValueInput).toBeVisible()
    await macroValueInput.fill("MACRO VALUE")
    // End: provide specific user input

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "LuaMacroInputAction",
      MacroName: "MACRO NAME",
      MacroValue: "MACRO VALUE",
    } as LuaMacroInputAction)
  })
})

test.describe("Input Config Wizard - ProSim Input Action Panel", () => {
  test("Without presets shows Refresh Presets button and sends refresh command", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      13,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

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

  test("Summary information is displayed correctly", async ({
    configListPage,
    page,
  }) => {
    const actionPanel = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      13,
    )

    // Action type is shown in the summary
    await expect(actionPanel.getByText("ProSim Preset")).toBeVisible()

    // The preset path is shown in the summary
    await expect(
      actionPanel.getByText("prosim.test.path", { exact: true }),
    ).toBeVisible()

    // The optional parameter value is shown in the summary
    await expect(actionPanel.getByText("$", { exact: true })).toBeVisible()
  })

  test("With presets loaded: filter and select updates the path", async ({
    configListPage,
    page,
  }) => {
    const actionDialog = await openWizardAndReturnActionPanel(
      configListPage,
      page,
      13,
    )

    const actionEditButton = actionDialog.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")

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

  test("Newly created ProSim Input Action config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
    )

    const actionTypeComboBox = actionEditor.getByTestId("action-type-combobox")
    await expect(actionTypeComboBox).toBeVisible()

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

    await actionTypeComboBox.click()

    const actionTypeOption = page.getByRole("option", {
      name: "ProSim",
    })
    await expect(actionTypeOption).toBeVisible()
    await actionTypeOption.click()
    await expect(actionTypeOption).not.toBeVisible()

    // Provide specific user input
    const presetRow = actionEditor.getByText("Aircraft Heading")
    await expect(presetRow).toBeVisible()
    await presetRow.click()

    const pathValue = actionEditor.getByTestId("pathValue")
    await expect(pathValue).toBeVisible()
    await expect(pathValue).toHaveText("aircraft.heading")

    const parameterInput = actionEditor.getByLabel("Parameter")
    await expect(parameterInput).toBeVisible()
    await expect(parameterInput).toHaveValue("")
    await parameterInput.fill("Custom")
    // End: provide specific user input

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.onPress).toEqual({
      Type: "ProSimInputAction",
      Path: "aircraft.heading",
      Expression: "Custom",
    } as ProSimInputAction)
  })
})

test.describe("Input Config Wizard - Action Binding Panels", () => {
  test("Action bindings panel: each tab routes to the correct event slot", async ({
    configListPage,
    page,
  }) => {
    test.slow()
    const actionTestData = [
      {
        type: "Button",
        eventTypes: ["On Press", "On Release", "On Hold", "On Long Release"],
      },
      {
        type: "Encoder",
        eventTypes: ["On Left", "On Right", "On Left Fast", "On Right Fast"],
      },
      { type: "AnalogInput", eventTypes: ["On Change"] },
    ]

    for (const { type, eventTypes } of actionTestData) {
      // Opens after clicking "Add Input Config" button and goes through the creation flow
      await configListPage.gotoPage()
      await configListPage.mobiFlightPage.initWithTestData("inputaction")

      // Add new config
      const addInputConfigButton = page.getByRole("button", {
        name: "Add Input Config",
      })
      await addInputConfigButton.click()
      await configListPage.addNewConfigItem("InputConfigItem", 0, "inputaction")
      await expect(page.getByText("Edit Input Configuration")).toBeVisible()

      // Scan for input for device with respective input device type
      const triggerPanel = page.getByTestId("trigger-panel")
      await expect(triggerPanel).toBeVisible()

      const scanForInputButton = triggerPanel.getByRole("button", {
        name: "Scan for Input",
      })
      await expect(scanForInputButton).toBeVisible()
      await scanForInputButton.click()

      // fake the scan result for respective input device type
      await configListPage.mobiFlightPage.publishMessage({
        key: "ScanForInputResult",
        payload: {
          Controller: {
            Devices: [],
            Name: "Bravo Throttle Quadrant",
            Serial: "JS-87654321",
          },
          Device: {
            Name: `${type} 21`,
            Label: "Mode - ALT",
            Type: type,
          },
        } as ScanForInputResult,
      })

      const actionPanel = page.getByTestId("action-panel")
      const actionEditor = page.getByTestId("action-editor")

      // verify that we have correct buttons and that they all open the drawer
      for (const eventType of eventTypes) {
        const button = actionPanel.getByRole("button", {
          name: eventType,
          exact: true,
        })
        await expect(button).toBeVisible()
        await button.click()
        await expect(actionEditor).toBeVisible()

        const backButton = page.getByRole("button", { name: "Go back" })
        await expect(backButton).toBeVisible()
        await backButton.click()
        await expect(actionEditor).not.toBeVisible()
      }
    }
  })

  test("Button hold options are displayed and update correctly", async ({
    configListPage,
    page,
  }) => {
    const type = "Button"
    const eventType = "On Hold"

    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
      type,
      eventType,
    )

    // Verify that the input fields are visible
    const inputHoldDelay = actionEditor.getByRole("textbox", {
      name: "Hold delay (ms)",
    })
    const inputRepeatDelay = actionEditor.getByRole("textbox", {
      name: "Repeat delay (ms)",
    })

    await expect(inputHoldDelay).toBeVisible()
    await expect(inputRepeatDelay).toBeVisible()
    const inputHoldDelayValue = 500
    const inputRepeatDelayValue = 1000

    await inputHoldDelay.fill(inputHoldDelayValue.toString())
    await inputRepeatDelay.fill(inputRepeatDelayValue.toString())

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.HoldDelay).toBe(500)
    expect(payload.item.button?.RepeatDelay).toBe(1000)
  })

  test("Button long release options are displayed and update correctly", async ({
    configListPage,
    page,
  }) => {
    const type = "Button"
    const eventType = "On Long Release"

    const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
      configListPage,
      page,
      type,
      eventType,
    )

    // Verify that the input fields are visible
    const inputLongReleaseDelay = actionEditor.getByRole("textbox", {
      name: "Long release delay (ms)",
    })

    await expect(inputLongReleaseDelay).toBeVisible()
    const inputLongReleaseDelayValue = 500

    await inputLongReleaseDelay.fill(inputLongReleaseDelayValue.toString())

    const backButton = page.getByRole("button", { name: "Go back" })
    await expect(backButton).toBeVisible()
    await backButton.click()
    await expect(actionEditor).not.toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    const saveButton = page.getByRole("button", { name: "Save" })
    await expect(saveButton).toBeVisible()
    await saveButton.click()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands).toBeDefined()
    const payload = commands?.pop()?.payload
    expect(payload.item.button?.LongReleaseDelay).toBe(500)
  })

  test("Existing action can be removed correctly using delete button", async ({
    configListPage,
    page,
  }) => {
    test.slow()
    // For all of our 13 different input action config items
    // open and remove the action, then save and verify that the action is removed from the config item
    const maxConfigItems = 13
    for (
      let currentConfigItemIndex = 1;
      currentConfigItemIndex <= maxConfigItems;
      currentConfigItemIndex++
    ) {
      const actionDialog = await openWizardAndReturnActionPanel(
        configListPage,
        page,
        currentConfigItemIndex,
      )
      const actionEditButton = actionDialog.getByRole("button", {
        name: "Edit On Press Action",
      })
      const deleteActionButton = actionDialog.getByRole("button", {
        name: "Remove On Press Action",
      })

      await expect(actionEditButton).toBeVisible()
      await actionEditButton.hover()
      await expect(deleteActionButton).toBeVisible()
      await deleteActionButton.click()

      await configListPage.mobiFlightPage.trackCommand(
        "CommandUpdateConfigItem",
      )

      const saveButton = page.getByRole("button", { name: "Save" })
      await expect(saveButton).toBeVisible()
      await saveButton.click()

      const commands = await configListPage.mobiFlightPage.getTrackedCommands()
      expect(commands).toBeDefined()
      const payload = commands?.pop()?.payload
      expect(payload.item.button.onPress).toBeNull()
      await configListPage.mobiFlightPage.clearTrackedCommands()
    }
  })

  test("Action can be removed correctly using deselecting the action type", async ({
    configListPage,
    page,
  }) => {
    test.setTimeout(120_000)
    const actionTypeOptions = Object.keys(actionTypeOptionLabels).map(
      (key) => ({
        actionTypeOption: key,
        actionTypeLabel: actionTypeOptionLabels[key],
      }),
    )

    for (const { actionTypeOption, actionTypeLabel } of actionTypeOptions) {
      const projectOptions = {
        Sim: actionTypeOption != "XplaneInputAction" ? "msfs" : "xplane",
      } as Partial<Project>
      const actionEditor = await CreateNewInputConfigItemAndReturnActionEditor(
        configListPage,
        page,
        "Button",
        "On Press",
        projectOptions,
      )

      const actionTypeComboBox = actionEditor.getByTestId(
        "action-type-combobox",
      )
      // Open the action type combobox
      await expect(actionTypeComboBox).toBeVisible()
      await actionTypeComboBox.click()

      const actionInputOption = page.getByRole("option", {
        name: actionTypeLabel,
      })
      // Click on the current action option
      await expect(actionInputOption).toBeVisible()
      await actionInputOption.click()

      const backButton = page.getByRole("button", { name: "Go back" })
      // Close the side panel with back button
      await expect(backButton).toBeVisible()
      await backButton.click()
      await expect(actionEditor).not.toBeVisible()

      // Now the action is stored in the config item
      // If we save now, it will be saved with the action, so we need to remove it first
      const actionPanel = page.getByTestId("action-panel")
      // expect it to become visible
      // this ensures that react render has completed and, e.g., useEffects have run
      await expect(actionPanel).toBeVisible()

      const actionEditButton = actionPanel.getByRole("button", {
        name: "Edit On Press Action",
      })
      await expect(actionEditButton).toBeVisible()
      await actionEditButton.click()

      const actionEditorAfterEdit = page.getByTestId("action-editor")
      await expect(actionEditorAfterEdit).toBeVisible()

      // Open the action type combobox
      await expect(actionTypeComboBox).toBeVisible()
      await actionTypeComboBox.click()

      // Click on the current action option
      await expect(actionInputOption).toBeVisible()
      await actionInputOption.click()

      // Close the side panel with back button
      await expect(backButton).toBeVisible()
      await backButton.click()
      await expect(actionEditor).not.toBeVisible()

      await configListPage.mobiFlightPage.trackCommand(
        "CommandUpdateConfigItem",
      )

      const saveButton = page.getByRole("button", { name: "Save" })
      await expect(saveButton).toBeVisible()
      await saveButton.click()

      const commands = await configListPage.mobiFlightPage.getTrackedCommands()
      expect(commands).toBeDefined()
      const payload = commands?.pop()?.payload
      expect(payload.item.button?.onPress).toBeNull()
    }
  })
})

async function CreateNewInputConfigItemAndWaitForDialog(
  configListPage: ConfigListPage,
  page: Page,
  projectOptions?: Partial<Project>,
) {
  await configListPage.gotoPage()
  if (projectOptions) {
    await configListPage.mobiFlightPage.initWithTestDataAndSpecificProjectProps(
      projectOptions,
      "inputaction",
    )
  } else {
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
  }
  // Add new config
  const addInputConfigButton = page.getByRole("button", {
    name: "Add Input Config",
  })
  await addInputConfigButton.click()
  await configListPage.addNewConfigItem("InputConfigItem", 0, "inputaction")
  await expect(page.getByText("Edit Input Configuration")).toBeVisible()
}

async function CreateNewInputConfigItemAndReturnActionEditor(
  configListPage: ConfigListPage,
  page: Page,
  type: string = "Button",
  eventType: string = "On Press",
  projectOptions?: Partial<Project>,
) {
  await CreateNewInputConfigItemAndWaitForDialog(
    configListPage,
    page,
    projectOptions,
  )
  // Scan for input for device with respective input device type
  const triggerPanel = page.getByTestId("trigger-panel")
  await expect(triggerPanel).toBeVisible()

  const scanForInputButton = triggerPanel.getByRole("button", {
    name: "Scan for Input",
  })
  await expect(scanForInputButton).toBeVisible()
  await scanForInputButton.click()

  // fake the scan result for respective input device type
  await configListPage.mobiFlightPage.publishMessage({
    key: "ScanForInputResult",
    payload: {
      Controller: {
        Devices: [],
        Name: "Bravo Throttle Quadrant",
        Serial: "JS-87654321",
      },
      Device: {
        Name: `${type} 21`,
        Label: "Mode - ALT",
        Type: type,
      },
    } as ScanForInputResult,
  })

  const actionPanel = page.getByTestId("action-panel")
  const actionEditor = page.getByTestId("action-editor")

  const button = actionPanel.getByRole("button", {
    name: eventType,
    exact: true,
  })
  await expect(button).toBeVisible()
  await button.click()
  await expect(actionEditor).toBeVisible()
  return actionEditor
}

async function addModifierItemAndReturnEditor(
  modifierLabel: string,
  page: Page,
) {
  const modifiersPanel = page.getByTestId("modifiers-panel")
  await expect(modifiersPanel).toBeVisible()

  const addModifierButton = modifiersPanel.getByRole("button", {
    name: "Add modifier",
  })
  await expect(addModifierButton).toBeVisible()
  await addModifierButton.click()

  const modifierEditor = page.getByTestId("modifier-editor")
  await expect(modifierEditor).toBeVisible()

  const addModifierButtonInEditor = modifierEditor.getByRole("button", {
    name: "Add modifier",
  })
  await expect(addModifierButtonInEditor).toBeVisible()
  await addModifierButtonInEditor.click()

  const transformationOption = page.getByRole("menuitem", {
    name: modifierLabel,
  })
  await expect(transformationOption).toBeVisible()
  await transformationOption.click()
  return modifierEditor
}
