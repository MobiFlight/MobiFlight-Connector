import { IConfigItem, ILogMessage, Notification } from "@/types";
import { ConfigLoadedEvent, Message, AppMessage } from "@/types/messages";
import type { Page } from "@playwright/test";

export class MFPage {
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
    const stringifiedObject = JSON.stringify(JSON.stringify(message));
    await this.page.addScriptTag({
      content: `window.postMessage(${stringifiedObject}, "*")`,
    });
  }

  async finishInitialLoading() {
    await this.publishMessage({
      key: "StatusBarUpdate",
      payload: { Value: 100, Text: "Finished." },
    });
  }

  async loadConfig(config: ConfigLoadedEvent) {
    const message: AppMessage = { key: "ConfigFile", payload: config };
    await this.publishMessage(message);
  }

  async loadDefaultConfig() {
    const configItems: IConfigItem[] = [
      {
        GUID: "123",
        Active: true,
        Description: "My first item",
        Device: "Device 1",
        Component: "Component 1",
        Type: "Type 1",
        Tags: [],
        Status: [],
        RawValue: "10",
        ModifiedValue: "11",
      },
    ];
    await this.loadConfig({ FileName: "default", ConfigItems: configItems });
  }

  async addLogMessage(log: ILogMessage) {
    const message: AppMessage = { key: "LogMessage", payload: log };
    await this.publishMessage(message);
  }

  async gotoStartPage() {
    await this.page.goto("http://localhost:5173/", { waitUntil: "networkidle" });
  }

  async postNotification(notification: Notification) {
    const message: AppMessage = {
      key: "Notification",
      payload: notification,
    };
    await this.publishMessage(message);
  }
}
