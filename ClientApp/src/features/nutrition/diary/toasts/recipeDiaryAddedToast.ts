import { toast } from "sonner";

import { MealTypes } from "@/services/openapi";

type RecipeDiaryAddedToastProps = {
  name: string;
  mealType: MealTypes;
  numberOfServings: number;
};

export default function recipeDiaryAddedToast({
  name,
  mealType,
  numberOfServings,
}: RecipeDiaryAddedToastProps) {
  toast(`${name} `, {
    description: `${numberOfServings} servings has been added on ${mealType}`,
  });
}
