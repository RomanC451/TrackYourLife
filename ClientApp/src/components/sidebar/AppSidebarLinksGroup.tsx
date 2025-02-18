import {
  Bug,
  ChartBar,
  CookingPot,
  Home,
  LucideProps,
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
  links?: undefined;
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
        icon: Utensils,
      },
      {
        title: "Recipes",
        url: "/nutrition/recipes",
        icon: CookingPot,
      },
    ],
  },
  {
    title: "Debug",
    url: "/debug",
    icon: Bug,
  },
];

function AppSidebarLinksGroup() {
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
