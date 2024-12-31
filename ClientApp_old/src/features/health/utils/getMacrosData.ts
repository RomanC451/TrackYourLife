import { colors } from "~/constants/tailwindColors";
import { NutritionalContent } from "~/services/openapi";
import { getPercentages } from "~/utils/MathExtension";

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
      color: colors.violet,
    },
    fat: {
      name: "Fat",
      mass: (nutritionalContents.fat * nutritionMultiplier).toFixed(1),
      percentage: nutritionalPercentages[1],
      color: colors.green,
    },
    protein: {
      name: "Protein",
      mass: (nutritionalContents.protein * nutritionMultiplier).toFixed(1),
      percentage: nutritionalPercentages[2],
      color: colors.yellow,
    },
  };
};

export default getMacrosData;
