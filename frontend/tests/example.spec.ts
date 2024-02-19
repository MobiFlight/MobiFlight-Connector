import exp from 'constants';
import { test, expect } from './fixtures';
import * as Types from './../src/types';

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

test('Log messages are displayed correctly', async ({ mfPage, page }) => {
  await mfPage.page.goto('http://localhost:5173/');
  await mfPage.finishInitialLoading()
  var logMessage1:Types.ILogMessage = { Message: "This is a 1sttest message",  Severity: 'debug', Timestamp: new Date()}
  await mfPage.addLogMessage(logMessage1)
  await page.locator('div:nth-child(2) > a > .inline-flex').first().click();  
  await expect(mfPage.page.locator('tr:nth-child(1) > td:nth-child(3)').first()).toHaveText(logMessage1.Message)
  var logMessage2:Types.ILogMessage = { Message: "This is a 2nd test message",  Severity: 'info', Timestamp: new Date()}
  await mfPage.addLogMessage(logMessage2)
  await expect(mfPage.page.locator('tr:nth-child(2) > td:nth-child(3)').first()).toHaveText(logMessage2.Message)
});