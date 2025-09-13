import { useCustomMutation } from "@/hooks/useCustomMutation";
import { DateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import {
  DiaryType,
  FoodDiariesApi,
  NutritionalContent,
  NutritionDiaryDto,
  RecipeDiariesApi,
} from "@/services/openapi";

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

const recipeDiariesApi = new RecipeDiariesApi();

const useDeleteNutritionDiaryMutation = () => {
  const deleteNutritionDiaryMutation = useCustomMutation({
    mutationFn: (diary: NutritionDiaryDto) => {
      if (diary.diaryType === DiaryType.RecipeDiary) {
        return recipeDiariesApi.deleteRecipeDiary(diary.id);
      }
      return foodDiariesApi.deleteFoodDiary(diary.id);
    },
    meta: {
      invalidateQueries: [dailyNutritionOverviewsQueryKeys.all],
      onSuccessToast: {
        type: "success",
        message: "Nutrition diary deleted successfully",
      },
    },

    onSuccess: (_, diary) => {
      queryClient.setQueryData(
        nutritionDiariesQueryKeys.overview(
          diary.date as DateOnly,
          diary.date as DateOnly,
        ),
        (oldData: NutritionalContent) =>
          addNutritionalContent(
            oldData,
            multiplyNutritionalContent(diary.nutritionalContents, -1),
          ),
      );

      setNutritionDiariesQueryData({
        date: diary.date as DateOnly,
        mealType: diary.mealType,
        deleteDiaryId: diary.id,
      });
    },
    onSettled: () => {
      queryClient.invalidateQueries({
        queryKey: nutritionDiariesQueryKeys.all,
      });
    },
  });

  return deleteNutritionDiaryMutation;
};

export default useDeleteNutritionDiaryMutation;
