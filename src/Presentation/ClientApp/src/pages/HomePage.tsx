import React from "react";
import {
  ActivitiesListComponent,
  ActivityGraphComponent,
  CaloriesComponent,
  MacrosComponent,
  ProgressComponent,
  WorkoutsListComponent
} from "~/features/health";
import { SideBarLayout } from "~/layouts";

const Home: React.FC = (): JSX.Element => {
  return (
    <SideBarLayout>
      <main className="bg-main-dark-bg text-white w-full grid place-items-center">
        <div className="flex m-5 flex-wrap justify-around max-w-[1250px]">
          <CaloriesComponent />
          <ProgressComponent />
          <MacrosComponent />
          <ActivitiesListComponent />
          <ActivityGraphComponent />
          <WorkoutsListComponent />
        </div>
      </main>
    </SideBarLayout>
  );
};

export default Home;
