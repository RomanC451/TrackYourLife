import { expect, test } from "./fixtures/authenticatedTest";

import {
  clickSidebarLink,
  expectPageTitle,
  expectRecipesPage,
  gotoHome,
  navigateViaSidebar,
  openUserMenu,
} from "./fixtures/navigation";

test.describe("navigation", () => {
  test("sidebar reaches home", async ({ page }) => {
    await gotoHome(page);
    await clickSidebarLink(page, "Home");
    await expect(page).toHaveURL(/\/home/);
    await expectPageTitle(page, "Home");
  });

  test("sidebar reaches nutrition pages", async ({ page }) => {
    await gotoHome(page);

    await navigateViaSidebar(page, "Nutrition", "Overview");
    await expect(page).toHaveURL(/\/nutrition\/overview/);
    await expectPageTitle(page, "Nutrition Overview");

    await navigateViaSidebar(page, "Nutrition", "Diary");
    await expect(page).toHaveURL(/\/nutrition\/diary/);
    await expectPageTitle(page, "Food Diary");

    await navigateViaSidebar(page, "Nutrition", "Recipes");
    await expect(page).toHaveURL(/\/nutrition\/recipes/);
    await expectRecipesPage(page);
  });

  test("sidebar reaches trainings pages", async ({ page }) => {
    await gotoHome(page);

    await navigateViaSidebar(page, "Trainings", "Overview");
    await expect(page).toHaveURL(/\/trainings\/overview/);

    await navigateViaSidebar(page, "Trainings", "Exercises");
    await expect(page).toHaveURL(/\/trainings\/exercises/);
    await expectPageTitle(page, "Exercises");

    await navigateViaSidebar(page, "Trainings", "Workouts");
    await expect(page).toHaveURL(/\/trainings\/workouts/);
    await expectPageTitle(page, "Workouts");

    await navigateViaSidebar(page, "Trainings", "Workout history");
    await expect(page).toHaveURL(/\/trainings\/history/);
  });

  test("sidebar reaches reading pages", async ({ page }) => {
    await gotoHome(page);

    await navigateViaSidebar(page, "Reading", "Dashboard");
    await expect(page).toHaveURL(/\/reading\/dashboard/);
    await expectPageTitle(page, "Reading dashboard");

    await navigateViaSidebar(page, "Reading", "Books");
    await expect(page).toHaveURL(/\/books/);
    await expectPageTitle(page, "Books");

    await navigateViaSidebar(page, "Reading", "History");
    await expect(page).toHaveURL(/\/reading\/history/);
    await expectPageTitle(page, "Reading history");
  });

  test("sidebar reaches youtube pages", async ({ page }) => {
    await gotoHome(page);

    await navigateViaSidebar(page, "Youtube", "Channels");
    await expect(page).toHaveURL(/\/youtube\/channels/);
    await expectPageTitle(page, "Channels");

    await navigateViaSidebar(page, "Youtube", "Videos");
    await expect(page).toHaveURL(/\/youtube\/videos/);
    await expectPageTitle(page, "Videos");

    await navigateViaSidebar(page, "Youtube", "Library");
    await expect(page).toHaveURL(/\/youtube\/library/);

    await navigateViaSidebar(page, "Youtube", "Watch history");
    await expect(page).toHaveURL(/\/youtube\/history/);
    await expect(page.getByText("Watch history").first()).toBeVisible({
      timeout: 30_000,
    });

    await navigateViaSidebar(page, "Youtube", "Search");
    await expect(page).toHaveURL(/\/youtube\/search/);

    await navigateViaSidebar(page, "Youtube", "Settings");
    await expect(page).toHaveURL(/\/youtube\/settings/);
    await expectPageTitle(page, "YouTube Settings");
  });

  test("user menu opens and shows email", async ({ page, e2eUser }) => {
    await gotoHome(page);
    await openUserMenu(page, e2eUser.email);
    await expect(page.getByRole("menuitem", { name: "Log out" })).toBeVisible();
  });

  test("redirects unauthenticated deep link to login", async ({ browser }) => {
    const context = await browser.newContext({
      baseURL: process.env.E2E_BASE_URL ?? "http://127.0.0.1:5173",
    });
    const page = await context.newPage();
    await page.goto("/nutrition/diary");
    await expect(page).toHaveURL(/\/auth/);
    await expect(page).toHaveURL(/authMode=logIn/);
    await expect(page).toHaveURL(/redirect=/);
    await context.close();
  });
});
