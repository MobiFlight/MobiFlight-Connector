/* eslint-disable react-hooks/rules-of-hooks */
import { test as base } from '@playwright/test';
import { MobiFlightPage } from './fixtures/MobiFlightPage';
import { StartupPage } from './fixtures/StartupPage';

// Declare the types of your fixtures.
type MFFixtures = {
  mobiFlightPage: MobiFlightPage,
  startupPage: StartupPage,
};

export const test = base.extend<MFFixtures>({
  startupPage: async ({ page }, use) => {
    const settingsPage = new StartupPage(new MobiFlightPage(page))
    await use(settingsPage)
  }
});

export { expect } from '@playwright/test'