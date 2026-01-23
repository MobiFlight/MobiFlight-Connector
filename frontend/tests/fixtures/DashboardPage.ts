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
}
