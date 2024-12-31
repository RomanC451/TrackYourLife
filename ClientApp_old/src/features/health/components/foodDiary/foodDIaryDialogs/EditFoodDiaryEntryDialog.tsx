import React from "react";
import { Dialog, DialogContent } from "~/chadcn/ui/dialog";

import { getServingSizeIndex } from "~/features/health/data/filters";
import useUpdateFoodDiaryEntryDialog from "~/features/health/hooks/foodDiaries/useUpdateFoodDiaryEntryDialog";
import FoodDiaryDialogContent from "./FoodDiaryEntryDialogContent";

type EditFoodDialogProps = {
  diaryId: string;
  dialogState: boolean;
  toggleDialogState: () => void;
};

const EditFoodDiaryEntryDialog: React.FC<EditFoodDialogProps> = ({
  diaryId,
  dialogState,
  toggleDialogState,
}) => {
  const { foodDiaryQuery, onSubmit, isPending } = useUpdateFoodDiaryEntryDialog(
    diaryId,
    toggleDialogState,
  );

  if (foodDiaryQuery.data === undefined) return null;

  const diary = foodDiaryQuery.data;

  return (
    <Dialog open={dialogState} onOpenChange={toggleDialogState}>
      <DialogContent
        className="max-h-screen gap-2"
        onOpenAutoFocus={(e) => {
          e.preventDefault();
        }}
      >
        {isPending.isLoading ? (
          <FoodDiaryDialogContent.Loading />
        ) : (
          <FoodDiaryDialogContent
            food={diary.food}
            onSubmit={onSubmit}
            isPending={isPending}
            defaultValues={{
              nrOfServings: diary.quantity,
              servingSizeIndex: getServingSizeIndex(
                diary.food.servingSizes,
                diary.servingSize,
              ),
              mealType: diary.mealType,
            }}
            submitButtonText="Update food diary"
          />
        )}
      </DialogContent>
    </Dialog>
  );
};

export default EditFoodDiaryEntryDialog;
