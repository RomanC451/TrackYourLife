import {
  Bug,
  ChartBar,
  CookingPot,
  Dumbbell,
  Home,
  LucideProps,
  NotebookPen,
  PersonStanding,
  SquarePlay,
  Utensils,
} from "lucide-react";

import { FileRoutesByTo } from "@/routeTree.gen";

import {
  SidebarGroup,
  SidebarGroupContent,
  SidebarMenu,
  SidebarMenuItem,
} from "../ui/sidebar";
import AppSidebarLinkButton from "./AppSidebarLinkButton";
import AppSidebarLinksSubGroup from "./AppSidebarLinksSubGroup";

type SidebarLink = {
  title: string;
  url: keyof FileRoutesByTo;
  icon: React.ForwardRefExoticComponent<
    Omit<LucideProps, "ref"> & React.RefAttributes<SVGSVGElement>
  >;
  links?: never;
};

export type SidebarSubMenu = {
  title: string;
  icon: React.ForwardRefExoticComponent<
    Omit<LucideProps, "ref"> & React.RefAttributes<SVGSVGElement>
  >;
  links: SidebarLink[];
};

const sidebarSchema: (SidebarLink | SidebarSubMenu)[] = [
  {
    title: "Home",
    url: "/home",
    icon: Home,
  },
  {
    title: "Nutrition",
    icon: Utensils,
    links: [
      {
        title: "Overview",
        url: "/nutrition/overview",
        icon: ChartBar,
      },
      {
        title: "Diary",
        url: "/nutrition/diary",
        icon: NotebookPen,
      },
      {
        title: "Recipes",
        url: "/nutrition/recipes",
        icon: CookingPot,
      },
    ],
  },
  {
    title: "Trainings",
    icon: PersonStanding,
    links: [
      {
        title: "Overview",
        url: "/trainings/overview",
        icon: ChartBar,
      },
      {
        title: "Workouts",
        url: "/trainings/workouts",
        icon: Dumbbell,
      },
      {
        title: "Ongoing workout",
        url: "/trainings/ongoing-workout",
        icon: SquarePlay,
      },
      {
        title: "Exercises",
        url: "/trainings/exercises",
        icon: Dumbbell,
      },
    ],
  },
  import.meta.env.MODE === "development"
    ? {
        title: "Debug",
        url: "/debug",
        icon: Bug,
      }
    : null,
].filter(Boolean) as (SidebarLink | SidebarSubMenu)[];

function AppSidebarLinksGroup() {
  console.log(import.meta.env.MODE);
  return (
    <SidebarGroup>
      <SidebarGroupContent>
        <SidebarMenu>
          {sidebarSchema.map((item) => {
            if (item.links) {
              return (
                <AppSidebarLinksSubGroup subMenu={item} key={item.title} />
              );
            }

            return (
              <SidebarMenuItem key={item.title}>
                <AppSidebarLinkButton item={item} />
              </SidebarMenuItem>
            );
          })}
        </SidebarMenu>
      </SidebarGroupContent>
    </SidebarGroup>
  );
}

export default AppSidebarLinksGroup;
