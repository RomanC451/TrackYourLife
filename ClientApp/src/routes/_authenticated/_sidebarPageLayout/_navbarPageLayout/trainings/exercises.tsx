import { createFileRoute, Outlet, useNavigate } from "@tanstack/react-router";
import { Plus } from "lucide-react";

import { router } from "@/App";
import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import ExercisesList from "@/features/trainings/exercises/exercisesList/exercisesList";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/trainings/exercises",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const navigate = useNavigate();
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
