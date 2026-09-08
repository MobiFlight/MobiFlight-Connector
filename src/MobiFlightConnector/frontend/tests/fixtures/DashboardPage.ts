import { CommunityPost } from "@/types/feed"
import { MobiFlightPage } from "./MobiFlightPage"

export class DashboardPage {
  constructor(public readonly mobiFlightPage: MobiFlightPage) {}

  async gotoPage() {
    await this.mobiFlightPage.page.goto("http://localhost:5173/home", {
      waitUntil: "networkidle",
    })
  }

  async gotoPageAndTriggerError(testid: string) {
    await this.mobiFlightPage.page.goto(
      `http://localhost:5173/home?triggerError=true&testid=${testid}`,
      {
        waitUntil: "networkidle",
      },
    )
  }

  async disableDynamicFeed(remoteFeedBaseUrl: string, language = "en") {
    const feedLanguage = language.split("-")[0]

    await this.mobiFlightPage.page.route(
      `${remoteFeedBaseUrl}/${feedLanguage}/feed.json`,
      async (route) => {
        await route.fulfill({
          status: 404,
          contentType: "application/json",
          body: JSON.stringify({}),
        })
      },
    )
  }

  async mockDynamicFeed(
    remoteFeedBaseUrl: string,
    data: CommunityPost[],
    language = "en",
  ) {
    const feedLanguage = language.split("-")[0]

    await this.mobiFlightPage.page.route(
      `${remoteFeedBaseUrl}/${feedLanguage}/feed.json`,
      async (route) => {
        await route.fulfill({
          status: 200,
          contentType: "application/json",
          body: JSON.stringify({ community: data }),
        })
      },
    )
  }
}
