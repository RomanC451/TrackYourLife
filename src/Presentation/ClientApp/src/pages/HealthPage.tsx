import React, { useEffect } from "react";
import ProtectedPage from "~/auth/ProtectedPage";
import useUserData from "~/auth/useUserData";
import { userDataInitValue } from "~/contexts/AuthenticationContextProvider";
import {
  ActivitiesListComponent,
  ActivityGraphComponent,
  CaloriesComponent,
  MacrosComponent,
  ProgressComponent
} from "~/features/health";
import { SideBarLayout } from "~/layouts";

const HealthPage: React.FC = (): JSX.Element => {
  return (
    <ProtectedPage>
      <SideBarLayout>
        <main className="bg-main-dark-bg text-white w-full grid place-items-center relative">
          <div className="flex m-5 flex-wrap justify-between max-w-[1158px] gap-[20px]">
            <CaloriesComponent />
            <ProgressComponent />
            <MacrosComponent />
            <ActivitiesListComponent />
            <ActivityGraphComponent />
            {/* <WorkoutsListComponent /> */}
          </div>
        </main>
      </SideBarLayout>
    </ProtectedPage>
  );
};

export default HealthPage;
