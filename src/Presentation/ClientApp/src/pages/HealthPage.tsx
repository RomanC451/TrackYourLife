import React from "react";
import NavBar from "~/components/navbar/NavBar";
import {
  ActivitiesListComponent,
  ActivityGraphComponent,
  CaloriesComponent,
  MacrosComponent,
  ProgressComponent,
} from "~/features/health";
import { SideBarLayout } from "~/layouts";
import FullSizeLayout from "~/layouts/FullSizeLayout";
import NavBarLayout from "~/layouts/NavBarLayout";
import RootLayout from "~/layouts/RootLayout";

const HealthPage: React.FC = (): JSX.Element => {
  return (
    <RootLayout>
      <SideBarLayout>
        <FullSizeLayout>
          <NavBarLayout navBarElement={<NavBar />}>
            <main className=" relative grid w-[100%]  scroll-p-0 place-items-center text-foreground ">
              <div className="justify-arround m-5 flex max-w-[1158px] flex-wrap gap-[20px] ">
                <CaloriesComponent />
                <ProgressComponent />
                <MacrosComponent />
                <ActivitiesListComponent />
                <ActivityGraphComponent />
              </div>
            </main>
          </NavBarLayout>
        </FullSizeLayout>
      </SideBarLayout>
    </RootLayout>
  );
};

export default HealthPage;
