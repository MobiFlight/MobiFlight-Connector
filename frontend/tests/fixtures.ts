import { test as base, expect } from '@playwright/test';
import { MFPage } from './../src/lib/mfpage';

// Declare the types of your fixtures.
type MFFixtures = {
  mfPage: MFPage;
};

export const test = base.extend<{mfPage: MFPage}>({
  mfPage: async ({ page }, use) => {
    const mfPage = new MFPage(page);
    await use(mfPage);
  }
});

export { expect } from '@playwright/test'