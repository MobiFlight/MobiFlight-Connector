import { test, expect } from '@playwright/test';

test('has title', async ({ page }) => {
  await page.goto('/');

  // Expect a title "to contain" a substring.
  await expect(page).toHaveTitle(/MobiFlight/);
});

test('Navigate to first project', async ({ page }) => {
  
  await page.goto('http://localhost:5173/');
  await expect(page.getByText('Starting...')).toHaveCount(1)

  await page.goto('http://localhost:5173/?progress=100');
});

