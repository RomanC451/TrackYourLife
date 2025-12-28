import { useMemo, useState } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { SearchIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Combobox,
  ComboboxContent,
  ComboboxInput,
  ComboboxItem,
  ComboboxList,
  ComboboxTrigger,
} from "@/components/ui/shadcn-io/combobox";
import { usePerformanceMonitor } from "@/hooks/use-performance-monitor";

import { exercisesQueryOptions } from "../../queries/exercisesQuery";
import ExerciseListItem from "./exercisesListItem";

function ExercisesList() {
  usePerformanceMonitor("ExercisesList");

  const { data: exercises } = useSuspenseQuery(exercisesQueryOptions.all);

  const sortedExercises = exercises.toSorted((a, b) =>
    a.createdOnUtc.localeCompare(b.createdOnUtc),
  );

  const muscleGroups = useMemo(() => {
    const uniqueMuscleGroups = Array.from(
      new Set(sortedExercises.flatMap((exercise) => exercise.muscleGroups)),
    ).sort((a, b) => a.localeCompare(b));

    return uniqueMuscleGroups.map((muscleGroup) => ({
      label: muscleGroup,
      value: muscleGroup,
    }));
  }, [sortedExercises]);

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

    return sortedExercises;
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
          <Combobox
            data={muscleGroups}
            type="Muscle Group"
            value={selectedMuscleGroup}
            onValueChange={(value) => setSelectedMuscleGroup(value)}
          >
            <ComboboxTrigger className="min-w-[150px]" />
            <ComboboxContent className="mt-2">
              <ComboboxInput placeholder="Muscle Group" />

              <ComboboxList>
                {muscleGroups.map((muscleGroup) => (
                  <ComboboxItem
                    className="m-1"
                    key={muscleGroup.value}
                    value={muscleGroup.value}
                  >
                    {muscleGroup.label}
                  </ComboboxItem>
                ))}
              </ComboboxList>
            </ComboboxContent>
          </Combobox>
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
