import { Location } from "react-router-dom";
import {
  AccountSvg,
  CalendarSvg,
  FinancesSvg,
  LogOutSvg,
  NutritionSvg,
  OverviewSvg,
  SettingsSvg,
  TasksSvg,
  WorkoutSvg
} from "~/assets/sidebar";

export enum sidebarSectionsEnum {
  Auth = "Auth",
  Tasks = "Tasks",
  Calendar = "Calendar",
  Finances = "Finances",
  Workout = "Workout",
  Nutrition = "Nutrition",
  LogOut = "Log Out",
  Settings = "Settings",
  Account = "Account"
}

export const getSidebarActiveElement = (
  location: Location
): sidebarSectionsEnum => {
  const activeElement = Object.values(sidebarSectionsEnum).find(
    (enumValue) => enumValue === location.pathname
  );

  return activeElement ?? sidebarSectionsEnum.Auth;
};

export const sidebarSections = [
  [
    { svg: <OverviewSvg />, section: sidebarSectionsEnum.Auth, link: "/auth" },
    { svg: <TasksSvg />, section: sidebarSectionsEnum.Tasks, link: "" },
    { svg: <CalendarSvg />, section: sidebarSectionsEnum.Calendar, link: "" },
    { svg: <FinancesSvg />, section: sidebarSectionsEnum.Finances, link: "" },
    { svg: <WorkoutSvg />, section: sidebarSectionsEnum.Workout, link: "" },
    { svg: <NutritionSvg />, section: sidebarSectionsEnum.Nutrition, link: "" }
  ],
  [
    { svg: <LogOutSvg />, section: sidebarSectionsEnum.LogOut, link: "" },
    { svg: <SettingsSvg />, section: sidebarSectionsEnum.Settings, link: "" }
  ],
  [{ svg: <AccountSvg />, section: sidebarSectionsEnum.Account, link: "" }]
];
