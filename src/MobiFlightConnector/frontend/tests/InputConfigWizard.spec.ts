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

const jeehellPresetsContent = `FCU_KNOBS:GROUP
FCU_HDGKNOB_PRESS:6:FCU Heading Knob Press
FCU_HDGKNOB_LONGPRESS:7:FCU Heading Knob Long Press
AP_ENGAGE:8:Autopilot Engage`

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
        .filter({ hasText: "AP_PANEL_HEADING_HOLD" }),
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

    // Open the preset ComboBox (currently shows the selected preset)
    await actionEditor
      .getByRole("combobox")
      .filter({ hasText: "AP_PANEL_HEADING_HOLD" })
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

    await expect(countLabel).toHaveText("4 preset(s) found")
    await resetFiltersButton.click()

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

  test("Newly created MSFS config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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

  test("Preset filter narrows the list and the count updates", async ({
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

    const filterInput = actionEditor.getByPlaceholder("Filter presets")
    const countLabel = actionEditor.getByRole("status")

    await expect(countLabel).toHaveText("4 preset(s) found")

    await filterInput.fill("land_alt_press_dn")
    await expect(countLabel).toHaveText("1 preset(s) found")

    await filterInput.fill("NoMatch")
    await expect(countLabel).toHaveText("0 preset(s) found")

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

    await expect(countLabel).toHaveText("4 preset(s) found")
    await resetFiltersButton.click()

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
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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

  test("Newly created (number) Variable config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const type = "Button"
    const eventType = "On Press"

    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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

    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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

  test("Newly created retrigger config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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
    await expect(actionEditor.getByText("Key combo:None")).toBeVisible()
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
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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
    await page.keyboard.down("D")

    const stopScanForKeyboardButton = actionEditor.getByRole("button", {
      name: "Stop scanning",
    })
    await expect(stopScanForKeyboardButton).toBeVisible()
    await stopScanForKeyboardButton.click()
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
      Key: 68,
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
    await expect(
      actionEditor.getByTestId("vjoy-button-command-state"),
    ).toHaveText("Pressed")
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

  test("Newly created vJoy Input Action (Button) config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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
    await page.getByRole("option", { name: "vJoy Device 1" }).click()
    await expect(
      page.getByRole("option", { name: "vJoy Device 1" }),
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
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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
    await page.getByRole("option", { name: "vJoy Device 1" }).click()
    await expect(
      page.getByRole("option", { name: "vJoy Device 1" }),
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

  test("BCDMode and Mask Visibility are displayed correctly based on type", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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
      Value: "",
      Modifiers: [],
    } as FsuipcOffsetInputAction)
  })

  test("Newly created FSUIPC Input Action (Float) config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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
      Value: "",
      Modifiers: [],
    } as FsuipcOffsetInputAction)
  })

  test("Newly created FSUIPC Input Action (String) config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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
      Value: "",
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

  test("Newly created FSUIPC EventID Input Action config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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

  test("Newly created FSUIPC PMDG EventID Input Action config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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

  test("Newly created FSUIPC Lua Macro Input Action config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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
    const macroNameInput = actionEditor.getByLabel("Macro Name:")
    await expect(macroNameInput).toBeVisible()
    await macroNameInput.fill("MACRO NAME")

    const macroValueInput = actionEditor.getByLabel("Macro Value:")
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
    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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

    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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

    const actionEditor = await CreateNewInputConfigItemAndReturnActionPanel(
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
})
async function CreateNewInputConfigItemAndReturnActionPanel(
  configListPage: ConfigListPage,
  page: Page,
  type: string = "Button",
  eventType: string = "On Press",
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
