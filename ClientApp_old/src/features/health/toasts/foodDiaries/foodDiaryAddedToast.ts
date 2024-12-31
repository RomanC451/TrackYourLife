import { toast } from "sonner";
import { FoodDto, MealTypes, ServingSizeDto } from "~/services/openapi";

type FoodDiaryAddedToastProps = {
  food: FoodDto;
  quantity: number;
  mealType: MealTypes;
  servingSize: ServingSizeDto;
};

const foodDiaryAddedToast = ({
  food,
  quantity,
  mealType,
  servingSize,
}: FoodDiaryAddedToastProps) =>
  toast(`${food.name} (${food.brandName})`, {
    description: `${quantity * servingSize.value} ${
      servingSize.unit
    } has been added on ${mealType}`,
  });

export default foodDiaryAddedToast;
