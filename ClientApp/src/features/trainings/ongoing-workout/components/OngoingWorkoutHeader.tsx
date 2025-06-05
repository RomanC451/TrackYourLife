import { OngoingTrainingDto } from "@/services/openapi";

import useActiveOngoingTrainingQuery from "../queries/useActiveOngoingTrainingQuery";

function OngoingWorkoutHeader() {
  // Example values; in a real app, these would be props or state

  const { activeOngoingTrainingQuery: ongoingTrainingQuery } =
    useActiveOngoingTrainingQuery();

  if (!ongoingTrainingQuery.data) {
    return null;
  }

  const progress = calculateProgress(ongoingTrainingQuery.data); // percent
  const exercise = ongoingTrainingQuery.data.exerciseIndex + 1;
  const totalExercises = ongoingTrainingQuery.data.training.exercises.length;
  const set = ongoingTrainingQuery.data.setIndex + 1;
  const totalSets =
    ongoingTrainingQuery.data.training.exercises[
      ongoingTrainingQuery.data.exerciseIndex
    ].exerciseSets.length;
  const exerciseName =
    ongoingTrainingQuery.data.training.exercises[
      ongoingTrainingQuery.data.exerciseIndex
    ].name;

  return (
    <div className="space-y-6">
      <div className="mt-4 flex w-full items-center justify-center">
        <h1 className="text-2xl">{exerciseName}</h1>
      </div>
      {/* Loader/Progress Bar */}
      <div className="mb-1 h-2 w-full rounded-full bg-[#232b3a]">
        <div
          className="h-2 rounded-full bg-violet-600 transition-all duration-300"
          style={{ width: `${progress}%` }}
        ></div>
      </div>
      <div className="flex justify-between text-xs text-[#b3c0d5]">
        <span>
          Exercise {exercise} of {totalExercises} - Set {set} of {totalSets}
        </span>
        <span>{progress}% complete</span>
      </div>
    </div>
  );
}

export default OngoingWorkoutHeader;

function calculateProgress(data: OngoingTrainingDto) {
  const totalSets = data.training.exercises.reduce((acc, exercise) => {
    return acc + exercise.exerciseSets.length;
  }, 0);
  let completedSets = data.training.exercises
    .slice(0, data.exerciseIndex)
    .reduce((acc, exercise) => {
      return acc + exercise.exerciseSets.length;
    }, 0);

  console.log(completedSets);

  completedSets += data.training.exercises[
    data.exerciseIndex
  ].exerciseSets.slice(0, data.setIndex).length;

  console.log(completedSets, totalSets);

  return ((completedSets / totalSets) * 100).toFixed();
}
