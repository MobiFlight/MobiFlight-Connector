import { test as base } from '@playwright/test';
import { MFPage } from './fixtures/mfpage';
import { DevicePage } from './fixtures/DevicePage';
import { SettingsPage } from './fixtures/SettingsPage';

// Declare the types of your fixtures.
type MFFixtures = {
  mfPage: MFPage,
  devicePage: DevicePage,
  settingsPage: SettingsPage
};

export const test = base.extend<MFFixtures>({
  mfPage: async ({ page }, use) => {
    const mfPage = new MFPage(page)
    await use(mfPage)
  },
  devicePage: async ({ page }, use) => {
    const devicePage = new DevicePage(new MFPage(page))
    await use(devicePage)
  },
  settingsPage: async ({ page }, use) => {
    const settingsPage = new SettingsPage(new MFPage(page))
    await use(settingsPage)
  }
});

export { expect } from '@playwright/test'