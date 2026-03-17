import { HubHopState } from "../src/types/messages"
import { test, expect } from "./fixtures"

test("Confirm HubHop update notifications show correctly", async ({
  configListPage,
  page,
}) => {
  await configListPage.gotoPage()
  await configListPage.mobiFlightPage.initWithTestData()

  const hubhopToast = page.getByTestId("toast-hubhop-auto-update")
  await expect(hubhopToast).not.toBeVisible()

  await configListPage.mobiFlightPage.publishCommand({
    key: "HubHopState",
    payload: {
      ShouldUpdate: true,
      Result: "Pending",
      UpdateProgress: 0,
    } as HubHopState,
  })

  await expect(hubhopToast).toBeVisible()

  const updateButton = hubhopToast.getByRole("button")
  await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")
  await updateButton.click()

  const commandsAfterClick =
    await configListPage.mobiFlightPage.getTrackedCommands()
  expect(commandsAfterClick).toHaveLength(1)
  expect(commandsAfterClick![0].key).toBe("CommandMainMenu")
  expect(commandsAfterClick![0].payload.action).toBe("extras.hubhop.download")

  // Simulate update in progress
  await configListPage.mobiFlightPage.publishCommand({
    key: "HubHopState",
    payload: {
      ShouldUpdate: true,
      Result: "InProgress",
      UpdateProgress: 0,
    },
  })

  const progressBar = hubhopToast.getByRole("progressbar")
  await expect(hubhopToast).toHaveCount(1)
  await expect(hubhopToast).toBeVisible()
  await expect(progressBar).toBeVisible()

  await configListPage.mobiFlightPage.publishCommand({
    key: "HubHopState",
    payload: {
      ShouldUpdate: true,
      Result: "Success",
      UpdateProgress: 100,
    },
  })

  await expect(hubhopToast).not.toBeVisible({ timeout: 5000 })
})

test.describe("Generic Notifications tests", () => {
  test("Confirm Auto-Bind Controller Notification shows correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const toastAutoBindControllerSuccessful  = page.getByTestId(
      "toast-autobind-controllers-successful",
    )
    await expect(toastAutoBindControllerSuccessful).not.toBeVisible()

    await configListPage.mobiFlightPage.publishMessage({
      key: "Notification",
      payload: {
        Event: "ControllerAutoBindSuccessful",
        Context: { Count: "1", Controllers: "Alpha Flight Controls"},
      },
    })

    await expect(toastAutoBindControllerSuccessful).toBeVisible()
  })

  test("Confirm Manual Binding Required Notification shows correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const toastManualBindingRequired  = page.getByTestId(
      "toast-manual-binding-required",
    )
    await expect(toastManualBindingRequired).not.toBeVisible()

    await configListPage.mobiFlightPage.publishMessage({
      key: "Notification",
      payload: {
        Event: "ControllerManualBindRequired",
        Context: { Count: "2", Controllers: "Alpha Flight Controls"},
      },
    })

    await expect(toastManualBindingRequired).toBeVisible()
  })

  test("Confirm Project File Extension Migrated Notification shows correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const toastFileExtensionMigration = page.getByTestId(
      "toast-file-extension-migrated",
    )
    await expect(toastFileExtensionMigration).not.toBeVisible()

    await configListPage.mobiFlightPage.publishMessage({
      key: "Notification",
      payload: {
        Event: "ProjectFileExtensionMigrated",
        Context: {},
      },
    })

    await expect(toastFileExtensionMigration).toBeVisible()
  })

  test("Confirm Sim Connection Lost Notification shows correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const toastSimConnectionLost = page.getByTestId("toast-sim-connection-lost")
    await expect(toastSimConnectionLost).not.toBeVisible()

    await configListPage.mobiFlightPage.publishMessage({
      key: "Notification",
      payload: {
        Event: "SimConnectionLost",
        Context: { SimType: "SimConnect" },
      },
    })

    await expect(toastSimConnectionLost).toBeVisible()
    await expect(toastSimConnectionLost).toContainText("Connection Lost")
    await expect(toastSimConnectionLost).toContainText("SimConnect")
  })

  test("Confirm Sim Stopped Notification shows correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const toastSimStopped = page.getByTestId("toast-sim-stopped")
    await expect(toastSimStopped).not.toBeVisible()

    await configListPage.mobiFlightPage.publishMessage({
      key: "Notification",
      payload: {
        Event: "SimStopped",
        Context: {},
      },
    })

    await expect(toastSimStopped).toBeVisible()
    await expect(toastSimStopped).toContainText("Flight Simulator Closed")
  })

  test("Confirm Test Mode Exception Notification shows correctly", async ({
    configListPage,
    page,
  }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const toastTestModeException = page.getByTestId("toast-test-mode-exception")
    await expect(toastTestModeException).not.toBeVisible()

    await configListPage.mobiFlightPage.publishMessage({
      key: "Notification",
      payload: {
        Event: "TestModeException",
        Context: { ErrorMessage: "Test error occurred" },
      },
    })

    await expect(toastTestModeException).toBeVisible()
    await expect(toastTestModeException).toContainText("Test Mode Error")
    await expect(toastTestModeException).toContainText("Test error occurred")
  })
})
