import { useMemo, useState } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { SearchIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

import { exercisesQueryOptions } from "../../queries/exercisesQuery";
import MuscleGroupsFilter from "../common/MuscleGroupsFilter";
import ExerciseListItem from "./exercisesListItem";

function ExercisesList() {
  const { data: exercises } = useSuspenseQuery(exercisesQueryOptions.all);

  const [selectedMuscleGroup, setSelectedMuscleGroup] = useState("");

  const [searchQuery, setSearchQuery] = useState("");

  const filteredExercises = useMemo(() => {
    let sortedExercises = [...exercises];
    if (selectedMuscleGroup !== "") {
      sortedExercises = sortedExercises.filter((exercise) =>
        exercise.muscleGroups.includes(selectedMuscleGroup),
      );
    }

    if (searchQuery !== "") {
      sortedExercises = sortedExercises.filter((exercise) =>
        exercise.name.toLowerCase().startsWith(searchQuery.toLowerCase()),
      );
    }

    // Deduplicate by exercise ID to prevent duplicate key errors
    const uniqueExercises = Array.from(
      new Map(
        sortedExercises.map((exercise) => [exercise.id, exercise]),
      ).values(),
    );

    return uniqueExercises;
  }, [exercises, selectedMuscleGroup, searchQuery]);

  return (
    <>
      <div className="flex w-full justify-between gap-2">
        <div className="flex gap-2">
          <div className="relative">
            <Input
              placeholder="Search"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
            <SearchIcon
              size={16}
              className="absolute right-2 top-1/2 -translate-y-1/2"
            />
          </div>
          <MuscleGroupsFilter
            exercises={exercises}
            selectedMuscleGroup={selectedMuscleGroup}
            setSelectedMuscleGroup={setSelectedMuscleGroup}
          />
        </div>
        <Button
          variant="secondary"
          onClick={() => {
            setSelectedMuscleGroup("");
            setSearchQuery("");
          }}
        >
          Reset filters
        </Button>
      </div>
      <div className="grid grid-cols-1 gap-6 @3xl/page-card:grid-cols-2 @6xl/page-card:grid-cols-3">
        {filteredExercises.map((exercise) => (
          <ExerciseListItem key={exercise.id} exercise={exercise} />
        ))}
      </div>
    </>
  );
}

export default ExercisesList;
