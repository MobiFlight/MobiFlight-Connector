import { test, expect } from '@playwright/test';

test('has title', async ({ page }) => {
  await page.goto('/');

  // Expect a title "to contain" a substring.
  await expect(page).toHaveTitle(/MobiFlight/);
});

test('Navigate to first project', async ({ page }) => {
  
  await page.goto('http://localhost:5173/');
  await expect(page.getByRole('heading', { name: 'Hello world' })).toHaveCount(1)
  await expect(page.getByRole('heading', { name: 'Projects' })).toHaveCount(1)

  await page.getByRole('link', { name: 'MSFS2020 - FBW A320 Devices:' }).first().click();

  await expect(page.getByRole('heading', { name: 'Hello world' })).toHaveCount(0)
  await expect(page.getByRole('heading', { name: 'Projects' })).toHaveCount(0)

  await expect(page.getByText('Breadcrumb: Projects >')).toHaveText("Breadcrumb: Projects > MSFS2020 - FBW A320")
  await page.getByText('Breadcrumb: Projects >').click();
  await page.getByText('Files:').click();
  await page.getByText('Configs:').click();
  await page.getByText('Status: New').click();
});

