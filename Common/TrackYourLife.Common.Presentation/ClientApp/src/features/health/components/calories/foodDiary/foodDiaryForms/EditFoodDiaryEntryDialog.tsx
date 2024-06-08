import React from "react";
import { Dialog, DialogContent } from "~/chadcn/ui/dialog";
import useDelayedLoading from "~/hooks/useDelayedLoading";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { FoodApi } from "~/services/openapi";
import { TMealTypes } from "../../../../data/enums";
import { TFoodDiaryFormSchema } from "../../../../hooks/useAddFoodDiaryForm";

import {
  FoodDiaryApi,
  UpdateFoodDiaryEntryRequest,
} from "~/services/openapi/api";
import { FoodDiaryEntry } from "../diaryTable/columns";
import FoodDiaryEntryDialogContent from "./FoodDiaryEntryDialogContent";
import FoodDiaryEntryDialogLoading from "./FoodDiaryEntryDialogLoading";

type EditFoodDiaryEntryDialogProps = {
  diaryEntry: FoodDiaryEntry;
  dialogState: boolean;
  toggleDialogState: () => void;
};

const foodApi = new FoodApi();

const foodDiaryApi = new FoodDiaryApi();

const EditFoodDiaryEntryDialog: React.FC<EditFoodDiaryEntryDialogProps> = ({
  diaryEntry,
  dialogState,
  toggleDialogState,
}) => {
  const queryClient = useQueryClient();

  const foodQuery = useQuery({
    queryKey: ["foodDiary", diaryEntry.id, "food"],
    queryFn: () => foodApi.getById(diaryEntry.foodId).then((res) => res.data),
  });

  const updateFoodDiaryMutation = useMutation({
    mutationFn: (data: UpdateFoodDiaryEntryRequest) =>
      foodDiaryApi.updateEntry(data),
    onSuccess: () => {
      toggleDialogState();
      queryClient.invalidateQueries({
        queryKey: ["foodDiary"],
      });
    },
  });

  function onSubmit(formData: TFoodDiaryFormSchema) {
    if (foodQuery.data === undefined) return;

    const requestBody: UpdateFoodDiaryEntryRequest = {
      id: diaryEntry.id,
      mealType: formData.mealType as TMealTypes,
      servingSizeId: foodQuery.data.servingSizes[formData.servingSizeIndex].id,
      quantity: formData.nrOfServings,
    };

    updateFoodDiaryMutation.mutate(requestBody);
  }

  const loadingState = useDelayedLoading(100, foodQuery.data);

  if (loadingState.isStarting) return null;

  return (
    <Dialog open={dialogState} onOpenChange={toggleDialogState}>
      <DialogContent
        className="max-h-screen gap-2"
        onOpenAutoFocus={(e) => {
          e.preventDefault();
        }}
      >
        {loadingState.isStarting ? (
          <FoodDiaryEntryDialogLoading />
        ) : (
          <FoodDiaryEntryDialogContent
            food={foodQuery.data}
            onSubmit={onSubmit}
            isPending={updateFoodDiaryMutation.isPending}
            defaultValues={{
              nrOfServings: diaryEntry.nrOfServings,
              servingSizeIndex: foodQuery.data!.servingSizes.findIndex(
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
