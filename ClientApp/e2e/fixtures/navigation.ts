import { expect, Page } from "@playwright/test";

function sidebar(page: Page) {
  return page.locator('[data-sidebar="sidebar"]');
}

function pageCard(page: Page) {
  return page.locator('[class*="@container/page-card"]');
}

function escapeRegExp(value: string) {
  return value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

async function ensureSidebarExpanded(page: Page) {
  const sidebarEl = sidebar(page);
  await sidebarEl.hover();

  const homeLink = sidebarEl.getByRole("link", { name: "Home", exact: true });
  if (await homeLink.isVisible()) {
    return;
  }

  await page.locator('[data-sidebar="trigger"]').click();
  await expect(homeLink).toBeVisible({ timeout: 5_000 });
}

export async function gotoHome(page: Page) {
  await page.goto("/home");
  await expectPageTitle(page, "Home", 30_000);
}

export async function expandSidebarGroup(page: Page, groupTitle: string) {
  const groupButton = sidebar(page)
    .getByRole("button")
    .filter({ hasText: groupTitle })
    .first();
  await groupButton.click();
}

export async function clickSidebarLink(page: Page, linkTitle: string) {
  await ensureSidebarExpanded(page);
  const link = sidebar(page).getByRole("link", { name: linkTitle, exact: true });
  await link.scrollIntoViewIfNeeded();
  await link.click();
}

export async function navigateViaSidebar(
  page: Page,
  groupTitle: string,
  linkTitle: string,
) {
  await ensureSidebarExpanded(page);

  const sidebarEl = sidebar(page);
  const link = sidebarEl.getByRole("link", { name: linkTitle, exact: true });

  if (!(await link.isVisible())) {
    await expandSidebarGroup(page, groupTitle);
  }

  const href = await link.getAttribute("href");
  await link.scrollIntoViewIfNeeded();
  await link.click();

  if (href) {
    const path = escapeRegExp(href.split("?")[0]);
    await expect(page).toHaveURL(new RegExp(`${path}(\\?|$)`), {
      timeout: 15_000,
    });
  }
}

export async function openUserMenu(page: Page, email: string) {
  await page.getByRole("button", { name: email }).click();
}

export async function expectPageTitle(
  page: Page,
  title: string,
  timeout = 15_000,
) {
  await expect(page).not.toHaveURL(/\/auth/, { timeout: 30_000 });
  await expect(
    pageCard(page).getByText(title, { exact: true }).first(),
  ).toBeVisible({ timeout });
}

export async function expectRecipesPage(page: Page) {
  await expect(page).not.toHaveURL(/\/auth/, { timeout: 30_000 });
  await expect(
    page
      .getByRole("heading", { name: "Recipes" })
      .or(page.getByText("You have no recipes")),
  ).toBeVisible({ timeout: 15_000 });
}
