import { test, expect } from "./fixtures"
import { IConfigItem } from "../src/types/index"

test("Confirm empty list view", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithEmptyData()
  const noConfigsMessage = page.getByText("This is a new configuration. Please add some items.")
  await expect(noConfigsMessage).toBeVisible()

  const addOutputConfigButton = page.getByRole("button", { name: "Add Output Config" })
  const addInputConfigButton = page.getByRole("button", { name: "Add Input Config" })

  await expect(addOutputConfigButton).toBeVisible()
  await expect(addInputConfigButton).toBeVisible()

  // the filter toolbar is not yet visible because we don't have any items
  await expect(
    page.getByRole("textbox", { name: "Filter items..." }),
  ).not.toBeVisible()
})

test("Confirm populated list view", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await expect(
    page.getByRole("cell", { name: "No results." }),
  ).not.toBeVisible()
})

test("Confirm active toggle is working", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await configListPage.setupConfigItemEditConfirmationResponse()  
  const firstRow = page.locator("tbody tr").nth(1)
  const toggleSwitch = firstRow.getByRole("switch")
  await toggleSwitch.click()
  await expect(toggleSwitch).not.toBeChecked()
  await toggleSwitch.click()
  await expect(toggleSwitch).toBeChecked()
})

test("Confirm edit function for name is working", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await configListPage.setupConfigItemEditConfirmationResponse()
  await configListPage.mobiFlightPage.trackCommand("CommandUpdateConfigItem")

  const nameCell = page.getByRole("cell", { name: "LED 1" })
  
  // Click on the text span to enter edit mode
  await nameCell.getByText("LED 1").nth(1).click()
  
  // Now find the textbox that appears after clicking
  const inlineEdit = nameCell.getByRole("textbox")
  await inlineEdit.fill("LED 1245")

  // We confirm the change by pressing Enter
  await page.keyboard.press("Enter")

  // The value should now be updated
  await expect(page.getByRole("cell", { name: "LED 1245" })).toBeVisible()

  // verify that the correct command was sent to the backend
  let postedCommands =
    await configListPage.mobiFlightPage.getTrackedCommands()
  const lastCommand = postedCommands!.pop()
  expect(lastCommand.key).toEqual("CommandUpdateConfigItem")
  expect((lastCommand.payload.item as IConfigItem).Name).toEqual("LED 1245")

  // Click on the text span to enter edit mode
  await nameCell.getByText("LED 1245").nth(1).click()
  await inlineEdit.fill("LED 9999")
  
  // We cancel the change by pressing Escape
  await page.keyboard.press("Escape")
  await expect(page.getByRole("cell", { name: "LED 1245" })).toBeVisible()

  postedCommands = await configListPage.mobiFlightPage.getTrackedCommands()
  expect(postedCommands?.length).toEqual(1)  
})

test("Confirm status icons working", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()

  const PreconditionIcon = configListPage.getStatusIconInRow("Precondition", 1)
  const TestIcon = configListPage.getStatusIconInRow("Test", 1)
  const ConfigRefIcon = configListPage.getStatusIconInRow("ConfigRef", 1)

  const SourceIcon = configListPage.getStatusIconInRow("Source", 1)
  const DeviceIcon = configListPage.getStatusIconInRow("Device", 1)
  const ModifierIcon = configListPage.getStatusIconInRow("Modifier", 1)

  const statusTests = [
    {
      status: { Precondition: "not satisfied" },
      icon: PreconditionIcon,
      toolTipText: "Precondition is not satisfied.",
      alwaysVisible: true,
    },
    {
      status: { Source: "SIMCONNECT_NOT_AVAILABLE" },
      icon: SourceIcon,
      toolTipText: "This config uses SimConnect,",
      alwaysVisible: false,
    },
    {
      status: { Source: "FSUIPC_NOT_AVAILABLE" },
      icon: SourceIcon,
      toolTipText: "This config uses FSUIPC,",
      alwaysVisible: false,
    },
    {
      status: { Source: "XPLANE_NOT_AVAILABLE" },
      icon: SourceIcon,
      toolTipText: "This config uses X-Plane,",
      alwaysVisible: false,
    },
    {
      status: { Source: "PROSIM_NOT_AVAILABLE" },
      icon: SourceIcon,
      toolTipText: "This config uses ProSim,",
      alwaysVisible: false,
    },
    {
      status: { Device: "NotConnected" },
      icon: DeviceIcon,
      toolTipText: "The device used in this config is not connected.",
      alwaysVisible: false,
    },
    {
      status: { Modifier: "Error" },
      icon: ModifierIcon,
      toolTipText: "A modifier is applied which has an error.",
      alwaysVisible: false,
    },
    {
      status: { Test: "Executing" },
      icon: TestIcon,
      toolTipText: "This config is currently being tested.",
      alwaysVisible: true,
    },
    {
      status: { ConfigRef: "Missing" },
      icon: ConfigRefIcon,
      toolTipText: "One or more referenced configs are missing.",
      alwaysVisible: true,
    },
  ]

  for (const test of statusTests) {
    if (test.alwaysVisible) {
      await expect(test.icon).toBeVisible()
      await expect(test.icon).toHaveAttribute("aria-disabled", "true")
    } else {
      await expect(test.icon).not.toBeVisible()
    }

    await configListPage.updateConfigItemStatus(0, test.status)
    await expect(test.icon).toBeVisible()
    await expect(test.icon).toHaveAttribute("aria-disabled", "false")
    await test.icon.hover()
    await expect(
      configListPage.mobiFlightPage.getTooltipByText(test.toolTipText),
    ).toBeVisible()
    await page.click("body")
    await page.waitForTimeout(500)

    const statusKey = Object.keys(test.status)[0];
    await configListPage.removeConfigItemStatus(0, statusKey);
  }
})

test("Confirm single drag n drop is working", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await configListPage.mobiFlightPage.trackCommand("CommandResortConfigItem")
  await page.getByRole("row").nth(1).getByRole("button").first().hover()
  await page.mouse.down()
  await page
    .getByRole("row", { name: "ShiftRegister" })
    .getByRole("button")
    .first()
    .hover()
  await page.mouse.up()
  await expect(page.getByRole("row").nth(6)).toContainText("7-Segment")

  const postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  const lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandResortConfigItem')
  expect (lastCommand.payload.items.length).toEqual(1)
  expect (lastCommand.payload.items[0].Name).toEqual("7-Segment")
  expect (lastCommand.payload.newIndex).toEqual(5)
})

test("Confirm multi drag n drop is working", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await configListPage.mobiFlightPage.trackCommand("CommandResortConfigItem")

  const firstRow = page.getByRole("row").nth(1)
  const thirdRow = page.getByRole("row").nth(3)
  const fifthRow = page.getByRole("row").nth(5)
  
  // select the first row
  await firstRow.click()
  await page.keyboard.down("Control")
  // add the third row to the selection
  await thirdRow.click()
    
  // activate drag and drop after fifth
  const dragHandle = thirdRow.getByRole("button").first()
  await dragHandle.hover()
  await page.mouse.down()
  await fifthRow.getByRole("button").first().hover()
  await page.mouse.up()

  await expect(page.getByRole("row").nth(4)).toContainText("7-Segment")
  await expect(page.getByRole("row").nth(5)).toContainText("Servo")

  const postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  const lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandResortConfigItem')
  expect (lastCommand.payload.items.length).toEqual(2)
  expect (lastCommand.payload.items[0].Name).toEqual("7-Segment")
  expect (lastCommand.payload.items[1].Name).toEqual("Servo")
  expect (lastCommand.payload.newIndex).toEqual(3)
})

test("Confirm dark mode is working", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await expect(page.locator("html")).toHaveAttribute("class", "light")
  await page.getByRole("button", { name: "Toggle dark mode" }).click()
  await expect(page.locator("html")).toHaveAttribute("class", "dark")
  await page.getByRole("button", { name: "Toggle light mode" }).click()
  await expect(page.locator("html")).toHaveAttribute("class", "light")
})

test("Confirm add output config is working", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.trackCommand("CommandAddConfigItem")

  const addOutputConfigButton = page.getByRole("button", { name: "Add Output Config" })
  await addOutputConfigButton.click()

  const postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  const lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandAddConfigItem')
  expect (lastCommand.payload.type).toEqual('OutputConfig')
})

test("Confirm add input config is working", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.trackCommand("CommandAddConfigItem")

  const addOutputConfigButton = page.getByRole("button", { name: "Add Input Config" })
  await addOutputConfigButton.click()

  const postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  const lastCommand = postedCommands!.pop()
  expect (lastCommand.key).toEqual('CommandAddConfigItem')
  expect (lastCommand.payload.type).toEqual('InputConfig')
})

test.describe('Filter toolbar tests', () => {
  test("Confirm `Search text` filter toolbar is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()

    const searchTextBox = page.getByRole("textbox", { name: "Filter items" })
    const rows = page.locator("tbody tr")
    await expect(rows).toHaveCount(14)

    await searchTextBox.fill("A")
    await expect(rows).toHaveCount(2)

    await searchTextBox.fill("Ana")
    await expect(rows).toHaveCount(1)

    await searchTextBox.fill("Anaz")
    await expect(rows).toHaveCount(0)

    const clearButton = page.getByRole("button", { name: "Reset filters" })
    await expect(clearButton).not.toHaveCount(0)

    await clearButton.first().click()
    await expect(rows).toHaveCount(14)

    await configListPage.mobiFlightPage.trackCommand("CommandConfigBulkAction")
    await rows.first().click()
    await searchTextBox.press("Backspace")
    const postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
    await expect(postedCommands?.length).toBeUndefined()
  })

  test("Confirm `Config Type` filter toolbar is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()

    const configTypeFilterButton = page.getByRole("button", {
      name: "Config Type",
    })
    const outputOption = page
      .getByRole("option", { name: "Output" })
      .locator("div")
    const inputOption = page.getByRole("option", { name: "Input" }).locator("div")
    const visibleRows = page.locator("tbody tr")
    const clearFiltersOption = page.getByRole("option", { name: "Clear filters" })

    await configTypeFilterButton.click()
    await outputOption.click()
    await expect(visibleRows).toHaveCount(7)

    await inputOption.click()
    await outputOption.click()
    await expect(visibleRows).toHaveCount(7)

    await clearFiltersOption.click()
    await expect(visibleRows).toHaveCount(14)

    const inputField = page.getByPlaceholder("Config Type")
    await inputField.click()
    await inputField.fill("In")
    await inputField.press("Enter")
    await expect(visibleRows).toHaveCount(7)
    await page.locator("#root").click()
    await page.waitForTimeout(500)

    await configTypeFilterButton.click()
    await clearFiltersOption.click()
    await inputField.click()
    await inputField.fill("Out")
    await inputField.press("Enter")
    await expect(visibleRows).toHaveCount(7)
  })

  test("Confirm `Controller` filter toolbar is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    const rows = page.locator("tbody tr")
    const clearFilterOption = page.getByRole("option", { name: "Clear filters" })

    await page.getByRole("button", { name: "Controller" }).click()
    await page
      .getByRole("option", { name: "WINWING Orion Joystick Base 2" })
      .locator("div")
      .click()
    await expect(rows).toHaveCount(1)
    await clearFilterOption.click()
    await expect(rows).toHaveCount(14)

    await page.getByRole("option", { name: "ProtoBoard" }).locator("div").click()
    await expect(rows).toHaveCount(8)
    await clearFilterOption.click()

    await page
      .getByRole("option", { name: "not set" })
      .locator("div")
      .first()
      .click()
    await expect(rows).toHaveCount(1)
    await clearFilterOption.click()

    await page.getByPlaceholder("Controller").click()
    await page.getByPlaceholder("Controller").fill("Proto")
    await page.getByPlaceholder("Controller").press("Enter")
    await expect(rows).toHaveCount(8)
  })

  test("Confirm `Devices` filter toolbar is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    const rows = page.locator("tbody tr")
    const clearFilterOption = page.getByRole("option", { name: "Clear filters" })

    await page.getByRole("button", { name: "Devices" }).click()
    
    // Inline function to test device type filters
    const testDeviceTypeFilter = async (deviceType: string, expectedCount: number) => {
      await page.getByRole("option", { name: deviceType }).first().click()
      await expect(rows).toHaveCount(expectedCount)
      await clearFilterOption.click()
      await expect(rows).toHaveCount(14)
    }

    const deviceTypes = [
      ["Analog Input", 2],
      ["Encoder", 2],
      ["Button", 3],
      ["Output", 1],
      ["Output Shift Register", 1],
      ["LCD Display", 1],
      ["Servo", 1],
      ["Stepper", 1],
      ["7-Segment", 1],
      ["not set", 1],
    ] as Array<[string, number]>

    for (const [deviceTypeName, expectedCount] of deviceTypes) {
      await testDeviceTypeFilter(deviceTypeName, expectedCount)
    }

  })

  test("Confirm `Names` filter toolbar is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    const rows = page.locator("tbody tr")
    const clearFilterOption = page.getByRole("option", { name: "Clear filters" })

    await page.getByRole("button", { name: "Names" }).click()

    // Inline function to test device type filters
    const testDeviceNameFilter = async (deviceType: string, expectedCount: number) => {
      await page.getByRole("option", { name: deviceType }).first().click()
      await expect(rows).toHaveCount(expectedCount)
      await clearFilterOption.click()
      await expect(rows).toHaveCount(14)
    }

    const deviceNames = [
      "POT 1",
      "Button 4",
      "ShiftRegister 1",
      "LED 2",
      "LCD 1",
      "Servo 1",
      "Stepper 1",
      "7-Segment",
      "not set",
    ]

    for (const deviceName of deviceNames) {
      await testDeviceNameFilter(deviceName, 1)
    }
  })

  test("Confirm filter toolbar reset is working on Project change", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    
    const searchTextBox = page.getByRole("textbox", { name: "Filter items" })
    await searchTextBox.fill("Test")
    await expect(searchTextBox).toHaveValue("Test")

    await configListPage.initWithTestDataAndSpecificProjectName("Specific Project")

    await expect(searchTextBox).toHaveValue("")
  })
})

test.describe('Controller device labels are displayed correctly', () => {
  test("Confirm Joystick device labels are displayed correctly", async ({
    configListPage,
    page
  }) => {
    await configListPage.gotoPage()
    await configListPage.initControllerDefinitions()
    await configListPage.initWithTestData()
    await expect(page.getByRole("cell", { name: "Line-Select-Key 1L" })).toBeVisible()
  })

  test("Confirm Midi controller device labels are displayed correctly", async ({
    configListPage,
    page
  }) => {
    await configListPage.gotoPage()
    await configListPage.initControllerDefinitions()
    await configListPage.initWithTestData()

    const midiButtonRow = page.getByRole("row").filter({ hasText: "Intech Studio: AC" }).first()
    await expect(midiButtonRow.getByRole("cell", { name: "Button 1" })).toBeVisible()

    const midiKnobRow = page.getByRole("row").filter({ hasText: "Intech Studio: AC" }).nth(1)
    await expect(midiKnobRow.getByRole("cell", { name: "Knob 1" })).toBeVisible()

    const midiSliderRow = page.getByRole("row").filter({ hasText: "Intech Studio: AC" }).nth(2)
    await expect(midiSliderRow.getByRole("cell", { name: "Slider 1" })).toBeVisible()
  })
})

test("Confirm `Controller Settings` link is working", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await configListPage.mobiFlightPage.trackCommand("CommandConfigContextMenu")

  const controllerSettingsLabel = page.getByRole('row').nth(1).getByText('ProtoBoard')
  const controllerSettingsButton = page.getByRole("link").first()
  await expect (controllerSettingsButton).toHaveCSS('opacity', '0')
  await controllerSettingsLabel.hover()
  await expect (controllerSettingsButton).toHaveCSS('opacity', '1')
  await controllerSettingsButton.click()
  const postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
  expect (postedCommands!.pop().key).toEqual('CommandConfigContextMenu')
})

test("Confirm Raw and Final Value update correctly", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()

  const rawValue = page.getByRole('row').nth(1).getByTitle("RawValue")	
  const finalValue = page.getByRole('row').nth(1).getByTitle("Value", { exact: true })

  await configListPage.updateConfigItemRawAndFinalValue(0, "1234", "5678")
  await expect(rawValue).toHaveText("1234")
  await expect(finalValue).toHaveText("5678")

  await configListPage.updateConfigItemRawAndFinalValue(0, "4321", "8765")
  await expect(rawValue).toHaveText("4321")
  await expect(finalValue).toHaveText("8765")
})

test.describe('Responsiveness: Full Screen', () => {
  test.use({ viewport: { width: 1920, height: 1080 } });
  test("Confirm `Columns are visible` in full screen", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    const activeColumn = page.getByRole("cell", { name: "Active" }).first() 
    const nameColumn = page.getByRole("cell", { name: "Name" }).first() 
    const controllerColumn = page.getByRole("cell", { name: "Controller" }).first() 
    const deviceColumn = page.getByRole("cell", { name: "Device" }).first() 
    const statusColumn = page.getByRole("cell", { name: "Status" }).first() 
    const rawValueColumn = page.getByRole("cell", { name: "Raw Value" }).first() 
    const finalValueColumn = page.getByRole("cell", { name: "Final Value" }).first() 
    const actionsColumn = page.getByRole("cell", { name: "Actions" }).first() 

    await expect(activeColumn).toBeVisible()
    await expect(nameColumn).toBeVisible()
    await expect(controllerColumn).toBeVisible()
    await expect(deviceColumn).toBeVisible()
    await expect(statusColumn).toBeVisible()
    await expect(rawValueColumn).toBeVisible()
    await expect(finalValueColumn).toBeVisible()
    await expect(actionsColumn).toBeVisible()
  })
})

test.describe('Responsiveness: Medium Screen', () => {
  test.use({ viewport: { width: 800, height: 600 } });
  test("Confirm `Columns are visible` in full screen", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    const activeColumn = page.getByRole("cell", { name: "Active" }).first() 
    const nameColumn = page.getByRole("cell", { name: "Name" }).first() 
    const controllerColumn = page.getByRole("cell", { name: "Controller" }).first() 
    const deviceColumn = page.getByRole("cell", { name: "Device" }).first() 
    const statusColumn = page.getByRole("cell", { name: "Status" }).first() 
    const rawValueColumn = page.getByRole("cell", { name: "Raw Value" }).first() 
    const finalValueColumn = page.getByRole("cell", { name: "Final Value" }).first() 
    const actionsColumn = page.getByRole("cell", { name: "Actions" }).first() 

    await expect(activeColumn).toBeVisible()
    await expect(nameColumn).toBeVisible()
    await expect(controllerColumn).not.toBeVisible()
    await expect(deviceColumn).toBeVisible()
    await expect(statusColumn).toBeVisible()
    await expect(rawValueColumn).toBeVisible()
    await expect(finalValueColumn).toBeVisible()
    await expect(actionsColumn).toBeVisible()
  })
})

test.describe('Responsiveness: Small Screen', () => {
  test.use({ viewport: { width: 784, height: 600 } });
  test("Confirm `Columns are visible` in full screen", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    const activeColumn = page.getByRole("cell", { name: "Active" }).first() 
    const nameColumn = page.getByRole("cell", { name: "Name" }).first() 
    const controllerColumn = page.getByRole("cell", { name: "Controller" }).first() 
    const deviceColumn = page.getByRole("cell", { name: "Device" }).first() 
    const statusColumn = page.getByRole("cell", { name: "Status" }).first() 
    const rawValueColumn = page.getByRole("cell", { name: "Raw Value" }).first() 
    const finalValueColumn = page.getByRole("cell", { name: "Final Value" }).first() 
    const actionsColumn = page.getByRole("cell", { name: "Actions" }).first() 

    await expect(activeColumn).toBeVisible()
    await expect(nameColumn).toBeVisible()
    await expect(controllerColumn).not.toBeVisible()
    await expect(deviceColumn).toBeVisible()
    await expect(statusColumn).toBeVisible()
    await expect(rawValueColumn).toBeVisible()
    await expect(finalValueColumn).toBeVisible()
    await expect(actionsColumn).toBeVisible()
  })
})

test.describe('Selection: Select / Deselect actions', () => {
  test.use({ viewport: { width: 1024, height: 800 } });
  test("Confirm `selection` of single item is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandConfigBulkAction")

    const firstRow = page.getByRole("row").nth(1)
    const selectionButton = page.getByRole('button', { name: 'rows selected' })
    
    await firstRow.click()
    await expect(selectionButton).toHaveText('1 rows selected')
  })

  test("Confirm `selection` of multiple items is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandConfigBulkAction")

    const firstRow = page.getByRole("row").nth(1)
    const fourthRow = page.getByRole("row").nth(4)
    const selectionButton = page.getByRole('button', { name: 'rows selected' })
    
    await firstRow.click()
    await expect(selectionButton).toHaveText('1 rows selected')
    await page.keyboard.down('Shift');
    await fourthRow.click()
    await page.keyboard.up('Shift');
    await expect(selectionButton).toHaveText('4 rows selected')
  })

  test("Confirm `selection` context menu is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandConfigBulkAction")

    const firstRow = page.getByRole("row").nth(1)
    const fourthRow = page.getByRole("row").nth(4)
    const selectionButton = page.getByRole('button', { name: 'rows selected' })
    const selectionDeleteButton = page.getByRole('option', { name: 'Delete selected' })
    const selectionToggleButton = page.getByRole('option', { name: 'Toggle selected' })
    const selectionClearButton = page.getByRole('option', { name: 'Clear selection' })

    await firstRow.click()
    await page.keyboard.down('Shift');
    await fourthRow.click()
    await page.keyboard.up('Shift');
    await selectionButton.click()
    
    await expect(selectionDeleteButton).toBeVisible()
    await expect(selectionToggleButton).toBeVisible()
    await expect(selectionClearButton).toBeVisible()
  })

  test("Confirm `delete` of multiple items is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandConfigBulkAction")

    const firstRow = page.getByRole("row").nth(1)
    const fourthRow = page.getByRole("row").nth(4)
    const selectionButton = page.getByRole('button', { name: 'rows selected' })
    const selectionDeleteButton = page.getByRole('option', { name: 'Delete selected' })
    
    await firstRow.click()
    await page.keyboard.down('Shift');
    await fourthRow.click()
    await page.keyboard.up('Shift');
    await selectionButton.click()
    
    await selectionDeleteButton.click()
    const postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
    const lastCommand = postedCommands!.pop()
    expect (lastCommand.key).toEqual('CommandConfigBulkAction')
    expect (lastCommand.payload.action).toEqual('delete')

    await expect(selectionButton).not.toBeVisible()
  })

  test("Confirm `toggle` of multiple items is working", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.initWithTestData()
    await configListPage.mobiFlightPage.trackCommand("CommandConfigBulkAction")

    const firstRow = page.getByRole("row").nth(1)
    const fourthRow = page.getByRole("row").nth(4)
    const selectionButton = page.getByRole('button', { name: 'rows selected' })
    const selectionToggleButton = page.getByRole('option', { name: 'Toggle selected' })
    
    await firstRow.click()
    await page.keyboard.down('Shift');
    await fourthRow.click()
    await page.keyboard.up('Shift');
    await selectionButton.click()
    
    await selectionToggleButton.click()
    const postedCommands = await configListPage.mobiFlightPage.getTrackedCommands();
    const lastCommand = postedCommands!.pop()
    expect (lastCommand.key).toEqual('CommandConfigBulkAction')
    expect (lastCommand.payload.action).toEqual('toggle')
  })
})
