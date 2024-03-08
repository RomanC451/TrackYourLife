import { toast } from "sonner";
import { toastDefaultServerError } from "~/data/apiSettings";
import { TMealtTypes } from "~/features/health/data/enums";
import { FoodElement } from "~/features/health/requests/getFoodListRequest";
import { useApiRequests } from "~/hooks/useApiRequests";
import { DateOnly } from "~/utils/date";

import { useMutation, useQueryClient } from "@tanstack/react-query";

import {
  deleteFoodDiaryRequest,
  postFoodDiaryRequest,
  TGetFoodDiaryListResponse,
  TPostFoodDiaryData
} from "../requests/";

const useAddFoodDiaryMutation = (food: FoodElement, onSuccess?: () => void) => {
  const { fetchRequest } = useApiRequests();

  const queryClient = useQueryClient();

  const deleteFoodDiaryMutation = useMutation({
    mutationFn: (variables: {
      id: string;
      mealType: TMealtTypes;
      date: DateOnly;
    }) => deleteFoodDiaryRequest(fetchRequest, variables.id),
    onSuccess: (_, variables) => {
      toast(`${food.name} (${food.brandName})`, {
        description: `Has been removed from ${variables.mealType}.`
      });

      queryClient.invalidateQueries({
        queryKey: ["foodDiary"]
      });

      queryClient.setQueryData(
        ["foodDiary", variables.date],
        (oldData: TGetFoodDiaryListResponse) => {
          return {
            ...oldData,
            [variables.mealType.toLowerCase()]: [
              ...oldData[
                variables.mealType.toLowerCase() as keyof TGetFoodDiaryListResponse
              ].filter((entry) => entry.id !== variables.id)
            ]
          };
        }
      );
    },
    onError: toastDefaultServerError
  });

  const addFoodDiaryMutation = useMutation({
    mutationFn: (variables: TPostFoodDiaryData) =>
      postFoodDiaryRequest(fetchRequest, variables),
    onSuccess: (resp, variables) => {
      const servingSize = food.servingSizes.find(
        (ss) => ss.id == variables.servingSizeId
      )!;

      toast(`${food.name} (${food.brandName})`, {
        description: `${variables.quantity * servingSize.value} ${
          servingSize.unit
        } has been added on ${variables.mealType}`,
        action: {
          label: "Undo",
          onClick: () =>
            deleteFoodDiaryMutation.mutate({
              id: resp.foodDiaryEntryId,
              mealType: variables.mealType,
              date: variables.date
            })
        }
      });

      queryClient.invalidateQueries({
        queryKey: ["foodDiary"]
      });

      queryClient.setQueryData(
        ["foodDiary", variables.date],
        (oldData: TGetFoodDiaryListResponse) => {
          return {
            ...oldData,
            [variables.mealType.toLowerCase()]: [
              ...oldData[
                variables.mealType.toLowerCase() as keyof TGetFoodDiaryListResponse
              ],
              {
                id: resp.foodDiaryEntryId,
                food: food,
                mealType: variables.mealType,
                quantity: variables.quantity,
                servingSize: servingSize,
                date: variables.date
              }
            ]
          };
        }
      );

      onSuccess?.();
    },
    onError: toastDefaultServerError
  });

  return { addFoodDiaryMutation };
};

export default useAddFoodDiaryMutation;
