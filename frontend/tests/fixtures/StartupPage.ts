import { StatusBarUpdate } from "@/types";
import { MobiFlightPage } from "./MobiFlightPage";
import * as Types from "@/types";

export class StartupPage {
  constructor(public readonly mobiFlightPage: MobiFlightPage) {}

  async gotoStartupPage() {
    await this.mobiFlightPage.page.goto("http://localhost:5173", { waitUntil: "networkidle" });
  }

  async setStatusBarUpdate(value: number, text: string) {
    const message: Types.AppMessage = {
      key: "StatusBarUpdate",
      payload: { 
        Value: value,
        Text: text,
      } as StatusBarUpdate,
    };
    await this.mobiFlightPage.publishMessage(message);
  }
}
