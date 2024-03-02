import { test, expect } from "./fixtures"

test.beforeEach(async ({ devicePage, page }) => {
  await page.goto("http://localhost:5173/")
  await devicePage.useDefaultDeviceList()
  await devicePage.mfPage.finishInitialLoading()
})

test("Device detail navigation works", async ({ page }) => {
  await page.locator("a:nth-child(2) > .inline-flex").first().click()
  await page.getByRole("link", { name: "MobiFlight Mega 1 MobiFlight" }).click()
  await expect(
    page.getByRole("navigation").getByText("MobiFlight Mega"),
  ).toHaveCount(1)
})

test("Device Button add and remove works", async ({ page }) => {
  await page.locator("a:nth-child(2) > .inline-flex").first().click()
  await page.getByRole("link", { name: "MobiFlight Mega 1 MobiFlight" }).click()
  // now we are on the detail page
  await page.getByRole("button", { name: "Add device" }).click()
  await page.locator("div").filter({ hasText: "Button" }).nth(2).click()
  await expect(page.getByRole("link", { name: "Button" })).toHaveCount(1)
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

test("Device Encoder add and remove works", async ({ page }) => {
  await page.locator("a:nth-child(2) > .inline-flex").first().click()
  await page.getByRole("link", { name: "MobiFlight Mega 1 MobiFlight" }).click()
  // now we are on the detail page
  await page.getByRole("button", { name: "Add device" }).click()
  await page.locator("div").filter({ hasText: "Encoder" }).nth(2).click()
  await expect(page.getByRole("link", { name: "Encoder" })).toHaveCount(1)
  await page.getByRole("textbox").click()
  await page.getByRole("textbox").press("Home")
  // change the encoder name and
  // check that the UI updates accordingly
  await page.getByRole("textbox").fill("My Encoder")
  await expect(page.getByRole("link", { name: "My Encoder" })).toHaveCount(1)
  await expect(page.getByRole("textbox")).toHaveValue("My Encoder")

  
  // check all available options for the encoder
  await page.getByText("1 detent per cycle").click()
  await page.getByRole('option', { name: '1 detent per cycle (00)' }).click();
  await page.getByRole('combobox').filter({hasText:'1 detent per cycle (00)'}).click();
  await page.getByRole('option', { name: '2 detent per cycle (00, 11)' }).click();
  await page.getByRole('combobox').filter({hasText:'2 detent per cycle (00, 11)'}).click();
  await page.getByRole('option', { name: '2 detent per cycle (01, 10)' }).click();
  await page.getByRole('combobox').filter({hasText:'2 detent per cycle (01, 10)'}).click();
  await page.getByRole('option', { name: '4 detent per cycle' }).click();

  await page.getByText('2 (PWM)').click();
  await page.getByRole('option', { name: '6 (PWM)' }).click();
  await page.locator('div').filter({ hasText: /^Right Pin3 \(PWM\)$/ }).getByRole('combobox').click();
  await page.getByRole('option', { name: '7 (PWM)' }).nth(1).click();
  
  await expect(page.getByText("6 (PWM)")).toHaveCount(1)
  await expect(page.getByText("7 (PWM)")).toHaveCount(1)
  await expect(
    page.getByRole("button", { name: "Upload changes" }),
  ).toBeVisible()
  // remove the button
  await page
    .getByRole("link", { name: "My Encoder" })
    .getByRole("button")
    .click()
  await expect(page.getByRole("link", { name: "My Encoder" })).toHaveCount(0)
})
