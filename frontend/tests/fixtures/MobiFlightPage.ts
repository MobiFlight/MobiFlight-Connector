import { CommandMessageKey, CommandMessage } from "@/types/commands"
import { AppMessage, ProjectStatus } from "@/types/messages"
import { expect, type Locator, type Page } from "@playwright/test"
import testProject from "../data/project.testdata.json" with { type: "json" }
import recentProjects from "../data/recentProjects.testdata.json" with { type: "json" }
import connectedControllers from "../data/connectedControllers.testdata.json" with { type: "json" }
import { Project } from "@/types"
import { ProjectInfo } from "@/types/project"
import { ControllerBinding } from "@/types/controller"
import { AuthContextProps } from "react-oidc-context"

declare global {
  interface Window {
    commands?: CommandMessage[]
    auth?: Partial<AuthContextProps>
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

  async gotoPage() {
    await this.page.goto("http://localhost:5173/", {
      waitUntil: "networkidle",
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
    await this.subscribeToCommand(key, async (message) => {
      if (window.commands === undefined) {
        window.commands = []
      }
      window.commands.push(message)
    })
  }

  async getTrackedCommands() {
    // Small delay to ensure commands are captured
    // this was needed when upgrading playwright version to 1.56.1
    await this.page.waitForTimeout(10)
    return await this.page.evaluate(() => window.commands)
  }

  async clearTrackedCommands() {
    await this.page.evaluate(() => {
      window.commands = []
    })
  }

  getTooltipByText(text: string): Locator {
    return this.page.getByRole("tooltip").filter({ hasText: text })
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
          FSUIPC: false,
          ProSim: false,
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
    await this.initWithConnectedControllers()
  }

  async initWithTestDataAndSpecificProfileCount(profileCount: number) {
    const profiles = testProject.ConfigFiles.slice(0, profileCount)
    const testProjectWithProfiles = {
      ...testProject,
      ConfigFiles: profiles,
    }

    const message: AppMessage = {
      key: "Project",
      payload: testProjectWithProfiles,
    }
    await this.publishMessage(message)
    await this.initWithRecentProjects()
    await this.initWithConnectedControllers()
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

  async initWithConnectedControllers() {
    const connectedControllersMessage: AppMessage = {
      key: "ConnectedControllers",
      payload: {
        Controllers: connectedControllers,
      },
    }
    await this.publishMessage(connectedControllersMessage)
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

  async openControllerBindingsDialog() {
    const menuItemExtras = this.page
      .getByRole("menubar")
      .getByRole("menuitem", { name: "Extras" })
    const menuItemManageControllerBindings = this.page.getByRole("menuitem", {
      name: "Controller Bindings",
    })
    const dialog = this.page.getByRole("dialog", {
      name: "Controller Bindings",
    })

    await menuItemExtras.click()
    await menuItemManageControllerBindings.click()
    await dialog.waitFor({ state: "visible" })
  }

  getControllerBindings() {
    return (testProject as Project).ControllerBindings as ControllerBinding[]
  }

  async updateProjectState(projectStatus: Partial<ProjectStatus>) {
    const message: AppMessage = {
      key: "ProjectStatus",
      payload: projectStatus,
    }
    await this.publishMessage(message)
  }

  async setupSignInUser(user: {
    email: string
    password: string
    name: string
  }) {
    await this.page.goto("http://localhost:5173/home")
    await this.trackCommand("CommandUserAuthentication")

    const signInButton = this.page.getByRole("button", { name: "Sign in" })
    await expect(signInButton).toBeVisible()
    await expect(signInButton).toBeEnabled()

    await signInButton.click()

    // Validate correct command is sent to the backend
    const signInCommands = await this.getTrackedCommands()
    expect(signInCommands).toBeDefined()
    expect(signInCommands!.length).toBe(1)

    const signInCommand = signInCommands!.pop()
    expect(signInCommand).toBeDefined()
    expect(signInCommand?.key).toBe("CommandUserAuthentication")
    expect(signInCommand?.payload).toEqual({
      flow: "login",
      state: "started",
      url: `http://localhost:5173/auth/login`,
    })

    await this.clearTrackedCommands()

    // Initiate the sign in flow
    // This is done by the second WebView once it receives
    // CommandUserAuthentication with flow: login and state: started
    await this.page.goto(`http://localhost:5173/auth/login`)

    const emailInput = this.page.getByPlaceholder("Email address")
    const nextButton = this.page.getByRole("button", { name: "Next" })
    await expect(emailInput).toBeVisible()
    await emailInput.fill(user.email)
    await nextButton.click()

    const passwordInput = this.page.getByPlaceholder("Password")
    await expect(passwordInput).toBeVisible()
    await passwordInput.fill(user.password)
    await signInButton.click()

    await expect(this.page.getByText("Stay signed in")).toBeVisible()
    const noButton = this.page.getByRole("button", { name: "No" })
    await expect(noButton).toBeVisible()
    await noButton.click()

    await this.page.waitForURL(
      /http:\/\/localhost:5173\/auth\/callback\/login\?code=.+&state=.+/,
    )

    await expect(this.page.getByText("Success!")).toBeVisible()
  }

  async signInUser() {
    // Go back to the dashboard (required for the hook that listens to authentication changes)
    // simulate the response by the second WebView
    // after successful sign in
    // this triggers the reload of auth object
    await this.page.goto("http://localhost:5173/home")
    await this.publishMessage({
      key: "AuthenticationStatus",
      payload: {
        Authenticated: true,
      },
    })
  }
}
