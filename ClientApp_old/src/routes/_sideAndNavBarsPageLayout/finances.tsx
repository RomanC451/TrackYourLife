import { createFileRoute } from "@tanstack/react-router";
import protectedRouteBeforeLoad from "~/auth/protectedRouteBeforeLoad";
import FinancesPage from "~/pages/FinancesPage";

export const Route = createFileRoute("/_sideAndNavBarsPageLayout/finances")({
  beforeLoad: protectedRouteBeforeLoad,
  component: FinancesPage,
});
