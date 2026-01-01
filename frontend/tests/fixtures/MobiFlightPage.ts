import { CommandMessageKey, CommandMessage } from "@/types/commands"
import { AppMessage } from "@/types/messages"
import type { Locator, Page } from "@playwright/test"
import testProject from "../data/project.testdata.json" with { type: "json" }
import recentProjects from "../data/recentProjects.testdata.json" with { type: "json" }
import { Project } from "@/types"
import { ProjectInfo } from "@/types/project"


declare global {
  interface Window {
    commands?: CommandMessage[];
  }
}

export class MobiFlightPage {
  readonly PostedMessages: AppMessage[] = []
  readonly PostedCommands: CommandMessage[] = []

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

  async trackCommand(key: CommandMessageKey) {
    await this.subscribeToCommand(
      key,
      async (message) => {
        if (window.commands === undefined) {
          window.commands = []
        }
        window.commands.push(message)
      },
    )
  }

  async getTrackedCommands() {
    // Small delay to ensure commands are captured
    // this was needed when upgrading playwright version to 1.56.1
    await this.page.waitForTimeout(10) 
    return await this.page.evaluate(() => window.commands);
  }

  getTooltipByText(text: string): Locator {
    return this.page.getByRole("tooltip").filter({hasText:text})
  }

  async initWithEmptyData() {
    const message: AppMessage = {
      key: "Project",
      payload: {
        Name: "Test Project",
        FilePath: "SomeFilePath.mfproj",
        ConfigFiles: [],
        Sim: "msfs",
        Features: {
            "FSUIPC": false,
            "ProSim": false
        },
        ControllerBindings: [],
      } as Project,
    }
    await this.publishMessage(message)
  }

  async initWithTestData() {
    const message: AppMessage = {
      key: "Project",
      payload: testProject,
    }
    await this.publishMessage(message)
    await this.initWithRecentProjects()
  }

  async initWithRecentProjects() {
    const recentProjectsMessage: AppMessage = {
      key: "RecentProjects",
      payload: {
        Projects: recentProjects as ProjectInfo[],
      },
    }
    await this.publishMessage(recentProjectsMessage)
  }

  getRecentProjects(): ProjectInfo[] {
    return recentProjects as ProjectInfo[]
  }

  async initWithTestDataAndSpecificProjectProps(props: Partial<Project>) {
    const testProjectWithProps = {
      ...testProject,
      ...props,
    }

    const message: AppMessage = {
      key: "Project",
      payload: testProjectWithProps,
    }
    await this.publishMessage(message)
    await this.initWithRecentProjects()
  }
}
