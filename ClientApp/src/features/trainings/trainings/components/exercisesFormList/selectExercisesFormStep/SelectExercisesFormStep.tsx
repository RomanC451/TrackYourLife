import { useEffect, useState } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { Search } from "lucide-react";
import { useFormContext, useWatch } from "react-hook-form";

import { Input } from "@/components/ui/input";
import { exercisesQueryOptions } from "@/features/trainings/exercises/queries/exercisesQuery";
import { ExerciseDto, ExerciseSet } from "@/services/openapi";

import { TrainingFormSchema } from "../../../data/trainingsSchemas";
import { ExercisesFormStep } from "../ExercisesFormList";
import SelectExercisesFormStepFooter from "./SelectExercisesFormStepFooter";
import SelectExercisesList from "./SelectExercisesList";

function SelectExercisesFormStep({
  onCancel,
  setStep,
}: {
  onCancel: () => void;
  setStep: (step: ExercisesFormStep) => void;
}) {
  const { control, setValue, getValues } = useFormContext<TrainingFormSchema>();

  const [searchQuery, setSearchQuery] = useState("");

  const selectedExercises = useWatch({ control, name: "exercises" });

  const onExerciseSelect = (exercise: ExerciseDto) => {
    if (exercise.isDeleting || exercise.isLoading) {
      return;
    }

    const isSelected = selectedExercises.some((e) => e.id === exercise.id);

    if (isSelected) {
      setValue(
        "exercises",
        selectedExercises.filter((e) => e.id !== exercise.id),
      );
    } else {
      setValue("exercises", [...selectedExercises, exercise]);
    }
  };

  const exercisesQuery = useSuspenseQuery(exercisesQueryOptions.all);

  useEffect(() => {
    if (exercisesQuery.data) {
      const currentExercises = getValues("exercises").map((e) => ({
        ...e,
        isDeleting: false,
        isLoading: false,
        createdOnUtc: "",
      }));

      const exercisesToUpdate = currentExercises.filter((exercise) => {
        const newExercise = exercisesQuery.data.find(
          (e) => e.id === exercise.id,
        );

        return !compareExercises(exercise, newExercise || exercise);
      });

      if (exercisesToUpdate.length > 0) {
        return;
      }

      const newExercises = currentExercises.map((exercise) => {
        const newExercise = exercisesToUpdate.find((e) => e.id === exercise.id);
        return newExercise || exercise;
      });

      setValue("exercises", newExercises);
    }
  }, [exercisesQuery.data, getValues, setValue]);

  useEffect(() => {
    if (exercisesQuery.data) {
      const existingExercisesIds = exercisesQuery.data.map(
        (exercise) => exercise.id,
      );

      const currentExercises = getValues("exercises").filter(
        (exercise) => exercise.id && existingExercisesIds.includes(exercise.id),
      );

      setValue("exercises", currentExercises);
    }
  }, [exercisesQuery.data, getValues, setValue]);

  const filteredExercises = exercisesQuery.data?.filter((exercise) =>
    exercise.name.toLowerCase().includes(searchQuery.toLowerCase()),
  );

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-semibold">Step 1: Select Exercises</h2>
      </div>

      <div className="relative">
        <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
        <Input
          placeholder="Search exercises..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="pl-8"
        />
      </div>

      <SelectExercisesList
        exercises={filteredExercises || []}
        selectedExercises={selectedExercises}
        onSelect={(exercise) => {
          onExerciseSelect(exercise);
        }}
      />

      <SelectExercisesFormStepFooter
        selectedExercises={selectedExercises}
        onCancel={onCancel}
        setStep={setStep}
      />
    </div>
  );
}

SelectExercisesFormStep.Loading = function Loading({
  searchQuery,
  setSearchQuery,
  empty,
}: {
  searchQuery: string;
  setSearchQuery: (value: string) => void;
  empty?: boolean;
}) {
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-semibold">Step 1: Select Exercises</h2>
      </div>

      <div className="relative">
        <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
        <Input
          placeholder="Search exercises..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="pl-8"
        />
      </div>

      {empty ? <SelectExercisesList.Empty /> : <SelectExercisesList.Loading />}

      <SelectExercisesFormStepFooter
        selectedExercises={[]}
        onCancel={() => {}}
        setStep={() => {}}
      />
    </div>
  );
};

function compareExercises(exercise1: ExerciseDto, exercise2: ExerciseDto) {
  return (
    exercise1.id === exercise2.id &&
    exercise1.name === exercise2.name &&
    exercise1.description === exercise2.description &&
    exercise1.equipment === exercise2.equipment &&
    exercise1.videoUrl === exercise2.videoUrl &&
    exercise1.exerciseSets.length === exercise2.exerciseSets.length &&
    exercise1.exerciseSets.every((set, index) =>
      compareExerciseSets(set, exercise2.exerciseSets[index]),
    )
  );
}

function compareExerciseSets(set1: ExerciseSet, set2: ExerciseSet) {
  if (set1.count1 !== set2.count1) {
    return false;
  }

  if (set1.unit1 !== set2.unit1) {
    return false;
  }

  if (set1.count2 !== set2.count2) {
    return false;
  }

  if (set1.unit2 !== set2.unit2) {
    return false;
  }

  return true;
}

export default SelectExercisesFormStep;
