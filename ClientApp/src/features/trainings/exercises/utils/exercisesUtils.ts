import { v4 as uuidv4 } from "uuid";

import { Difficulty, ExerciseSet } from "@/services/openapi";

export const getDifficultyColor = (difficulty: Difficulty) => {
  switch (difficulty) {
    case "Easy":
      return "bg-green-500/10 text-green-400 border-green-500/30";
    case "Medium":
      return "bg-orange-500/10 text-orange-400 border-orange-500/30";
    case "Hard":
      return "bg-red-500/10 text-red-400 border-red-500/30";
    default:
      return "bg-gray-500/10 text-gray-400 border-gray-500/30";
  }
};

export function createDefaultExerciseSet(
  currentSets: ExerciseSet[],
): ExerciseSet {
  const length = currentSets.length;

  if (length == 0) {
    return {
      id: uuidv4(),
      name: `Set ${length + 1}`,
      orderIndex: length,
      count1: 0,
      unit1: "kg",
      count2: 0,
      unit2: "reps",
    };
  }

  const existingSet = currentSets[length - 1];

  return {
    ...existingSet,
    id: uuidv4(),
    name: `Set ${length + 1}`,
    count1: 0,
    count2: existingSet.count2 ? 0 : undefined,
  };
}
