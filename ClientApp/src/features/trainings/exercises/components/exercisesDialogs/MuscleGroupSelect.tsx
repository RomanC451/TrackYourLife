import { useMemo } from "react";
import { Tag } from "emblor-maintained";
import { useFormContext } from "react-hook-form";
import { useQuery } from "@tanstack/react-query";

import InputInnerTags from "@/components/input-inner-tags";
import {
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import {
  flattenMuscleGroups,
  type FlatMuscleGroup,
  muscleGroupsQueryOptions,
} from "@/features/trainings/exercises/queries/useMuscleGroupsQuery";

const SUBGROUP_INDENT = "\u00A0\u00A0\u00A0"; // non-breaking spaces for indent

export function MuscleGroupSelect() {
  const form = useFormContext();
  const { data: muscleGroupsTree = [] } = useQuery(
    muscleGroupsQueryOptions.all,
  );

  const { autocompleteOptions, optionDisplayText, subgroupToParent } =
    useMemo(() => {
      const flat = flattenMuscleGroups(muscleGroupsTree);
      const options: Tag[] = flat.map((g) => ({ id: g.id, text: g.name }));
      const childIds = new Set(
        flat.filter((g) => g.isSubgroup).map((g) => g.id),
      );
      const subgroupToParent = new Map(
        flat
          .filter((g): g is FlatMuscleGroup & { parentName: string } => !!g.parentName)
          .map((g) => [g.name, g.parentName]),
      );
      return {
        autocompleteOptions: options,
        optionDisplayText: (option: Tag) =>
          childIds.has(option.id) ? SUBGROUP_INDENT + option.text : option.text,
        subgroupToParent,
      };
    }, [muscleGroupsTree]);

  return (
    <FormField
      control={form.control}
      name="muscleGroups"
      render={({ field }) => (
        <FormItem className="flex flex-col">
          <FormLabel>Muscle Groups</FormLabel>
          <InputInnerTags
            setTags={(newTags) => {
              const names = new Set(newTags.map((tag) => tag.text));
              for (const name of names) {
                const parent = subgroupToParent.get(name);
                if (parent) names.add(parent);
              }
              field.onChange(Array.from(names));
            }}
            initialTags={field.value.map((tag: string, index: number) => ({
              id: index.toString(),
              text: tag,
            }))}
            autocompleteOptions={autocompleteOptions}
            placeholder="Select a muscle group"
            optionDisplayText={optionDisplayText}
            allowCustomTags={false}
          />
          <FormMessage />
        </FormItem>
      )}
    />
  );
}
