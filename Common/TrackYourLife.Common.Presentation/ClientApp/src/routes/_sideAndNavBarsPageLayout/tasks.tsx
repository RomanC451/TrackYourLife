import { createFileRoute } from "@tanstack/react-router";
import protectedRouteBeforeLoad from "~/auth/protectedRouteBeforeLoad";
import TasksPage from "~/pages/TasksPage";

export const Route = createFileRoute("/_sideAndNavBarsPageLayout/tasks")({
  beforeLoad: protectedRouteBeforeLoad,
  component: TasksPage,
});
