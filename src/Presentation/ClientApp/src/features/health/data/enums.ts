import { ObjectValues } from "~/types/defaultTypes";

export const mealTypes = {
  breakfast: "Breakfast",
  lunch: "Lunch",
  dinner: "Dinner",
  snacks: "Snacks"
} as const;

export type TMealtTypes = ObjectValues<typeof mealTypes>;
