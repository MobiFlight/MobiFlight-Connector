import { test, expect } from "./fixtures";

test("Device list is displayed correctly", async ({ devicePage, page }) => {
  let imageLoadError = false;
  await devicePage.mfPage.gotoStartPage()
  page.on("console", (msg) => {
    // check for net::ERR_INVALID_URL error
    // this error is thrown when the image url is not correct
    if (msg.text().includes('net::ERR_INVALID_URL')) {
      console.log('net::ERR_INVALID_URL error detected');
      imageLoadError = true;
    }
  });
  await devicePage.mfPage.loadDefaultConfig();
  await devicePage.useDefaultDeviceList();
  await devicePage.mfPage.finishInitialLoading();
  await expect(page.getByText("Number:")).toContainText("Number: 5");
  await page.getByRole("button").nth(1).click();
  await expect(page.getByText("MobiFlight Mega 1")).toHaveCount(1);
  await expect(page.getByText("FlightSimBuilder G1000 PFD")).toHaveCount(1);
  await expect(page.getByText("Midi Device 1")).toHaveCount(1);
  expect(imageLoadError).toBe(false);
});
