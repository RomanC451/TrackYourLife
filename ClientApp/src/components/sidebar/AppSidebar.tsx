import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";

import { NavUser } from "../ui/nav-user";
import { Separator } from "../ui/separator";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  useSidebar,
} from "../ui/sidebar";
import AppSidebarHeader from "./AppSidebarHeader";
import AppSidebarLinksGroup from "./AppSidebarLinksGroup";

function AppSidebar() {
  const { setIsHovered } = useSidebar();

  const { userData } = useAuthenticationContext();

  return (
    <Sidebar
      collapsible="icon"
      onMouseEnter={() => {
        setIsHovered(true);
      }}
      onMouseLeave={() => {
        setIsHovered(false);
      }}
    >
      <AppSidebarHeader />
      <SidebarContent>
        <Separator className="mx-auto w-[calc(100%-32px)] bg-white" />
        <AppSidebarLinksGroup />
      </SidebarContent>
      <SidebarFooter>
        <NavUser
          user={{
            name: `${userData?.firstName} ${userData?.lastName}`,
            email: userData?.email || "",
            avatar: "/src/assets/avatar.svg",
          }}
        />
      </SidebarFooter>
    </Sidebar>
  );
}

export default AppSidebar;