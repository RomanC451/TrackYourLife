import { useState } from "react";
import { MatchRoute } from "@tanstack/react-router";
import { ChevronDown } from "lucide-react";

import { cn } from "@/lib/utils";
import { FileRoutesByTo } from "@/routeTree.gen";

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
import AppSidebarLinkButton from "./AppSidebarLinkButton";
import { SidebarSubMenu } from "./AppSidebarLinksGroup";

function AppSidebarLinksSubGroup({ subMenu }: { subMenu: SidebarSubMenu }) {
  const { open, isHovered } = useSidebar();

  const [collapsibleOpen, setCollapsibleOpen] = useState(false);

  return (
    <Collapsible
      open={collapsibleOpen && (open || isHovered)}
      onOpenChange={setCollapsibleOpen}
      defaultOpen
      className="group/collapsible"
    >
      <SidebarMenuItem>
        <CollapsibleTrigger asChild>
          <SidebarMenuButton className="flex justify-between">
            <div className="inline-flex items-center gap-2">
              <subMenu.icon className="size-4" />
              <MatchRoute to={"/nutrition" as keyof FileRoutesByTo} fuzzy>
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
            {subMenu.links.map((link) => (
              <SidebarMenuSubItem key={link.title}>
                <AppSidebarLinkButton item={link} />
              </SidebarMenuSubItem>
            ))}
          </SidebarMenuSub>
        </CollapsibleContent>
      </SidebarMenuItem>
    </Collapsible>
  );
}

export default AppSidebarLinksSubGroup;
