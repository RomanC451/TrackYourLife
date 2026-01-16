import { useMemo } from "react";

import {
  Combobox,
  ComboboxContent,
  ComboboxInput,
  ComboboxItem,
  ComboboxList,
  ComboboxTrigger,
} from "@/components/ui/shadcn-io/combobox";
import { ExerciseDto } from "@/services/openapi";

type MuscleGroupsFilterProps = {
  exercises: ExerciseDto[];
  selectedMuscleGroup: string;
  setSelectedMuscleGroup: (value: string) => void;
};

function MuscleGroupsFilter({
  exercises,
  selectedMuscleGroup,
  setSelectedMuscleGroup,
}: MuscleGroupsFilterProps) {
  const muscleGroups = useMemo(() => {
    const uniqueMuscleGroups = Array.from(
      new Set(exercises.flatMap((exercise) => exercise.muscleGroups)),
    ).sort((a, b) => a.localeCompare(b));

    return [
      { label: "All", value: "" },
      ...uniqueMuscleGroups.map((muscleGroup) => ({
        label: muscleGroup,
        value: muscleGroup,
      })),
    ];
  }, [exercises]);

  return (
    <Combobox
      data={muscleGroups}
      type="Muscle Group"
      value={selectedMuscleGroup}
      onValueChange={(value) => setSelectedMuscleGroup(value)}
      modal
    >
      <ComboboxTrigger
        className="min-w-[150px]"
        type="button"
        onClick={(e) => e.stopPropagation()}
      />
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
  );
}

export default MuscleGroupsFilter;
