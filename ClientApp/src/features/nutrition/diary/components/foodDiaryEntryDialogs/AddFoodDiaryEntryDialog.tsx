import React from "react";

import { Dialog, DialogContent, DialogTrigger } from "@/components/ui/dialog";
import { DateOnly } from "@/lib/date";
import { FoodDto } from "@/services/openapi";

import FoodListElementOverview from "../../../common/components/foodList/FoodListElementOverview";
import FoodDiaryEntryDialogContent from "./FoodDiaryEntryDialogContent";
import useAddFoodDiaryEntryDialog from "./useAddFoodDiaryEntryDialog";

type AddFoodDiaryEntryDialogProps = {
  food: FoodDto;
  date: DateOnly;
  onSuccess: () => void;
};

const AddFoodDiaryEntryDialog: React.FC<AddFoodDiaryEntryDialogProps> = ({
  food,
  date,
  onSuccess,
}) => {
  const { dialogState, toggleDialogState, onSubmit, isPending } =
    useAddFoodDiaryEntryDialog(food, date, onSuccess);

  return (
    <Dialog open={dialogState} onOpenChange={toggleDialogState}>
      <DialogTrigger asChild className="w-full text-left">
        <button>
          <FoodListElementOverview
            name={food.name}
            nutritionalContents={food.nutritionalContents}
            brandName={food.brandName}
            quantity={
              food.servingSizes[0].value + " " + food.servingSizes[0].unit
            }
          />
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
          isPending={isPending}
          submitButtonText="Add to"
        />
      </DialogContent>
    </Dialog>
  );
};

export default AddFoodDiaryEntryDialog;
