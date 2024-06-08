import { Outlet } from "@tanstack/react-router";
import NavBar from "~/components/navbar/NavBar";
import { SideBarLayout } from "~/layouts";
import FullSizeLayout from "~/layouts/FullSizeLayout";
import NavBarLayout from "~/layouts/NavBarLayout";
import RootLayout from "~/layouts/RootLayout";

interface SideAndNavBarsPageLayoutProps extends React.PropsWithChildren<{}> {}

const SideAndNavBarsPageLayout: React.FC<
  SideAndNavBarsPageLayoutProps
> = () => {
  return (
    <RootLayout>
      <SideBarLayout>
        <FullSizeLayout>
          <NavBarLayout navBarElement={<NavBar />}>
            <div className="px-8 pb-8">
              <Outlet />
            </div>
          </NavBarLayout>
        </FullSizeLayout>
      </SideBarLayout>
    </RootLayout>
  );
};

export default SideAndNavBarsPageLayout;
