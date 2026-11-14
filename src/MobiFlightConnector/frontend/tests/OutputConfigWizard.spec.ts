import { Page } from "@playwright/test"
import { test, expect } from "./fixtures"
import { ConfigListPage } from "./fixtures/ConfigListPage"
import outputConnectedControllers from "./data/outputconfig/connectedControllers.testdata.json" with { type: "json" }
import msfsOutputPresetsResponse from "./data/outputconfig/msfspresets.testdata.json" with { type: "json" }
import xplanePresetsResponse from "./data/inputaction/xplanepresets.testdata.json" with { type: "json" }
import { Project } from "../src/types"
import { IConfigItem, Source } from "../src/types/config"

// Rows in the config list of the default test project (row 0 is the header)
const ROW = {
  SevenSegment: 1, // "7-Segment"
  Stepper: 2, // "Stepper 2"
  Servo: 3, // "Servo"
  LcdDisplay: 4, // "LCD"
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
  const dialog = page.getByTestId("configwizard-dialog")
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

const createEmptyOutputConfigAndOpenWizard = async (
  configListPage: ConfigListPage,
  page: Page,
  projectOptions?: Partial<Project>,
) =>
  createOutputConfigWithSourceAndOpenWizard(
    configListPage,
    page,
    null,
    projectOptions,
  )

// Creates a new output config without device that uses the given source
const createOutputConfigWithSourceAndOpenWizard = async (
  configListPage: ConfigListPage,
  page: Page,
  source: Source | null,
  projectOptions?: Partial<Project>,
) => {
  await initOutputTestData(configListPage, page, projectOptions)
  await configListPage.addNewConfigItem("OutputConfigItem", 0, "default", {
    Name: NEW_OUTPUT_CONFIG_NAME,
    Controller: undefined,
    Device: null,
    Source: source,
    ConfigRefs: [],
    Preconditions: [],
    Modifiers: { Items: [] },
  } as unknown as Partial<IConfigItem>)

  const dialog = page.getByTestId("configwizard-dialog")
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
  await expect(page.getByTestId("configwizard-dialog")).not.toBeVisible()

  const commands = await configListPage.mobiFlightPage.getTrackedCommands()
  expect(commands).toBeDefined()
  return commands?.pop()?.payload
}

// Footer only offers Cancel and Apply changes once the config has been edited
const renameConfig = async (page: Page, name: string) => {
  const configName = page.getByTestId("dialog-config-name")
  await configName.getByRole("button").dblclick()
  const nameInput = configName.getByRole("textbox")
  await nameInput.fill(name)
  await nameInput.press("Enter")
  await expect(nameInput).not.toBeVisible()
}

// The remove button of the source summary is only visible on hover
const removeSource = async (page: Page) => {
  const triggerPanel = page.getByTestId("trigger-panel")
  await triggerPanel.getByText("Microsoft Flight Simulator").hover()
  await triggerPanel.getByRole("button", { name: "Remove" }).click()
}

const closeDrawer = async (page: Page) => {
  await page.getByRole("button", { name: "Done" }).click()
}

// Creates a new output config, opens the source editor and selects the source type
const createConfigWithSourceType = async (
  configListPage: ConfigListPage,
  page: Page,
  sourceType: string,
  projectOptions?: Partial<Project>,
) => {
  await createEmptyOutputConfigAndOpenWizard(configListPage, page, projectOptions)
  await page
    .getByTestId("trigger-panel")
    .getByRole("button", { name: "Source", exact: true })
    .click()
  const sourceEditor = page.getByTestId("source-editor")
  await selectComboBoxOption(
    page,
    sourceEditor.getByTestId("source-type-combobox"),
    sourceType,
  )
  return sourceEditor
}

const selectComboBoxOption = async (
  page: Page,
  comboBox: ReturnType<Page["getByRole"]>,
  option: string,
) => {
  await comboBox.click()
  await page.getByRole("option", { name: option, exact: true }).click()
}

const openOutputDevicePanel = async (page: Page) => {
  await page
    .getByTestId("action-panel")
    .getByRole("button", { name: "Output device", exact: true })
    .click()
}

test.describe("General Output Config Wizard Tests", () => {
  test("Dialog title shows the config type and name", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)

    const dialogTitle = page.getByTestId("dialog-config-name")
    await expect(dialogTitle).toContainText("Output Configuration")
    await expect(dialogTitle).toContainText("LED 1")
    await expect(
      page.getByText(
        "Edit all settings for your output configuration in this dialog.",
      ),
    ).toBeVisible()
  })

  test("Dialog open for output config items - via Edit button", async ({
    configListPage,
    page,
  }) => {
    await initOutputTestData(configListPage, page)
    const dialog = page.getByTestId("configwizard-dialog")

    await expect(dialog).not.toBeVisible()
    await configListPage.clickEditButtonForRow(ROW.Output)
    await expect(dialog).toBeVisible()
  })

  test("Dialog open for output config items - via double click", async ({
    configListPage,
    page,
  }) => {
    await initOutputTestData(configListPage, page)
    const row = await configListPage.getConfigItemRow(ROW.Output)
    const dialog = page.getByTestId("configwizard-dialog")

    await expect(dialog).not.toBeVisible()
    await row.dblclick()
    await expect(dialog).toBeVisible()
  })

  test("Dialog open for output config items - via context menu", async ({
    configListPage,
    page,
  }) => {
    await initOutputTestData(configListPage, page)
    const row = await configListPage.getConfigItemRow(ROW.Output)
    const dialog = page.getByTestId("configwizard-dialog")

    await expect(dialog).not.toBeVisible()
    await row.getByRole("button", { name: "Open menu" }).first().click()
    const contextMenu = page.getByTestId("config-item-context-menu")
    await contextMenu.getByRole("menuitem", { name: "Edit" }).click()

    await expect(dialog).toBeVisible()
    await expect(page.getByTestId("dialog-config-name")).toContainText("LED 1")
  })

  test("Dialog open for output config items - via 'Add Output Config' button", async ({
    configListPage,
    page,
  }) => {
    await initOutputTestData(configListPage, page)
    const dialog = page.getByTestId("configwizard-dialog")

    await expect(dialog).not.toBeVisible()
    await page.getByRole("button", { name: "Add Output Config" }).click()
    // Simulate the backend response after adding the item
    await configListPage.addNewConfigItem("OutputConfigItem", 0, "default", {
      Name: NEW_OUTPUT_CONFIG_NAME,
    })

    await expect(dialog).toBeVisible()
    const nameInput = page.getByTestId("dialog-config-name").getByRole("textbox")
    await expect(nameInput).toBeFocused()
    await expect(nameInput).toHaveValue(NEW_OUTPUT_CONFIG_NAME)
  })

  test("Dialog open for output config items - via duplicate", async ({
    configListPage,
    page,
  }) => {
    await initOutputTestData(configListPage, page)
    const row = await configListPage.getConfigItemRow(ROW.SevenSegment)
    const dialog = page.getByTestId("configwizard-dialog")

    await row.getByRole("button", { name: "Open menu" }).first().click()
    const contextMenu = page.getByTestId("config-item-context-menu")
    await contextMenu.getByRole("menuitem", { name: "Duplicate" }).click()
    // Simulate the backend response after duplicating the item
    await configListPage.duplicateConfigItem(0)

    await expect(dialog).toBeVisible()
    const nameInput = page.getByTestId("dialog-config-name").getByRole("textbox")
    await expect(nameInput).toBeFocused()
    await expect(nameInput).toHaveValue("7-Segment")
  })

  test("Apply changes button sends the config item to the backend", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)
    await renameConfig(page, "Renamed output")

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.GUID).toBe("f79f75dd-0d45-4c91-870d-599cfc75ede1")
    expect(payload.item.Type).toBe("OutputConfigItem")
    expect(payload.item.Name).toBe("Renamed output")
  })

  test("Dialog cancel button discards changes", async ({
    configListPage,
    page,
  }) => {
    const dialog = await openWizardForRow(configListPage, page, ROW.Output)
    await renameConfig(page, "Renamed output")
    await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

    await page.getByRole("button", { name: "Cancel" }).click()
    await expect(dialog).not.toBeVisible()

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands?.length).toBe(0)
  })

  test("Dialog without pending changes only shows a Close button", async ({
    configListPage,
    page,
  }) => {
    const dialog = await openWizardForRow(configListPage, page, ROW.Output)
    const footer = page.getByTestId("configwizard-dialog-footer")

    await test.step("No changes: only Close is offered", async () => {
      await expect(footer.getByRole("button", { name: "Close" })).toBeVisible()
      await expect(
        footer.getByRole("button", { name: "Apply changes" }),
      ).toHaveCount(0)
      await expect(footer.getByRole("button", { name: "Cancel" })).toHaveCount(
        0,
      )
    })

    await test.step("After a change: Cancel and Apply changes are offered", async () => {
      await renameConfig(page, "Renamed output")
      await expect(footer.getByRole("button", { name: "Cancel" })).toBeVisible()
      await expect(
        footer.getByRole("button", { name: "Apply changes" }),
      ).toBeVisible()
      await expect(footer.getByRole("button", { name: "Close" })).toHaveCount(0)
    })

    await test.step("Close without changes sends no update", async () => {
      await footer.getByRole("button", { name: "Cancel" }).click()
      await expect(dialog).not.toBeVisible()

      await openDialogForRow(configListPage, page, ROW.Output)
      await configListPage.mobiFlightPage.trackCommand(
        "CommandUpdateConfigItem",
      )
      await footer.getByRole("button", { name: "Close" }).click()
      await expect(dialog).not.toBeVisible()

      const commands = await configListPage.mobiFlightPage.getTrackedCommands()
      expect(commands?.length).toBe(0)
    })
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

    // Breadcrumb: config name > Trigger > Source value
    await expect(
      sourceEditor.getByText(NEW_OUTPUT_CONFIG_NAME, { exact: true }),
    ).toBeVisible()
    await expect(sourceEditor.getByText("Trigger", { exact: true })).toBeVisible()
    await expect(
      sourceEditor.getByText("Source value", { exact: true }),
    ).toBeVisible()
  })

  test("Double click on the source summary opens the source editor", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)

    const triggerPanel = page.getByTestId("trigger-panel")
    const sourceEditor = page.getByTestId("source-editor")

    await test.step("Open via double click", async () => {
      await triggerPanel.getByText("Microsoft Flight Simulator").dblclick()
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
    await expect(triggerPanel.getByText("Microsoft Flight Simulator")).not.toBeVisible()

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

  test("Deselecting the source type removes the source", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)
    await page.getByTestId("trigger-panel").getByText("Microsoft Flight Simulator").dblclick()

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

test.describe("Output Config Wizard - MSFS Source", () => {
  test("Summary shows the MSFS source of an existing config", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)

    const triggerPanel = page.getByTestId("trigger-panel")
    await expect(triggerPanel.getByText("Microsoft Flight Simulator")).toBeVisible()
    await expect(
      triggerPanel.getByText("BRAKE_PARKING_INDICATOR", { exact: true }),
    ).toBeVisible()
    await expect(
      triggerPanel.getByText("(A:BRAKE PARKING INDICATOR,Bool)", {
        exact: true,
      }),
    ).toBeVisible()
  })

  test("MSFS source editor shows only output presets", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)
    await page.getByTestId("trigger-panel").getByText("Microsoft Flight Simulator").dblclick()

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

  test("Selecting an MSFS preset updates the code and is saved correctly", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)
    await page.getByTestId("trigger-panel").getByText("Microsoft Flight Simulator").dblclick()

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
})

test.describe("Output Config Wizard - X-Plane Source", () => {
  test("Summary shows the X-Plane source of an existing config", async ({
    configListPage,
    page,
  }) => {
    await createOutputConfigWithSourceAndOpenWizard(
      configListPage,
      page,
      {
        Type: "XplaneSource",
        XplaneDataRef: { Path: "ixeg/733/MCP/mcp_vs_dial_ann" },
      },
      { Sim: "xplane" },
    )

    const triggerPanel = page.getByTestId("trigger-panel")
    await expect(triggerPanel.getByText("X-Plane", { exact: true })).toBeVisible()
    await expect(
      triggerPanel.getByText("IXEG B733 MCP VS Display", { exact: true }),
    ).toBeVisible()
    await expect(
      triggerPanel.getByText("ixeg/733/MCP/mcp_vs_dial_ann", { exact: true }),
    ).toBeVisible()
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

  test("Selecting an X-Plane preset updates the path and is saved correctly", async ({
    configListPage,
    page,
  }) => {
    const sourceEditor = await createConfigWithSourceType(
      configListPage,
      page,
      "X-Plane",
      { Sim: "xplane" },
    )

    await sourceEditor
      .getByRole("listitem")
      .getByText("IXEG B733 MCP VS Display", { exact: true })
      .click()

    await test.step("Path is updated by the preset", async () => {
      await expect(sourceEditor.getByLabel("Path")).toHaveValue(
        "ixeg/733/MCP/mcp_vs_dial_ann",
      )
      // Output sources can only read DataRefs, so there is no value field
      await expect(sourceEditor.getByLabel("Value")).not.toBeVisible()
    })

    await closeDrawer(page)
    await expect(sourceEditor).not.toBeVisible()

    await test.step("Summary shows the preset", async () => {
      const triggerPanel = page.getByTestId("trigger-panel")
      await expect(triggerPanel.getByText("X-Plane")).toBeVisible()
      await expect(
        triggerPanel.getByText("IXEG B733 MCP VS Display", { exact: true }),
      ).toBeVisible()
    })

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Source).toEqual({
      Type: "XplaneSource",
      XplaneDataRef: { Path: "ixeg/733/MCP/mcp_vs_dial_ann" },
    })
  })

  test("Custom X-Plane path is saved correctly", async ({
    configListPage,
    page,
  }) => {
    const sourceEditor = await createConfigWithSourceType(
      configListPage,
      page,
      "X-Plane",
      { Sim: "xplane" },
    )

    await sourceEditor.getByLabel("Path").fill("sim/custom/dataref")

    await closeDrawer(page)
    await expect(sourceEditor).not.toBeVisible()

    await expect(
      page
        .getByTestId("trigger-panel")
        .getByText("sim/custom/dataref", { exact: true }),
    ).toBeVisible()

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Source).toEqual({
      Type: "XplaneSource",
      XplaneDataRef: { Path: "sim/custom/dataref" },
    })
  })
})

test.describe("Output Config Wizard - FSUIPC Source", () => {
  test("Summary shows the FSUIPC source of an existing config", async ({
    configListPage,
    page,
  }) => {
    await createOutputConfigWithSourceAndOpenWizard(
      configListPage,
      page,
      {
        Type: "FsuipcSource",
        FSUIPC: {
          OffsetType: 0,
          Size: 4,
          Offset: 0x66cc,
          Mask: 0xaabbccdd,
          BcdMode: true,
        },
      },
      { Sim: "msfs", Features: { FSUIPC: true, ProSim: false } },
    )

    const triggerPanel = page.getByTestId("trigger-panel")
    await expect(triggerPanel.getByText("FSUIPC", { exact: true })).toBeVisible()
    await expect(triggerPanel.getByText("4", { exact: true })).toBeVisible()
    await expect(triggerPanel.getByText("66CC", { exact: true })).toBeVisible()
    await expect(triggerPanel.getByText("AABBCCDD", { exact: true })).toBeVisible()
    await expect(triggerPanel.getByText("True", { exact: true })).toBeVisible()
    // A source only reads the offset, so there is no value to write
    await expect(triggerPanel.getByText("Value", { exact: true })).not.toBeVisible()
  })

  test("Newly created FSUIPC source is saved correctly", async ({
    configListPage,
    page,
  }) => {
    const sourceEditor = await createConfigWithSourceType(
      configListPage,
      page,
      "FSUIPC",
      { Sim: "msfs", Features: { FSUIPC: true, ProSim: false } },
    )

    await test.step("Configure the offset", async () => {
      await selectComboBoxOption(
        page,
        sourceEditor.getByRole("combobox").filter({ hasText: "1 Byte" }),
        "4 Bytes",
      )
      await sourceEditor.getByRole("textbox", { name: "Offset" }).fill("66CC")
      await sourceEditor.getByRole("textbox", { name: "Mask" }).fill("AABBCCDD")
      await sourceEditor.getByRole("switch").click()
      await expect(sourceEditor.getByRole("switch")).toBeChecked()
    })

    await closeDrawer(page)
    await expect(sourceEditor).not.toBeVisible()

    await test.step("Summary shows the offset", async () => {
      const triggerPanel = page.getByTestId("trigger-panel")
      await expect(triggerPanel.getByText("FSUIPC")).toBeVisible()
      await expect(triggerPanel.getByText("66CC", { exact: true })).toBeVisible()
      await expect(
        triggerPanel.getByText("AABBCCDD", { exact: true }),
      ).toBeVisible()
      await expect(triggerPanel.getByText("True", { exact: true })).toBeVisible()
    })

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Source).toEqual({
      Type: "FsuipcSource",
      FSUIPC: {
        OffsetType: 0,
        Size: 4,
        Offset: 0x66cc,
        Mask: 0xaabbccdd,
        BcdMode: true,
      },
    })
  })
})

test.describe("Output Config Wizard - ProSim Source", () => {
  const publishProSimDataRefs = async (configListPage: ConfigListPage) => {
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
            CanWrite: false,
            DataType: "float",
            DataUnit: "feet",
          },
          "system.reset": {
            Name: "system.reset",
            Description: "System Reset",
            CanRead: false,
            CanWrite: true,
            DataType: "bool",
            DataUnit: "",
          },
        },
      },
    })
  }

  test("Summary shows the ProSim source of an existing config", async ({
    configListPage,
    page,
  }) => {
    await createOutputConfigWithSourceAndOpenWizard(
      configListPage,
      page,
      { Type: "ProSimSource", ProSimDataRef: { Path: "autopilot.altitude" } },
      { Sim: "msfs", Features: { FSUIPC: false, ProSim: true } },
    )

    const triggerPanel = page.getByTestId("trigger-panel")
    await expect(triggerPanel.getByText("ProSim", { exact: true })).toBeVisible()
    await expect(
      triggerPanel.getByText("autopilot.altitude", { exact: true }),
    ).toBeVisible()
    // Parameters are only used by input actions
    await expect(triggerPanel.getByText("Parameter (optional)")).not.toBeVisible()
  })

  test("ProSim source editor shows only readable DataRefs", async ({
    configListPage,
    page,
  }) => {
    await createEmptyOutputConfigAndOpenWizard(configListPage, page, {
      Sim: "msfs",
      Features: { FSUIPC: false, ProSim: true },
    })
    await publishProSimDataRefs(configListPage)
    await page
      .getByTestId("trigger-panel")
      .getByRole("button", { name: "Source", exact: true })
      .click()
    const sourceEditor = page.getByTestId("source-editor")
    await selectComboBoxOption(
      page,
      sourceEditor.getByTestId("source-type-combobox"),
      "ProSim",
    )

    await expect(sourceEditor.getByText("Step 2 - Select preset")).toBeVisible()
    await expect(sourceEditor.getByText("Aircraft Heading")).toBeVisible()
    await expect(sourceEditor.getByText("Autopilot Altitude")).toBeVisible()
    await expect(sourceEditor.getByText("System Reset")).not.toBeVisible()
  })

  test("Selecting a ProSim DataRef updates the path and is saved correctly", async ({
    configListPage,
    page,
  }) => {
    await createEmptyOutputConfigAndOpenWizard(configListPage, page, {
      Sim: "msfs",
      Features: { FSUIPC: false, ProSim: true },
    })
    await publishProSimDataRefs(configListPage)
    await page
      .getByTestId("trigger-panel")
      .getByRole("button", { name: "Source", exact: true })
      .click()
    const sourceEditor = page.getByTestId("source-editor")
    await selectComboBoxOption(
      page,
      sourceEditor.getByTestId("source-type-combobox"),
      "ProSim",
    )

    await sourceEditor.getByPlaceholder("Filter presets").fill("Altitude")
    await expect(sourceEditor.getByText("Aircraft Heading")).not.toBeVisible()
    // The editor only highlights the selected DataRef, so the path is
    // verified via the summary and the payload
    await sourceEditor.getByText("Autopilot Altitude").click()

    await closeDrawer(page)
    await expect(sourceEditor).not.toBeVisible()

    await expect(
      page
        .getByTestId("trigger-panel")
        .getByText("autopilot.altitude", { exact: true }),
    ).toBeVisible()

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Source).toEqual({
      Type: "ProSimSource",
      ProSimDataRef: { Path: "autopilot.altitude" },
    })
  })
})

test.describe("Output Config Wizard - Variable Source", () => {
  test("Summary shows the Variable source of an existing config", async ({
    configListPage,
    page,
  }) => {
    await createOutputConfigWithSourceAndOpenWizard(configListPage, page, {
      Type: "VariableSource",
      MobiFlightVariable: {
        Name: "MyOutputVar",
        TYPE: "number",
        Expression: "$ * 2",
        Number: 0,
        Text: "",
      },
    })

    const triggerPanel = page.getByTestId("trigger-panel")
    await expect(
      triggerPanel.getByText("MobiFlight - Variable", { exact: true }),
    ).toBeVisible()
    await expect(
      triggerPanel.getByText("MyOutputVar", { exact: true }),
    ).toBeVisible()
    await expect(triggerPanel.getByText("Number", { exact: true })).toBeVisible()
    // The expression is only used by input actions
    await expect(triggerPanel.getByText("Expression", { exact: true })).not.toBeVisible()
    await expect(
      triggerPanel.getByText("$ * 2", { exact: true }),
    ).not.toBeVisible()
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
    await expect(triggerPanel.getByText("Variable")).toBeVisible()
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
      actionPanel.getByRole("button", { name: "Output device", exact: true }),
    ).not.toBeVisible()
  })

  test("Config without device shows the add buttons", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.NoDevice)

    const actionPanel = page.getByTestId("action-panel")
    await expect(
      actionPanel.getByRole("button", { name: "Output device", exact: true }),
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
      actionPanel.getByRole("button", { name: "Output device", exact: true }),
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
    await openOutputDevicePanel(page)

    const deviceEditor = page.getByTestId("output-device-editor")
    await expect(deviceEditor).toBeVisible()

    await deviceEditor.getByRole("combobox").first().click()
    await expect(page.getByRole("option")).toHaveText([
      "ProtoBoard-v2",
      "Mega Output Board",
      "Arcaze v5.54",
    ])
  })

  test("Device types are based on the devices of the selected controller", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.NoDevice)
    await openOutputDevicePanel(page)

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
        "Output Shift Register",
        "Custom Device",
      ])
      await page.keyboard.press("Escape")
    })

    await test.step("Mega Output Board offers only Output", async () => {
      await selectComboBoxOption(page, controllerComboBox, "Mega Output Board")
      await deviceTypeComboBox.click()
      await expect(page.getByRole("option")).toHaveText(["Output"])
      await page.keyboard.press("Escape")
    })

    await test.step("Arcaze with display driver offers Output and 7-Segment", async () => {
      await selectComboBoxOption(page, controllerComboBox, "Arcaze v5.54")
      await deviceTypeComboBox.click()
      await expect(page.getByRole("option")).toHaveText([
        "Output",
        "7-Segment Display",
      ])
      await page.keyboard.press("Escape")
    })
  })
})

test.describe("Output Config Wizard - Device Test Panel", () => {
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

  test("Changing the config during a test restarts the test with the new settings", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)
    await page.getByTestId("action-panel").getByRole("button").last().click()

    const deviceEditor = page.getByTestId("output-device-editor")
    await deviceEditor.getByPlaceholder("Enter raw value").fill("42")
    await deviceEditor.getByPlaceholder("Enter raw value").blur()

    await configListPage.mobiFlightPage.trackCommand("CommandConfigContextMenu")
    await deviceEditor.getByRole("button", { name: "Test" }).click()
    await expect(deviceEditor.getByRole("button", { name: "Stop" })).toBeVisible()

    await deviceEditor.getByRole("checkbox", { name: "LED 1" }).click()

    await expect
      .poll(async () => {
        const commands =
          await configListPage.mobiFlightPage.getTrackedCommands()
        return commands?.map((command) => command.payload.action)
      })
      .toEqual(["testStart", "testStart"])

    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    const payload = commands?.pop()?.payload
    expect(payload.item.Device.Pin).toContain("LED 1")
    // the test value is kept
    expect(payload.item.TestValue).toMatchObject({ Float64: 42 })
  })

  test("Changing the config without a running test does not start a test", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Output)
    await page.getByTestId("action-panel").getByRole("button").last().click()

    const deviceEditor = page.getByTestId("output-device-editor")
    await configListPage.mobiFlightPage.trackCommand("CommandConfigContextMenu")
    await deviceEditor.getByRole("checkbox", { name: "LED 1" }).click()
    await expect(
      deviceEditor.getByRole("checkbox", { name: "LED 1" }),
    ).toBeChecked()

    // wait longer than the delay before a test is restarted
    await page.waitForTimeout(500)
    const commands = await configListPage.mobiFlightPage.getTrackedCommands()
    expect(commands ?? []).toHaveLength(0)
  })
})

test.describe("Output Config Wizard - Output Device", () => {
  test("Newly created Output device config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    await createEmptyOutputConfigAndOpenWizard(configListPage, page)
    const actionPanel = page.getByTestId("action-panel")
    await openOutputDevicePanel(page)

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
    await openOutputDevicePanel(page)

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
})

test.describe("Output Config Wizard - Servo Device", () => {
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
})

test.describe("Output Config Wizard - Stepper Device", () => {
  test("Existing Stepper config is displayed and saved correctly", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Stepper)

    const actionPanel = page.getByTestId("action-panel")
    await test.step("Summary shows the stepper settings", async () => {
      await expect(
        actionPanel.getByText("Stepper 1", { exact: true }),
      ).toBeVisible()
      await expect(actionPanel.getByText("1000 / 2040")).toBeVisible()
      await expect(actionPanel.getByText("Compass mode")).not.toBeVisible()
    })

    await actionPanel.getByRole("button").last().click()
    const deviceEditor = page.getByTestId("output-device-editor")
    await expect(deviceEditor).toBeVisible()

    await test.step("Details show the stepper settings", async () => {
      await expect(
        deviceEditor.getByRole("combobox").filter({ hasText: "Stepper 1" }),
      ).toBeVisible()
      await expect(deviceEditor.getByLabel("Input revolution")).toHaveValue(
        "1000",
      )
      await expect(deviceEditor.getByLabel("Output revolution")).toHaveValue(
        "2040",
      )
      await expect(deviceEditor.getByLabel("Test value")).toHaveValue("500")
      await expect(deviceEditor.getByLabel("Acceleration")).toHaveValue("800")
      await expect(deviceEditor.getByLabel("Speed")).toHaveValue("467")
      await expect(
        deviceEditor.getByRole("switch", { name: "Compass mode" }),
      ).not.toBeChecked()
    })

    await test.step("Change settings and enable compass mode", async () => {
      await deviceEditor.getByLabel("Input revolution").fill("360")
      await deviceEditor.getByLabel("Test value").fill("180")
      await deviceEditor.getByRole("switch", { name: "Compass mode" }).click()
    })

    await closeDrawer(page)
    await expect(deviceEditor).not.toBeVisible()

    await test.step("Summary shows the changed settings", async () => {
      await expect(actionPanel.getByText("360 / 2040")).toBeVisible()
      await expect(actionPanel.getByText("Compass mode")).toBeVisible()
    })

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Device).toEqual({
      Type: "Stepper",
      Name: "Stepper 1",
      Address: "Stepper 1",
      InputRev: 360,
      OutputRev: 2040,
      TestValue: 180,
      Acceleration: 800,
      Speed: 467,
      CompassMode: true,
    })
  })

  test("New Stepper uses the device profile as defaults", async ({
    configListPage,
    page,
  }) => {
    await createEmptyOutputConfigAndOpenWizard(configListPage, page)
    await openOutputDevicePanel(page)

    const deviceEditor = page.getByTestId("output-device-editor")
    await selectComboBoxOption(
      page,
      deviceEditor.getByRole("combobox").first(),
      "ProtoBoard-v2",
    )
    await selectComboBoxOption(
      page,
      deviceEditor.getByRole("combobox").nth(1),
      "Stepper",
    )

    await expect(deviceEditor.getByLabel("Input revolution")).toHaveValue(
      "1000",
    )
    await expect(deviceEditor.getByLabel("Output revolution")).toHaveValue(
      "2040",
    )
    await expect(deviceEditor.getByLabel("Test value")).toHaveValue("500")
    await expect(deviceEditor.getByLabel("Acceleration")).toHaveValue("800")
    await expect(deviceEditor.getByLabel("Speed")).toHaveValue("467")
  })

  test("Reset restores the value of the device profile", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.Stepper)
    const actionPanel = page.getByTestId("action-panel")
    await actionPanel.getByRole("button").last().click()

    const deviceEditor = page.getByTestId("output-device-editor")
    const resetButton = deviceEditor.getByRole("button", { name: "Reset" })

    // values match the device profile
    await expect(resetButton).toHaveCount(0)

    await deviceEditor.getByLabel("Speed").fill("300")
    await expect(resetButton).toHaveCount(1)

    await resetButton.click()
    await expect(deviceEditor.getByLabel("Speed")).toHaveValue("467")
    await expect(resetButton).toHaveCount(0)
  })

  test.describe("Calibration", () => {
    // Opens the device editor of the existing stepper config
    // and starts tracking the calibration commands
    const openStepperCalibration = async (
      configListPage: ConfigListPage,
      page: Page,
    ) => {
      await openWizardForRow(configListPage, page, ROW.Stepper)
      await page.getByTestId("action-panel").getByRole("button").last().click()
      const deviceEditor = page.getByTestId("output-device-editor")
      await expect(deviceEditor).toBeVisible()
      await configListPage.mobiFlightPage.trackCommand(
        "CommandStepperCalibration",
      )
      return deviceEditor
    }

    // Waits until the expected number of commands has been sent,
    // because they are tracked asynchronously
    const getCalibrationPayloads = async (
      configListPage: ConfigListPage,
      expectedCount: number,
    ) => {
      const getCommands = async () =>
        (await configListPage.mobiFlightPage.getTrackedCommands()) ?? []
      await expect.poll(async () => (await getCommands()).length).toBe(
        expectedCount,
      )
      const commands = await getCommands()
      await configListPage.mobiFlightPage.clearTrackedCommands()
      return commands.map((command) => command.payload)
    }

    const expectedTarget = {
      controller: { Name: "ProtoBoard-v2", Serial: "SN-5FC-1CF" },
      stepper: { Type: "Stepper", Address: "Stepper 1" },
    }

    test("Calibration section shows the jog buttons and the hint", async ({
      configListPage,
      page,
    }) => {
      const deviceEditor = await openStepperCalibration(configListPage, page)

      await expect(
        deviceEditor.getByText("Step 3 - Calibrate stepper"),
      ).toBeVisible()
      for (const step of ["-50", "-10", "-1", "+1", "+10", "+50"]) {
        await expect(
          deviceEditor.getByRole("button", { name: step, exact: true }),
        ).toBeVisible()
      }
      await expect(
        deviceEditor.getByRole("button", { name: "Set home position" }),
      ).toBeVisible()
      await expect(
        deviceEditor.getByText(
          "Use the arrow keys to select the step size. Use space to move the stepper. Use enter to set the home position.",
        ),
      ).toBeVisible()
    })

    test("Jog buttons move the stepper and set home resets it", async ({
      configListPage,
      page,
    }) => {
      const deviceEditor = await openStepperCalibration(configListPage, page)

      await test.step("Jog buttons send a move with their step count", async () => {
        await deviceEditor
          .getByRole("button", { name: "-10", exact: true })
          .click()
        await deviceEditor
          .getByRole("button", { name: "+50", exact: true })
          .click()

        const payloads = await getCalibrationPayloads(configListPage, 2)
        expect(payloads).toHaveLength(2)
        expect(payloads[0]).toMatchObject({
          ...expectedTarget,
          action: "move",
          steps: -10,
        })
        expect(payloads[1]).toMatchObject({
          ...expectedTarget,
          action: "move",
          steps: 50,
        })
      })

      await test.step("Set home position sends home", async () => {
        await deviceEditor
          .getByRole("button", { name: "Set home position" })
          .click()

        const payloads = await getCalibrationPayloads(configListPage, 1)
        expect(payloads).toHaveLength(1)
        expect(payloads[0]).toMatchObject({ ...expectedTarget, action: "home" })
      })
    })

    test("Keyboard selects the step size, moves and sets home", async ({
      configListPage,
      page,
    }) => {
      const deviceEditor = await openStepperCalibration(configListPage, page)
      const minusOne = deviceEditor.getByRole("button", {
        name: "-1",
        exact: true,
      })
      const plusOne = deviceEditor.getByRole("button", {
        name: "+1",
        exact: true,
      })

      await test.step("Arrow keys move the focus between the step sizes", async () => {
        await minusOne.focus()
        await page.keyboard.press("ArrowRight")
        await expect(plusOne).toBeFocused()

        // focusing a button does not move the stepper
        expect(await getCalibrationPayloads(configListPage, 0)).toHaveLength(0)
      })

      await test.step("Space moves the stepper exactly once per press", async () => {
        await page.keyboard.press("Space")
        await page.keyboard.press("Space")

        const payloads = await getCalibrationPayloads(configListPage, 2)
        expect(payloads).toHaveLength(2)
        for (const payload of payloads) {
          expect(payload).toMatchObject({
            ...expectedTarget,
            action: "move",
            steps: 1,
          })
        }
      })

      await test.step("Enter sets the home position", async () => {
        await page.keyboard.press("Enter")

        const payloads = await getCalibrationPayloads(configListPage, 1)
        expect(payloads).toHaveLength(1)
        expect(payloads[0]).toMatchObject({ ...expectedTarget, action: "home" })
      })
    })
  })
})

test.describe("Output Config Wizard - 7-Segment Device", () => {
  test("Existing 7-Segment config is displayed and saved correctly", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.SevenSegment)

    const actionPanel = page.getByTestId("action-panel")
    await test.step("Summary shows the 7-segment settings", async () => {
      await expect(
        actionPanel.getByText("7-Segment", { exact: true }).last(),
      ).toBeVisible()
      await expect(actionPanel.getByText("Padding with")).not.toBeVisible()
      await expect(actionPanel.getByText("Reverse digits")).not.toBeVisible()
    })

    await actionPanel.getByRole("button").last().click()
    const deviceEditor = page.getByTestId("output-device-editor")
    await expect(deviceEditor).toBeVisible()

    const paddingCheckbox = deviceEditor.getByRole("checkbox", {
      name: "Padding with",
    })
    const reverseDigitsCheckbox = deviceEditor.getByRole("checkbox", {
      name: "Reverse digits",
    })

    await test.step("Details show the 7-segment settings", async () => {
      await expect(
        deviceEditor.getByRole("combobox").filter({ hasText: /^7-Segment$/ }),
      ).toBeVisible()
      await expect(paddingCheckbox).not.toBeChecked()
      await expect(reverseDigitsCheckbox).not.toBeChecked()
    })

    await test.step("Enable padding with space and reverse digits", async () => {
      await paddingCheckbox.click()
      await selectComboBoxOption(
        page,
        deviceEditor.getByRole("combobox").filter({ hasText: /^0$/ }),
        "Space",
      )
      await reverseDigitsCheckbox.click()
      await expect(paddingCheckbox).toBeChecked()
      await expect(reverseDigitsCheckbox).toBeChecked()
    })

    await closeDrawer(page)
    await expect(deviceEditor).not.toBeVisible()

    await test.step("Summary shows the enabled options", async () => {
      await expect(actionPanel.getByText("Padding with")).toBeVisible()
      await expect(actionPanel.getByText("Reverse digits")).toBeVisible()
    })

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Device).toMatchObject({
      Type: "LedModule",
      Name: "7-Segment",
      DisplayLedAddress: "7-Segment",
      DisplayLedPadding: true,
      DisplayLedPaddingChar: " ",
      DisplayLedReverseDigits: true,
    })
  })

  test("Brightness can be controlled by another output config", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.SevenSegment)

    const actionPanel = page.getByTestId("action-panel")
    await actionPanel.getByRole("button").last().click()
    const deviceEditor = page.getByTestId("output-device-editor")
    await expect(deviceEditor).toBeVisible()

    const brightnessComboBox = deviceEditor
      .getByRole("combobox")
      .filter({ hasText: /^None$/ })

    await test.step("Other output configs are offered as brightness source", async () => {
      await brightnessComboBox.click()
      await expect(
        page.getByRole("option", { name: "LED 1", exact: true }),
      ).toBeVisible()
      // a config can't control its own brightness
      await expect(
        page.getByRole("option", { name: "7-Segment", exact: true }),
      ).not.toBeVisible()
      // input configs don't provide a value
      await expect(
        page.getByRole("option", { name: "Button", exact: true }),
      ).not.toBeVisible()
      await page.getByRole("option", { name: "LED 1", exact: true }).click()
      await expect(
        deviceEditor.getByRole("combobox").filter({ hasText: /^LED 1$/ }),
      ).toBeVisible()
    })

    await closeDrawer(page)
    await expect(deviceEditor).not.toBeVisible()

    await test.step("Summary shows the brightness source", async () => {
      await expect(actionPanel.getByText("Brightness: LED 1")).toBeVisible()
    })

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Device).toMatchObject({
      Type: "LedModule",
      DisplayLedBrightnessReference: "f79f75dd-0d45-4c91-870d-599cfc75ede1",
    })
  })

  test("Brightness source can be removed again", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.SevenSegment)

    const actionPanel = page.getByTestId("action-panel")
    await actionPanel.getByRole("button").last().click()
    const deviceEditor = page.getByTestId("output-device-editor")

    await selectComboBoxOption(
      page,
      deviceEditor.getByRole("combobox").filter({ hasText: /^None$/ }),
      "LED 1",
    )
    await selectComboBoxOption(
      page,
      deviceEditor.getByRole("combobox").filter({ hasText: /^LED 1$/ }),
      "None",
    )

    await closeDrawer(page)
    await expect(actionPanel.getByText("Brightness: LED 1")).not.toBeVisible()

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Device).toMatchObject({
      Type: "LedModule",
      DisplayLedBrightnessReference: "",
    })
  })
})

test.describe("Output Config Wizard - Arcaze", () => {
  // Opens the device editor of a new config
  // and selects the given device type of the Arcaze
  const selectArcazeDevice = async (
    configListPage: ConfigListPage,
    page: Page,
    deviceType: string,
  ) => {
    await openWizardForRow(configListPage, page, ROW.NoDevice)
    await openOutputDevicePanel(page)

    const deviceEditor = page.getByTestId("output-device-editor")
    await selectComboBoxOption(
      page,
      deviceEditor.getByRole("combobox").first(),
      "Arcaze v5.54",
    )
    await selectComboBoxOption(
      page,
      deviceEditor.getByRole("combobox").nth(1),
      deviceType,
    )
    return deviceEditor
  }

  test("Outputs are the pins of the Arcaze ports", async ({
    configListPage,
    page,
  }) => {
    const deviceEditor = await selectArcazeDevice(configListPage, page, "Output")

    await expect(deviceEditor.getByRole("checkbox")).toHaveCount(40)
    await expect(
      deviceEditor.getByRole("checkbox", { name: "A01", exact: true }),
    ).toBeVisible()
    await expect(
      deviceEditor.getByRole("checkbox", { name: "B20", exact: true }),
    ).toBeVisible()
  })

  test("Displays are the address and connector of the display driver", async ({
    configListPage,
    page,
  }) => {
    const deviceEditor = await selectArcazeDevice(
      configListPage,
      page,
      "7-Segment Display",
    )

    const displayComboBox = deviceEditor
      .getByRole("combobox")
      .filter({ hasText: "0:1" })
    await expect(displayComboBox).toBeVisible()
    await displayComboBox.click()
    await expect(page.getByRole("option")).toHaveText(["0:1", "0:2"])
  })
})

test.describe("Output Config Wizard - Shift Register Device", () => {
  // Opens the device editor of a new config
  // and selects the shift register device type of ProtoBoard-v2
  const selectShiftRegisterDevice = async (
    configListPage: ConfigListPage,
    page: Page,
  ) => {
    await openWizardForRow(configListPage, page, ROW.NoDevice)
    await openOutputDevicePanel(page)

    const deviceEditor = page.getByTestId("output-device-editor")
    await selectComboBoxOption(
      page,
      deviceEditor.getByRole("combobox").first(),
      "ProtoBoard-v2",
    )
    await selectComboBoxOption(
      page,
      deviceEditor.getByRole("combobox").nth(1),
      "Output Shift Register",
    )
    return deviceEditor
  }

  test("Outputs are based on the pins of the selected shift register", async ({
    configListPage,
    page,
  }) => {
    const deviceEditor = await selectShiftRegisterDevice(configListPage, page)
    const outputs = deviceEditor.getByRole("checkbox")

    await test.step("First shift register and its first output are preselected", async () => {
      await expect(
        deviceEditor.getByRole("combobox").filter({ hasText: "ShiftRegister 1" }),
      ).toBeVisible()
      await expect(outputs).toHaveCount(8)
      await expect(
        deviceEditor.getByRole("checkbox", { name: "Output 0", exact: true }),
      ).toBeChecked()
    })

    await test.step("Second shift register offers 16 outputs", async () => {
      await selectComboBoxOption(
        page,
        deviceEditor.getByRole("combobox").filter({ hasText: "ShiftRegister 1" }),
        "ShiftRegister 2",
      )
      await expect(outputs).toHaveCount(16)
    })
  })

  test("Selected outputs are shown in the summary and saved correctly", async ({
    configListPage,
    page,
  }) => {
    const deviceEditor = await selectShiftRegisterDevice(configListPage, page)

    await test.step("Changing the shift register resets the outputs", async () => {
      await deviceEditor
        .getByRole("checkbox", { name: "Output 1", exact: true })
        .click()
      await selectComboBoxOption(
        page,
        deviceEditor.getByRole("combobox").filter({ hasText: "ShiftRegister 1" }),
        "ShiftRegister 2",
      )
      await expect(
        deviceEditor.getByRole("checkbox", { name: "Output 1", exact: true }),
      ).not.toBeChecked()
    })

    await test.step("Select outputs", async () => {
      await deviceEditor
        .getByRole("checkbox", { name: "Output 0", exact: true })
        .click()
      await deviceEditor
        .getByRole("checkbox", { name: "Output 12", exact: true })
        .click()
    })

    await closeDrawer(page)
    await expect(deviceEditor).not.toBeVisible()

    const actionPanel = page.getByTestId("action-panel")
    await test.step("Summary shows the shift register and its outputs", async () => {
      await expect(actionPanel.getByText("Output Shift Register")).toBeVisible()
      await expect(
        actionPanel.getByText("ShiftRegister 2", { exact: true }),
      ).toBeVisible()
      await expect(actionPanel.getByText("Output 0", { exact: true })).toBeVisible()
      await expect(
        actionPanel.getByText("Output 12", { exact: true }),
      ).toBeVisible()
    })

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Device).toEqual({
      Type: "ShiftRegister",
      Name: "ShiftRegister 2",
      Address: "ShiftRegister 2",
      Pin: "Output 0|Output 12",
    })
  })
})

test.describe("Output Config Wizard - LCD Display Device", () => {
  test("Existing LCD Display config is displayed and saved correctly", async ({
    configListPage,
    page,
  }) => {
    await openWizardForRow(configListPage, page, ROW.LcdDisplay)

    const actionPanel = page.getByTestId("action-panel")
    await test.step("Summary shows the LCD display and its line count", async () => {
      await expect(actionPanel.getByText("LCD 1", { exact: true })).toBeVisible()
      await expect(actionPanel.getByText("4", { exact: true })).toBeVisible()
    })

    await actionPanel.getByRole("button").last().click()
    const deviceEditor = page.getByTestId("output-device-editor")
    await expect(deviceEditor).toBeVisible()

    // One input per display row, limited to the number of display columns
    const lineInputs = deviceEditor.locator('input[maxlength="16"]')

    await test.step("Details show one line per display row", async () => {
      await expect(
        deviceEditor.getByRole("combobox").filter({ hasText: /^LCD 1$/ }),
      ).toBeVisible()
      await expect(lineInputs).toHaveCount(4)
      await expect(lineInputs.nth(0)).toHaveValue("1234567890123456")
      await expect(lineInputs.nth(1)).toHaveValue("******Test******")
    })

    await test.step("Change the second line", async () => {
      await lineInputs.nth(1).fill("Hello MobiFlight")
      await expect(lineInputs.nth(1)).toHaveValue("Hello MobiFlight")
    })

    await closeDrawer(page)
    await expect(deviceEditor).not.toBeVisible()

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Device).toMatchObject({
      Type: "LcdDisplay",
      Name: "LCD 1",
      Address: "LCD 1",
      Lines: [
        "1234567890123456",
        "Hello MobiFlight",
        "1234567890123456",
        "******Test******",
      ],
    })
  })
})

test.describe("Output Config Wizard - Custom Device", () => {
  test("Newly created Custom Device config values are saved correctly", async ({
    configListPage,
    page,
  }) => {
    await createEmptyOutputConfigAndOpenWizard(configListPage, page)
    const actionPanel = page.getByTestId("action-panel")
    await openOutputDevicePanel(page)

    const deviceEditor = page.getByTestId("output-device-editor")
    await selectComboBoxOption(
      page,
      deviceEditor.getByRole("combobox").first(),
      "ProtoBoard-v2",
    )
    await selectComboBoxOption(
      page,
      deviceEditor.getByRole("combobox").nth(1),
      "Custom Device",
    )

    const messageTypeComboBox = deviceEditor
      .getByRole("combobox")
      .filter({ hasText: /^Custom message 1$/ })

    await test.step("First device and message type are selected by default", async () => {
      await expect(
        deviceEditor.getByRole("combobox").filter({ hasText: /^Custom Display$/ }),
      ).toBeVisible()
      await expect(messageTypeComboBox).toBeVisible()
      await expect(
        deviceEditor.getByText("$ will be displayed as custom 1 value"),
      ).toBeVisible()
    })

    await test.step("Select message type and enter value", async () => {
      await selectComboBoxOption(page, messageTypeComboBox, "Custom message 2")
      await expect(
        deviceEditor.getByText("$ will be displayed as custom 2 value"),
      ).toBeVisible()
      await deviceEditor.getByLabel("Value").fill("$ * 2")
    })

    await closeDrawer(page)
    await expect(deviceEditor).not.toBeVisible()

    await test.step("Summary shows the selected device", async () => {
      await expect(actionPanel.getByText("ProtoBoard-v2")).toBeVisible()
      await expect(
        actionPanel.getByText("Custom Display", { exact: true }),
      ).toBeVisible()
    })

    const payload = await applyChangesAndReturnPayload(configListPage, page)
    expect(payload.item.Device).toMatchObject({
      Type: "CustomDevice",
      Name: "Custom Display",
      CustomName: "Custom Display",
      MessageType: 1,
      Value: "$ * 2",
    })
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
      actionPanel.getByRole("button", { name: "Output device", exact: true }),
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
      actionPanel.getByRole("button", { name: "Output device", exact: true }),
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
      actionPanel.getByRole("button", { name: "Output device", exact: true }),
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
