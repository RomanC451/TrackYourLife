import { useNavigate } from "@tanstack/react-router";
import { Calendar, Check, Dumbbell, MoreHorizontal, Plus, Trash2 } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import useDeleteWorkoutPlanMutation from "@/features/trainings/workoutPlans/mutations/useDeleteWorkoutPlanMutation";
import useUpdateWorkoutPlanMutation from "@/features/trainings/workoutPlans/mutations/useUpdateWorkoutPlanMutation";
import { WorkoutPlanDto } from "@/services/openapi";

type WorkoutPlanListProps = {
  plans: WorkoutPlanDto[];
};

function WorkoutPlanList({ plans }: WorkoutPlanListProps) {
  const navigate = useNavigate();
  const deleteWorkoutPlanMutation = useDeleteWorkoutPlanMutation();
  const updateWorkoutPlanMutation = useUpdateWorkoutPlanMutation();

  const isMutating =
    updateWorkoutPlanMutation.isPending ||
    deleteWorkoutPlanMutation.isPending;

  const getCreatedAtLabel = (createdOnUtc?: string) => {
    if (!createdOnUtc) return "Just now";

    const createdAt = new Date(createdOnUtc);
    if (Number.isNaN(createdAt.getTime())) return "Just now";

    const diffMs = Date.now() - createdAt.getTime();
    const diffMin = Math.floor(diffMs / (1000 * 60));
    const diffHours = Math.floor(diffMs / (1000 * 60 * 60));
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
    const diffMonths = Math.floor(diffDays / 30);

    if (diffMin < 1) return "Just now";
    if (diffMin < 60) return `${diffMin} min ago`;
    if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? "s" : ""} ago`;
    if (diffDays < 30) return `${diffDays} day${diffDays > 1 ? "s" : ""} ago`;
    return `${diffMonths} month${diffMonths > 1 ? "s" : ""} ago`;
  };

  return (
    <div className="mt-4 space-y-4">
      {plans.map((plan) => (
        <div
          key={plan.id}
          role="button"
          tabIndex={0}
          onClick={() =>
            navigate({
              to: "/trainings/workouts/plan/edit/$planId",
              params: { planId: plan.id },
            })
          }
          onKeyDown={(event) => {
            if (event.key === "Enter" || event.key === " ") {
              event.preventDefault();
              navigate({
                to: "/trainings/workouts/plan/edit/$planId",
                params: { planId: plan.id },
              });
            }
          }}
          className={`flex items-center justify-between rounded-lg border p-3 transition-colors ${plan.isActive
              ? "border-primary/30 bg-primary/10"
              : "border-border bg-card-secondary hover:bg-muted"
            }`}
        >
          <div className="flex items-center gap-3">
            <div
              className={`rounded-lg p-2 ${plan.isActive ? "bg-primary/20" : "bg-muted"}`}
            >
              <Dumbbell
                className={`h-4 w-4 ${plan.isActive ? "text-primary" : "text-muted-foreground"}`}
              />
            </div>
            <div>
              <div className="flex items-center gap-2">
                <span className="font-medium text-foreground">{plan.name}</span>
                {plan.isActive && (
                  <Badge
                    variant="outline"
                    className="border-primary/30 bg-primary/10 text-xs text-primary"
                  >
                    Active
                  </Badge>
                )}
              </div>
              <div className="flex items-center gap-3 text-xs text-muted-foreground">
                <span>{plan.workouts.length} workouts</span>
                <span className="flex items-center gap-1">
                  <Calendar className="h-3 w-3" />
                  {getCreatedAtLabel(plan.createdOnUtc)}
                </span>
              </div>
            </div>
          </div>

          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button
                variant="ghost"
                size="icon"
                className="h-8 w-8"
                onClick={(event) => event.stopPropagation()}
                disabled={isMutating}
              >
                <MoreHorizontal className="h-4 w-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent
              align="end"
              onClick={(event) => event.stopPropagation()}
            >
              {!plan.isActive && (
                <DropdownMenuItem
                  onClick={() => {
                    updateWorkoutPlanMutation.mutate({
                      id: plan.id,
                      request: {
                        name: plan.name,
                        isActive: true,
                        trainingIds: plan.workouts.map((workout) => workout.id),
                      },
                    });
                  }}
                  className="cursor-pointer"
                >
                  <Check className="h-4 w-4 mr-2" />
                  Set as active
                </DropdownMenuItem>
              )}
              <DropdownMenuItem
                onClick={() => {
                  deleteWorkoutPlanMutation.mutate({ id: plan.id });
                }}
                className="text-red-400 hover:text-red-300 hover:bg-red-900/20 cursor-pointer"
                disabled={plan.isActive}
              >
                <Trash2 className="h-4 w-4 mr-2" />
                Delete plan
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      ))}

      <Button
        onClick={() => {
          navigate({ to: "/trainings/workouts/plan/create" });
        }}
        variant="outline"
        className="w-full border-dashed"
        disabled={isMutating}
      >
        <Plus className="h-4 w-4 mr-2" />
        Create new plan
      </Button>
    </div>
  );
}

export default WorkoutPlanList;
