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

  await configListPage.mobiFlightPage.publishCommand(
    { key: "HubHopState" ,
      payload: {
        ShouldUpdate: true,
        Result: "Pending",
        UpdateProgress: 0,
      } as HubHopState
    }
  )

  await expect(hubhopToast).toBeVisible()

  const updateButton = hubhopToast.getByRole("button")
  await configListPage.mobiFlightPage.trackCommand("CommandMainMenu")
  await updateButton.click()

  const commandsAfterClick = await configListPage.mobiFlightPage.getTrackedCommands()
  expect(commandsAfterClick).toHaveLength(1)
  expect(commandsAfterClick![0].key).toBe("CommandMainMenu")
  expect(commandsAfterClick![0].payload.action).toBe("extras.hubhop.download")

  // Simulate update in progress
  await configListPage.mobiFlightPage.publishCommand(
    { key: "HubHopState" ,
      payload: {
        ShouldUpdate: true,
        Result: "InProgress",
        UpdateProgress: 0,
      }
    }
  )

  const progressBar = hubhopToast.getByRole("progressbar")
  await expect(hubhopToast).toHaveCount(1)
  await expect(hubhopToast).toBeVisible()
  await expect(progressBar).toBeVisible()

  await configListPage.mobiFlightPage.publishCommand(
    { key: "HubHopState" ,
      payload: {
        ShouldUpdate: true,
        Result: "Success",
        UpdateProgress: 100,
      }
    }
  )

  await expect(hubhopToast).not.toBeVisible({ timeout: 5000 })
})