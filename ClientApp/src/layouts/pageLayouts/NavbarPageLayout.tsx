import { Outlet } from "@tanstack/react-router";

import AppNavbar from "@/components/navbar/AppNavbar";
import ReadingSessionHostProvider from "@/features/reading/ongoing-session/context/ReadingSessionHostProvider";
import { YoutubePlayerHostProvider } from "@/features/youtube/contexts/YoutubePlayerHostContext";

import FullSizeLayout from "../FullSizeLayout";
import NavBarLayout from "../NavBarLayout";

function NavbarPageLayout() {
  return (
    <YoutubePlayerHostProvider>
      <ReadingSessionHostProvider>
        <NavBarLayout navBarElement={<AppNavbar />}>
          <FullSizeLayout>
            <Outlet />
          </FullSizeLayout>
        </NavBarLayout>
      </ReadingSessionHostProvider>
    </YoutubePlayerHostProvider>
  );
}

export default NavbarPageLayout;
