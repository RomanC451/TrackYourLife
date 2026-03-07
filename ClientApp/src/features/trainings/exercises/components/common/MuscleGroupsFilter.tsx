import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";

import {
  Combobox,
  ComboboxContent,
  ComboboxInput,
  ComboboxItem,
  ComboboxList,
  ComboboxTrigger,
} from "@/components/ui/shadcn-io/combobox";
import {
  flattenMuscleGroups,
  muscleGroupsQueryOptions,
} from "@/features/trainings/exercises/queries/useMuscleGroupsQuery";

const SUBGROUP_INDENT = "\u00A0\u00A0\u00A0";

type MuscleGroupsFilterProps = {
  selectedMuscleGroup: string;
  setSelectedMuscleGroup: (value: string) => void;
};

function MuscleGroupsFilter({
  selectedMuscleGroup,
  setSelectedMuscleGroup,
}: MuscleGroupsFilterProps) {
  const { data: muscleGroupsTree = [] } = useQuery(
    muscleGroupsQueryOptions.all,
  );

  const muscleGroups = useMemo(() => {
    const flat = flattenMuscleGroups(muscleGroupsTree);
    return [
      { label: "All", value: "" },
      ...flat.map((g) => ({
        label: g.isSubgroup ? SUBGROUP_INDENT + g.name : g.name,
        value: g.name,
      })),
    ];
  }, [muscleGroupsTree]);

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
