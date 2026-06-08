import { Page } from "@playwright/test";

export function uniqueSuffix() {
  return `${Date.now()}-${Math.random().toString(36).slice(2, 6)}`;
}

export async function selectComboboxOption(
  page: Page,
  comboboxName: RegExp | string,
  optionName: string,
) {
  await page.getByRole("combobox", { name: comboboxName }).click();
  await page.getByRole("option", { name: optionName }).click();
}

export async function openRowActionsMenu(page: Page, rowLocator: ReturnType<Page["locator"]>) {
  await rowLocator.getByRole("button", { name: "Open menu" }).click();
}
