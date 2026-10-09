import type { Page } from "@playwright/test"
import { test, expect } from "./fixtures"

const mockGoldSponsors = [
  {
    name: "Test Sponsor A",
    logo: "/sponsors/test-a.png",
    href: "https://example.com/a",
  },
  {
    name: "Test Sponsor B",
    logo: "/sponsors/test-b.png",
    href: "https://example.com/b",
  },
  {
    name: "Test Sponsor C",
    logo: "/sponsors/test-c.png",
    href: "https://example.com/c",
  },
]

const mockGoldSponsorsEndpoint = async (page: Page) => {
  await page.route("**/sponsors.json", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(mockGoldSponsors),
    })
  })
}

const mockGoldSponsorsFailure = async (page: Page) => {
  await page.route("**/sponsors.json", async (route) => {
    await route.fulfill({
      status: 500,
      contentType: "application/json",
      body: "{}",
    })
  })
}

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

test("Test that frontend sends ready message", async ({ startupPage }) => {
  await startupPage.mobiFlightPage.trackCommand("CommandFrontendState")
  await startupPage.gotoStartupPage()

  const postedCommands = await startupPage.mobiFlightPage.getTrackedCommands()
  const lastCommand = postedCommands!.pop()
  expect(lastCommand.key).toEqual("CommandFrontendState")
  expect(lastCommand.payload.route).toEqual("/start")
  expect(lastCommand.payload.state).toEqual("ready")
})

test("Test that gold sponsors are shown on startup page", async ({
  startupPage,
  page,
}) => {
  await mockGoldSponsorsEndpoint(page)

  await startupPage.gotoStartupPage()

  await expect(
    page.getByText(
      "Thanks to our Gold Sponsors for fueling the development of MobiFlight.",
    ),
  ).toBeVisible()

  for (const sponsor of mockGoldSponsors) {
    await expect(
      page.getByRole("img", {
        name: `${sponsor.name} logo`,
      }),
    ).toBeVisible()
  }
})

test("Test that gold sponsor links open in external browser", async ({
  startupPage,
  page,
}) => {
  await mockGoldSponsorsEndpoint(page)

  await startupPage.mobiFlightPage.trackCommand("CommandOpenLinkInBrowser")

  await startupPage.gotoStartupPage()

  for (const sponsor of mockGoldSponsors) {
    await startupPage.mobiFlightPage.clearTrackedCommands()

    await page
      .getByRole("button", {
        name: `Open ${sponsor.name} website`,
      })
      .dispatchEvent("click")

    const trackedCommands =
      await startupPage.mobiFlightPage.getTrackedCommands()

    expect(trackedCommands).toBeDefined()
    expect(trackedCommands).toHaveLength(1)

    expect(trackedCommands![0]).toEqual({
      key: "CommandOpenLinkInBrowser",
      payload: {
        url: sponsor.href,
      },
    })
  }
})

test("Gold sponsor request failure does not block startup", async ({
  startupPage,
  page,
}) => {
  await mockGoldSponsorsFailure(page)

  await startupPage.gotoStartupPage()

  await expect(
    page.getByText(
      "Thanks to our Gold Sponsors for fueling the development of MobiFlight.",
    ),
  ).toHaveCount(0)

  await startupPage.setStatusBarUpdate(100, "Finished!")

  await expect(page).toHaveURL("http://localhost:5173/home")
})
