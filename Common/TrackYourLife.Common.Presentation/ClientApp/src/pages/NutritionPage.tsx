import { Link, Outlet, useRouterState } from "@tanstack/react-router";
import React from "react";
import { Tabs, TabsList, TabsTrigger } from "~/chadcn/ui/tabs";

const ACCEPTED_PATHS = ["health", "foodDiary"];

const NutritionPage: React.FC = (): JSX.Element => {
  const router = useRouterState();

  const fullPathName = router.location.pathname
    .split("/")
    .filter((p) => p !== "");

  const pathName = fullPathName[fullPathName.length - 1];

  return (
    // <SideAndNavBarsPageLayout>
    <main className=" flex-grow ">
      {/* <TestPage /> */}
      <Tabs
        defaultValue={
          ACCEPTED_PATHS.includes(pathName) ? pathName : ACCEPTED_PATHS[0]
        }
        className=""
      >
        <TabsList>
          <Link to="/nutrition">
            <TabsTrigger value="health">Main</TabsTrigger>
          </Link>
          <Link to="/nutrition/foodDiary" preload={false}>
            <TabsTrigger value="foodDiary">Diary</TabsTrigger>
          </Link>
        </TabsList>

        <Outlet />
      </Tabs>
    </main>
    // </SideAndNavBarsPageLayout>
  );
};

export default NutritionPage;
