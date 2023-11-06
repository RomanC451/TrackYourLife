import { Location } from "react-router-dom";
import {
  CalendarSvg,
  FinancesSvg,
  NutritionSvg,
  OverviewSvg,
  TasksSvg,
  WorkoutSvg
} from "~/assets/sidebar";

export enum sidebarSectionsEnum {
  Overview = "Overview",
  Tasks = "Tasks",
  Calendar = "Calendar",
  Finances = "Finances",
  Workout = "Workout",
  Nutrition = "Nutrition"
}

export const getSidebarActivePage = (
  location: Location
): sidebarSectionsEnum => {
  const activeElement = Object.values(sidebarSectionsEnum).find(
    (enumValue) => enumValue === location.pathname
  );

  return activeElement ?? sidebarSectionsEnum.Overview;
};

export const sidebarLinks = [
  {
    svg: <OverviewSvg />,
    section: sidebarSectionsEnum.Overview,
    link: "/auth"
  },
  { svg: <TasksSvg />, section: sidebarSectionsEnum.Tasks, link: "" },
  { svg: <CalendarSvg />, section: sidebarSectionsEnum.Calendar, link: "" },
  { svg: <FinancesSvg />, section: sidebarSectionsEnum.Finances, link: "" },
  { svg: <WorkoutSvg />, section: sidebarSectionsEnum.Workout, link: "" },
  { svg: <NutritionSvg />, section: sidebarSectionsEnum.Nutrition, link: "" }
];
