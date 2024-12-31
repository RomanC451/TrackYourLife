import { useMutation } from "@tanstack/react-query";
import { toastDefaultServerError } from "~/data/apiSettings";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import {
  DiaryType,
  FoodDiariesApi,
  NutritionDiaryDto,
  RecipeDiariesApi,
} from "~/services/openapi";
import { DateOnly } from "~/utils/date";
import { setNutritionDiariesQueryData } from "../../queries/foodDiaries/useNutritionDiariesQuery";
import { setTotalCaloriesQueryData } from "../../queries/useTotalCaloriesQuery";
import foodDiaryDeletedToast from "../../toasts/foodDiaries/foodDiaryDeletedToast";

const foodDiariesApi = new FoodDiariesApi();

const recipeDiariesApi = new RecipeDiariesApi();

const useDeleteNutritionDiaryMutation = () => {
  const deleteNutritionDiaryMutation = useMutation({
    mutationFn: (diary: NutritionDiaryDto) => {
      if (diary.diaryType === DiaryType.RecipeDiary) {
        return recipeDiariesApi.deleteRecipeDiary(diary.id);
      }
      return foodDiariesApi.deleteFoodDiary(diary.id);
    },

    onSuccess: (_, diary) => {
      foodDiaryDeletedToast({
        name: diary.name,
        mealType: diary.mealType,
        action: () => {
          // TODO: Implement undo functionality
        },
      });

      setTotalCaloriesQueryData({
        adjustment: -diary.nutritionalContents.energy.value * diary.quantity,
        invalidate: true,
      });

      setNutritionDiariesQueryData({
        date: diary.date as DateOnly,
        mealType: diary.mealType,
        filter: (entry) => entry.id !== diary.id,
        invalidate: true,
      });
    },

    onError: toastDefaultServerError,
  });

  const isPending = useDelayedLoading(deleteNutritionDiaryMutation.isPending);

  return { deleteNutritionDiaryMutation, isPending };
};

export default useDeleteNutritionDiaryMutation;
