import { test as base } from '@playwright/test';
import { MFPage } from './../src/fixtures/mfpage';
import { DevicePage } from './../src/fixtures/DevicePage';

// Declare the types of your fixtures.
type MFFixtures = {
  mfPage: MFPage,
  devicePage: DevicePage
};

export const test = base.extend<MFFixtures>({
  mfPage: async ({ page }, use) => {
    const mfPage = new MFPage(page)
    await use(mfPage)
  },
  devicePage: async ({ page }, use) => {
    const devicePage = new DevicePage(new MFPage(page))
    await use(devicePage)
  }
});

export { expect } from '@playwright/test'