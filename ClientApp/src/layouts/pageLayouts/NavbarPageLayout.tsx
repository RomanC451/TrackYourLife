import { Outlet } from "@tanstack/react-router";

import AppNavbar from "@/components/navbar/AppNavbar";
import { YoutubePlayerHostProvider } from "@/features/youtube/contexts/YoutubePlayerHostContext";

import FullSizeLayout from "../FullSizeLayout";
import NavBarLayout from "../NavBarLayout";

function NavbarPageLayout() {
  return (
    <YoutubePlayerHostProvider>
      <NavBarLayout navBarElement={<AppNavbar />}>
        <FullSizeLayout>
          <Outlet />
        </FullSizeLayout>
      </NavBarLayout>
    </YoutubePlayerHostProvider>
  );
}

export default NavbarPageLayout;
