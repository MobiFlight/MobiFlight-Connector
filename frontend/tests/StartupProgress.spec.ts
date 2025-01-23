import { test, expect } from './fixtures';

test('Go beyond progress bar', async ({ startupPage, page }) => {
  await startupPage.gotoStartupPage()
  await startupPage.setStatusBarUpdate(50,"Loading!")
  // expect to have exactly one progressBar
  await expect(page.getByRole('progressbar')).toHaveCount(1)  
  // expect the progressBar to be visible
  await expect(page.getByRole('progressbar').isVisible()).toBeTruthy()
  await startupPage.setStatusBarUpdate(100,"Finished!")
  // expect the progressBar to be removed  
  await expect(page.getByRole('progressbar')).toHaveCount(0)
  // expect the heading to be visible
  // this will have to change later to a more specific test
  await page.getByRole('heading', { name: 'MobiFlight 2025' }).isVisible()
})