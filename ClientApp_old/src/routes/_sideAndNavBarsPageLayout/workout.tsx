import { createFileRoute } from "@tanstack/react-router";
import protectedRouteBeforeLoad from "~/auth/protectedRouteBeforeLoad";
import WorkoutPage from "~/pages/WorkoutPage";

export const Route = createFileRoute("/_sideAndNavBarsPageLayout/workout")({
  beforeLoad: protectedRouteBeforeLoad,
  component: WorkoutPage,
});
