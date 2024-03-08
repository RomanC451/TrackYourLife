import React from "react";
import { Skeleton } from "~/chadcn/ui/skeleton";

import { FoodElement } from "../../../requests/getFoodListRequest";
import AddFoodDiaryEntryDialog from "../foodDiaryForms/AddFoodDiaryEntryDialog";
import AddFoodToMealSelectButton from "./SelectMealToAddButton";

type FoodListElementProps = {
  food?: FoodElement;
  date: Date;
};

const FoodListElement: React.FC<FoodListElementProps> = ({ food, date }) => {
  if (food == undefined) {
    return (
      <div className="relative">
        <div className="w-full">
          <Skeleton className="h-4 w-[100px]" />
          <Skeleton className="mt-2 h-3 w-[200px]" />
        </div>
        <Skeleton className="absolute right-2 top-[50%] h-[40px] w-[40px] translate-y-[-50%]" />
      </div>
    );
  }

  return (
    <div className="relative">
      <AddFoodDiaryEntryDialog food={food} date={date} />
      <AddFoodToMealSelectButton
        food={food}
        date={date}
        className="absolute right-2 top-[50%] translate-y-[-50%]"
      />
    </div>
  );
};

export default FoodListElement;
