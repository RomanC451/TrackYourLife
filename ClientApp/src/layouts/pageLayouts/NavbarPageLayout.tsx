import { Outlet } from "@tanstack/react-router";

import AppNavbar from "@/components/navbar/AppNavbar";

import FullSizeLayout from "../FullSizeLayout";
import NavBarLayout from "../NavBarLayout";

function NavbarPageLayout() {
  return (
    <NavBarLayout navBarElement={<AppNavbar />}>
      <FullSizeLayout>
        <Outlet />
      </FullSizeLayout>
    </NavBarLayout>
  );
}

export default NavbarPageLayout;
