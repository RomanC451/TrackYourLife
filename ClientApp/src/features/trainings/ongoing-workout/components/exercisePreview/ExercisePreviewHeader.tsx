import { useSuspenseQuery } from "@tanstack/react-query";
import { CheckCircle, Target } from "lucide-react";

import { Card } from "@/components/ui/card";
import { OngoingTrainingDto } from "@/services/openapi";

import { ongoingTrainingsQueryOptions } from "../../queries/ongoingTrainingsQuery";

function OngoingWorkoutHeader() {
  const ongoingTrainingQuery = useSuspenseQuery(
    ongoingTrainingsQueryOptions.active,
  );

  const data = ongoingTrainingQuery.data;
  const exerciseIndex = data.exerciseIndex;
  const exercise = exerciseIndex + 1;
  const totalExercises = data.training.exercises.length;
  const sets = data.training.exercises[exerciseIndex].exerciseSets;
  const exerciseName = data.training.exercises[exerciseIndex].name;
  // Placeholder for muscle group (replace with real data if available)
  const muscleGroup =
    data.training.exercises[exerciseIndex].muscleGroups.join(", ");
  // Placeholder for muscle group icon (replace with real icon if available)

  // Progress calculation
  const progress = calculateProgress(data);

  return (
    <Card className="w-full rounded-xl p-6 shadow-lg">
      {/* Header row */}
      <div className="mb-2 flex items-center justify-between">
        <h1 className="text-2xl font-bold tracking-tight">{exerciseName}</h1>
        <div className="flex items-center gap-1">
          <Target className="text-primary" />
          <span className="text-base font-semibold text-primary">
            {muscleGroup}
          </span>
        </div>
      </div>
      {/* Subheader row */}
      <div className="mb-2 flex items-center justify-between">
        <span className="text-sm">
          Exercise {exercise} of {totalExercises}
        </span>
        <span className="text-base font-semibold text-green-400">
          {progress}% complete
        </span>
      </div>
      {/* Progress bar */}
      <div className="mb-4 h-3 w-full rounded-full bg-slate-600 dark:bg-slate-600">
        <div
          className="h-3 rounded-full bg-violet-600 transition-all duration-300"
          style={{ width: `${progress}%` }}
        ></div>
      </div>
      {/* Set status row */}
      <div className="mt-2 flex items-center justify-around">
        {sets.map((_, idx) => {
          const isCompleted = idx < data.setIndex;
          const isCurrent = idx === data.setIndex;
          return (
            <div key={idx} className="flex flex-col items-center">
              {isCompleted ? (
                <span className="flex h-10 w-10 items-center justify-center rounded-full bg-green-500">
                  <CheckCircle className="text-white" size={24} />
                </span>
              ) : isCurrent ? (
                <span className="flex h-10 w-10 items-center justify-center rounded-full border-2 border-primary bg-slate-300 dark:bg-slate-800">
                  <span className="text-lg font-bold">{idx + 1}</span>
                </span>
              ) : (
                <span className="flex h-10 w-10 items-center justify-center rounded-full border-2 bg-slate-300 dark:bg-slate-800">
                  <span className="text-lg font-bold">{idx + 1}</span>
                </span>
              )}
              <span
                className={
                  isCompleted
                    ? "mt-1 text-sm font-semibold text-green-400"
                    : "mt-1 text-sm font-semibold"
                }
              >
                Set {idx + 1}
              </span>
            </div>
          );
        })}
      </div>
    </Card>
  );
}

export default OngoingWorkoutHeader;

function calculateProgress(data: OngoingTrainingDto) {
  const totalExercises = data.training.exercises.length;
  const completedCount = data.completedExerciseIds?.length || 0;
  const skippedCount = data.skippedExerciseIds?.length || 0;
  const completedOrSkippedCount = completedCount + skippedCount;

  if (totalExercises === 0) {
    return "0";
  }

  return Math.round(
    (completedOrSkippedCount / totalExercises) * 100,
  ).toString();
}
