import exp from 'constants';
import { test, expect } from './fixtures';
import * as Types from './../src/types';

test('Device list is displayed correctly', async ({ devicePage, page }) => {
  await devicePage.page.page.goto('http://localhost:5173/');
  await devicePage.page.loadDefaultConfig();
  await devicePage.useDefaultDeviceList();
  await devicePage.page.finishInitialLoading();
  await expect(devicePage.page.page.getByText('Number:')).toContainText('Number: 5');
  await page.getByRole('button').nth(1).click();
  await expect(devicePage.page.page.getByText('MobiFlight Mega 1')).toHaveCount(1);
  await expect(devicePage.page.page.getByText('FlightSimBuilder G1000 PFD')).toHaveCount(1);
  await expect(devicePage.page.page.getByText('Midi Device 1')).toHaveCount(1);
})