import { colors } from "@/constants/tailwindColors";
import { getPercentages } from "@/lib/MathExtension";
import { NutritionalContent } from "@/services/openapi";

const getMacrosData = (
  nutritionalContents: NutritionalContent,
  nutritionMultiplier: number,
) => {
  const nutritionalPercentages = getPercentages([
    nutritionalContents.carbohydrates,
    nutritionalContents.fat,
    nutritionalContents.protein,
  ]);

  return {
    carbohydrates: {
      name: "Carbs",
      mass: (nutritionalContents.carbohydrates * nutritionMultiplier).toFixed(
        1,
      ),
      percentage: nutritionalPercentages[0],
      color: colors.green,
    },
    fat: {
      name: "Fat",
      mass: (nutritionalContents.fat * nutritionMultiplier).toFixed(1),
      percentage: nutritionalPercentages[1],
      color: colors.yellow,
    },
    protein: {
      name: "Protein",
      mass: (nutritionalContents.protein * nutritionMultiplier).toFixed(1),
      percentage: nutritionalPercentages[2],
      color: colors.violet,
    },
  };
};

export default getMacrosData;
