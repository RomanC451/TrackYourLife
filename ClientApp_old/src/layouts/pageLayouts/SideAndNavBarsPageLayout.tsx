import { Outlet } from "@tanstack/react-router";
import NavBar from "~/components/navbar/NavBar";
import RootLayout from "~/layouts/RootLayout";
import FullSizeLayout from "../FullSizeLayout";
import NavBarLayout from "../NavBarLayout";
import SideBarLayout from "../SideBarLayout";

interface SideAndNavBarsPageLayoutProps extends React.PropsWithChildren<{}> {}

const SideAndNavBarsPageLayout: React.FC<
  SideAndNavBarsPageLayoutProps
> = () => {
  return (
    <RootLayout>
      <SideBarLayout>
        <FullSizeLayout>
          <NavBarLayout navBarElement={<NavBar />}>
            <div className="flex flex-grow px-8 pb-8">
              <Outlet />
            </div>
          </NavBarLayout>
        </FullSizeLayout>
      </SideBarLayout>
    </RootLayout>
  );
};

export default SideAndNavBarsPageLayout;
