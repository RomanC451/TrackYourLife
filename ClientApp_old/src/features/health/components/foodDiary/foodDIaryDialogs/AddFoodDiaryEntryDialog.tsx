import React from "react";
import { Dialog, DialogContent, DialogTrigger } from "~/chadcn/ui/dialog";

import useAddFoodDiaryEntryDialog from "~/features/health/hooks/foodDiaries/useAddFoodDiaryEntryDialog";
import { FoodDto } from "~/services/openapi";
import { DateOnly } from "~/utils/date";
import DiaryEntryOverview from "../../foodSearch/DiaryEntryOverview";
import FoodDiaryEntryDialogContent from "./FoodDiaryEntryDialogContent";

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
      <DialogTrigger asChild className=" w-full text-left">
        <button>
          <DiaryEntryOverview
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
