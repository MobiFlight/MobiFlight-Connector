import { Message } from "@/types/messages";
import type { Page } from "@playwright/test";

export class MobiFlightPage {
  constructor(public readonly page: Page) {
    this.page.addInitScript(() => {
      if (!window.chrome?.webview?.postMessage) {
        console.log(
          "Setting up window.chrome.webview.postMessage for playwright testing."
        );
        window.chrome = {
          webview: {
            postMessage(message: object) {
              window.postMessage(message, "*");
            },
            addEventListener(
              message: string,
              callback: (event: Event) => void
            ) {
              window.addEventListener(message, callback);
            },
            removeEventListener(
              message: string,
              callback: (event: Event) => void
            ) {
              window.removeEventListener(message, callback);
            },
          },
        };
      }
    });
  }

  async publishMessage(message: Message) {
    const stringifiedObject = JSON.stringify(message);
    await this.page.addScriptTag({
      content: `window.postMessage(${stringifiedObject}, "*")`,
    });
  }
}
