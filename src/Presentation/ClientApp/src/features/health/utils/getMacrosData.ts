import { colors } from "~/constants/tailwindColors";
import { FoodElement } from "~/features/health/requests/getFoodListRequest";
import { getPercentages } from "~/utils/MathExtention";

const getMacrosData = (food: FoodElement, nutritionMultiplier: number) => {
  const nutritionalPercentages = getPercentages([
    food.nutritionalContents.carbohydrates,
    food.nutritionalContents.fat,
    food.nutritionalContents.protein
  ]);

  (food.nutritionalContents.carbohydrates * nutritionMultiplier).toFixed(1);

  return {
    carbohidrates: {
      name: "Carbs",
      mass: (
        food.nutritionalContents.carbohydrates * nutritionMultiplier
      ).toFixed(1),
      percentage: nutritionalPercentages[0],
      color: colors.violet
    },
    fat: {
      name: "Fat",
      mass: (food.nutritionalContents.fat * nutritionMultiplier).toFixed(1),
      percentage: nutritionalPercentages[1],
      color: colors.green
    },
    protein: {
      name: "Protein",
      mass: (food.nutritionalContents.protein * nutritionMultiplier).toFixed(1),
      percentage: nutritionalPercentages[2],
      color: colors.yellow
    }
  };
};

export default getMacrosData;
