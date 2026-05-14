import type { ExerciseSet, OngoingTrainingDto } from "@/services/openapi";

/**
 * Set lines and totals below use `OngoingTrainingDto.training.exercises[].exerciseSets`
 * (the snapshot embedded on the session). Per-exercise adjustments during a live workout
 * are stored separately in exercise history (old vs new set JSON); a future read model or
 * endpoint could merge those for “final logged” stats without changing this heuristic layer.
 */
export type WorkoutSessionDerivedStats = {
  totalExercises: number;
  completedCount: number;
  skippedCount: number;
  skipRatePercent: number;
  completionRatePercent: number;
  totalSetsCompleted: number;
  /** Sum of count1×count2 when count2 is set, else count1 (numeric only). */
  volumeHeuristic: number;
  /** Sum of per-set restTimeSeconds on completed exercises’ sets. */
  plannedRestSecondsFromSets: number;
  /** actual duration / (planned template duration in minutes × 60); null if not comparable. */
  pacingRatioVsPlanned: number | null;
  actualDurationSeconds: number | null;
};

function volumeForSet(set: ExerciseSet): number {
  const c1 = set.count1;
  const c2 = set.count2;
  if (c2 != null && Number.isFinite(c1) && Number.isFinite(c2)) {
    return c1 * c2;
  }
  return Number.isFinite(c1) ? c1 : 0;
}

export function deriveWorkoutSessionStats(
  ongoing: OngoingTrainingDto,
): WorkoutSessionDerivedStats {
  const exercises = ongoing.training.exercises ?? [];
  const totalExercises = exercises.length;
  const completedIds = new Set(ongoing.completedExerciseIds ?? []);
  const skippedIds = new Set(ongoing.skippedExerciseIds ?? []);
  const completedCount = completedIds.size;
  const skippedCount = skippedIds.size;

  const skipRatePercent =
    totalExercises > 0 ? (skippedCount / totalExercises) * 100 : 0;
  const completionRatePercent =
    totalExercises > 0 ? (completedCount / totalExercises) * 100 : 0;

  let totalSetsCompleted = 0;
  let volumeHeuristic = 0;
  let plannedRestSecondsFromSets = 0;

  for (const ex of exercises) {
    if (!completedIds.has(ex.id)) {
      continue;
    }
    const sets = ex.exerciseSets ?? [];
    totalSetsCompleted += sets.length;
    for (const set of sets) {
      volumeHeuristic += volumeForSet(set);
      if (typeof set.restTimeSeconds === "number") {
        plannedRestSecondsFromSets += set.restTimeSeconds;
      }
    }
  }

  let actualDurationSeconds: number | null = null;
  let pacingRatioVsPlanned: number | null = null;
  if (ongoing.finishedOnUtc && ongoing.startedOnUtc) {
    const ms =
      new Date(ongoing.finishedOnUtc).getTime() -
      new Date(ongoing.startedOnUtc).getTime();
    actualDurationSeconds = Math.max(0, Math.round(ms / 1000));
    const plannedMinutes = ongoing.training.duration;
    if (plannedMinutes > 0 && actualDurationSeconds != null) {
      const plannedSeconds = plannedMinutes * 60;
      pacingRatioVsPlanned = actualDurationSeconds / plannedSeconds;
    }
  }

  return {
    totalExercises,
    completedCount,
    skippedCount,
    skipRatePercent,
    completionRatePercent,
    totalSetsCompleted,
    volumeHeuristic,
    plannedRestSecondsFromSets,
    pacingRatioVsPlanned,
    actualDurationSeconds,
  };
}
