import React from "react";
import { useToggle } from "usehooks-ts";
import { Dialog, DialogContent, DialogTrigger } from "~/chadcn/ui/dialog";
import { getDateOnly } from "~/utils/date";

import {
  AddFoodDiaryEntryRequest,
  FoodResponse,
  MealTypes,
} from "~/services/openapi";
import { TFoodDiaryFormSchema } from "../../../../hooks/useAddFoodDiaryForm";
import useAddFoodDiaryMutation from "../../../../hooks/useAddFoodDiaryMutation";
import FoodOverview from "../../foodSearch/FoodOverview";
import FoodDiaryEntryDialogContent from "./FoodDiaryEntryDialogContent";

type AddFoodDiaryEntryDialogProps = {
  food: FoodResponse;
  date: Date;
};

const AddFoodDiaryEntryDialog: React.FC<AddFoodDiaryEntryDialogProps> = ({
  food,
  date,
}) => {
  const [dialogState, toggleDialogState] = useToggle(false);

  const { addFoodDiaryMutation } = useAddFoodDiaryMutation(
    food,
    toggleDialogState,
  );

  function onSubmit(formData: TFoodDiaryFormSchema) {
    const requestBody: AddFoodDiaryEntryRequest = {
      foodId: food.id,
      mealType: formData.mealType as MealTypes,
      servingSizeId: food.servingSizes[formData.servingSizeIndex].id,
      quantity: formData.nrOfServings,
      entryDate: getDateOnly(date),
    };

    addFoodDiaryMutation.mutate(requestBody);
  }

  return (
    <Dialog open={dialogState} onOpenChange={toggleDialogState}>
      <DialogTrigger asChild className=" w-full text-left">
        <button>
          <FoodOverview food={food} />
        </button>
      </DialogTrigger>

      <DialogContent
        className="gap-2"
        onOpenAutoFocus={(e) => {
          e.preventDefault();
        }}
      >
        <FoodDiaryEntryDialogContent
          food={food}
          onSubmit={onSubmit}
          isPending={addFoodDiaryMutation.isPending}
          submitButtonText="Add to"
        />
      </DialogContent>
    </Dialog>
  );
};

export default AddFoodDiaryEntryDialog;
