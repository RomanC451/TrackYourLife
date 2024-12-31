import React from "react";
import { Link } from "@tanstack/react-router";

import { SidebarMenuButton, useSidebar } from "../ui/sidebar";

type LinkItem = {
  title: string;
  url: string;
  icon: React.FC;
};

function AppSidebarLinkButton({ item }: { item: LinkItem }) {
  const { isMobile, setOpenMobile } = useSidebar();
  return (
    <SidebarMenuButton asChild>
      <Link
        to={item.url}
        onClick={() => {
          if (isMobile) {
            setOpenMobile(false);
          }
        }}
      >
        {({ isActive }) => (
          <>
            <item.icon />
            <span className={isActive ? "font-extrabold" : ""}>
              {item.title}
            </span>
          </>
        )}
      </Link>
    </SidebarMenuButton>
  );
}

export default AppSidebarLinkButton;
