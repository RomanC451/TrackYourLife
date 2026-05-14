import {
  createFileRoute,
  Outlet,
  useNavigate,
  useRouterState,
} from "@tanstack/react-router";
import { Plus } from "lucide-react";

import { router } from "@/App";
import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import ExercisesList from "@/features/trainings/exercises/components/exercisesList/exercisesList";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/exercises",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const navigate = useNavigate();
  const pathname = useRouterState({
    select: (state) => state.location.pathname,
  });
  const isExerciseStatsPage = /^\/trainings\/exercises\/[^/]+\/stats$/.test(pathname);

  if (isExerciseStatsPage) {
    return <Outlet />;
  }

  return (
    <PageCard>
      <PageTitle title="Exercises">
        <Button
          onClick={() => {
            navigate({
              to: "/trainings/exercises/create",
            });
          }}
          onMouseEnter={() => {
            router.preloadRoute({
              to: "/trainings/exercises/create",
            });
          }}
          onTouchStart={() => {
            router.preloadRoute({
              to: "/trainings/exercises/create",
            });
          }}
        >
          <Plus />
          Create
        </Button>
      </PageTitle>
      <ExercisesList />
      <Outlet />
    </PageCard>
  );
}
