import { createFileRoute } from "@tanstack/react-router";
import protectedRouteBeforeLoad from "~/auth/protectedRouteBeforeLoad";
import AboutPage from "~/pages/AboutPage";

export const Route = createFileRoute("/_sideAndNavBarsPageLayout/about")({
  beforeLoad: protectedRouteBeforeLoad,
  component: AboutPage,
});
