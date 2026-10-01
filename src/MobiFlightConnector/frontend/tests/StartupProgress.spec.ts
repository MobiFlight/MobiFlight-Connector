import { test, expect } from "./fixtures"
import type { GoldSponsor } from "../src/types/messages"

const testLogo =
  "data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIyMDAiIGhlaWdodD0iNjAiPjxyZWN0IHdpZHRoPSIyMDAiIGhlaWdodD0iNjAiIGZpbGw9ImJsYWNrIi8+PC9zdmc+"

const goldSponsors: GoldSponsor[] = [
  {
    Name: "FliteSim",
    LogoDataUri: testLogo,
    Url: "https://flitesim.com/?ref=mobiflight",
  },
  {
    Name: "Honeycomb Aeronautical",
    LogoDataUri: testLogo,
    Url: "https://flyhoneycomb.com/?ref=MOBIFLIGHT",
  },
  {
    Name: "MOZA",
    LogoDataUri: testLogo,
    Url: "https://mozaracing.com/mobiflight",
  },
  {
    Name: "VKB Sim",
    LogoDataUri: testLogo,
    Url: "https://vkb-sim.pro/?utm_source=mobiflight",
  },
  {
    Name: "WingFlex",
    LogoDataUri: testLogo,
    Url: "https://www.wingflex.com?sca_ref=11453765.OPCgaGgkUj",
  },
]

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
  await startupPage.gotoStartupPage()

  for (const sponsor of goldSponsors) {
    await expect(
      page.getByRole("img", {
        name: `${sponsor.Name} logo`,
      }),
    ).toHaveCount(0)
  }

  // Simulate the sponsor data sent by the backend.
  await startupPage.setGoldSponsors(goldSponsors)

  await expect(
    page.getByText(
      "Thanks to our Gold Sponsors for fueling the development of MobiFlight.",
    ),
  ).toBeVisible()

  for (const sponsor of goldSponsors) {
    const logo = page.getByRole("img", {
      name: `${sponsor.Name} logo`,
    })

    await expect(logo).toBeVisible()
    await expect(logo).toHaveAttribute("src", sponsor.LogoDataUri)
  }
})

test("Test that gold sponsor links open in external browser", async ({
  startupPage,
  page,
}) => {
  await startupPage.mobiFlightPage.trackCommand("CommandOpenLinkInBrowser")

  await startupPage.gotoStartupPage()
  await startupPage.setGoldSponsors(goldSponsors)

  for (const sponsor of goldSponsors) {
    await startupPage.mobiFlightPage.clearTrackedCommands()

    await page
      .getByRole("button", {
        name: `Open ${sponsor.Name} website`,
      })
      .dispatchEvent("click")

    const trackedCommands =
      await startupPage.mobiFlightPage.getTrackedCommands()

    expect(trackedCommands).toBeDefined()
    expect(trackedCommands).toHaveLength(1)

    expect(trackedCommands![0]).toEqual({
      key: "CommandOpenLinkInBrowser",
      payload: {
        url: sponsor.Url,
      },
    })
  }
})
