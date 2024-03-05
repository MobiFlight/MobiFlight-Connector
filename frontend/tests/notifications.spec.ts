import { IDeviceItem } from "../src/types"
import { test, expect } from "./fixtures"
import { TestMobiflightBoard } from "./fixtures/data/devices"

test("Notification with string {{Value}} and no Button is displayed correctly", async ({ devicePage, page }) => {
  await devicePage.mfPage.gotoStartPage()
  await devicePage.mfPage.loadDefaultConfig()
  await devicePage.useDefaultDeviceList()
  await devicePage.mfPage.finishInitialLoading()
  await devicePage.mfPage.postNotification({
    Type: "sim.aircraft.changed",
    Value: "Cezznuh 172P",
  })
  await expect(
    page.getByRole("status").getByText("Aircraft changed"),
  ).toHaveCount(1)
  await expect(
    page.getByRole("status").getByText("The aircraft was changed to"),
  ).toHaveCount(1)
  await expect(page.getByRole("status").getByRole("button")).toHaveCount(0)
})

test("Notification without {{Value}} and Button is working correctly", async ({ devicePage, page }) => {
  await devicePage.mfPage.gotoStartPage()
  await devicePage.mfPage.loadDefaultConfig()
  await devicePage.useDefaultDeviceList()
  await devicePage.mfPage.finishInitialLoading()

  await devicePage.mfPage.postNotification({
    Type: "hubhop.update",
    Action: "navigate",
  })
  await expect(
    page.getByRole("status").getByText("Perform HubHop update"),
  ).toHaveCount(1)
  await expect(
    page.getByRole("status").getByText("Your HubHop presets are older than 7 days."),
  ).toHaveCount(1)
  await expect(page.getByRole("status").getByRole("button")).toHaveCount(1)
  await page.getByRole("status").getByRole("button").click()
  await expect(page.getByRole('heading', { name: 'Global settings' })).toBeVisible()
})

test("Notification with IDeviceItem {{Value}} is working correctly", async ({ devicePage, page }) => {
  await devicePage.mfPage.gotoStartPage()
  await devicePage.mfPage.loadDefaultConfig()
  await devicePage.useDefaultDeviceList()
  await devicePage.mfPage.finishInitialLoading()

  const device = TestMobiflightBoard as IDeviceItem

  await devicePage.mfPage.postNotification({
    Type: "device.connected",    
    Value: device,
    Action: "navigate",
  })

  await expect(
    page.getByRole("status").getByText("Device connected"),
  ).toHaveCount(1)
  await expect(
    page.getByRole("status").getByText("A new device"),
  ).toHaveCount(1)
  await expect(page.getByRole("status").getByRole("button")).toHaveCount(1)
  await page.getByRole("status").getByRole("button").click()
  await expect(page.getByRole('navigation').getByText('MobiFlight Mega')).toBeVisible()
})
