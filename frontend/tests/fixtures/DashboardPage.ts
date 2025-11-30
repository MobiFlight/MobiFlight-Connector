import { MobiFlightPage } from "./MobiFlightPage";

export class DashboardPage {
  constructor(public readonly mobiFlightPage: MobiFlightPage) {}

  async gotoPage() {
    await this.mobiFlightPage.page.goto("http://localhost:5173/home", { waitUntil: "networkidle" });
  }
}
