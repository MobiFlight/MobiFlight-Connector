import { test, expect } from "./fixtures"
import { Page } from "@playwright/test"
import Settings from "../src/types/settings"
import defaultSettings from "./data/settings.testdata.json" with { type: "json" }
import dotenv from "dotenv"
dotenv.config()
test("Confirm `Extras > Settings` menu item opens the correct modal", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()

  const menuItemExtras = page
    .getByRole("menubar")
    .getByRole("menuitem", { name: "Extras" })
  const menuItemSettings = page.getByRole("menuitem", { name: "Settings" })
  const dialog = page.getByRole("dialog", { name: "Settings" })

  // Verify modal is not visible initially
  await expect(dialog).not.toBeVisible()

  // Open Extras menu and click Settings
  await menuItemExtras.click()
  await menuItemSettings.click()

  // Verify modal is visible
  await expect(dialog).toBeVisible()
})

test("Confirm modal can be closed", async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  const menuItemExtras = page
    .getByRole("menubar")
    .getByRole("menuitem", { name: "Extras" })
  const menuItemSettings = page.getByRole("menuitem", { name: "Settings" })
  const dialog = page.getByRole("dialog", { name: "Settings" })

  // Open Settings dialog
  await menuItemExtras.click()
  await menuItemSettings.click()
  await expect(dialog).toBeVisible()

  // Click Cancel button and verify dialog closes
  const cancelButton = dialog.getByRole("button", { name: "Cancel" })
  await cancelButton.click()
  await expect(dialog).not.toBeVisible()
})

test("Confirm saves changes and publishes CommandUpdateSettings", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()
  await configListPage.mobiFlightPage.trackCommand("CommandUpdateSettings")

  const menuItemExtras = page
    .getByRole("menubar")
    .getByRole("menuitem", { name: "Extras" })
  const menuItemSettings = page.getByRole("menuitem", { name: "Settings" })
  const dialog = page.getByRole("dialog", { name: "Settings" })

  // Open Settings dialog
  await menuItemExtras.click()
  await menuItemSettings.click()
  await expect(dialog).toBeVisible()

  // Modify a setting value
  const betaUpdatesSwitch = dialog.locator("#beta-updates")
  await betaUpdatesSwitch.click()

  // Click Save button
  const saveButton = dialog.getByRole("button", { name: "Save" })
  await saveButton.click()

  // Verify dialog is closed
  await expect(dialog).not.toBeVisible()

  // Verify CommandUpdateSettings was published with updated values
  const trackedCommands =
    await configListPage.mobiFlightPage.getTrackedCommands()
  if (trackedCommands == undefined) {
    throw new Error(`No commands tracked after saving settings`)
  }
  expect(trackedCommands).toBeDefined()
  expect(trackedCommands.length).toBeGreaterThan(0)
  const lastCommand = trackedCommands.pop()
  expect(lastCommand?.key).toBe("CommandUpdateSettings")
  expect(lastCommand?.payload.Settings.BetaUpdates).toBe(true)
})

test("Confirm cancels changes without publishing command", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()
  await configListPage.mobiFlightPage.trackCommand("CommandUpdateSettings")

  const menuItemExtras = page
    .getByRole("menubar")
    .getByRole("menuitem", { name: "Extras" })
  const menuItemSettings = page.getByRole("menuitem", { name: "Settings" })
  const dialog = page.getByRole("dialog", { name: "Settings" })

  // Open Settings dialog
  await menuItemExtras.click()
  await menuItemSettings.click()
  await expect(dialog).toBeVisible()

  // Modify a setting value
  const betaUpdatesSwitch = dialog.locator("#beta-updates")
  await betaUpdatesSwitch.click()

  // Click Cancel button
  const cancelButton = dialog.getByRole("button", { name: "Cancel" })
  await cancelButton.click()

  // Discard confirmation appears
  const discardDialog = page.getByRole("dialog", { name: "Discard changes?" })
  await expect(discardDialog).toBeVisible()
  const discardChangesButton = discardDialog.getByRole("button", {
    name: "Discard changes",
  })
  await discardChangesButton.click()

  // Verify dialog is closed
  await expect(dialog).not.toBeVisible()

  // Verify no update command was published
  const trackedCommands =
    await configListPage.mobiFlightPage.getTrackedCommands()
  if (trackedCommands == undefined) {
    throw new Error(`No commands tracked after cancelling settings`)
  }
  expect(trackedCommands.length).toBe(0)
})

test("Confirm tab switching works between General and Simulator", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  const menuItemExtras = page
    .getByRole("menubar")
    .getByRole("menuitem", { name: "Extras" })
  const menuItemSettings = page.getByRole("menuitem", { name: "Settings" })
  const dialog = page.getByRole("dialog", { name: "Settings" })

  // Open Settings dialog
  await menuItemExtras.click()
  await menuItemSettings.click()
  await expect(dialog).toBeVisible()

  // Verify General tab content is visible by default
  await expect(dialog.locator("#beta-updates")).toBeVisible()

  // Switch to Simulator tab
  const simulatorTab = dialog.getByRole("tab", { name: "Simulator" })
  await simulatorTab.click()
  await expect(dialog.locator("#prosim-host")).toBeVisible()
  await expect(dialog.locator("#prosim-port")).toBeVisible()

  // Switch back to General tab
  const generalTab = dialog.getByRole("tab", { name: "General" })
  await generalTab.click()
  await expect(dialog.locator("#beta-updates")).toBeVisible()
})

test("Confirm discard changes confirmation dialog appears when closing with unsaved changes", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  const menuItemExtras = page
    .getByRole("menubar")
    .getByRole("menuitem", { name: "Extras" })
  const menuItemSettings = page.getByRole("menuitem", { name: "Settings" })
  const dialog = page.getByRole("dialog", { name: "Settings" })

  // Open Settings dialog
  await menuItemExtras.click()
  await menuItemSettings.click()
  await expect(dialog).toBeVisible()

  // Modify a setting value
  const betaUpdatesSwitch = dialog.locator("#beta-updates")
  await betaUpdatesSwitch.click()

  // Click Cancel button
  const cancelButton = dialog.getByRole("button", { name: "Cancel" })
  await cancelButton.click()

  // Verify discard confirmation dialog appears
  const discardDialog = page.getByRole("dialog", { name: "Discard changes?" })
  await expect(discardDialog).toBeVisible()
  await expect(
    discardDialog.getByText(
      "You have unsaved changes. Are you sure you want to discard them?",
    ),
  ).toBeVisible()

  // Click Keep editing button
  const keepEditingButton = discardDialog.getByRole("button", {
    name: "Keep editing",
  })
  await keepEditingButton.click()

  // Verify discard dialog is closed but Settings dialog remains open
  await expect(discardDialog).not.toBeVisible()
  await expect(dialog).toBeVisible()

  // Click Cancel button again
  await cancelButton.click()
  await expect(discardDialog).toBeVisible()

  // Click Discard changes button
  const discardChangesButton = discardDialog.getByRole("button", {
    name: "Discard changes",
  })
  await discardChangesButton.click()

  // Verify all dialogs are closed
  await expect(discardDialog).not.toBeVisible()
  await expect(dialog).not.toBeVisible()
})

test("Confirm note about manage controllers is displayed", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  const menuItemExtras = page
    .getByRole("menubar")
    .getByRole("menuitem", { name: "Extras" })
  const menuItemSettings = page.getByRole("menuitem", { name: "Settings" })
  const dialog = page.getByRole("dialog", { name: "Settings" })

  // Open Settings dialog
  await menuItemExtras.click()
  await menuItemSettings.click()
  await expect(dialog).toBeVisible()

  // Verify note about manage controllers is displayed
  const manageControllersNote = dialog.getByText(
    "Manage MobiFlight Boards and Peripherals in Extras > Controller Management",
  )
  await expect(manageControllersNote).toBeVisible()
})

test("Confirm Cancel button uses localized label when non-English language is set", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  // Publish settings with German language
  await configListPage.mobiFlightPage.sendSettings({
    Language: "de-DE",
  })

  const menuItemExtras = page
    .getByRole("menubar")
    .getByRole("menuitem", { name: "Extras" })
  const menuItemSettings = page.getByRole("menuitem", {
    name: "Einstellungen",
  })
  const dialog = page.getByRole("dialog", { name: "Einstellungen" })

  // Open Settings dialog
  await menuItemExtras.click()
  await menuItemSettings.click()
  await expect(dialog).toBeVisible()

  // Verify localized Cancel button is displayed (Abbrechen)
  const cancelButton = dialog.getByRole("button", { name: "Abbrechen" })
  await expect(cancelButton).toBeVisible()

  // Verify English "Cancel" is not displayed
  await expect(dialog.getByRole("button", { name: "Cancel" })).not.toBeVisible()

  // Verify clicking the localized Cancel button closes the dialog
  await cancelButton.click()
  await expect(dialog).not.toBeVisible()
})

const basicCredentialsAvailable =
  !!process.env.TESTS_BASIC_EMAIL &&
  !!process.env.TESTS_BASIC_PASSWORD &&
  !!process.env.TESTS_BASIC_NAME

type CloudMockOptions = {
  getStatus?: number
  getContent?: Partial<Settings>
  putStatus?: number
}

// Mocks the cloud settings API and records every request it receives
const mockCloudSettingsApi = async (
  page: Page,
  options: CloudMockOptions = {},
) => {
  const { getStatus = 404, getContent, putStatus = 200 } = options
  const requests: { method: string; body: unknown }[] = []

  await page.route("**/api/settings/1", async (route) => {
    const request = route.request()
    const method = request.method()
    requests.push({ method, body: request.postDataJSON() })

    if (method === "GET") {
      if (getStatus !== 200) {
        await route.fulfill({ status: getStatus })
        return
      }
      await route.fulfill({
        json: {
          schemaVersion: 1,
          revision: 1,
          contentHash: "hash",
          content: getContent,
        },
      })
      return
    }

    if (method === "PUT") {
      const body = request.postDataJSON()
      await route.fulfill({
        status: putStatus,
        json: {
          schemaVersion: 1,
          revision: 2,
          contentHash: "hash2",
          content: body?.content,
        },
      })
      return
    }

    await route.fulfill({ status: 200 })
  })

  return requests
}

const openSettingsDialog = async (page: Page) => {
  await page
    .getByRole("menubar")
    .getByRole("menuitem", { name: "Extras" })
    .click()
  await page.getByRole("menuitem", { name: "Settings" }).click()
  const dialog = page.getByRole("dialog", { name: "Settings" })
  await expect(dialog).toBeVisible()
  return dialog
}

test.describe("Cloud Settings Sync", () => {
  test.describe("Anonymous user", () => {
    test.use({ storageState: { cookies: [], origins: [] } })

    test("Confirm sign in hint is shown and no cloud request is made", async ({
      configListPage,
      page,
    }) => {
      const requests = await mockCloudSettingsApi(page)
      await configListPage.gotoPage()
      await configListPage.mobiFlightPage.sendSettings({
        CloudSettingsSyncEnabled: true,
      })

      const dialog = await openSettingsDialog(page)
      await expect(dialog.getByText("Sign in to sync settings")).toBeVisible()
      expect(requests).toHaveLength(0)
    })
  })

  test.describe("Signed in user", () => {
    test.skip(
      !basicCredentialsAvailable,
      "Skipping cloud sync tests: required secrets are missing",
    )
    test.use({ storageState: "./tests/.auth/basic.json" })

    test("Confirm sync paused status when sync is disabled", async ({
      configListPage,
      page,
    }) => {
      const requests = await mockCloudSettingsApi(page)
      await configListPage.gotoPage()
      await configListPage.mobiFlightPage.sendSettings({
        CloudSettingsSyncEnabled: false,
      })

      const dialog = await openSettingsDialog(page)
      await expect(dialog.getByText("Cloud sync paused")).toBeVisible()
      expect(requests).toHaveLength(0)
    })

    test("Confirm initial upload when cloud has no settings", async ({
      configListPage,
      page,
    }) => {
      const requests = await mockCloudSettingsApi(page, { getStatus: 404 })
      await configListPage.gotoPage()
      await configListPage.mobiFlightPage.sendSettings({
        CloudSettingsSyncEnabled: true,
      })

      await expect
        .poll(() => requests.find((r) => r.method === "PUT"))
        .toBeDefined()
      const put = requests.find((r) => r.method === "PUT")!
      expect(put.body).toMatchObject({
        schemaVersion: 1,
        content: { CloudSettingsSyncEnabled: true },
      })
      await expect(
        page.getByRole("heading", { name: "Settings saved" }),
      ).toBeVisible()
    })

    test("Confirm nothing happens when cloud matches local", async ({
      configListPage,
      page,
    }) => {
      const local = {
        ...(defaultSettings as Settings),
        CloudSettingsSyncEnabled: true,
      }
      const requests = await mockCloudSettingsApi(page, {
        getStatus: 200,
        getContent: local,
      })
      await configListPage.gotoPage()
      await configListPage.mobiFlightPage.sendSettings({
        CloudSettingsSyncEnabled: true,
      })

      await expect
        .poll(() => requests.some((r) => r.method === "GET"))
        .toBe(true)
      await expect(
        page.getByRole("dialog", { name: "Settings conflict" }),
      ).not.toBeVisible()
      expect(requests.some((r) => r.method === "PUT")).toBe(false)
    })

    test.describe("Conflict", () => {
      const cloudContent = {
        ...(defaultSettings as Settings),
        CloudSettingsSyncEnabled: true,
        BetaUpdates: true,
      }

      test.beforeEach(async ({ configListPage }) => {
        await configListPage.gotoPage()
        await configListPage.mobiFlightPage.trackCommand(
          "CommandUpdateSettings",
        )
      })

      test("Confirm Keep local disables sync", async ({
        configListPage,
        page,
      }) => {
        const requests = await mockCloudSettingsApi(page, {
          getStatus: 200,
          getContent: cloudContent,
        })
        await configListPage.mobiFlightPage.sendSettings({
          CloudSettingsSyncEnabled: true,
          BetaUpdates: false,
        })

        const conflictDialog = page.getByRole("dialog", {
          name: "Settings conflict",
        })
        await expect(conflictDialog).toBeVisible()
        await conflictDialog
          .getByRole("button", { name: /Keep local settings/ })
          .click()
        await expect(conflictDialog).not.toBeVisible()

        const commands =
          await configListPage.mobiFlightPage.getTrackedCommands()
        expect(commands?.at(-1)?.payload).toMatchObject({
          Settings: { CloudSettingsSyncEnabled: false, BetaUpdates: false },
        })
        expect(requests.some((r) => r.method === "PUT")).toBe(false)
      })

      test("Confirm Use cloud applies cloud settings", async ({
        configListPage,
        page,
      }) => {
        await mockCloudSettingsApi(page, {
          getStatus: 200,
          getContent: cloudContent,
        })
        await configListPage.mobiFlightPage.sendSettings({
          CloudSettingsSyncEnabled: true,
          BetaUpdates: false,
        })

        const conflictDialog = page.getByRole("dialog", {
          name: "Settings conflict",
        })
        await conflictDialog
          .getByRole("button", { name: /Use cloud settings/ })
          .click()
        await expect(conflictDialog).not.toBeVisible()

        const commands =
          await configListPage.mobiFlightPage.getTrackedCommands()
        expect(commands?.at(-1)?.payload).toMatchObject({
          Settings: { BetaUpdates: true },
        })
        await expect(
          page.getByRole("heading", { name: "Settings restored" }),
        ).toBeVisible()
      })

      test("Confirm Upload local overwrites cloud", async ({
        configListPage,
        page,
      }) => {
        const requests = await mockCloudSettingsApi(page, {
          getStatus: 200,
          getContent: cloudContent,
        })
        await configListPage.mobiFlightPage.sendSettings({
          CloudSettingsSyncEnabled: true,
          BetaUpdates: false,
        })

        const conflictDialog = page.getByRole("dialog", {
          name: "Settings conflict",
        })
        await conflictDialog
          .getByRole("button", { name: /Upload local to cloud/ })
          .click()
        await expect(conflictDialog).not.toBeVisible()

        const put = requests.find((r) => r.method === "PUT")
        expect(put?.body).toMatchObject({ content: { BetaUpdates: false } })
        await expect(
          page.getByRole("heading", { name: "Settings synced" }),
        ).toBeVisible()
      })
    })

    test("Confirm enabling sync in dialog triggers a sync check", async ({
      configListPage,
      page,
    }) => {
      const requests = await mockCloudSettingsApi(page, { getStatus: 404 })
      await configListPage.gotoPage()
      await configListPage.mobiFlightPage.sendSettings({
        CloudSettingsSyncEnabled: false,
      })

      const dialog = await openSettingsDialog(page)
      await dialog.locator("#cloud-settings-sync").click()
      await dialog.getByRole("button", { name: "Save" }).click()

      await expect
        .poll(() => requests.some((r) => r.method === "GET"))
        .toBe(true)
      await expect
        .poll(() => requests.some((r) => r.method === "PUT"))
        .toBe(true)
    })

    test("Confirm save uploads changes when sync is enabled", async ({
      configListPage,
      page,
    }) => {
      const local = {
        ...(defaultSettings as Settings),
        CloudSettingsSyncEnabled: true,
      }
      const requests = await mockCloudSettingsApi(page, {
        getStatus: 200,
        getContent: local,
      })
      await configListPage.gotoPage()
      await configListPage.mobiFlightPage.sendSettings({
        CloudSettingsSyncEnabled: true,
      })

      const dialog = await openSettingsDialog(page)
      await dialog.locator("#beta-updates").click()
      await dialog.getByRole("button", { name: "Save" }).click()

      await expect
        .poll(() => requests.find((r) => r.method === "PUT"))
        .toBeDefined()
      expect(requests.find((r) => r.method === "PUT")?.body).toMatchObject({
        content: { BetaUpdates: true },
      })
      await expect(
        page.getByRole("heading", { name: "Settings saved" }),
      ).toBeVisible()
    })

    test("Confirm fetch error shows notification", async ({
      configListPage,
      page,
    }) => {
      await mockCloudSettingsApi(page, { getStatus: 500 })
      await configListPage.gotoPage()
      await configListPage.mobiFlightPage.sendSettings({
        CloudSettingsSyncEnabled: true,
      })

      await expect(
        page.getByRole("heading", { name: "Cloud unavailable" }),
      ).toBeVisible()
    })

    test("Confirm upload error shows notification", async ({
      configListPage,
      page,
    }) => {
      await mockCloudSettingsApi(page, { getStatus: 404, putStatus: 500 })
      await configListPage.gotoPage()
      await configListPage.mobiFlightPage.sendSettings({
        CloudSettingsSyncEnabled: true,
      })

      await expect(
        page.getByRole("heading", { name: "Sync failed" }),
      ).toBeVisible()
    })
  })
})
