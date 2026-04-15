"use no memo"
import { useNavigate } from "@tanstack/react-router";
import { endOfMonth, startOfMonth } from "date-fns";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Card } from "@/components/ui/card";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { trainingsOverviewQueryOptions } from "@/features/trainings/overview/queries/useTrainingsOverviewQuery";
import { MuscleGroupWorkoutIcon } from "@/features/trainings/utils/muscleGroupWorkoutIcon";
import { workoutPlansQueryOptions } from "@/features/trainings/workoutPlans/queries/workoutPlansQueries";
import { workoutStreakQueryOptions } from "@/features/trainings/workoutPlans/queries/workoutStreakQuery";
import {
  getCurrentWeekDateRange,
  workoutsWeeklyGoalQueryOptions,
} from "@/features/trainings/workoutPlans/queries/workoutsWeeklyGoalQuery";
import useCreateOngoingTrainingMutation from "@/features/trainings/ongoing-workout/mutations/useCreateOngoingTrainingMutation";
import { getDateOnly } from "@/lib/date";
import { WorkoutPlanDto } from "@/services/openapi";
import { useQuery, useSuspenseQuery } from "@tanstack/react-query";

import {
  Calendar,
  CheckCircle2,
  ChevronRight,
  Flame,
  MoreVertical,
  Play,
  Target,
} from "lucide-react";

const MAX_PREVIEW_WORKOUTS = 3;

function truncateNameAfterTwoWords(name: string): string {
  const words = name.trim().split(/\s+/).filter(Boolean);
  if (words.length <= 2) return name.trim();
  return `${words.slice(0, 2).join(" ")}...`;
}

function getPlanPreviewWorkouts(
  plan: WorkoutPlanDto | undefined,
  nextWorkoutId: string | null,
) {
  if (!plan) return [];

  const workouts = plan.workouts ?? [];
  if (!workouts.length) return [];
  if (workouts.length <= MAX_PREVIEW_WORKOUTS) return workouts;

  // If we can't determine the current "next" workout, fall back to showing the first items.
  if (!nextWorkoutId) {
    return workouts.slice(0, MAX_PREVIEW_WORKOUTS);
  }

  const nextIdx = workouts.findIndex((w) => w.id === nextWorkoutId);
  if (nextIdx < 0) {
    return workouts.slice(0, MAX_PREVIEW_WORKOUTS);
  }

  // Show prev / current(next) / next in a cyclic way.
  const prevIdx = (nextIdx - 1 + workouts.length) % workouts.length;
  const currentIdx = nextIdx;
  const nextNextIdx = (nextIdx + 1) % workouts.length;

  return [workouts[prevIdx], workouts[currentIdx], workouts[nextNextIdx]];
}

function WorkoutPlansTopSection() {
  const navigate = useNavigate();
  const createOngoingTrainingMutation = useCreateOngoingTrainingMutation();
  const today = new Date();
  const currentMonthStart = getDateOnly(startOfMonth(today));
  const currentMonthEnd = getDateOnly(endOfMonth(today));

  const { data: activePlan } = useSuspenseQuery(workoutPlansQueryOptions.active);
  const { data: nextWorkout } = useSuspenseQuery(workoutPlansQueryOptions.nextWorkout);
  const { data: monthOverview } = useSuspenseQuery(
    trainingsOverviewQueryOptions.byDateRange(currentMonthStart, currentMonthEnd),
  );
  const { weekStart, weekEnd } = getCurrentWeekDateRange();
  const { data: weekOverview } = useSuspenseQuery(
    trainingsOverviewQueryOptions.byDateRange(weekStart, weekEnd),
  );
  const { data: workoutsGoal, isPending: isGoalPending } = useQuery(
    workoutsWeeklyGoalQueryOptions.current(),
  );
  const { data: workoutStreak } = useSuspenseQuery(workoutStreakQueryOptions.current());

  if (!activePlan) {
    return <WorkoutPlansTopSectionEmpty />
  }

  const nextWorkoutId = nextWorkout?.id ?? null;
  const previewWorkouts = getPlanPreviewWorkouts(activePlan, nextWorkoutId);

  const nextWorkoutIndexInPreview = previewWorkouts.findIndex(
    (w) => w.id === nextWorkoutId,
  );
  const safeNextWorkoutIndexInPreview =
    Math.max(nextWorkoutIndexInPreview, 0);

  const dayStreak = workoutStreak.dayStreak;
  const workoutsThisMonth = monthOverview.totalWorkoutsCompleted;
  const workoutsThisWeek = weekOverview.totalWorkoutsCompleted;

  const goalValue = workoutsGoal?.value ?? 0;
  const showSetGoalOverlay = isGoalPending === false && workoutsGoal === null;
  const weeklyProgressPercent =
    workoutsGoal && goalValue > 0
      ? Math.min(100, Math.round((workoutsThisWeek / goalValue) * 100))
      : 0;

  let weeklyProgressLabel: string;
  if (isGoalPending) {
    weeklyProgressLabel = "…";
  } else if (workoutsGoal) {
    weeklyProgressLabel = `${workoutsThisWeek}/${goalValue} workouts`;
  } else {
    weeklyProgressLabel = "—";
  }

  return (
    <div className="relative mb-6">
      <Card className="overflow-hidden">
        <div className="p-5 pb-4">
          <div className="mb-4 flex items-start justify-between">
            <div className="flex items-center gap-3">
              <div className="rounded-xl bg-primary/15 p-2.5">
                <Target className="h-5 w-5 text-primary" />
              </div>

              <div>
                <div className="flex items-center gap-2">
                  <span className="text-sm text-muted-foreground">Active Workout Plan</span>
                  {/* <CheckCircle2 className="h-4 w-4 text-primary" /> */}
                </div>
                <h2 className="text-xl font-semibold text-foreground">{activePlan.name}</h2>
              </div>
            </div>

            <div className="flex shrink-0 items-center gap-2">
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button
                    variant="outline"
                    className="h-9 gap-1 text-sm"
                    aria-label="Manage plan"
                    size="icon"
                  >
                    <MoreVertical className="h-4 w-4" />
                    {/* Manage plans
                    <ChevronDown className="h-4 w-4 opacity-60" /> */}
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end">
                  <DropdownMenuItem
                    className="cursor-pointer"
                    onClick={() =>
                      navigate({
                        to: "/trainings/workouts/plan/edit/$planId",
                        params: { planId: activePlan.id },
                      })
                    }
                  >
                    Edit active plan
                  </DropdownMenuItem>
                  <DropdownMenuItem
                    className="cursor-pointer"
                    onClick={() => navigate({ to: "/trainings/workouts/plan/manage" })}
                  >
                    Choose another plan
                  </DropdownMenuItem>
                  <DropdownMenuItem
                    className="cursor-pointer"
                    onClick={() => navigate({ to: "/trainings/workouts/workouts-goal" })}
                  >
                    Update weekly workout target
                  </DropdownMenuItem>

                </DropdownMenuContent>
              </DropdownMenu>
            </div>
          </div>

          <div className="mb-4 flex items-center gap-6">
            <div className="flex-1">
              <div className="mb-2 flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Weekly Progress</span>
                <span className="text-sm font-medium text-primary">{weeklyProgressLabel}</span>
              </div>

              <div className="h-2.5 w-full overflow-hidden rounded-full border border-primary/40 bg-primary/35">
                <div
                  className="h-full rounded-full bg-primary transition-all duration-500"
                  style={{
                    width:
                      workoutsGoal && goalValue > 0 ? `${weeklyProgressPercent}%` : "0%",
                  }}
                />
              </div>
            </div>

            <div className="flex items-center gap-4 border-l border-border pl-4">
              <div className="text-center">
                <div className="flex items-center justify-center gap-1 text-primary">
                  <Flame className="h-4 w-4" />
                  <span className="text-lg font-semibold text-foreground">{dayStreak}</span>
                </div>
                <span className="text-xs text-muted-foreground">Day Streak</span>
              </div>

              <div className="text-center">
                <div className="flex items-center justify-center gap-1 text-primary">
                  <Calendar className="h-4 w-4" />
                  <span className="text-lg font-semibold text-foreground">{workoutsThisMonth}</span>
                </div>
                <span className="text-xs text-muted-foreground">This Month</span>
              </div>
            </div>
          </div>

          {nextWorkoutId && (
            <div className="flex items-center justify-between rounded-xl border border-primary/20 bg-primary/10 p-4">
              <div className="flex items-center gap-3">
                <div className="rounded-lg bg-primary/20 p-2">
                  <MuscleGroupWorkoutIcon
                    muscleGroups={nextWorkout?.muscleGroups}
                    className="h-7 w-7 text-primary"
                  />
                </div>
                <div>
                  <span className="text-xs font-medium tracking-wide text-primary uppercase">
                    Up Next
                  </span>
                  <p className="font-medium text-foreground">{nextWorkout?.name}</p>
                </div>
              </div>

              <ButtonWithLoading
                onClick={() => {
                  createOngoingTrainingMutation.mutate(
                    { trainingId: nextWorkoutId },
                    {
                      onSuccess: () => {
                        navigate({ to: "/trainings/ongoing-workout" });
                      },
                    },
                  );
                }}
                disabled={createOngoingTrainingMutation.isPending}
                isLoading={createOngoingTrainingMutation.isDelayedPending}
                className="gap-2"
              >
                <Play className="h-4 w-4" /> Start Workout
              </ButtonWithLoading>
            </div>
          )}
        </div>

        <div className="border-t border-border bg-card-secondary/50 px-5 py-4">
          <div className="mb-3 flex items-center gap-2">
            <span className="text-xs font-medium tracking-wide text-muted-foreground uppercase">
              Workout Schedule
            </span>
          </div>

          <div className="grid grid-cols-3 gap-3">
            {previewWorkouts.map((workout, index) => {
              let state: "completed" | "ready" | "locked";
              if (index < safeNextWorkoutIndexInPreview) {
                state = "completed";
              } else if (index === safeNextWorkoutIndexInPreview) {
                state = "ready";
              } else {
                state = "locked";
              }

              const isCompleted = state === "completed";
              const isNext = state === "ready";
              let workoutCardStateClass = "bg-muted/30";
              if (isNext) {
                workoutCardStateClass = "bg-card-secondary ring-1 ring-primary/30";
              } else if (isCompleted) {
                workoutCardStateClass = "bg-muted/50";
              }

              let workoutBadgeStateClass = "bg-muted text-muted-foreground";
              if (isCompleted) {
                workoutBadgeStateClass = "bg-primary/10 text-primary";
              } else if (isNext) {
                workoutBadgeStateClass = "bg-primary text-primary-foreground";
              }

              return (
                <div
                  key={workout.id}
                  className={`relative flex cursor-pointer flex-col items-center gap-2 rounded-xl p-3 text-center transition-all hover:bg-muted/60 sm:flex-row sm:items-center sm:gap-3 sm:text-left ${workoutCardStateClass}`}
                >
                  <div
                    className={`flex h-8 w-8 shrink-0 items-center justify-center rounded-full text-sm font-medium ${workoutBadgeStateClass}`}
                  >
                    {isCompleted ? (
                      <CheckCircle2 className="h-4 w-4" />
                    ) : (
                      index + 1
                    )}
                  </div>

                  <div className="min-w-0 w-full flex-1 sm:text-left">
                    <div className="flex items-center justify-center gap-2 sm:justify-start">
                      <MuscleGroupWorkoutIcon
                        muscleGroups={workout.muscleGroups}
                        className={`h-6 w-6 shrink-0 ${isCompleted ? "text-muted-foreground" : "text-primary"}`}
                      />
                      <p
                        className={`text-sm font-medium sm:truncate ${isCompleted ? "text-muted-foreground" : "text-foreground"
                          }`}
                        title={workout.name}
                      >
                        {truncateNameAfterTwoWords(workout.name ?? "")}
                      </p>
                    </div>

                    {isNext && (
                      <span className="text-xs text-primary">
                        Ready to go
                      </span>
                    )}

                    {isCompleted && (
                      <span className="text-xs text-muted-foreground">Completed</span>
                    )}
                  </div>

                  {isNext && (
                    <ChevronRight className="h-4 w-4 shrink-0 text-primary" />
                  )}
                </div>
              );
            })}
          </div>
        </div>
      </Card>

      {showSetGoalOverlay ? (
        <div className="pointer-events-auto absolute inset-0 z-10 flex flex-col items-center justify-center gap-4 rounded-xl border border-border bg-background/65 p-6 text-center shadow-sm backdrop-blur-md">
          <div className="max-w-sm space-y-2">
            <p className="text-base font-semibold text-foreground">Set your weekly workout target</p>
            <p className="text-sm text-muted-foreground">
              Choose how many workouts you want to complete each week. We will track your progress
              from Monday through Sunday.
            </p>
          </div>
          <Button
            type="button"
            onClick={() => navigate({ to: "/trainings/workouts/workouts-goal" })}
          >
            Set weekly target
          </Button>
        </div>
      ) : null}
    </div>
  );
}

export default WorkoutPlansTopSection;

function WorkoutPlansTopSectionEmpty() {

  const navigate = useNavigate();

  return (
    <Card className="mb-6 p-5">
      <div className="flex flex-col gap-3 @3xl:flex-row @3xl:items-center @3xl:justify-between">
        <div className="space-y-1">
          <p className="text-base font-semibold text-foreground">No active workout plan</p>
          <p className="text-sm text-muted-foreground">
            Create a workout plan to get your next workout suggestion.
          </p>
        </div>

        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            onClick={() => navigate({ to: "/trainings/workouts/plan/manage" })}
          >
            Manage plans
          </Button>
          <Button onClick={() => navigate({ to: "/trainings/workouts/plan/create" })}>
            Create plan
          </Button>
        </div>
      </div>
    </Card>
  );
}
