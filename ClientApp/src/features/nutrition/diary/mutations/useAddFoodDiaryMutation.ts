import { useCustomMutation } from "@/hooks/useCustomMutation";
import { DateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import {
  AddFoodDiaryRequest,
  DiaryType,
  FoodDiariesApi,
  FoodDto,
  NutritionalContent,
} from "@/services/openapi/api";

import { invalidateFoodSearchQuery } from "../../common/queries/useFoodSearchQuery";
import {
  addNutritionalContent,
  multiplyNutritionalContent,
} from "../../common/utils/nutritionalContent";
import { dailyNutritionOverviewsQueryKeys } from "../../overview/queries/useDailyNutritionOverviewsQuery";
import {
  nutritionDiariesQueryKeys,
  setNutritionDiariesQueryData,
} from "../queries/useDiaryQuery";

const foodDiariesApi = new FoodDiariesApi();

export type AddFoodDiaryMutationVariables = AddFoodDiaryRequest;

const useAddFoodDiaryMutation = (food: FoodDto) => {
  const addFoodDiaryMutation = useCustomMutation({
    mutationFn: (variables: AddFoodDiaryRequest) => {
      const { ...addFoodDiaryRequest } = variables;
      return foodDiariesApi
        .addFoodDiary(addFoodDiaryRequest)
        .then((resp) => resp.data);
    },

    meta: {
      invalidateQueries: [dailyNutritionOverviewsQueryKeys.all],
      onSuccessToast: {
        type: "success",
        message: "Food diary added successfully",
      },
    },

    onSuccess: (resp, variables) => {
      const servingSize = Object.values(food.servingSizes).find(
        (ss) => ss.id == variables.servingSizeId,
      )!;

      invalidateFoodSearchQuery();

      queryClient.setQueryData(
        nutritionDiariesQueryKeys.overview(
          variables.entryDate as DateOnly,
          variables.entryDate as DateOnly,
        ),
        (oldData: NutritionalContent) =>
          addNutritionalContent(
            oldData,
            multiplyNutritionalContent(
              food.nutritionalContents,
              variables.quantity * servingSize.nutritionMultiplier,
            ),
          ),
      );

      setNutritionDiariesQueryData({
        date: variables.entryDate as DateOnly,
        mealType: variables.mealType,
        newDiary: {
          id: resp.id,
          name: food.name,
          nutritionalContents: multiplyNutritionalContent(
            food.nutritionalContents,
            servingSize.nutritionMultiplier,
          ),
          nutritionMultiplier: servingSize.nutritionMultiplier,
          servingSize: servingSize,
          diaryType: DiaryType.FoodDiary,
          quantity: variables.quantity,
          mealType: variables.mealType,
          date: variables.entryDate,
          isLoading: true,
          isDeleting: false,
        },
      });
    },
    onSettled: () => {
      queryClient.invalidateQueries({
        queryKey: nutritionDiariesQueryKeys.all,
      });
    },
  });

  return addFoodDiaryMutation;
};

export default useAddFoodDiaryMutation;
