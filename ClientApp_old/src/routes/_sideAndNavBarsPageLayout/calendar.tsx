import { createFileRoute } from "@tanstack/react-router";
import protectedRouteBeforeLoad from "~/auth/protectedRouteBeforeLoad";
import CalendarPage from "~/pages/CalendarPage";

export const Route = createFileRoute("/_sideAndNavBarsPageLayout/calendar")({
  beforeLoad: protectedRouteBeforeLoad,
  component: CalendarPage,
});
