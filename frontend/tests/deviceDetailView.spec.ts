import exp from "constants"
import { test, expect } from "./fixtures"

test("Device detail navigation works", async ({ devicePage, page }) => {
  await page.goto("http://localhost:5173/")
  await devicePage.useDefaultDeviceList()
  await devicePage.mfPage.finishInitialLoading()
  await page.locator("a:nth-child(2) > .inline-flex").first().click()
  await page.getByRole("link", { name: "MobiFlight Mega 1 MobiFlight" }).click()
  await expect(
    page.getByRole("navigation").getByText("MobiFlight Mega"),
  ).toHaveCount(1)

  await page.getByRole("button", { name: "Add device" }).click()
  await page
    .locator("div:nth-child(2) > .inline-block > .stroke-teal-600")
    .click()
  await page.getByRole("link", { name: "Encoder0" }).click()
  await page.getByRole("textbox").click()
  await page.getByRole("textbox").press("End")
  await page.getByRole("textbox").fill("Encoder 2")
  await page.getByRole("link", { name: "Encoder" }).click()
  await expect(
    page
      .getByRole("link", { name: "Encoder" })
      .filter({ hasText: "Encoder 2" }),
  ).toHaveCount(1)
})

test("Device Button add and removeworks", async ({ devicePage, page }) => {
  await page.goto("http://localhost:5173/")
  await devicePage.useDefaultDeviceList()
  await devicePage.mfPage.finishInitialLoading()
  await page.locator("a:nth-child(2) > .inline-flex").first().click()
  await page.getByRole("link", { name: "MobiFlight Mega 1 MobiFlight" }).click()
  // now we are on the detail page
  await page.getByRole("button", { name: "Add device" }).click()
  await page.locator("div").filter({ hasText: "Button" }).nth(2).click()
  await expect(page.getByRole("link", { name: "Button0" })).toHaveCount(1)
  await page.getByRole("textbox").click()
  await page.getByRole("textbox").press("Home")
  await page.getByRole("textbox").fill("My Button")
  await page.getByText("(PWM)").click()
  await page.getByRole("option", { name: "4 (PWM)" }).click()
  await expect(page.getByRole("link", { name: "My Button" })).toHaveCount(1)
  await expect(page.getByRole("textbox")).toHaveValue("My Button")
  await expect(page.getByText("(PWM)").getByText("(PWM)")).toHaveCount(1)
  await expect(
    page.getByRole("button", { name: "Upload changes" }),
  ).toBeVisible()
  // remove the button
  await page
    .getByRole("link", { name: "My Button" })
    .getByRole("button")
    .click()
  await expect(page.getByRole("link", { name: "My Button" })).toHaveCount(0)
})
