import { queryOptions } from "@tanstack/react-query";

import { MuscleGroupDto, MuscleGroupsApi } from "@/services/openapi";

const muscleGroupsApi = new MuscleGroupsApi();

export const muscleGroupsQueryKeys = {
  all: ["muscleGroups"] as const,
};

export const muscleGroupsQueryOptions = {
  all: queryOptions({
    queryKey: muscleGroupsQueryKeys.all,
    queryFn: () => muscleGroupsApi.getMuscleGroups().then((res) => res.data),
  }),
};

export type FlatMuscleGroup = {
  id: string;
  name: string;
  isSubgroup: boolean;
  /** Set for subgroups: the parent muscle group name. */
  parentName?: string;
};

/**
 * Flattens the muscle groups tree (roots + children) into a list.
 * Subgroups (children) are marked with isSubgroup: true and include parentName for indentation / auto-select parent.
 */
export function flattenMuscleGroups(groups: MuscleGroupDto[]): FlatMuscleGroup[] {
  const result: FlatMuscleGroup[] = [];
  for (const g of groups) {
    result.push({ id: g.id, name: g.name, isSubgroup: false });
    for (const c of g.children ?? []) {
      result.push({
        id: c.id,
        name: c.name,
        isSubgroup: true,
        parentName: g.name,
      });
    }
  }
  return result;
}
