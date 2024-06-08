import { TMealTypes } from "~/features/health/data/enums";
import { DateOnly } from "~/utils/date";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import { toastDefaultServerError } from "~/data/apiSettings";
import {
  AddFoodDiaryEntryRequest,
  FoodDiaryApi,
  FoodDiaryEntryListResponse,
  FoodResponse,
} from "~/services/openapi/api";

const foodDiaryApi = new FoodDiaryApi();

const useAddFoodDiaryMutation = (
  food: FoodResponse,
  onSuccess?: () => void,
) => {
  const queryClient = useQueryClient();

  const deleteFoodDiaryMutation = useMutation({
    mutationFn: (variables: {
      id: string;
      mealType: TMealTypes;
      date: DateOnly;
    }) => {
      console.log(variables);
      return foodDiaryApi.deleteEntry(variables.id);
    },
    onSuccess: (_, variables) => {
      toast(`${food.name} (${food.brandName})`, {
        description: `Has been removed from ${variables.mealType}.`,
      });

      console.log(1);

      queryClient.invalidateQueries({
        queryKey: ["foodDiary"],
      });
      console.log(2);
      queryClient.invalidateQueries({
        queryKey: ["totalCalories"],
      });
      console.log(3);

      queryClient.setQueryData(
        ["foodDiary", variables.date],
        (oldData: FoodDiaryEntryListResponse) => {
          return {
            ...oldData,
            [variables.mealType.toLowerCase()]: [
              ...oldData[
                variables.mealType.toLowerCase() as keyof FoodDiaryEntryListResponse
              ].filter((entry) => entry.id !== variables.id),
            ],
          };
        },
      );
      console.log(4);
    },
    onError: toastDefaultServerError,
  });

  const addFoodDiaryMutation = useMutation({
    mutationFn: (variables: AddFoodDiaryEntryRequest) =>
      foodDiaryApi.addEntry(variables).then((resp) => resp.data),
    // postFoodDiaryRequest(fetchRequest, variables),
    onSuccess: (resp, variables) => {
      const servingSize = food.servingSizes.find(
        (ss) => ss.id == variables.servingSizeId,
      )!;

      toast(`${food.name} (${food.brandName})`, {
        description: `${variables.quantity * servingSize.value} ${
          servingSize.unit
        } has been added on ${variables.mealType}`,
        action: {
          label: "Undo",
          onClick: () =>
            deleteFoodDiaryMutation.mutate({
              id: resp.id,
              mealType: variables.mealType,
              date: variables.entryDate as DateOnly,
            }),
        },
      });

      queryClient.invalidateQueries({
        queryKey: ["foodDiary"],
      });

      queryClient.invalidateQueries({
        queryKey: ["totalCalories"],
      });

      queryClient.setQueryData(
        ["foodDiary", variables.entryDate],
        (oldData: FoodDiaryEntryListResponse) => {
          return {
            ...oldData,
            [variables.mealType.toLowerCase()]: [
              ...oldData[
                variables.mealType.toLowerCase() as keyof FoodDiaryEntryListResponse
              ],
              {
                id: resp.id,
                food: food,
                mealType: variables.mealType,
                quantity: variables.quantity,
                servingSize: servingSize,
                date: variables.entryDate,
              },
            ],
          };
        },
      );

      onSuccess?.();
    },
    onError: toastDefaultServerError,
  });

  return { addFoodDiaryMutation };
};

export default useAddFoodDiaryMutation;
