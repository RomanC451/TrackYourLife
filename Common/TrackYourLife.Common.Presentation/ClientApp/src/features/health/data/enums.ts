import { ObjectValues } from "~/types/defaultTypes";

export const MealTypes = {
  breakfast: "Breakfast",
  lunch: "Lunch",
  dinner: "Dinner",
  snacks: "Snacks",
} as const;

export type TMealTypes = ObjectValues<typeof MealTypes>;

export const UserGoalTypes = {
  calories: "Calories",
} as const;

export type TUserGoalTypes = ObjectValues<typeof UserGoalTypes>;

export const UserGoalPerPeriods = {
  day: "Day",
  week: "Week",
  month: "Month",
} as const;

export type TUserGoalPerPeriods = ObjectValues<typeof UserGoalPerPeriods>;
