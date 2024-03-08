import React from "react";
import { Dialog, DialogContent } from "~/chadcn/ui/dialog";
import { useApiRequests } from "~/hooks/useApiRequests";
import useDelayedLoading from "~/hooks/useDelayedLoading";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { TMealtTypes } from "../../../data/enums";
import { TFoodDiaryFormSchema } from "../../../hooks/useAddFoodDiaryForm";
import {
  getFoodRequest,
  putFoodDiaryRequest,
  TPutFoodDiaryData,
} from "../../../requests";
import { FoodDiaryEntry } from "../foodDiary/diaryTable/columns";
import FoodDiaryEntryDialogContent from "./FoodDiaryEntryDialogContent";
import FoodDiaryEntryDialogLoading from "./FoodDiaryEntryDialogLoading";

type EditFoodDiaryEntryDialogProps = {
  diaryEntry: FoodDiaryEntry;
  dialogState: boolean;
  toggleDialogState: () => void;
};

const EditFoodDiaryEntryDialog: React.FC<EditFoodDiaryEntryDialogProps> = ({
  diaryEntry,
  dialogState,
  toggleDialogState,
}) => {
  const { fetchRequest } = useApiRequests();
  const queryClient = useQueryClient();

  const query = useQuery({
    queryKey: ["foodDiary", diaryEntry.id],
    queryFn: () =>
      getFoodRequest({
        fetchRequest,
        foodId: diaryEntry.foodId,
      }),
  });

  const updateFoodDiaryMutation = useMutation({
    mutationFn: (data: TPutFoodDiaryData) =>
      putFoodDiaryRequest(fetchRequest, data),
    onSuccess: () => {
      toggleDialogState();
      queryClient.invalidateQueries({
        queryKey: ["foodDiary"],
      });
    },
  });

  function onSubmit(formData: TFoodDiaryFormSchema) {
    if (query.data === undefined) return;

    const requestBody: TPutFoodDiaryData = {
      foodDiaryEntryId: diaryEntry.id,
      mealType: formData.mealType as TMealtTypes,
      servingSizeId: query.data.food.servingSizes[formData.servingSizeIndex].id,
      quantity: formData.nrOfServings,
    };

    updateFoodDiaryMutation.mutate(requestBody);
  }

  const loadingState = useDelayedLoading(100, query.data);

  if (loadingState.isStarting) return null;

  return (
    <Dialog open={dialogState} onOpenChange={toggleDialogState}>
      <DialogContent
        className="gap-2"
        onOpenAutoFocus={(e) => {
          e.preventDefault();
        }}
      >
        {loadingState.isStarting ? (
          <FoodDiaryEntryDialogLoading />
        ) : (
          <FoodDiaryEntryDialogContent
            food={query.data!.food}
            onSubmit={onSubmit}
            isPending={updateFoodDiaryMutation.isPending}
            defaultValues={{
              nrOfServings: diaryEntry.nrOfServings,
              servingSizeIndex: query.data!.food.servingSizes.findIndex(
                (servingSize) => servingSize.id === diaryEntry.servingSize.id,
              ),
              mealType: diaryEntry.meal,
            }}
            submitButtonText="Update food diary"
          />
        )}
      </DialogContent>
    </Dialog>
  );
};

export default EditFoodDiaryEntryDialog;
