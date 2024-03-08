import { FileRoutesByPath } from "@tanstack/react-router";
import {
  CalendarSvg,
  FinancesSvg,
  NutritionSvg,
  OverviewSvg,
  TasksSvg,
  WorkoutSvg,
} from "~/assets/sidebar";

export enum sidebarSectionsEnum {
  About = "About",
  Auth = "Auth",
  Calendar = "Calendar",
  Finances = "Finances",
  Workout = "Workout",
  Nutrition = "Nutrition",
  TestPage = "TestPage",
}

export const getSidebarActivePage = (
  location: Location,
): sidebarSectionsEnum => {
  const activeElement = Object.values(sidebarSectionsEnum).find(
    (enumValue) => enumValue === location.pathname,
  );

  return activeElement ?? sidebarSectionsEnum.About;
};

export const sidebarLinks: SidebarLink[] = [
  {
    svg: <OverviewSvg />,
    section: sidebarSectionsEnum.About,
    link: "/about",
  },
  { svg: <TasksSvg />, section: sidebarSectionsEnum.Auth, link: "/auth" },
  {
    svg: <CalendarSvg />,
    section: sidebarSectionsEnum.Calendar,
    link: "/",
  },
  { svg: <FinancesSvg />, section: sidebarSectionsEnum.Finances, link: "/" },
  { svg: <WorkoutSvg />, section: sidebarSectionsEnum.Workout, link: "/" },
  { svg: <NutritionSvg />, section: sidebarSectionsEnum.Nutrition, link: "/" },
  {
    svg: <CalendarSvg />,
    section: sidebarSectionsEnum.TestPage,
    link: "/test",
  },
];

type SidebarLink = {
  svg: JSX.Element;
  section: sidebarSectionsEnum;
  link: keyof FileRoutesByPath;
};
