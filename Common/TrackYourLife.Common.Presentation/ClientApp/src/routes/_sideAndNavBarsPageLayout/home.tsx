import { createFileRoute } from "@tanstack/react-router";
import protectedRouteBeforeLoad from "~/auth/protectedRouteBeforeLoad";
import HomePage from "~/pages/HomePage";
import LoadingPage from "~/pages/LoadingPage";

export const Route = createFileRoute("/_sideAndNavBarsPageLayout/home")({
  loader: protectedRouteBeforeLoad,
  component: HomePage,
  pendingComponent: LoadingPage,
  pendingMs: 200,
  pendingMinMs: 1000,
  gcTime: 0,
});
