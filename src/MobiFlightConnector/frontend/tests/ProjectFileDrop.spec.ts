import { Page } from "@playwright/test"
import { test, expect } from "./fixtures"

type DroppedMessage = { message: object; fileNames: string[] }

declare global {
  interface Window {
    droppedMessages?: DroppedMessage[]
  }
}

// postMessageWithAdditionalObjects only exists in WebView2,
// so the test records what the app would pass to the host.
const mockPostMessageWithAdditionalObjects = async (page: Page) => {
  await page.evaluate(() => {
    window.droppedMessages = []
    window.chrome!.webview!.postMessageWithAdditionalObjects = (
      message,
      additionalObjects,
    ) => {
      window.droppedMessages!.push({
        message,
        fileNames: Array.from(additionalObjects as ArrayLike<File>).map(
          (file) => file.name,
        ),
      })
    }
  })
}

const getDroppedMessages = async (page: Page) =>
  page.evaluate(() => window.droppedMessages)

// Dispatches dragover and drop like the browser does for files from the file system.
// Returns whether the app handled the drop, which keeps WebView2 from navigating away.
const dropFiles = async (page: Page, fileNames: string[]) =>
  page.evaluate((fileNames) => {
    const dataTransfer = new DataTransfer()
    fileNames.forEach((fileName) =>
      dataTransfer.items.add(new File(["{}"], fileName)),
    )
    const target = document.body
    const eventInit = { bubbles: true, cancelable: true, dataTransfer }
    const dragOver = new DragEvent("dragover", eventInit)
    target.dispatchEvent(dragOver)
    const drop = new DragEvent("drop", eventInit)
    target.dispatchEvent(drop)
    return {
      dragOverHandled: dragOver.defaultPrevented,
      dropHandled: drop.defaultPrevented,
    }
  }, fileNames)

test.describe("Project file drop", () => {
  test.beforeEach(async ({ configListPage, page }) => {
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()
    await mockPostMessageWithAdditionalObjects(page)
  })

  test("Dropping a project file opens it", async ({ page }) => {
    const result = await dropFiles(page, ["My Project.mfproj"])

    expect(result).toEqual({ dragOverHandled: true, dropHandled: true })
    expect(await getDroppedMessages(page)).toEqual([
      {
        message: { key: "CommandMainMenu", payload: { action: "file.open" } },
        fileNames: ["My Project.mfproj"],
      },
    ])
  })

  test("Dropping several files opens the first project file", async ({
    page,
  }) => {
    const result = await dropFiles(page, [
      "notes.txt",
      "Legacy Config.MCC",
      "My Project.mfproj",
    ])

    expect(result.dropHandled).toBe(true)
    expect(await getDroppedMessages(page)).toEqual([
      {
        message: { key: "CommandMainMenu", payload: { action: "file.open" } },
        fileNames: ["Legacy Config.MCC"],
      },
    ])
  })

  test("Dropping an unsupported file shows a notification", async ({
    page,
  }) => {
    const toastUnsupportedFile = page.getByTestId(
      "toast-project-file-drop-unsupported",
    )
    await expect(toastUnsupportedFile).not.toBeVisible()

    const result = await dropFiles(page, ["notes.txt"])

    expect(result).toEqual({ dragOverHandled: true, dropHandled: true })
    await expect(toastUnsupportedFile).toBeVisible()
    await expect(toastUnsupportedFile).toContainText("File can't be opened")
    await expect(toastUnsupportedFile).toContainText(".mfproj, .mcc, .aic")
    expect(await getDroppedMessages(page)).toEqual([])
  })

  test("Dragging without files is left to the app", async ({ page }) => {
    const result = await dropFiles(page, [])

    expect(result).toEqual({ dragOverHandled: false, dropHandled: false })
    expect(await getDroppedMessages(page)).toEqual([])
  })

  test("Dropping a project file while a modal is open is ignored", async ({
    page,
  }) => {
    await page.evaluate(() => {
      const modal = document.createElement("div")
      modal.setAttribute("role", "dialog")
      document.body.appendChild(modal)
    })

    const result = await dropFiles(page, ["My Project.mfproj"])

    expect(result).toEqual({ dragOverHandled: true, dropHandled: true })
    expect(await getDroppedMessages(page)).toEqual([])
  })

  test("Dropping a project file on the startup screen is ignored", async ({
    page,
  }) => {
    await page.goto("/start")
    await expect(page.locator("#root > div.fixed")).toBeVisible()
    await mockPostMessageWithAdditionalObjects(page)

    const result = await dropFiles(page, ["My Project.mfproj"])

    expect(result).toEqual({ dragOverHandled: true, dropHandled: true })
    expect(await getDroppedMessages(page)).toEqual([])
  })
})
