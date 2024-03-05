import { test, expect } from "./fixtures"

test("Settings are showing the default settings correctly", async ({
  settingsPage,
  mfPage,
  page,
}) => {
  await mfPage.gotoStartPage()
  await mfPage.loadDefaultConfig()
  await settingsPage.loadDefaultSettings()
  await mfPage.finishInitialLoading()

  // navigate to settings
  await page.getByRole("button").nth(3).click()

  expect(page.getByRole("button", { name: "Save changes" })).toBeDisabled()
  // check if the default settings are shown correctly
  // test the max and min settings for the recent files
  expect(page.getByRole("button", { name: "10" })).toBeVisible()
  await page
    .getByRole("button", { name: "10" })
    .getByRole("button")
    .nth(0)
    .click()
  expect(page.getByRole("button", { name: "10" })).toBeVisible()
  expect(page.getByRole("button", { name: "Save changes" })).toBeEnabled()
  await page
    .getByRole("button", { name: "10" })
    .getByRole("button")
    .nth(1)
    .click()
  await page
    .getByRole("button", { name: "9" })
    .getByRole("button")
    .nth(1)
    .click()
  await page
    .getByRole("button", { name: "8" })
    .getByRole("button")
    .nth(1)
    .click()
  await page
    .getByRole("button", { name: "7" })
    .getByRole("button")
    .nth(1)
    .click()
  await page
    .getByRole("button", { name: "6" })
    .getByRole("button")
    .nth(1)
    .click()
  await page
    .getByRole("button", { name: "5" })
    .getByRole("button")
    .nth(1)
    .click()
  await page
    .getByRole("button", { name: "4" })
    .getByRole("button")
    .nth(1)
    .click()
  await page
    .getByRole("button", { name: "3" })
    .getByRole("button")
    .nth(1)
    .click()
  await page
    .getByRole("button", { name: "2" })
    .getByRole("button")
    .nth(1)
    .click()
  expect(page.getByRole("button", { name: "1" })).toBeVisible()
  await page
    .getByRole("button", { name: "1" })
    .getByRole("button")
    .nth(1)
    .click()
  expect(page.getByRole("button", { name: "1" })).toBeVisible()
  // switch beta updates
  expect(
    await page
      .locator("div")
      .filter({ hasText: /^Beta updates$/ })
      .getByRole("switch")
      .isChecked(),
  ).toBeTruthy()
  await page
    .locator("div")
    .filter({ hasText: /^Beta updates$/ })
    .getByRole("switch")
    .click()
  expect(
    await page
      .locator("div")
      .filter({ hasText: /^Beta updates$/ })
      .getByRole("switch")
      .isChecked(),
  ).toBeFalsy()
  // switch community feedback
  expect(
    await page
      .locator("div")
      .filter({ hasText: /^Community feedback$/ })
      .getByRole("switch")
      .isChecked(),
  ).toBeTruthy()
  await page
    .locator("div")
    .filter({ hasText: /^Community feedback$/ })
    .getByRole("switch")
    .click()
  expect(
    await page
      .locator("div")
      .filter({ hasText: /^Community feedback$/ })
      .getByRole("switch")
      .isChecked(),
  ).toBeFalsy()
  // switch joystick support
  expect(
    page
      .locator("div")
      .filter({ hasText: /^Joystick support$/ })
      .getByRole("switch"),
  ).toBeChecked()
  await page
    .locator("div")
    .filter({ hasText: /^Joystick support$/ })
    .getByRole("switch")
    .click()
  expect(
    page
      .locator("div")
      .filter({ hasText: /^Joystick support$/ })
      .getByRole("switch"),
  ).not.toBeChecked()
  expect(
    page
      .locator("div")
      .filter({ hasText: /^Midi device support$/ })
      .getByRole("switch"),
  ).toBeChecked()
  await page
    .locator("div")
    .filter({ hasText: /^Midi device support$/ })
    .getByRole("switch")
    .click()
  expect(
    page
      .locator("div")
      .filter({ hasText: /^Midi device support$/ })
      .getByRole("switch"),
  ).not.toBeChecked()

  expect(page.getByRole("combobox").nth(0)).toHaveText("1 sec")
  // test the options one by one
  await page.getByRole("combobox").nth(0).click()
  await page.getByRole("option", { name: "500ms" }).click()
  expect(page.getByRole("combobox").nth(0)).toHaveText("500ms")
  await page.getByRole("combobox").nth(0).click()
  await page.getByRole("option", { name: "250ms" }).click()
  expect(page.getByRole("combobox").nth(0)).toHaveText("250ms")
  await page.getByRole("combobox").nth(0).click()
  await page.getByRole("option", { name: "100ms", exact: true }).click()
  expect(page.getByRole("combobox").nth(0)).toHaveText("100ms")
  await page.getByRole("combobox").nth(0).click()
  await page.getByRole("option", { name: "50ms", exact: true }).click()
  expect(page.getByRole("combobox").nth(0)).toHaveText("50ms")

  expect(await page.getByRole("combobox").nth(1)).toHaveText("System Default")
  await page.getByRole("combobox").nth(1).click()
  await page.getByRole("option", { name: "English" }).click()
  expect(page.getByRole("combobox").nth(1)).toHaveText("English")
  await page.getByRole("combobox").nth(1).click()
  await page.getByRole("option", { name: "Deutsch" }).click()
  expect(page.getByRole("combobox").nth(1)).toHaveText("Deutsch")

  // test logging options
  expect(
    page
      .locator("div")
      .filter({ hasText: /^Enabled$/ })
      .getByRole("switch"),
  ).toBeChecked()
  await page
    .locator("div")
    .filter({ hasText: /^Enabled$/ })
    .getByRole("switch")
    .click()
  expect(
    page
      .locator("div")
      .filter({ hasText: /^Enabled$/ })
      .getByRole("switch"),
  ).not.toBeChecked()

  expect(page.getByRole("combobox").nth(2)).toHaveText("Debug")
  await page.getByRole("combobox").nth(2).click()
  await page.getByRole("option", { name: "Warning" }).click()
  expect(page.getByRole("combobox").nth(2)).toHaveText("Warning")
  await page.getByRole("combobox").nth(2).click()
  await page.getByRole("option", { name: "Error" }).click()
  expect(page.getByRole("combobox").nth(2)).toHaveText("Error")

  expect(
    page
      .locator("div")
      .filter({ hasText: /^Log joystick axis$/ })
      .getByRole("switch"),
  ).toBeChecked()
  await page
    .locator("div")
    .filter({ hasText: /^Log joystick axis$/ })
    .getByRole("switch")
    .click()
  expect(
    page
      .locator("div")
      .filter({ hasText: /^Log joystick axis$/ })
      .getByRole("switch"),
  ).not.toBeChecked()

  expect(
    page
      .locator("div")
      .filter({ hasText: /^Auto retrigger$/ })
      .getByRole("switch"),
  ).toBeChecked()
  await page
    .locator("div")
    .filter({ hasText: /^Auto retrigger$/ })
    .getByRole("switch")
    .click()
  expect(
    page
      .locator("div")
      .filter({ hasText: /^Auto retrigger$/ })
      .getByRole("switch"),
  ).not.toBeChecked()

  expect(
    page
      .locator("div")
      .filter({ hasText: /^Auto run$/ })
      .getByRole("switch"),
  ).toBeChecked()
  await page
    .locator("div")
    .filter({ hasText: /^Auto run$/ })
    .getByRole("switch")
    .click()
  expect(
    page
      .locator("div")
      .filter({ hasText: /^Auto run$/ })
      .getByRole("switch"),
  ).not.toBeChecked()

  expect(
    page
      .locator("div")
      .filter({ hasText: /^Auto load linked config$/ })
      .getByRole("switch"),
  ).toBeChecked()
  await page
    .locator("div")
    .filter({ hasText: /^Auto load linked config$/ })
    .getByRole("switch")
    .click()
  expect(
    page
      .locator("div")
      .filter({ hasText: /^Auto load linked config$/ })
      .getByRole("switch"),
  ).not.toBeChecked()

  expect(
    page
      .locator("div")
      .filter({ hasText: /^Firmware auto update check$/ })
      .getByRole("switch"),
  ).toBeChecked()
  await page
    .locator("div")
    .filter({ hasText: /^Firmware auto update check$/ })
    .getByRole("switch")
    .click()
  expect(
    page
      .locator("div")
      .filter({ hasText: /^Firmware auto update check$/ })
      .getByRole("switch"),
  ).not.toBeChecked()

  expect(
    page
      .locator("div")
      .filter({ hasText: /^Hub hop auto check$/ })
      .getByRole("switch"),
  ).toBeChecked()
  await page
    .locator("div")
    .filter({ hasText: /^Hub hop auto check$/ })
    .getByRole("switch")
    .click()
  expect(
    page
      .locator("div")
      .filter({ hasText: /^Hub hop auto check$/ })
      .getByRole("switch"),
  ).not.toBeChecked()

  expect(
    page
      .locator("div")
      .filter({ hasText: /^Minimize on auto run$/ })
      .getByRole("switch"),
  ).toBeChecked()
  await page
    .locator("div")
    .filter({ hasText: /^Minimize on auto run$/ })
    .getByRole("switch")
    .click()
  expect(
    page
      .locator("div")
      .filter({ hasText: /^Minimize on auto run$/ })
      .getByRole("switch"),
  ).not.toBeChecked()
  await page.locator("div:nth-child(2) > a > .inline-flex").first().click()
  await page.getByRole("heading", { name: "You have unsaved changes" }).click()
  await page.getByRole("button", { name: "Cancel" }).click()
  await page.getByRole("button", { name: "Save changes" }).click()
  await expect(
    page.getByRole("button", { name: "Save changes" }),
  ).toBeDisabled()
})
