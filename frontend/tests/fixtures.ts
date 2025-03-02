/* eslint-disable react-hooks/rules-of-hooks */
import { test as base } from '@playwright/test';
import { MobiFlightPage } from './fixtures/MobiFlightPage';
import { StartupPage } from './fixtures/StartupPage';
import { ConfigListPage } from './fixtures/ConfigListPage';

// Declare the types of your fixtures.
type MFFixtures = {
  mobiFlightPage: MobiFlightPage,
  startupPage: StartupPage,
  configListPage: ConfigListPage
};

export const test = base.extend<MFFixtures>({
  startupPage: async ({ page }, use) => {
    const settingsPage = new StartupPage(new MobiFlightPage(page))
    await use(settingsPage)
  },
  configListPage: async ({ page }, use) => {
    const configListPage = new ConfigListPage(new MobiFlightPage(page))
    await use(configListPage)
  },
});

export { expect } from '@playwright/test'