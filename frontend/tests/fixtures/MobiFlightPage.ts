import { CommandMessageKey, CommandMessage } from "@/types/commands"
import { AppMessage } from "@/types/messages"
import type { Page } from "@playwright/test"

export class MobiFlightPage {
  constructor(public readonly page: Page) {
    this.page.addInitScript(() => {
      if (!window.chrome?.webview?.postMessage) {
        console.log(
          "Setting up window.chrome.webview.postMessage for playwright testing.",
        )
        window.chrome = {
          webview: {
            postMessage(message: object) {
              window.postMessage(message, "*")
            },
            addEventListener(
              message: string,
              callback: (event: Event) => void,
            ) {
              window.addEventListener(message, callback)
            },
            removeEventListener(
              message: string,
              callback: (event: Event) => void,
            ) {
              window.removeEventListener(message, callback)
            },
          },
        }
      }
    })
  }

  async publishCommand(message: CommandMessage) {
    const stringifiedObject = JSON.stringify(message)
    await this.page.addScriptTag({
      content: `window.postMessage(${stringifiedObject}, "*")`,
    })
  }

  async publishMessage(message: AppMessage) {
    const stringifiedObject = JSON.stringify(message)
    await this.page.addScriptTag({
      content: `window.postMessage(${stringifiedObject}, "*")`,
    })
  }

  async subscribeToCommand(
    key: CommandMessageKey,
    callback: (message: CommandMessage) => Promise<void>,
  ) {
    await this.page.evaluate(
      ({ key, callbackStr }) => {
        // Deserialize the function from string
        const callback = new Function("return " + callbackStr)()

        window.addEventListener("message", async (event: Event) => {
          const appMessage = (event as MessageEvent).data as CommandMessage
          if (appMessage.key === key) {
            await callback(appMessage) // Call the passed lambda function
          }
        })
      },
      {
        key,
        callbackStr: callback.toString(), // Serialize the function to a string
      },
    )
  }
}
