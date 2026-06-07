import { describe, expect, it } from "vitest";

import type { MuscleGroupDto } from "@/services/openapi";

import { flattenMuscleGroups } from "../useMuscleGroupsQuery";

describe("flattenMuscleGroups", () => {
  it("flattens roots and subgroups with parent names", () => {
    const groups: MuscleGroupDto[] = [
      {
        id: "arms",
        name: "Arms",
        isLoading: false,
        isDeleting: false,
        children: [
          { id: "biceps", name: "Biceps", children: [], isLoading: false, isDeleting: false },
          { id: "triceps", name: "Triceps", children: [], isLoading: false, isDeleting: false },
        ],
      },
      {
        id: "legs",
        name: "Legs",
        children: [],
        isLoading: false,
        isDeleting: false,
      },
    ];

    expect(flattenMuscleGroups(groups)).toEqual([
      { id: "arms", name: "Arms", isSubgroup: false },
      { id: "biceps", name: "Biceps", isSubgroup: true, parentName: "Arms" },
      { id: "triceps", name: "Triceps", isSubgroup: true, parentName: "Arms" },
      { id: "legs", name: "Legs", isSubgroup: false },
    ]);
  });

  it("handles groups without children", () => {
    expect(
      flattenMuscleGroups([{ id: "core", name: "Core", children: [], isLoading: false, isDeleting: false }]),
    ).toEqual([{ id: "core", name: "Core", isSubgroup: false }]);
  });
});
