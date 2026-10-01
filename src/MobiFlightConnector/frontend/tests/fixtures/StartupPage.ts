import { StatusBarUpdate } from "@/types";
import {
    GoldSponsor,
    GoldSponsorsUpdate,
} from "@/types/messages"
import { MobiFlightPage } from "./MobiFlightPage";
import * as Types from "@/types";

export class StartupPage {
  constructor(public readonly mobiFlightPage: MobiFlightPage) {}

  async gotoStartupPage() {
    await this.mobiFlightPage.page.goto("http://localhost:5173/start", { waitUntil: "networkidle" });
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

    async setGoldSponsors(sponsors: GoldSponsor[]) {
        const message: Types.AppMessage = {
            key: "GoldSponsorsUpdate",
            payload: {
                Sponsors: sponsors,
            } as GoldSponsorsUpdate,
        }

        await this.mobiFlightPage.publishMessage(message)
    }
}
