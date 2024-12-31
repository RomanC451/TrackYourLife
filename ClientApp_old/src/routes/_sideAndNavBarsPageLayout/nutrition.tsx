import { createFileRoute } from "@tanstack/react-router";
import protectedRouteBeforeLoad from "~/auth/protectedRouteBeforeLoad";
import { NutritionPage } from "~/pages";

export const Route = createFileRoute("/_sideAndNavBarsPageLayout/nutrition")({
  beforeLoad: protectedRouteBeforeLoad,
  component: NutritionPage,
});
