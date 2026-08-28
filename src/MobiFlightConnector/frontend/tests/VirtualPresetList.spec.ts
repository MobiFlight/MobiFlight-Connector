import { Locator, Page, Route } from "@playwright/test"
import { test, expect } from "./fixtures"
import { ConfigListPage } from "./fixtures/ConfigListPage"
import { Preset, XplanePreset } from "../src/types/preset"

// PresetId of the onPress action of config item row 1 in inputaction.testdata.json
const SELECTED_MSFS_PRESET_ID = "cf0526c0-da69-404f-a570-9f9d54d2803c"

const presetLabel = (index: number) =>
  `PRESET_${String(index).padStart(4, "0")}`

const makeMsfsPresets = (count: number): Preset[] =>
  Array.from({ length: count }, (_, index) => ({
    id: `preset-${String(index).padStart(4, "0")}`,
    vendor: "Microsoft",
    aircraft: "Generic",
    system: "Autopilot",
    label: presetLabel(index),
    description: `Test preset number ${index}`,
    code: `(>H:${presetLabel(index)})`,
    version: 1,
    status: "Submitted",
    author: "Mobiflight Community",
    createdDate: "2021-08-01T00:00:00.000Z",
    presetType: "Input",
  }))

const makeXplanePresets = (count: number): XplanePreset[] =>
  Array.from({ length: count }, (_, index) => ({
    id: `xp-preset-${String(index).padStart(4, "0")}`,
    vendor: "Laminar Research",
    aircraft: "Boeing 737-800",
    system: "Autopilot",
    label: presetLabel(index),
    description: `Test preset number ${index}`,
    code: `test/preset/${presetLabel(index)}`,
    version: 1,
    status: "Submitted",
    author: "Mobiflight Community",
    createdDate: "2021-08-01T00:00:00.000Z",
    presetType: "Input",
    codeType: "Command",
  }))

const openActionEditorWithPresets = async (
  configListPage: ConfigListPage,
  page: Page,
  options: {
    row: number
    presetUrl: string
    presetResponse: (route: Route) => Promise<void>
    sim?: string
  },
) => {
  await configListPage.gotoPage()
  if (options.sim) {
    await configListPage.mobiFlightPage.initWithTestDataAndSpecificProjectProps(
      { Sim: options.sim },
      "inputaction",
    )
  } else {
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
  }
  await page.route(options.presetUrl, options.presetResponse)

  await configListPage.clickEditButtonForRow(options.row)
  const actionPanel = page.getByTestId("action-panel")
  await expect(actionPanel).toBeVisible()

  const actionEditButton = actionPanel.getByRole("button", {
    name: "Edit On Press Action",
  })
  await expect(actionEditButton).toBeVisible()
  await actionEditButton.click()

  const actionEditor = page.getByTestId("action-editor")
  await expect(actionEditor).toBeVisible()
  return actionEditor
}

const scrollPresetListUntilInViewport = async (
  page: Page,
  presetList: Locator,
  target: Locator,
) => {
  await presetList.getByRole("listitem").first().hover()
  await expect(async () => {
    await page.mouse.wheel(0, 3000)
    await expect(target).toBeInViewport({ timeout: 200 })
  }).toPass({ intervals: [250], timeout: 20000 })
}

test.describe("Input Config Wizard - Virtual Preset List", () => {
  test("MSFS presets outside the visible area are not rendered and become visible after scrolling", async ({
    configListPage,
    page,
  }) => {
    const presetCount = 500
    const actionEditor = await openActionEditorWithPresets(
      configListPage,
      page,
      {
        row: 1,
        presetUrl: "*/**/presets/msfs2020_hubhop_presets.json",
        presetResponse: (route) =>
          route.fulfill({ json: makeMsfsPresets(presetCount) }),
      },
    )

    await expect(actionEditor.getByRole("status")).toHaveText(
      `${presetCount} preset(s) found`,
    )

    const presetList = actionEditor.getByRole("list")
    const listItems = presetList.getByRole("listitem")
    const firstItem = listItems.filter({ hasText: presetLabel(0) })
    const lastItem = listItems.filter({ hasText: presetLabel(presetCount - 1) })

    await expect(firstItem).toBeInViewport()

    await expect(lastItem).toHaveCount(0)

    // not all elements are visible
    expect(await listItems.count()).toBeLessThan(50)

    await scrollPresetListUntilInViewport(page, presetList, lastItem)

    // first preset is removed from the DOM again
    await expect(firstItem).toHaveCount(0)
  })

  test("X-Plane presets outside the visible area are not rendered and become visible after scrolling", async ({
    configListPage,
    page,
  }) => {
    const presetCount = 300
    const actionEditor = await openActionEditorWithPresets(
      configListPage,
      page,
      {
        row: 2,
        presetUrl: "*/**/presets/xplane_hubhop_presets.json",
        presetResponse: (route) =>
          route.fulfill({ json: makeXplanePresets(presetCount) }),
        sim: "xplane",
      },
    )

    await expect(actionEditor.getByRole("status")).toHaveText(
      `${presetCount} preset(s) found`,
    )

    const presetList = actionEditor.getByRole("list")
    const listItems = presetList.getByRole("listitem")
    const firstItem = listItems.filter({ hasText: presetLabel(0) })
    const lastItem = listItems.filter({ hasText: presetLabel(presetCount - 1) })

    await expect(firstItem).toBeInViewport()
    await expect(lastItem).toHaveCount(0)
    expect(await listItems.count()).toBeLessThan(50)

    await scrollPresetListUntilInViewport(page, presetList, lastItem)
    await expect(firstItem).toHaveCount(0)
  })

  test("Selected preset is automatically scrolled into view", async ({
    configListPage,
    page,
  }) => {
    const presets = makeMsfsPresets(500)

    // preset outside of the visible area
    const selectedIndex = 450
    presets[selectedIndex].id = SELECTED_MSFS_PRESET_ID

    const actionEditor = await openActionEditorWithPresets(
      configListPage,
      page,
      {
        row: 1,
        presetUrl: "*/**/presets/msfs2020_hubhop_presets.json",
        presetResponse: (route) => route.fulfill({ json: presets }),
      },
    )

    await page.mouse.move(0, 0)

    const selectedItem = actionEditor
      .getByRole("listitem")
      .filter({ hasText: presetLabel(selectedIndex) })

    await expect(selectedItem).toBeInViewport({ timeout: 5000 })
    await expect(selectedItem).toHaveClass(/border-primary/)
  })

  test("Multi-line preset descriptions are displayed completely without overlapping other items", async ({
    configListPage,
    page,
  }) => {
    const presets = makeMsfsPresets(5)
    const longDescription =
      "This is a very long preset description that is definitely going to " +
      "wrap onto multiple lines in the preset list because it contains a " +
      "lot of detail about what the preset does, which aircraft variants " +
      "it applies to and which cockpit hardware it has been tested with " +
      "by the amazing MobiFlight community over a long period of time."
    presets[1].description = longDescription

    const actionEditor = await openActionEditorWithPresets(
      configListPage,
      page,
      {
        row: 1,
        presetUrl: "*/**/presets/msfs2020_hubhop_presets.json",
        presetResponse: (route) => route.fulfill({ json: presets }),
      },
    )

    await expect(actionEditor.getByRole("status")).toHaveText(
      "5 preset(s) found",
    )

    const listItems = actionEditor.getByRole("listitem")
    const singleLineItem = listItems.filter({ hasText: presetLabel(0) })
    const multiLineItem = listItems.filter({ hasText: presetLabel(1) })
    const followingItem = listItems.filter({ hasText: presetLabel(2) })

    await expect(multiLineItem.getByText(longDescription)).toBeVisible()

    await expect(async () => {
      const singleLineBox = await singleLineItem.boundingBox()
      const multiLineBox = await multiLineItem.boundingBox()
      const followingBox = await followingItem.boundingBox()

      expect(singleLineBox).not.toBeNull()
      expect(multiLineBox).not.toBeNull()
      expect(followingBox).not.toBeNull()

      // row is larger than the single line height
      expect(multiLineBox!.height).toBeGreaterThan(singleLineBox!.height + 10)

      // The taller row does not overlap its neighbors
      expect(multiLineBox!.y).toBeGreaterThanOrEqual(
        singleLineBox!.y + singleLineBox!.height,
      )
      expect(followingBox!.y).toBeGreaterThanOrEqual(
        multiLineBox!.y + multiLineBox!.height,
      )
    }).toPass({ timeout: 5000 })
  })

  test("Preset list is ready quickly for a large number of presets", async ({
    configListPage,
    page,
  }) => {
    const presetCount = 5000

    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData("inputaction")
    await page.route(
      "*/**/presets/msfs2020_hubhop_presets.json",
      async (route) => {
        await route.fulfill({ json: makeMsfsPresets(presetCount) })
      },
    )

    await configListPage.clickEditButtonForRow(1)
    const actionPanel = page.getByTestId("action-panel")
    await expect(actionPanel).toBeVisible()

    const actionEditButton = actionPanel.getByRole("button", {
      name: "Edit On Press Action",
    })
    await expect(actionEditButton).toBeVisible()

    // measures the pure render time
    const renderStart = Date.now()
    await actionEditButton.click()

    const actionEditor = page.getByTestId("action-editor")
    const listItems = actionEditor.getByRole("listitem")
    await expect(listItems.first()).toBeVisible({ timeout: 10000 })
    const renderDuration = Date.now() - renderStart

    expect(renderDuration).toBeLessThan(2000)

    await expect(actionEditor.getByRole("status")).toHaveText(
      `${presetCount} preset(s) found`,
    )

    expect(await listItems.count()).toBeLessThan(50)
  })
})
