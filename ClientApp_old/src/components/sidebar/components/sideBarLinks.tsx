import { LoginOutlined } from "@mui/icons-material";
import { HomeIcon, TestTube2Icon } from "lucide-react";
import {
  CalendarSvg,
  FinancesSvg,
  NutritionSvg,
  OverviewSvg,
  TasksSvg,
  WorkoutSvg,
} from "~/assets/sidebar";

type SidebarLink = {
  svg: JSX.Element;
  text: string;
  link: string;
};

export const sidebarLinks: SidebarLink[] = [
  {
    svg: <HomeIcon />,
    text: "Home",
    link: "/home",
  },
  {
    svg: <OverviewSvg />,
    text: "About",
    link: "/about",
  },
  { svg: <TasksSvg />, text: "Tasks", link: "/tasks" },
  {
    svg: <CalendarSvg />,
    text: "Calendar",
    link: "/calendar",
  },
  {
    svg: <FinancesSvg />,
    text: "Finances",
    link: "/finances",
  },
  {
    svg: <WorkoutSvg />,
    text: "Workout",
    link: "/workout",
  },
  {
    svg: <NutritionSvg />,
    text: "Nutrition",
    link: "/nutrition",
  },
  {
    svg: <TestTube2Icon />,
    text: "TestPage",
    link: "/test",
  },
  {
    svg: <LoginOutlined />,
    text: "Auth",
    link: "/auth",
  },
];
