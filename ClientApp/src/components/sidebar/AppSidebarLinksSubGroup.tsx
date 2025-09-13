import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { MatchRoute, useLocation } from "@tanstack/react-router";
import { ChevronDown } from "lucide-react";

import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import { cn } from "@/lib/utils";

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "../ui/collapsible";
import {
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarMenuSub,
  SidebarMenuSubItem,
  useSidebar,
} from "../ui/sidebar";
import { Skeleton } from "../ui/skeleton";
import AppSidebarLinkButton from "./AppSidebarLinkButton";
import { SidebarSubMenu } from "./AppSidebarLinksGroup";

function AppSidebarLinksSubGroup({ subMenu }: { subMenu: SidebarSubMenu }) {
  const { open, isHovered, openMobile } = useSidebar();

  const pathname = useLocation({
    select: (location) => location.pathname,
  });

  const [collapsibleOpen, setCollapsibleOpen] = useState(
    pathname.includes(subMenu.title.toLowerCase()),
  );

  const activeOngoingTrainingQuery = useQuery(
    ongoingTrainingsQueryOptions.active,
  );

  return (
    <Collapsible
      open={collapsibleOpen && (open || isHovered || openMobile)}
      onOpenChange={(state) => setCollapsibleOpen(state)}
      className="group/collapsible"
    >
      <SidebarMenuItem>
        <CollapsibleTrigger asChild>
          <SidebarMenuButton className="flex justify-between">
            <div className="inline-flex items-center gap-2">
              <subMenu.icon className="size-4" />
              <MatchRoute to={subMenu.title} fuzzy>
                {(match) => (
                  <span className={match ? "font-extrabold" : ""}>
                    {subMenu.title}
                  </span>
                )}
              </MatchRoute>
            </div>
            <ChevronDown
              className={cn(
                "size-4 transition-all",
                collapsibleOpen && (open || isHovered) ? "rotate-180" : "",
              )}
            />
          </SidebarMenuButton>
        </CollapsibleTrigger>

        <CollapsibleContent>
          <SidebarMenuSub>
            {subMenu.links.map((link) => {
              if (
                link.url === "/trainings/ongoing-workout" &&
                activeOngoingTrainingQuery.isPending
              ) {
                return <Skeleton className="h-7 w-1/2" key={link.title} />;
              }

              if (
                link.url === "/trainings/ongoing-workout" &&
                activeOngoingTrainingQuery.isError
              ) {
                return null;
              }

              return (
                <SidebarMenuSubItem key={link.title}>
                  <AppSidebarLinkButton item={link} />
                </SidebarMenuSubItem>
              );
            })}
          </SidebarMenuSub>
        </CollapsibleContent>
      </SidebarMenuItem>
    </Collapsible>
  );
}

export default AppSidebarLinksSubGroup;
