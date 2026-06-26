import { useEffect, useMemo, useState } from "react";
import { useLocation } from "@tanstack/react-router";
import { ChevronDown } from "lucide-react";

import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import { useCustomQuery } from "@/hooks/useCustomQuery";
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

function isSidebarLinkActive(pathname: string, url: string) {
  return pathname === url || pathname.startsWith(`${url}/`);
}

function AppSidebarLinksSubGroup({ subMenu }: { subMenu: SidebarSubMenu }) {
  const { open, isHovered, openMobile } = useSidebar();

  const pathname = useLocation({
    select: (location) => location.pathname,
  });

  const isSubMenuActive = useMemo(
    () =>
      subMenu.links.some((link) => isSidebarLinkActive(pathname, link.url)),
    [pathname, subMenu.links],
  );

  const [collapsibleOpen, setCollapsibleOpen] = useState(isSubMenuActive);

  useEffect(() => {
    if (isSubMenuActive) {
      setCollapsibleOpen(true);
    }
  }, [isSubMenuActive]);

  const { query, pendingState } = useCustomQuery(
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
              <span className={isSubMenuActive ? "font-extrabold" : ""}>
                {subMenu.title}
              </span>
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
                pendingState.isPending
              ) {
                return <Skeleton className="h-7 w-1/2" key={link.title} />;
              }

              if (link.url === "/trainings/ongoing-workout" && query.isError) {
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
