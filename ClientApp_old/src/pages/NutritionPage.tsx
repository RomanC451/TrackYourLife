import { Outlet } from "@tanstack/react-router";
import React from "react";

const NutritionPage: React.FC = (): JSX.Element => {
  return (
    <main className="flex flex-grow overflow-hidden">
      <Outlet />
    </main>
  );
};

export default NutritionPage;
