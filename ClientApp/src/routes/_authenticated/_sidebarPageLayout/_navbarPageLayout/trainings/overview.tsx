import { createFileRoute } from "@tanstack/react-router";
import { subDays } from "date-fns";

import { prefetchTrainingsOverviewPageQueries } from "@/features/trainings/overview/queries/trainingsOverviewQueries";
import { getDateOnly } from "@/lib/date";
import OverviewPage from "@/pages/trainings/OverviewPage";
import { queryClient } from "@/queryClient";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/overview",
)({
  loader: async () => {
    const defaultStartDate = getDateOnly(subDays(new Date(), 31));
    const defaultEndDate = getDateOnly(new Date());

    await prefetchTrainingsOverviewPageQueries(
      queryClient,
      defaultStartDate,
      defaultEndDate,
    );
  },
  component: RouteComponent,
});

function RouteComponent() {
  return <OverviewPage />;
}
