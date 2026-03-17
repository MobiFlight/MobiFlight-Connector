import { test, expect } from "./fixtures"

test("Go beyond progress bar", async ({ startupPage, page }) => {
  await startupPage.gotoStartupPage()
  await startupPage.setStatusBarUpdate(50, "Loading...")
  // expect to have exactly one progressBar
  await expect(page.getByRole("progressbar")).toHaveCount(1)
  // expect the progressBar to be visible
  await expect(page.getByRole("progressbar")).toBeVisible()

  await startupPage.setStatusBarUpdate(100, "Finished!")

  // Once finished loading, we are redirected to the home page
  await expect(page).toHaveURL("http://localhost:5173/home")
})

test("Test that backend progress state is localized", async ({
  startupPage,
  page,
}) => {
  await startupPage.gotoStartupPage()

  const backendMessages = [
    { value: 10, text: "Startup.Starting" },
    { value: 20, text: "Startup.CheckingFirmwareUpdates" },
    { value: 30, text: "Startup.LoadingLastConfig" },
    { value: 50, text: "Startup.ScanningControllers" },
    { value: 99, text: "Startup.Finished" },
  ]

  for (const message of backendMessages) {
    await startupPage.setStatusBarUpdate(message.value, message.text)
    await expect(page.getByText(message.text)).toHaveCount(0)
  }
})
