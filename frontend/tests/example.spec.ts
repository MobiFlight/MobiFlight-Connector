import exp from 'constants';
import { test, expect } from './fixtures';

test('Go beyond progress bar', async ({ mfPage, page }) => {
  await mfPage.page.goto('http://localhost:5173/');
  await mfPage.publishMessage({ key: "StatusBarUpdate", payload: { Value: 50, Text: "Loading!" } })
  // expect the progressBar to show 50%
  await expect(mfPage.page.getByRole('progressbar')).toHaveCount(1)  
  await expect(mfPage.page.getByRole('progressbar').isVisible()).toBeTruthy()
  await mfPage.publishMessage({ key: "StatusBarUpdate", payload: { Value: 100, Text: "Finished!" } })
  await expect(mfPage.page.getByRole('progressbar')).toHaveCount(0)
  await expect(mfPage.page.getByRole('heading', { name: 'Projects' })).toHaveCount(1)
});

test('Go to configs', async ({ mfPage, page }) => {
  await mfPage.page.goto('http://localhost:5173/');
  await mfPage.finishInitialLoading()
  await mfPage.loadDefaultConfig()
  await page.locator('.inline-flex').first().click();
  await expect(mfPage.page.getByRole('cell', { name: 'My first item' })).toHaveCount(1)
  await page.getByRole('button', { name: 'Devices' }).click();
  await page.getByRole('option', { name: 'Device 1' }).locator('div').click();
});