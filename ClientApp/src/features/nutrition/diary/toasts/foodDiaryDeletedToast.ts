import { toast } from "sonner";

import { MealTypes } from "@/services/openapi";

type FoodDiaryDeletedToastProps = {
  name: string;
  mealType: MealTypes;
  action: () => void;
};

const foodDiaryDeletedToast = ({
  name,
  mealType,
  action,
}: FoodDiaryDeletedToastProps) =>
  toast(name, {
    description: `Has been removed from ${mealType}.`,
    action: {
      label: "Undo",
      onClick: action,
    },
  });

export default foodDiaryDeletedToast;
