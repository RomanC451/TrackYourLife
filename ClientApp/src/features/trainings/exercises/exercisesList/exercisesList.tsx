import { useSuspenseQuery } from "@tanstack/react-query";

import { exercisesQueryOptions } from "../queries/exercisesQuery";
import ExerciseListItem from "./exercisesListItem";

function ExercisesList() {
  const { data: exercises } = useSuspenseQuery(exercisesQueryOptions.all);

  const sortedExercises = exercises.toSorted((a, b) =>
    a.createdOnUtc.localeCompare(b.createdOnUtc),
  );

  return (
    <div className="grid grid-cols-1 gap-6 @3xl/page-card:grid-cols-2 @6xl/page-card:grid-cols-3">
      {sortedExercises.map((exercise) => (
        <ExerciseListItem key={exercise.id} exercise={exercise} />
      ))}
    </div>
  );
}

export default ExercisesList;
