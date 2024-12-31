import { Outlet } from "@tanstack/react-router";

import AppSidebar from "@/components/sidebar/AppSidebar";
import { SidebarProvider } from "@/components/ui/sidebar";

import FullSizeLayout from "../FullSizeLayout";

function SideBarPageLayout() {
  return (
    <SidebarProvider>
      <AppSidebar />
      <FullSizeLayout>
        <Outlet />
      </FullSizeLayout>
    </SidebarProvider>
  );
}

export default SideBarPageLayout;
