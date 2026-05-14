import { useSuspenseQuery } from "@tanstack/react-query";
import { format } from "date-fns";
import {
  ArrowLeft,
  BarChart3,
  CheckCircle2,
  Clock,
  Dumbbell,
  Flame,
  Layers,
  MinusCircle,
  Timer,
  TrendingUp,
} from "lucide-react";
import { Link } from "@tanstack/react-router";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ongoingTrainingsQueryOptions } from "@/features/trainings/ongoing-workout/queries/ongoingTrainingsQuery";
import { deriveWorkoutSessionStats } from "@/lib/workoutSessionStats";
import { formatDurationMs } from "@/lib/time";

function StatRow({
  label,
  value,
  icon: Icon,
}: {
  label: string;
  value: string;
  icon: typeof Clock;
}) {
  return (
    <div className="flex items-center justify-between gap-4 rounded-lg border border-border/60 bg-secondary/30 px-4 py-3">
      <div className="flex items-center gap-3">
        <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary">
          <Icon className="h-4 w-4" />
        </div>
        <span className="text-sm text-muted-foreground">{label}</span>
      </div>
      <span className="text-right text-sm font-semibold tabular-nums text-foreground">
        {value}
      </span>
    </div>
  );
}

function WorkoutSessionStatsPage({
  ongoingTrainingId,
}: {
  ongoingTrainingId: string;
}) {
  const { data: ongoing } = useSuspenseQuery(
    ongoingTrainingsQueryOptions.byId(ongoingTrainingId),
  );

  const derived = deriveWorkoutSessionStats(ongoing);
  const isFinished = Boolean(ongoing.finishedOnUtc);
  const trainingName = ongoing.training.name;

  const durationLabel =
    derived.actualDurationSeconds != null
      ? formatDurationMs(derived.actualDurationSeconds * 1000)
      : "—";

  const pacingLabel =
    derived.pacingRatioVsPlanned != null
      ? `${Math.round(derived.pacingRatioVsPlanned * 100)}% of planned time (${ongoing.training.duration} min template)`
      : ongoing.training.duration > 0
        ? "—"
        : "No planned duration on template";

  const volumeLabel =
    derived.volumeHeuristic > 0
      ? derived.volumeHeuristic.toLocaleString(undefined, {
          maximumFractionDigits: 1,
        })
      : "—";

  const restLabel =
    derived.plannedRestSecondsFromSets > 0
      ? formatDurationMs(derived.plannedRestSecondsFromSets * 1000)
      : "—";

  return (
    <PageCard className="max-w-2xl">
      <div className="mb-6 flex flex-wrap items-center justify-between gap-3">
        <PageTitle title="Workout stats" />
        <Button variant="outline" size="sm" asChild>
          <Link to="/trainings/history">
            <ArrowLeft className="mr-2 h-4 w-4" />
            History
          </Link>
        </Button>
      </div>

      {!isFinished ? (
        <Alert className="mb-6">
          <AlertTitle>Workout still in progress</AlertTitle>
          <AlertDescription>
            Timestamps and pacing reflect a session that has not finished yet. Finish the
            workout for a complete summary.
          </AlertDescription>
        </Alert>
      ) : null}

      <Card className="mb-6 border-border/60">
        <CardHeader className="pb-2">
          <div className="flex items-start gap-3">
            <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-primary/10 text-primary">
              <BarChart3 className="h-5 w-5" />
            </div>
            <div>
              <CardTitle className="text-xl leading-tight">{trainingName}</CardTitle>
              <p className="mt-1 text-sm text-muted-foreground">
                {ongoing.finishedOnUtc
                  ? `Finished ${format(new Date(ongoing.finishedOnUtc), "MMM d, yyyy 'at' h:mm a")}`
                  : `Started ${format(new Date(ongoing.startedOnUtc), "MMM d, yyyy 'at' h:mm a")}`}
              </p>
            </div>
          </div>
        </CardHeader>
        <CardContent className="space-y-2">
          <StatRow
            icon={Clock}
            label="Session duration"
            value={durationLabel}
          />
          {typeof ongoing.caloriesBurned === "number" ? (
            <StatRow
              icon={Flame}
              label="Calories"
              value={`${ongoing.caloriesBurned.toLocaleString()} kcal`}
            />
          ) : null}
          <StatRow
            icon={CheckCircle2}
            label="Exercises completed / planned"
            value={`${ongoing.completedExerciseIds?.length ?? 0} / ${derived.totalExercises}`}
          />
          <StatRow
            icon={MinusCircle}
            label="Exercises skipped"
            value={`${derived.skippedCount}`}
          />
          <StatRow
            icon={TrendingUp}
            label="Completion rate"
            value={`${derived.completionRatePercent.toFixed(0)}%`}
          />
          <StatRow
            icon={TrendingUp}
            label="Skip rate"
            value={`${derived.skipRatePercent.toFixed(0)}%`}
          />
          <StatRow
            icon={Layers}
            label="Sets (completed exercises)"
            value={`${derived.totalSetsCompleted}`}
          />
          <StatRow
            icon={Dumbbell}
            label="Volume (heuristic)"
            value={volumeLabel}
          />
          <StatRow
            icon={Timer}
            label="Planned rest in sets (sum)"
            value={restLabel}
          />
          <StatRow icon={Clock} label="Pacing vs template duration" value={pacingLabel} />
        </CardContent>
      </Card>

      <Alert>
        <AlertTitle>How volume and sets are counted</AlertTitle>
        <AlertDescription>
          Numbers use the exercise list and set rows on this session&apos;s training snapshot.
          If you changed loads or reps during the workout, the app stores those adjustments
          separately; a dedicated read could merge that history later for exact logged totals.
        </AlertDescription>
      </Alert>

      <div className="mt-6 flex flex-wrap gap-2">
        <Button variant="secondary" asChild>
          <Link to="/trainings/workouts">
            <ArrowLeft className="mr-2 h-4 w-4" />
            Workouts
          </Link>
        </Button>
        <Button variant="outline" asChild>
          <Link
            to="/trainings/workout-template-stats/$workoutId"
            params={{ workoutId: ongoing.training.id }}
          >
            Template stats
          </Link>
        </Button>
      </div>
    </PageCard>
  );
}

export default WorkoutSessionStatsPage;
