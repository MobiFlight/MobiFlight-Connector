import { test, expect } from './fixtures';

test('Confirm empty list view', async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithEmptyData()
  await expect (page.getByRole('cell', { name: 'This is a new configuration. Please add some items.' })).toBeVisible()
  await expect (page.getByRole('button', { name: 'Add Output Config' })).toBeVisible()
  await expect (page.getByRole('button', { name: 'Add Input Config' })).toBeVisible()
  await expect (page.getByRole('textbox', { name: 'Filter items...' })).toBeVisible()
  await page.getByRole('button', { name: 'Devices' }).click();
  await expect(page.getByText('No results found.')).toBeVisible();
  await page.getByRole('button', { name: 'Type' }).click();
  await page.waitForTimeout(500);
  await expect(page.getByRole('listbox').getByText('No results found.', { exact: true })).toBeVisible();
  await page.getByRole('button', { name: 'Name' }).click();
  await page.waitForTimeout(500);
  await expect(page.getByText('No results found.')).toBeVisible();
})

test('Confirm populated list view', async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await expect (page.getByRole('cell', { name: 'No results.' })).not.toBeVisible()
})

test('Confirm active toggle is working', async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await configListPage.setupConfigItemEditConfirmationResponse()
  const rowSelector = { name: 'LED 1 Edit ProtoBoard-v2 SN-5FC-' }
  await page.getByRole('row', rowSelector).getByRole('switch').click()
  await expect(page.getByRole('row', rowSelector).getByRole('switch')).not.toBeChecked()
  await page.getByRole('row', rowSelector).getByRole('switch').click()
  await expect(page.getByRole('row', rowSelector).getByRole('switch')).toBeChecked()
})

test('Confirm edit function for name is working', async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await configListPage.setupConfigItemEditConfirmationResponse()
  await page.getByRole('cell', { name: 'LED 1' }).getByRole('button', { name: 'Edit' }).click();
  await page.locator('input[type="text"]').click();
  await page.locator('input[type="text"]').fill('LED 1245');
  await page.getByRole('cell', { name: 'LED 1245' }).getByRole('button', { name: 'Save' }).click();
  await expect(page.getByRole('cell', { name: 'LED 1245' })).toBeVisible();
  await page.getByRole('cell', { name: 'LED 1245' }).getByRole('button', { name: 'Edit' }).click();
  await page.locator('input[type="text"]').click();
  await page.locator('input[type="text"]').fill('LED 9999');
  await page.getByRole('cell', { name: 'LED 9999' }).getByRole('button', { name: 'Discard' }).click();
  await expect(page.getByRole('cell', { name: 'LED 1245' })).toBeVisible();
})

test('Confirm status icons working', async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await expect(page.getByRole('row').nth(1).getByRole('status').first()).toHaveAttribute('aria-disabled', 'true');
  await expect(page.getByRole('row').nth(1).getByRole('status').nth(1)).toHaveAttribute('aria-disabled', 'true');
  await expect(page.getByRole('row').nth(1).getByRole('status').nth(2)).toHaveAttribute('aria-disabled', 'true');
  await expect(page.getByRole('row').nth(1).getByRole('status').nth(3)).toHaveAttribute('aria-disabled', 'true');
  await expect(page.getByRole('row').nth(1).getByRole('status').nth(4)).toHaveAttribute('aria-disabled', 'true');
  await expect(page.getByRole('row').nth(1).getByRole('status').nth(5)).toHaveAttribute('aria-disabled', 'true');
  
  await configListPage.updateConfigItemStatus(0, { "Precondition": "not satisfied" });
  await expect(page.getByRole('row').nth(1).getByRole('status').first()).toHaveAttribute('aria-disabled', 'false');
  
  await configListPage.updateConfigItemStatus(0, { "Source": "not satisfied" });
  await expect(page.getByRole('row').nth(1).getByRole('status').nth(1)).toHaveAttribute('aria-disabled', 'false');
  
  await configListPage.updateConfigItemStatus(0, { "Device": "not satisfied" });
  await expect(page.getByRole('row').nth(1).getByRole('status').nth(2)).toHaveAttribute('aria-disabled', 'false');
  
  await configListPage.updateConfigItemStatus(0, { "Modifier": "not satisfied" });
  await expect(page.getByRole('row').nth(1).getByRole('status').nth(3)).toHaveAttribute('aria-disabled', 'false');
  
  await configListPage.updateConfigItemStatus(0, { "Test": "being tested" });
  await expect(page.getByRole('row').nth(1).getByRole('status').nth(4)).toHaveAttribute('aria-disabled', 'false');

  await configListPage.updateConfigItemStatus(0, { "ConfigRef": "being tested" });
  await expect(page.getByRole('row').nth(1).getByRole('status').nth(5)).toHaveAttribute('aria-disabled', 'false');
})

test('Confirm drag n drop is working', async ({ configListPage, page }) => {
  await configListPage.gotoPage()
  await configListPage.initWithTestData()
  await page.getByRole('row').nth(1).getByRole('button').first().hover();
  await page.mouse.down();
  await page.getByRole('row', { name: 'ShiftRegister' }).getByRole('button').first().hover();
  await page.mouse.up();
  await expect(page.getByRole('row').nth(6)).toContainText("7-Segment");
})