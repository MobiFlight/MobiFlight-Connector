import { Page } from "@playwright/test"
import { test, expect } from "./fixtures"
import { ConfigListPage } from "./fixtures/ConfigListPage"
import outputConnectedControllers from "./data/outputconfig/connectedControllers.testdata.json" with { type: "json" }
import msfsOutputPresetsResponse from "./data/outputconfig/msfspresets.testdata.json" with { type: "json" }
import xplanePresetsResponse from "./data/inputaction/xplanepresets.testdata.json" with { type: "json" }
import { Project } from "../src/types"
import { IConfigItem } from "../src/types/config"

// Rows in the config list of the default test project (row 0 is the header)
const ROW = {
  Servo: 3, // "Servo"
  Output: 5, // "LED 1" - has config references and preconditions
  NoDevice: 7, // "New Output Config" - source, but no device
  InputAction: 15, // "Output Config With Input Action"
}

const NEW_OUTPUT_CONFIG_NAME = "New output config (created from test)"

// Sets up the config list with the default project,
// output capable controllers and the output presets.
const initOutputTestData = async (
  configListPage: ConfigListPage,
  page: Page,
  projectOptions?: Partial<Project>,
) => {
  await page.route(
    "*/**/presets/msfs2020_hubhop_presets.json",
    async (route) => {
      await route.fulfill({ json: msfsOutputPresetsResponse })
    },
  )
  await page.route("*/**/presets/xplane_hubhop_presets.json", async (route) => {
    await route.fulfill({ json: xplanePresetsResponse })
  })

  await configListPage.gotoPage()
  if (projectOptions) {
    await configListPage.mobiFlightPage.initWithTestDataAndSpecificProjectProps(
      projectOptions,
    )
  } else {
    await configListPage.mobiFlightPage.initWithTestData()
  }

  // The default controller test data has no output devices
  await configListPage.mobiFlightPage.publishMessage({
    key: "ConnectedControllers",
    payload: { Controllers: outputConnectedControllers },
  })
}

const openDialogForRow = async (
  configListPage: ConfigListPage,
  page: Page,
  row: number,
) => {
  const dialog = page.getByTestId("inputconfigwizard-dialog")
  await expect(dialog).not.toBeVisible()
  await configListPage.clickEditButtonForRow(row)
  await expect(dialog).toBeVisible()
  return dialog
}

const openWizardForRow = async (
  configListPage: ConfigListPage,
  page: Page,
  row: number,
  projectOptions?: Partial<Project>,
) => {
  await initOutputTestData(configListPage, page, projectOptions)
  return openDialogForRow(configListPage, page, row)
}

// The "Add Output Config" button still opens the legacy dialog,
// so we add an empty output config directly and open it via its row.
const createEmptyOutputConfigAndOpenWizard = async (
  configListPage: ConfigListPage,
  page: Page,
  projectOptions?: Partial<Project>,
) => {
  await initOutputTestData(configListPage, page, projectOptions)
  await configListPage.addNewConfigItem("OutputConfigItem", 0, "default", {
    Name: NEW_OUTPUT_CONFIG_NAME,
    Controller: undefined,
    Device: null,
    Source: null,
    ConfigRefs: [],
    Preconditions: [],
    Modifiers: { Items: [] },
  } as unknown as Partial<IConfigItem>)

  const dialog = page.getByTestId("inputconfigwizard-dialog")
  const row = page.getByRole("row").filter({ hasText: NEW_OUTPUT_CONFIG_NAME })
  await row.getByRole("button", { name: "Edit" }).click()
  await expect(dialog).toBeVisible()
  return dialog
}

const applyChangesAndReturnPayload = async (
  configListPage: ConfigListPage,
  page: Page,
) => {
  await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")
  await page.getByRole("button", { name: "Apply changes" }).click()
  await expect(page.getByTestId("inputconfigwizard-dialog")).not.toBeVisible()

  const commands = await configListPage.mobiFlightPage.getTrackedCommands()
  expect(commands).toBeDefined()
  return commands?.pop()?.payload
}

// The remove button of the source summary is only visible on hover
const removeSource = async (page: Page) => {
  const triggerPanel = page.getByTestId("trigger-panel")
  await triggerPanel.getByText("SimConnectSource").hover()
  await triggerPanel.getByRole("button", { name: "Remove" }).click()
}

const closeDrawer = async (page: Page) => {
  await page.getByRole("button", { name: "Done" }).click()
}

const selectComboBoxOption = async (
  page: Page,
  comboBox: ReturnType<Page["getByRole"]>,
  option: string,
) => {
  await comboBox.click()
  await page.getByRole("option", { name: option, exact: true }).click()
}

test.describe("General Output Config Wizard Tests", () => {
  test("Dialog open for output config items - via Edit button", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)

    await expect(page.getByTestId("dialog-config-name")).toContainText(
      "Edit output configuration",
    )
    await expect(
      page.getByText(
        "Edit all settings for your output configuration in this dialog.",
      ),
    ).toBeVisible()
    await expect(page.getByTestId("dialog-config-name")).toContainText("LED 1")
  })

  test("Dialog open for output config items - via double click", async ({
    configListPage,
    page,
  }) => {
    await initOutputTestData(configListPage, page)
    const row = await configListPage.getConfigItemRow(ROW.Output)

    await expect(page.getByText("Edit output configuration")).not.toBeVisible()
    await row.dblclick()
    await expect(page.getByText("Edit output configuration")).toBeVisible()
  })

  test("Apply changes button sends the config item to the backend", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.GUID).toBe("f79f75dd-0d45-4c91-870d-599cfc75ede1")
    expect(payload.item.Type).toBe("OutputConfigItem")
  })

  test("Dialog cancel button discards changes", async ({
    configListPage,
    page,
  }) => {
    const dialog = await openWizardForRow(configListPage, page, ROW.Output)
    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    await page.getByRole("button", { name: "Cancel" }).click()
    await expect(dialog).not.toBeVisible()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands?.length).toBe(0)
  })

  test("Dialog without pending changes can be closed by hitting ESC", async ({
    configListPage,
    page,
  }) => {
    const dialog = await openWizardForRow(configListPage, page, ROW.Output)

    await page.keyboard.press("Escape")
    await expect(dialog).not.toBeVisible()
  })

  test("Dialog with pending changes cannot be closed by hitting ESC", async ({
    configListPage,
    page,
  }) => {
    const dialog = await openWizardForRow(configListPage, page, ROW.Output)

    // Removing the source creates a pending change
    const triggerPanel = page.getByTestId("trigger-panel")
    await removeSource(page)
    await expect(
      triggerPanel.getByRole("button", { name: "Source", exact: true }),
    ).toBeVisible()

    await page.keyboard.press("Escape")
    await expect(dialog).toBeVisible()
    await expect(
      page.getByText(
        "You have pending changes. Apply them first, or click Cancel to close this dialog without saving.",
      ),
    ).toBeVisible()
  })

  test("Wizard shows the add buttons for preconditions, references and modifiers", async ({
    configListPage,
    page,
  }) => {
    const dialog = await openWizardForRow(configListPage, page, ROW.NoDevice)

    await expect(
      dialog.getByRole("button", { name: "Add precondition" }),
    ).toBeVisible()
    await expect(
      dialog.getByRole("button", { name: "Add reference" }),
    ).toBeVisible()
    await expect(
      dialog.getByRole("button", { name: "Add modifier" }),
    ).toBeVisible()
  })
})

test.describe("Output Config Wizard - Source Panel", () => {
  test("Summary shows the MSFS source of an existing config", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)

    const triggerPanel = page.getByTestId("trigger-panel")
    await expect(triggerPanel.getByText("SimConnectSource")).toBeVisible()
    await expect(
      triggerPanel.getByText("BRAKE_PARKING_INDICATOR", { exact: true }),
    ).toBeVisible()
    await expect(
      triggerPanel.getByText("(A:BRAKE PARKING INDICATOR,Bool)", {
        exact: true,
      }),
    ).toBeVisible()
  })

  test("New config without source shows the select value button", async ({
    configListPage,
    page,
  }) => {
    await createEmptyOutputConfigAndOpenWizard(configListPage, page)

    const triggerPanel = page.getByTestId("trigger-panel")
    const selectValueButton = triggerPanel.getByRole("button", {
      name: "Source",
      exact: true,
    })
    await expect(selectValueButton).toBeVisible()

    await selectValueButton.click()
    const sourceEditor = page.getByTestId("source-editor")
    await expect(sourceEditor).toBeVisible()
    await expect(sourceEditor.getByText("Specify source value")).toBeVisible()
  })

  test("Double click on the source summary opens the source editor", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)

    const triggerPanel = page.getByTestId("trigger-panel")
    const sourceEditor = page.getByTestId("source-editor")

    await test.step("Open via double click", async () => {
      await triggerPanel.getByText("SimConnectSource").dblclick()
      await expect(sourceEditor).toBeVisible()
      await closeDrawer(page)
      await expect(sourceEditor).not.toBeVisible()
    })
  })

  test("Removing the source shows the select value button again", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)

    const triggerPanel = page.getByTestId("trigger-panel")
    await removeSource(page)

    await expect(
      triggerPanel.getByRole("button", { name: "Source", exact: true }),
    ).toBeVisible()
    await expect(triggerPanel.getByText("SimConnectSource")).not.toBeVisible()

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Source).toBeUndefined()
  })

  test("Source types honor project settings and features", async ({
    configListPage,
    page,
  }) => {
    const testCases: {
      projectOptions: Partial<Project>
      expected: string[]
    }[] = [
      {
        projectOptions: { Sim: "msfs" },
        expected: ["Microsoft Flight Simulator", "MobiFlight - Variable"],
      },
      {
        projectOptions: { Sim: "xplane" },
        expected: ["X-Plane", "MobiFlight - Variable"],
      },
      {
        projectOptions: {
          Sim: "msfs",
          Features: { FSUIPC: true, ProSim: true },
        },
        expected: [
          "Microsoft Flight Simulator",
          "FSUIPC",
          "ProSim",
          "MobiFlight - Variable",
        ],
      },
    ]

    for (const { projectOptions, expected } of testCases) {
      await test.step(`Project ${JSON.stringify(projectOptions)}`, async () => {
        await createEmptyOutputConfigAndOpenWizard(
          configListPage,
          page,
          projectOptions,
        )
        await page
          .getByTestId("trigger-panel")
          .getByRole("button", { name: "Source", exact: true })
          .click()

        await page.getByTestId("source-type-combobox").click()
        const options = page.getByRole("option")
        await expect(options).toHaveText(expected)
      })
    }
  })

  test("MSFS source editor shows only output presets", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)
    await page.getByTestId("trigger-panel").getByText("SimConnectSource").dblclick()

    const sourceEditor = page.getByTestId("source-editor")
    await expect(
      sourceEditor
        .getByTestId("source-type-combobox")
        .filter({ hasText: "Microsoft Flight Simulator" }),
    ).toBeVisible()

    const resetFiltersButton = sourceEditor.getByRole("button", {
      name: "Reset filters",
    })
    await resetFiltersButton.click()

    // the input preset of the test data is filtered out
    await expect(sourceEditor.getByRole("status")).toHaveText(
      "3 preset(s) found",
    )

    const filterInput = sourceEditor.getByPlaceholder("Filter presets")
    await filterInput.fill("COM1")
    await expect(sourceEditor.getByRole("status")).toHaveText(
      "1 preset(s) found",
    )

    await filterInput.fill("AP_PANEL_HEADING_HOLD_INPUT_ONLY")
    await expect(sourceEditor.getByRole("status")).toHaveText(
      "0 preset(s) found",
    )
  })

  test("X-Plane source editor shows only output presets", async ({
    configListPage,
    page,
  }) => {
    await createEmptyOutputConfigAndOpenWizard(configListPage, page, {
      Sim: "xplane",
    })
    await page
      .getByTestId("trigger-panel")
      .getByRole("button", { name: "Source", exact: true })
      .click()
    await page.getByTestId("source-type-combobox").click()
    await page.getByRole("option", { name: "X-Plane" }).click()

    const sourceEditor = page.getByTestId("source-editor")

    // only the InputOutput DataRef of the test data can be read as output
    await expect(sourceEditor.getByRole("status")).toHaveText(
      "1 preset(s) found",
    )
    await expect(
      sourceEditor
        .getByRole("listitem")
        .filter({ hasText: "IXEG B733 MCP VS Display" }),
    ).toBeVisible()
  })

  test("Selecting an MSFS preset updates the code and is saved correctly", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)
    await page.getByTestId("trigger-panel").getByText("SimConnectSource").dblclick()

    const sourceEditor = page.getByTestId("source-editor")
    await sourceEditor.getByRole("button", { name: "Reset filters" }).click()

    // Open the preset list via the currently selected preset
    await sourceEditor
      .getByRole("listitem")
      .getByText("BRAKE_PARKING_INDICATOR", { exact: true })
      .click()
    await page
      .getByRole("listitem")
      .getByText("PROP_RPM_1", { exact: true })
      .click()

    await expect(
      sourceEditor
        .getByTestId("preset-code-panel")
        .getByRole("textbox", { name: "Enter RPN code" }),
    ).toHaveValue("(A:PROP RPM:1,Rpm)")

    await closeDrawer(page)
    await expect(sourceEditor).not.toBeVisible()

    // Summary reflects the new preset
    const triggerPanel = page.getByTestId("trigger-panel")
    await expect(
      triggerPanel.getByText("PROP_RPM_1", { exact: true }),
    ).toBeVisible()

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Source).toMatchObject({
      Type: "SimConnectSource",
      SimConnectValue: {
        UUID: "02d438f0-b1cd-4920-8884-e3fc4998e49c",
        Value: "(A:PROP RPM:1,Rpm)",
      },
    })
  })

  test("Newly created Variable source is saved correctly", async ({
    configListPage,
    page,
  }) => {
    await createEmptyOutputConfigAndOpenWizard(configListPage, page)
    await page
      .getByTestId("trigger-panel")
      .getByRole("button", { name: "Source", exact: true })
      .click()

    const sourceEditor = page.getByTestId("source-editor")
    await selectComboBoxOption(
      page,
      sourceEditor.getByTestId("source-type-combobox"),
      "MobiFlight - Variable",
    )

    // Output variables are read only, so there is no expression field
    await expect(
      sourceEditor.getByPlaceholder("Enter expression..."),
    ).not.toBeVisible()

    await sourceEditor
      .getByPlaceholder("Enter variable name...")
      .fill("MyOutputVar")

    await closeDrawer(page)
    await expect(sourceEditor).not.toBeVisible()

    const triggerPanel = page.getByTestId("trigger-panel")
    await expect(triggerPanel.getByText("VariableSource")).toBeVisible()
    await expect(triggerPanel.getByText("MyOutputVar")).toBeVisible()

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Source).toMatchObject({
      Type: "VariableSource",
      MobiFlightVariable: {
        Name: "MyOutputVar",
        TYPE: "number",
      },
    })
  })

  test("Deselecting the source type removes the source", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)
    await page.getByTestId("trigger-panel").getByText("SimConnectSource").dblclick()

    const sourceEditor = page.getByTestId("source-editor")
    // Selecting the already selected option deselects it
    await selectComboBoxOption(
      page,
      sourceEditor.getByTestId("source-type-combobox"),
      "Microsoft Flight Simulator",
    )
    await expect(sourceEditor.getByTestId("preset-code-panel")).not.toBeVisible()

    await closeDrawer(page)
    await expect(
      page
        .getByTestId("trigger-panel")
        .getByRole("button", { name: "Source", exact: true }),
    ).toBeVisible()
  })
})

test.describe("Output Config Wizard - Output Device Panel", () => {
  test("Summary shows the output device of an existing config", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)

    const actionPanel = page.getByTestId("action-panel")
    await expect(actionPanel.getByText("ProtoBoard-v2")).toBeVisible()
    await expect(actionPanel.getByText("Output", { exact: true })).toBeVisible()
    await expect(actionPanel.getByText("LED 2", { exact: true })).toBeVisible()

    // There is a device, so no add buttons are shown
    await expect(
      actionPanel.getByRole("button", { name: "Output device" }),
    ).not.toBeVisible()
  })

  test("Config without device shows the add buttons", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.NoDevice)

    const actionPanel = page.getByTestId("action-panel")
    await expect(
      actionPanel.getByRole("button", { name: "Output device" }),
    ).toBeVisible()
    await expect(
      actionPanel.getByRole("button", { name: "Button action" }),
    ).toBeVisible()
    await expect(
      actionPanel.getByRole("button", { name: "Analog action" }),
    ).toBeVisible()
  })

  test("Removing the output device shows the add buttons again", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)

    const actionPanel = page.getByTestId("action-panel")
    // TODO: the remove button for the output device reuses the "Remove input action" label
    await actionPanel.getByRole("button", { name: "Remove input action" }).click()

    await expect(
      actionPanel.getByRole("button", { name: "Output device" }),
    ).toBeVisible()

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Device).toBeNull()
    expect(payload.item.Controller).toBeUndefined()
  })

  test("Controller list only shows controllers with output devices", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.NoDevice)
    await page
      .getByTestId("action-panel")
      .getByRole("button", { name: "Output device" })
      .click()

    const deviceEditor = page.getByTestId("output-device-editor")
    await expect(deviceEditor).toBeVisible()

    await deviceEditor.getByRole("combobox").first().click()
    await expect(page.getByRole("option")).toHaveText([
      "ProtoBoard-v2",
      "Mega Output Board",
    ])
  })

  test("Device types are based on the devices of the selected controller", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.NoDevice)
    await page
      .getByTestId("action-panel")
      .getByRole("button", { name: "Output device" })
      .click()

    const deviceEditor = page.getByTestId("output-device-editor")
    const controllerComboBox = deviceEditor.getByRole("combobox").first()
    const deviceTypeComboBox = deviceEditor.getByRole("combobox").nth(1)

    await test.step("ProtoBoard-v2 offers all its output device types", async () => {
      await selectComboBoxOption(page, controllerComboBox, "ProtoBoard-v2")
      await deviceTypeComboBox.click()
      await expect(page.getByRole("option")).toHaveText([
        "Output",
        "Servo",
        "Stepper",
        "LCD Display",
        "7-Segment Display",
      ])
      await page.keyboard.press("Escape")
    })

    await test.step("Mega Output Board offers only Output", async () => {
      await selectComboBoxOption(page, controllerComboBox, "Mega Output Board")
      await deviceTypeComboBox.click()
      await expect(page.getByRole("option")).toHaveText(["Output"])
      await page.keyboard.press("Escape")
    })
  })

  test("Newly created Output device config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    await createEmptyOutputConfigAndOpenWizard(configListPage, page)
    const actionPanel = page.getByTestId("action-panel")
    await actionPanel.getByRole("button", { name: "Output device" }).click()

    const deviceEditor = page.getByTestId("output-device-editor")
    await selectComboBoxOption(
      page,
      deviceEditor.getByRole("combobox").first(),
      "ProtoBoard-v2",
    )
    await selectComboBoxOption(
      page,
      deviceEditor.getByRole("combobox").nth(1),
      "Output",
    )

    await test.step("First pin is selected by default", async () => {
      await expect(
        deviceEditor.getByRole("checkbox", { name: "LED 1" }),
      ).toBeChecked()
      await expect(
        deviceEditor.getByRole("checkbox", { name: "LED 2" }),
      ).not.toBeChecked()
    })

    await test.step("Select second pin and enable dynamic brightness", async () => {
      await deviceEditor.getByRole("checkbox", { name: "LED 2" }).click()
      await expect(
        deviceEditor.getByRole("checkbox", { name: "LED 2" }),
      ).toBeChecked()

      // LED 2 supports PWM
      const pwmSwitch = deviceEditor.getByRole("switch", {
        name: "Dynamic brightness",
      })
      await expect(pwmSwitch).toBeEnabled()
      await pwmSwitch.click()
      await expect(pwmSwitch).toBeChecked()
    })

    await closeDrawer(page)
    await expect(deviceEditor).not.toBeVisible()

    await test.step("Summary shows the selected device", async () => {
      await expect(actionPanel.getByText("ProtoBoard-v2")).toBeVisible()
      await expect(actionPanel.getByText("LED 1", { exact: true })).toBeVisible()
      await expect(actionPanel.getByText("LED 2", { exact: true })).toBeVisible()
      await expect(actionPanel.getByText("Dynamic brightness", { exact: true })).toBeVisible()
    })

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Controller).toMatchObject({
      Name: "ProtoBoard-v2",
      Serial: "SN-5FC-1CF",
    })
    expect(payload.item.Controller.Devices).toBeUndefined()
    expect(payload.item.Device).toEqual({
      Type: "Output",
      Name: "LED 1|LED 2",
      Pin: "LED 1|LED 2",
      Brightness: 255,
      PwmMode: true,
    })
  })

  test("Dynamic brightness is disabled if no pin supports PWM", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.NoDevice)
    await page
      .getByTestId("action-panel")
      .getByRole("button", { name: "Output device" })
      .click()

    const deviceEditor = page.getByTestId("output-device-editor")
    await selectComboBoxOption(
      page,
      deviceEditor.getByRole("combobox").first(),
      "Mega Output Board",
    )
    await selectComboBoxOption(
      page,
      deviceEditor.getByRole("combobox").nth(1),
      "Output",
    )

    // Device label is shown for the pin
    await expect(
      deviceEditor.getByRole("checkbox", { name: "Gear Light" }),
    ).toBeChecked()
    await expect(
      deviceEditor.getByRole("switch", { name: "Dynamic brightness" }),
    ).toBeDisabled()
    await expect(
      deviceEditor.getByText("No dimmable pins available."),
    ).toBeVisible()
  })

  test("Existing Servo config is displayed and saved correctly", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Servo)

    const actionPanel = page.getByTestId("action-panel")
    await test.step("Summary shows the servo settings", async () => {
      await expect(actionPanel.getByText("Servo 1", { exact: true })).toBeVisible()
      await expect(actionPanel.getByText("0 – 255")).toBeVisible()
      await expect(actionPanel.getByText("100 %")).toBeVisible()
    })

    await actionPanel.getByRole("button").last().click()
    const deviceEditor = page.getByTestId("output-device-editor")
    await expect(deviceEditor).toBeVisible()

    await test.step("Details show the servo settings", async () => {
      await expect(
        deviceEditor.getByRole("combobox").filter({ hasText: "Servo 1" }),
      ).toBeVisible()
      await expect(deviceEditor.getByLabel("Min value")).toHaveValue("0")
      await expect(deviceEditor.getByLabel("Max value")).toHaveValue("255")
      await expect(deviceEditor.getByLabel("Max rotation (%)")).toHaveValue(
        "100",
      )
    })

    await test.step("Change servo and range", async () => {
      await selectComboBoxOption(
        page,
        deviceEditor.getByRole("combobox").filter({ hasText: "Servo 1" }),
        "Servo 2",
      )
      await deviceEditor.getByLabel("Min value").fill("10")
      await deviceEditor.getByLabel("Max value").fill("200")
      await deviceEditor.getByLabel("Max rotation (%)").fill("50")
    })

    await closeDrawer(page)
    await expect(deviceEditor).not.toBeVisible()

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Device).toEqual({
      Type: "Servo",
      Name: "Servo 2",
      Address: "Servo 2",
      Min: "10",
      Max: "200",
      MaxRotationPercent: "50",
    })
  })

  test("Test panel starts and stops the test of the output device", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)
    const actionPanel = page.getByTestId("action-panel")
    await actionPanel.getByRole("button").last().click()

    const deviceEditor = page.getByTestId("output-device-editor")
    await expect(
      deviceEditor.getByText("Step - Test output device"),
    ).toBeVisible()

    await configListPage.mobiFlightPage.trackCommand("CommandConfigContextMenu")

    await test.step("Start test with a custom value", async () => {
      // the value is committed when the input loses focus
      await deviceEditor.getByPlaceholder("Enter raw value").fill("42")
      await deviceEditor.getByPlaceholder("Enter raw value").blur()
      await deviceEditor.getByRole("button", { name: "Test" }).click()
      await expect(
        deviceEditor.getByRole("button", { name: "Stop" }),
      ).toBeVisible()

      const commands = await configListPage.mobiFlightPage.getTrackedCommands()
      const payload = commands?.pop()?.payload
      expect(payload.action).toBe("testStart")
      expect(payload.item.GUID).toBe("f79f75dd-0d45-4c91-870d-599cfc75ede1")
      expect(payload.item.TestValue).toMatchObject({ Float64: 42 })
    })

    await test.step("Stop test", async () => {
      await deviceEditor.getByRole("button", { name: "Stop" }).click()
      await expect(
        deviceEditor.getByRole("button", { name: "Test" }),
      ).toBeVisible()

      const commands = await configListPage.mobiFlightPage.getTrackedCommands()
      expect(commands?.pop()?.payload.action).toBe("testStop")
    })
  })

  test("Closing the device editor stops a running test", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)
    await page.getByTestId("action-panel").getByRole("button").last().click()

    const deviceEditor = page.getByTestId("output-device-editor")
    await configListPage.mobiFlightPage.trackCommand("CommandConfigContextMenu")
    await deviceEditor.getByRole("button", { name: "Test" }).click()
    await expect(deviceEditor.getByRole("button", { name: "Stop" })).toBeVisible()

    await closeDrawer(page)
    await expect(deviceEditor).not.toBeVisible()

    await expect
      .poll(async () => {
        const commands =
          await configListPage.mobiFlightPage.getTrackedCommands()
        return commands?.map((command) => command.payload.action)
      })
      .toEqual(["testStart", "testStop"])
  })
})

test.describe("Output Config Wizard - Input Action Panel", () => {
  test("Adding a button action replaces the output device panel", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.NoDevice)

    const actionPanel = page.getByTestId("action-panel")
    await actionPanel.getByRole("button", { name: "Button action" }).click()

    for (const event of ["On Press", "On Release"]) {
      await expect(
        actionPanel.getByRole("button", { name: event, exact: true }),
      ).toBeVisible()
    }
    await expect(
      actionPanel.getByRole("button", { name: "Output device" }),
    ).not.toBeVisible()

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.ButtonInputConfig).toBeDefined()
    expect(payload.item.AnalogInputConfig).toBeUndefined()
    expect(payload.item.Device).toBeNull()
  })

  test("Adding an analog action replaces the output device panel", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.NoDevice)

    const actionPanel = page.getByTestId("action-panel")
    await actionPanel.getByRole("button", { name: "Analog action" }).click()

    await expect(
      actionPanel.getByRole("button", { name: "Output device" }),
    ).not.toBeVisible()

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.AnalogInputConfig).toBeDefined()
    expect(payload.item.ButtonInputConfig).toBeUndefined()
  })

  test("Existing input action is displayed and opens the action editor", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.InputAction)

    const actionPanel = page.getByTestId("action-panel")
    await actionPanel
      .getByRole("button", { name: "On Press", exact: true })
      .click()

    const actionEditor = page.getByTestId("action-editor")
    await expect(actionEditor).toBeVisible()
    await expect(actionEditor.getByTestId("action-type-combobox")).toBeVisible()
  })

  test("Newly created button action is saved correctly", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.InputAction)

    const actionPanel = page.getByTestId("action-panel")
    await actionPanel
      .getByRole("button", { name: "On Press", exact: true })
      .click()

    const actionEditor = page.getByTestId("action-editor")
    await selectComboBoxOption(
      page,
      actionEditor.getByTestId("action-type-combobox"),
      "MobiFlight - Retrigger switches",
    )
    await closeDrawer(page)
    await expect(actionEditor).not.toBeVisible()

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.ButtonInputConfig.onPress).toEqual({
      Type: "RetriggerInputAction",
    })
  })

  test("Removing the input action shows the add buttons again", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.InputAction)

    const actionPanel = page.getByTestId("action-panel")
    await actionPanel
      .getByRole("button", { name: "Remove input action" })
      .click()

    await expect(
      actionPanel.getByRole("button", { name: "Output device" }),
    ).toBeVisible()

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.ButtonInputConfig).toBeUndefined()
    expect(payload.item.AnalogInputConfig).toBeUndefined()
  })
})

test.describe("Output Config Wizard - Preconditions, References and Modifiers", () => {
  test("Existing config references and preconditions are shown", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)

    const configReferencesPanel = page.getByTestId("config-reference-panel")
    await expect(
      configReferencesPanel.getByText("#", { exact: true }),
    ).toBeVisible()
    await expect(
      configReferencesPanel.getByText("!", { exact: true }),
    ).toBeVisible()

    const preconditionsPanel = page.getByTestId("precondition-panel")
    await expect(preconditionsPanel.getByText(/7-Segment/)).toBeVisible()
  })

  test("Config without references and preconditions hides their summaries", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Servo)

    await expect(page.getByTestId("config-reference-panel")).not.toBeVisible()
    await expect(page.getByTestId("precondition-panel")).not.toBeVisible()
    await expect(page.getByTestId("modifier-panel")).not.toBeVisible()
  })

  test("Added modifier is shown in summary and saved correctly", async ({
    configListPage,
    page,
  }) => {
    const dialog = await openWizardForRow(configListPage, page, ROW.Servo)

    await dialog.getByRole("button", { name: "Add modifier" }).click()
    const modifierEditor = page.getByTestId("modifier-editor")
    await expect(modifierEditor).toBeVisible()

    await modifierEditor.getByRole("button", { name: "Add modifier" }).click()
    await page.getByRole("menuitem", { name: "Transformation" }).click()

    await closeDrawer(page)
    await expect(modifierEditor).not.toBeVisible()

    const modifiersPanel = page.getByTestId("modifier-panel")
    await expect(modifiersPanel.getByText("Transformation")).toBeVisible()

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Modifiers.Items).toHaveLength(1)
    expect(payload.item.Modifiers.Items[0].Type).toBe("Transformation")
  })
})
