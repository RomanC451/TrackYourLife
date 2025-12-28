import { Difficulty } from "@/services/openapi";

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
